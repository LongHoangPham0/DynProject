import React from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
import { Tab } from '../controls/Tab';
import { ItemDetail } from './ItemDetail';
import { LinkedMode } from './SubItemList';

export const SubItem = (p: { data: any, parent: Dyn.View, dataIndex: number }) => {
    const opt = p.parent.props.options;
    const listViewName = opt.listName;
    const idField = opt.idField;
    const displayField = opt.displayField;
    const mode = opt.mode;
    const itemName = opt.itemName;
    const listStorage = opt.listStorageName;
    const showComment = opt.showComment as boolean;
    const showHistory = opt.showHistory as boolean;
    const editable = opt.editable as boolean;
    const authenticated = opt.authenticated as boolean;
    const tabInstance = opt.tabInstance as Tab;
    const viewClickHandler = (evt: React.MouseEvent<HTMLButtonElement>, data: any) => {
        //if (mode == LinkedMode.AddNew) modal.current.open(null, null, <Dyn.View name={itemName + '_DetailView'} options={{ itemId: data[idField] }} />);
        //navigate to view or edit item
        const detailViewName = itemName + '_DetailView';
        //const listStorage = 'storage_' + itemName + '_ItemView';
        const tabId = 'itemdetail' + detailViewName + data[idField];
        tabInstance.addTab(tabId, {
            closable: true,
            title: data[displayField],
            content: <ItemDetail 
            displayField={displayField} 
            detailName={detailViewName} 
            listStorage={listStorage} 
            tabId={tabId} 
            itemId={data[idField]} 
            showComment={showComment} 
            editable={mode == LinkedMode.AddNew} 
            showHistory={showHistory}
            listName = {listViewName}
            authenticated={authenticated}  />
        });
        evt.preventDefault();
    };
    const deleteClickHandler = (evt: React.MouseEvent<HTMLButtonElement>, data: any) => {

        data = { parentID: opt.linkedItemId, itemId: data[idField] };
        const url = Dyn.DataPool.allViews['sys_anyItem_sellistView'].deleteApi;
        const headers = new Headers();
        headers.append('Accept', 'application/json');
        headers.append('Content-Type', 'application/json');
        const submitData = Dyn.DataPool.allViews['sys_anyItem_sellistView'].deleteApiParamsFunc(null, data);
        Dyn.execApiAsync(url, submitData).then(() => {
            Dyn.DataStorage.set(listStorage, { dts: new Date().getTime() });
        });

        evt.preventDefault();
    };
    return (
        <>
            <div className="card">
                <div className="card-body">
                    <Dyn.View name={listViewName} readonly={true} id={'cidItem_' + listViewName + p.dataIndex} key={'cidItem_' + listViewName + p.dataIndex} dataSource={p.data} />
                </div>
                <div className="card-footer center">
                    {editable ?<>
                        <button type="button" className="btn btn-primary fas fa-eye" onClick={(evt) => { viewClickHandler(evt, p.data) }}> View</button>
                        <button type="button" className={'btn btn-danger fas ' + (mode == LinkedMode.AddNew ? 'fa-trash' : 'fa-prescription')} onClick={(evt) => { deleteClickHandler(evt, p.data) }}> {mode == LinkedMode.AddNew ? 'Delete' : 'Remove'}</button>
                        </>
                        : null}
                </div>
            </div>

        </>
    );
}
