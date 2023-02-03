import React, { useState } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
import { Tab, TabRef } from '../controls/Tab';
import { ItemDetail } from './ItemDetail';
import { Modal } from '../controls/Modal';

export const ItemList = (p: {
    id: string,
    title: string,
    itemName: string,
    idField: string, displayField: string,
    editable: boolean,
    authenticated: boolean,
    showComment: boolean, showHistory: boolean, approvalList: boolean,
    listViewName?: string,
    tabRef: TabRef
}) => {
    const detailName = p.itemName + '_DetailView';
    const listName = p.itemName + '_ItemView';

    const detailStorage = 'storage_' + detailName;
    const [, forceUpdate] = useState(null);
    const searchInputCtrl = React.createRef<HTMLInputElement>();

    let viewListName = 'sys_anyItem_listView';
    if (p.approvalList) {
        viewListName = p.listViewName || 'sys_anyItem_approvalList';
    }

    const storageName = 'storage_' + listName + viewListName;
    Dyn.DataStorage.register(storageName, forceUpdate);


    const options = {
        listName: listName,
        idField: p.idField,
        displayField: p.displayField,
        detailName: detailName,
        listStorage: storageName,
        detailtStorage: detailStorage,
        itemName: p.itemName,
        showComment: p.showComment,
        showHistory: p.showHistory,
        editable: p.approvalList ? false : p.editable,
        authenticated: p.authenticated,
        containerId: p.id,
        searchValue: '',
        tabInstance: p.tabRef.instance
    };



    const addNewHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        modal.current.open();
        evt.preventDefault();
    };
    const searchClickHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {

        Dyn.DataStorage.set(storageName, { searchValue: searchInputCtrl.current.value, dts: new Date().getTime() });
        evt.preventDefault();
    };
    const modal = React.createRef<Modal>();
    const itemDetailView = React.createRef<Dyn.View>();
    const saveActionModalHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>): boolean => {

        //submit final
        const pm = itemDetailView.current.submitData();
        if (pm == null) return false;
        else {
            pm.then(() =>
                Dyn.DataStorage.update(storageName, { dts: new Date().getTime() })
            );
            return true;
        }
    };
    return (
        <>
            {viewListName !== 'sys_anyItem_approvalList' ? <div className="row">
                <div className="col-sm-4"></div>
                <div className="col-sm-4">
                    <div className="input-group rounded search">
                        <input type="search" ref={searchInputCtrl} className="form-control rounded" placeholder="Search" aria-label="Search"
                            aria-describedby="search-addon" />
                        <span className="input-group-text action" onClick={searchClickHandler}>
                            <i className="fas fa-search"></i>
                        </span>
                    </div>

                </div>
                <div className="col-sm-4"></div>
            </div>
                : null}

            <div className="row">
                <div className="col-sm-12">
                    <h3>{p.title}</h3>
                </div>
            </div>
            <div className="row">
                <div className="col-sm-12">
                    <Dyn.View name={viewListName} id={p.id + '_List_' + listName} key={'sl' + JSON.stringify(Dyn.DataStorage.get(storageName))} options={options} />
                </div>
            </div>
            <br />
            {p.editable ? <>
                <div className="row">
                    <div className="col-sm-12">
                        <div className="center"><button type="button" className="btn btn-primary fas fa-plus-circle" onClick={(evt) => { addNewHandler(evt); }}> Add more</button></div>
                    </div>
                </div>
                <Modal title="[New Item]" ref={modal} saveOpt={{ text: 'Save', actionHandler: saveActionModalHandler }} cancelOpt={{ text: 'Cancel', actionHandler: null }}>
                    <Dyn.View ref={itemDetailView} name={detailName} id={p.id + '_Modal_' + detailName} key={new Date().getTime()} options={{ storageName }} dataSource={{}} />
                </Modal>
            </>
                : null}


        </>
    );
};
ItemList.defaultProps = {
    tabRef: { instance: null } as TabRef
};

export const ListItem = (p: { data: any, parent: Dyn.View, dataIndex: number }) => {

    const opt = p.parent.props.options;
    const id = opt.containerId;
    const listViewName = opt.listName;
    const idField = opt.idField;
    const displayField = opt.displayField;
    const detailViewName = opt.detailName;
    const listStorage = opt.listStorage;
    const showComment = opt.showComment;
    const showHistory = opt.showHistory;
    const editable = opt.editable as boolean;
    const authenticated = opt.authenticated as boolean;
    const tabInstance = opt.tabInstance as Tab;
    const editClickHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>, data: any) => {
        const tabId = id + '_tab' + detailViewName + data[idField];
        tabInstance.addTab(tabId, {
            closable: true,
            title: data[displayField],
            content: <ItemDetail
                displayField={displayField}
                detailName={detailViewName}
                listStorage={listStorage}
                tabId={tabId}
                showComment={showComment}
                showHistory={showHistory}
                itemId={data[idField]}
                listName={listViewName}
                editable={editable}
                authenticated={authenticated}
            />
        });
        evt.preventDefault();
    };
    const deleteClickHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>, data: any) => {

        const url = Dyn.DataPool.allViews[detailViewName].deleteApi;
        const submitData = Dyn.DataPool.allViews[detailViewName].deleteApiParamsFunc(null, { itemId: data[idField] });
        Dyn.execApiAsync(url, submitData).then(() => {
            Dyn.DataStorage.update(listStorage, { dts: new Date().getTime() });
        });
        evt.preventDefault();
    };
    const HtmlTemplate = Dyn.DataPool.allControls['htmltemplate'];
    return (
        <>
            <div className="card">

                <div className="card-body">

                    <Dyn.View name={listViewName} id={id + '_item_' + listViewName + p.dataIndex} readonly={true} key={'cidItem_' + listViewName + p.dataIndex} dataSource={p.data} />

                </div>
                <div className="card-footer center">
                    <button type="button" className="btn btn-primary fas fa-eye" onClick={(evt) => { editClickHandler(evt, p.data) }}> View</button>
                    {editable ? <button type="button" className="btn btn-danger fas fa-trash" onClick={(evt) => { deleteClickHandler(evt, p.data) }}> Delete</button> : null}
                </div>
            </div>

        </>
    );
}
