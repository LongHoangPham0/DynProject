import React, { useState, MouseEvent } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
import { Modal } from '../controls/Modal';
import { SelectionList } from './SelectionList';
import { TabRef } from '../controls/Tab';
export enum LinkedMode {
    AddNew = 0,
    Selection = 1,
    Extension = 2,
    Attachment = 3
}
export const SubItemList = (p: { 
    mode: LinkedMode, 
    itemName: string, 
    idField: string, 
    displayField: string, 
    linkedItemId: string, 
    showComment: boolean, 
    editable: boolean, 
    authenticated: boolean, 
    showHistory: boolean,
    tabRef: TabRef
}) => {
    const listName = p.itemName + '_ItemView';
    const storageName = 'storage_sub_' + listName + p.linkedItemId;
    const [, forceUpdate] = useState(null);
    Dyn.DataStorage.register(storageName, forceUpdate);

    const modal = React.createRef<Modal>();
    const options = {
        listName: listName,
        idField: p.idField,
        displayField: p.displayField,
        listStorageName: storageName,
        mode: p.mode,
        itemName: p.itemName,
        linkedItemId: p.linkedItemId,
        showComment: p.showComment,
        editable: p.editable,
        showHistory: p.showHistory,
        authenticated: p.authenticated,
        tabInstance: p.tabRef.instance
    };
    const newRelationClickHandler = () => {
        modal.current.open(p.mode === LinkedMode.AddNew ? '[New Item]' : 'Select Items');
    };
    const saveModalHandler = (evt: MouseEvent<HTMLButtonElement>, modal: Modal): boolean => {
        switch (p.mode) {
            case LinkedMode.AddNew:
                {
                    const view = modal.childRef.current;
                    const pm = view.submitData();
                    if (pm == null) return false;
                    else {
                        pm.then(() => {
                            Dyn.DataStorage.set(storageName, { dts: new Date().getTime() });
                        });
                    }
                }

                break;
            case LinkedMode.Selection:
                {
                    const comp = modal.childRef.current;

                    const url = Dyn.DataPool.allViews['sys_anyItem_sellistView'].submitApi;
                    const headers = new Headers();
                    headers.append('Accept', 'application/json');
                    headers.append('Content-Type', 'application/json');
                    const submitData = Dyn.DataPool.allViews['sys_anyItem_sellistView'].submitApiParamsFunc(comp.view.current, { selectedItems: comp.getValues() });
                    Dyn.execApiAsync(url, submitData).then(() => {
                        Dyn.DataStorage.set(storageName, { dts: new Date().getTime() });
                    });

                }
                break;
        }
        return true;
    }
    return (
        <>
            {p.editable ? <>
                <div className="row">
                    <div className="col-sm-12">

                        <button className="btn btn-outline-success fas fa-plus-circle" onClick={newRelationClickHandler}>{p.mode == LinkedMode.AddNew ? ' Add New Item' : ' Select Items'}</button>
                    </div>
                </div>
                <br />
            </>
                : null}

            <div className="row">
                <div className="col-sm-12">
                    <Dyn.View name="sys_anyItem_sublistView" id={'cidList_' + listName} key={'sl' + JSON.stringify(Dyn.DataStorage.get(storageName))} options={options} linkingObjects={{ modal }} />
                </div>
            </div>
            <Modal title="Field Detail" ref={modal} saveOpt={{ text: 'Save', actionHandler: saveModalHandler }} cancelOpt={{ text: 'Cancel', actionHandler: null }}>
                {p.mode == LinkedMode.AddNew ?
                    <Dyn.View name={p.itemName + '_DetailView'} dataSource={{}} options={{ linkedItemId: options.linkedItemId }} linkingObjects={{}} />
                    : <SelectionList itemName={p.itemName} linkedItemId={options.linkedItemId} title="My title" idField={p.idField} displayField={p.displayField} />
                }
            </Modal>
        </>
    );
};


