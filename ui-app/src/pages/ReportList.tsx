import React, { useState } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
import { Tab, TabRef } from '../controls/Tab';
import { ReportDetail } from './ReportDetail';

export const ReportList = (p: {  editable: boolean, tabRef: TabRef }) => {
    const storageName = 'storage_report_list';
    const [, forceUpdate] = useState(null);
    const searchInputCtrl = React.createRef<HTMLInputElement>();
    Dyn.DataStorage.register(storageName, forceUpdate);

    const options = {
        listStorage: storageName,
        editable: p.editable,
        tabInstance: p.tabRef.instance
    };
    const addNewHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        const tabId = 'report_new';
        p.tabRef.instance.addTab(tabId, {
            closable: true,
            title: '[New report]',
            content: <ReportDetail
                reportId = {0}
                tabId = {tabId}
                editable={true}
                editMode={true}
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
                    <h3>Report list</h3>
                </div>
            </div>
            <div className="row">
                <div className="col-sm-12">
                    <Dyn.View name="sys_reporting_list" id={'cidList_Report'} key={'sl' + JSON.stringify(Dyn.DataStorage.get(storageName))} options={options} />
                </div>
            </div>
            <br />
            {p.editable ? <>
                <div className="row">
                    <div className="col-sm-12">
                        <div className="center"><button type="button" className="btn btn-primary fas fa-plus-circle" onClick={(evt) => { addNewHandler(evt); }}> Add more</button></div>
                    </div>
                </div>
            </>
                : null}

        </>
    );
};
ReportList.defaultProps = {
    tabRef: { instance: null } as TabRef
};

export const ReportItem = (p: { data: any, parent: Dyn.View, dataIndex: number }) => {
    const fieldId = Dyn.DataPool.allFields['reporting_id'].dataField;
    const opt = p.parent.props.options;
    const listStorage = opt.listStorage;
    const editable = opt.editable as boolean;
    const tabInstance = opt.tabInstance as Tab;
    const editClickHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>, data: any) => {
        const tabId = 'report_' + data[fieldId];
        tabInstance.addTab(tabId, {
            closable: true,
            title: data[Dyn.DataPool.allFields['reporting_name'].dataField],
            content: <ReportDetail
                reportId = {data[fieldId]}
                tabId = {tabId}
                editable={editable}
                storageName={listStorage} />
        });
        evt.preventDefault();
    };
    const deleteClickHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>, data: any) => {
        const viewDef = Dyn.DataPool.allViews['sys_reporting_detail'];
        const url = viewDef.deleteApi;
        const headers = new Headers();
        headers.append('Accept', 'application/json');
        headers.append('Content-Type', 'application/json');
        let deleteData = {}; deleteData[fieldId] = data[fieldId];
        deleteData = viewDef.deleteApiParamsFunc(null, deleteData);
        Dyn.execApiAsync(url, deleteData).then(() => {
            const val = Dyn.DataStorage.get(listStorage);
            Dyn.DataStorage.set(listStorage, { ...val, dts: new Date().getTime() });
        });
        evt.preventDefault();
    };
    return (
        <>
            <div className="card">
                <div className="card-body">

                    <Dyn.View name="sys_reporting_item" id={'cidItem_Reporting' + p.dataIndex} readonly={true} key={'cidItem_Reporting' + p.dataIndex} dataSource={p.data} />

                </div>
                <div className="card-footer center">
                    <button type="button" className="btn btn-primary fas fa-eye" onClick={(evt) => { editClickHandler(evt, p.data) }}> View</button>
                    {editable ? <button type="button" className="btn btn-danger fas fa-trash" onClick={(evt) => { deleteClickHandler(evt, p.data) }}> Delete</button> : null}

                </div>
            </div>

        </>
    );
}
