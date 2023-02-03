import React, { useState } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
import { Tab, TabRef } from '../controls/Tab';
import { RuleDetail } from './RuleDetail';

export const RuleList = (p: {tabRef: TabRef}) => {
    const storageName = 'storage_rule_list';
    const [, forceUpdate] = useState(null);
    const searchInputCtrl = React.createRef<HTMLInputElement>();
    Dyn.DataStorage.register(storageName, forceUpdate);

    const options = {
        listStorage: storageName,
        tabInstance: p.tabRef.instance
    };
    const addNewHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        const tabId = 'rule_new';
        p.tabRef.instance.addTab(tabId, {
            title: '[New rule]',
            closable: true,
            content: <RuleDetail
                ruleId={0}
                tabId={tabId}
                newMode={true}
                storageName={storageName} />
        });
        evt.preventDefault();
    };
    const searchClickHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        Dyn.DataStorage.set(storageName, { searchValue: searchInputCtrl.current.value, dts: new Date().getTime() });
        evt.preventDefault();
    };
    return (
        <>
            <div className="row">
                <div className="col-sm-4"></div>
                <div className="col-sm-4">
                    <div className="input-group rounded">
                        <input type="search" ref={searchInputCtrl} className="form-control rounded" placeholder="Search" aria-label="Search"
                            aria-describedby="search-addon" />
                        <span className="input-group-text border-0 action" id="search-addon" onClick={searchClickHandler}>
                            <i className="fas fa-search"></i>
                        </span>
                    </div>

                </div>
                <div className="col-sm-4"></div>
            </div>
            <div className="row">
                <div className="col-sm-12">
                    <h3>Rule list</h3>
                </div>
            </div>
            <div className="row">
                <div className="col-sm-12">
                    <Dyn.View name="sys_rule_list" id={'cidList_Rule'} key={'sl' + JSON.stringify(Dyn.DataStorage.get(storageName))} options={options} />
                </div>
            </div>
            <br />
            <div className="row">
                <div className="col-sm-12">
                    <div className="center"><button type="button" className="btn btn-primary fas fa-plus-circle" onClick={(evt) => { addNewHandler(evt); }}> Add more</button></div>
                </div>
            </div>

        </>
    );
};
RuleList.defaultProps = {
    tabRef: { instance: null } as TabRef
};

export const RuleItem = (p: { data: any, parent: Dyn.View, dataIndex: number }) => {
    const fieldId = Dyn.DataPool.allFields['rule_id'].dataField;
    const opt = p.parent.props.options;
    const listStorage = opt.listStorage;
    const tabInstance = opt.tabInstance as Tab;
    const editClickHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>, data: any) => {
        const tabId = 'rule_' + data[fieldId];
        tabInstance.addTab(tabId, {
            title: data[Dyn.DataPool.allFields['rule_name'].dataField],
            closable: true,
            content: <RuleDetail
                ruleId={data[fieldId]}
                tabId={tabId}
                storageName = {listStorage}
             />
        });
        evt.preventDefault();
    };
    const deleteClickHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>, data: any) => {
        const viewDef = Dyn.DataPool.allViews['sys_rule_detail'];
        const url = viewDef.deleteApi;
        const headers = new Headers();
        headers.append('Accept', 'application/json');
        headers.append('Content-Type', 'application/json');
        let deleteData = {}; deleteData[fieldId] = data[fieldId];
        deleteData = viewDef.deleteApiParamsFunc(null, deleteData);
        Dyn.execApiAsync(url, deleteData).then(() => {
            Dyn.DataStorage.update(listStorage, { dts: new Date().getTime() });
        });
        evt.preventDefault();
    };
    return (
        <>
            <div className="card">
                <div className="card-body">

                    <Dyn.View name="sys_rule_item" id={'cidItem_Rule' + p.dataIndex} readonly={true} key={'cidItem_Rule' + p.dataIndex} dataSource={p.data} />

                </div>
                <div className="card-footer center">
                    <button type="button" className="btn btn-primary fas fa-eye" onClick={(evt) => { editClickHandler(evt, p.data) }}> View</button>
                    <button type="button" className="btn btn-danger fas fa-trash" onClick={(evt) => { deleteClickHandler(evt, p.data) }}> Delete</button>

                </div>
            </div>

        </>
    );
}
