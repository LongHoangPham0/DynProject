import React, { useEffect, useState } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
import { Tab, TabRef } from '../controls/Tab';
import { LinkedMode, SubItemList } from './SubItemList';
import { ExtSubItem } from './ExtSubItem';
import { ItemComment } from './ItemComment';
import { ItemHistory } from './ItemHistory';
import { AttachmentList } from './AttachmentList';


export const ItemDetail = (p: {
    authenticated: boolean,
    displayField: string,
    listStorage: string,
    detailName: string,
    tabId: string,
    itemId: any,
    listName: string,
    showComment: boolean,
    editable: boolean,
    showHistory: boolean,
    tabRef: TabRef
}) => {

    
    const itemDetailView = React.createRef<Dyn.View>();
    const editButton = React.createRef<HTMLButtonElement>();

    p.tabRef.instance.closedTab[p.tabId] = () => {
        if (p.showHistory) Dyn.DataStorage.delete('storage_History_' + p.itemId);
    };
    const [readonly, setReadonly] = useState(true);
    const submitHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {

        if (!readonly) {
            //submit final
            const pm = itemDetailView.current.submitData();
            const vv = itemDetailView.current;//keep current view to be used in pm promise
            if (pm == null) return;
            else {
                pm.then(() => {
                    vv.rebind();
                    Dyn.DataStorage.update(p.listStorage, { dts: new Date().getTime() });
                    if (p.showHistory) Dyn.DataStorage.update('storage_History_' + p.itemId, { dts: new Date().getTime() });
                });
            }
            
        }


        itemDetailView.current.setReadonly(!readonly);
        
        setReadonly(!readonly);

    };
    const updateTabTitle = (sender: Dyn.View)=>{
        const val = sender.getValues();
        p.tabRef.instance.updateHeadTitle(p.tabId, val[p.displayField]);
    };
    const cancelHandler = (_: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        if (!readonly) {
            itemDetailView.current.bindData(itemDetailView.current.getData());
        }
        itemDetailView.current.setReadonly(!readonly);
        setReadonly(!readonly);
    };
    const tabs = {} as Dyn.TabDS;
    if (p.showComment) {
        tabs['commentTab'] = {
            title: "Comments",
            content: <ItemComment itemId={p.itemId} authenticated={p.authenticated} />
        }
    }

    if (p.showHistory) {
        tabs['historyTab'] = {
            title: "Histories",
            content: <ItemHistory itemId={p.itemId} />
        }
    }
    return (

        <>

            <Dyn.View name="sys_item_approval_loader" options={{ itemId: p.itemId, tabId: p.tabId, editButton, storageName: p.listStorage, listName: p.listName, tabInstance: p.tabRef.instance }} />

            <div className="row">
                <div className={p.showComment || p.showHistory ? 'col-sm-6' : 'col-sm-12'}>
                    <div className="row">
                        <div className="col-sm-12"><Dyn.View ref={itemDetailView} readonly={readonly} name={p.detailName} id={'cidDetail_' + p.detailName} options={{ itemId: p.itemId }} onDidUpdate={updateTabTitle} /></div>
                    </div>

                    <div className="row">
                        <div className="col-sm-12">
                            {p.editable ?
                                <button type="button" ref={editButton} className={'btn btn-primary ' + (readonly ? 'fas fa-pen' : 'fas fa-save')} onClick={(evt) => { submitHandler(evt); }}> {readonly ? 'Edit' : 'Save'}</button>
                                : null}
                            {readonly ? null :
                                <button type="button" className={'btn btn-secondary fas fa-undo'} onClick={(evt) => { cancelHandler(evt); }}> Cancel</button>}

                        </div>
                    </div>

                </div>
                <div className="col-sm-6">
                    <Tab dataSource={tabs} />
                </div>
            </div>
            <br />
            <div className="row">
                <div className="col-sm-12">
                    <Dyn.View name="sys_anyItem_linkedView" id={'cidLinkedItems_' + p.detailName} options={{ itemId: p.itemId, editable: p.editable, authenticated: p.authenticated, tabInstance: p.tabRef.instance }} />
                </div>
            </div>
        </>
    );
};
ItemDetail.defaultProps = {
    tabRef: { instance: null } as TabRef
};
export class TabExt extends Dyn.BaseComponent {
    public setValue(ds: any, isDefault?: boolean): void {
        //const ds = val;
        let activeTab = null;
        const opt = this.getView().props.options;
        let editable = null;
        const authenticated = opt.authenticated;
        const linkedItemId = opt.itemId;
        const tabInstance = opt.tabInstance as Tab;
        for (let name in ds) {
            editable = opt.editable;
            if(ds[name].editable!=undefined) editable = ds[name].editable;
            switch (ds[name].linkedMode as LinkedMode) {
                case LinkedMode.Attachment:
                    ds[name].content = <AttachmentList itemId={ds[name].itemId} editable={editable} />
                    break;
                case LinkedMode.Extension:
                    ds[name].content = <ExtSubItem itemName={ds[name].itemName} itemId={ds[name].itemId} editable={editable} />
                    break;

                default:
                    ds[name].content = <SubItemList authenticated={authenticated} showComment={ds[name].showComment} showHistory={ds[name].showHistory} itemName={ds[name].itemName} linkedItemId={linkedItemId} editable={editable} idField={ds[name].idField} displayField={ds[name].displayField} mode={ds[name].linkedMode} tabRef={{ instance: tabInstance }} />
                    break;

            }

            if (!activeTab) activeTab = name;
        }
        this.tab.current.setState({ dataSource: ds, activeTab });
    }
    private tab = React.createRef<Tab>();
    protected renderComponent(): React.ReactNode {

        return (

            <Tab ref={this.tab} dataSource={null} />
        );
    }
}
export class ItemApproval extends Dyn.BaseComponent {
    approvalView = React.createRef<Dyn.View>();
    viewOptions = this.getView().props.options;
    tabInstance = this.viewOptions.tabInstance as Tab;

    private approveHandler(evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) {
        const pm = this.approvalView.current.submitData();
        if (pm == null) return;
        pm.then(() => {
            const storageName = 'storage_' + this.viewOptions.listName;
            const now = { dts: new Date().getTime() };
            Dyn.DataStorage.update(storageName + 'sys_anyItem_listView', now);
            Dyn.DataStorage.update(storageName + 'sys_anyItem_approvalList', now);
            Dyn.DataStorage.update(storageName + 'sys_anyItem_submittedList', now);
            this.tabInstance.closeTab(this.viewOptions.tabId);
        });

    }
    private declineHandler(evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) {
        if (!this.approvalView.current.isValidData()) return;
        const pm = this.approvalView.current.deleteData();
        if (pm == null) return;
        pm.then(() => {
            const storageName = 'storage_' + this.viewOptions.listName;
            const now = { dts: new Date().getTime() };
            Dyn.DataStorage.update(storageName + 'sys_anyItem_approvalList', now);
            Dyn.DataStorage.update(storageName + 'sys_anyItem_submittedList', now);
            Dyn.DataStorage.update(storageName + 'sys_anyItem_rejectedList', now);
            this.tabInstance.closeTab(this.viewOptions.tabId);
        });

    }
    protected renderComponent(): React.ReactNode {

        const itemId = this.getView().props.options.itemId;
        const val = this.getValue() || { show: false };
        const histories = [];
        let cnt = 0;
        if (val.histories) {
            for (const h of val.histories) {
                histories.push(<div key={'appHis_' + (++cnt)}>
                    <div>On {h.time}, {h.approver}</div>
                    <li>...set state: <strong>{h.state}</strong> </li>
                    <li>...put comment: <i>{h.comment}</i></li>
                </div>);
            }
            const bt = this.viewOptions.editButton.current;
            if (bt) bt.style.display = 'none';
        }
        if (val.show) return <>
            <div className="row">
                <div className={val.histories ? 'col-sm-6' : 'col-sm-12'}>

                    {val.submittable ? <>
                        <div className="comment-panel">
                            <Dyn.View ref={this.approvalView} name="sys_item_approval" id={'cidApprovalPanel_' + itemId} options={{ itemId: itemId }} />
                        </div>
                        <div>
                            {val.submitter ? <>
                                <button type="button" className="btn btn-success fas fa-check-circle" onClick={(evt) => { this.approveHandler(evt); }}> Submit</button>
                            </>
                                : <>
                                    <button type="button" className="btn btn-success fas fa-check-circle" onClick={(evt) => { this.approveHandler(evt); }}> Approve</button>
                                    <button type="button" className="btn btn-danger fas fa-ban" onClick={(evt) => { this.declineHandler(evt); }}> Decline</button>
                                </>}

                        </div>
                    </> : null}
                </div>
                {val.histories ? <>
                    <div className={val.submittable ? 'col-sm-6' : 'col-sm-12'}>
                        <h5>Approval history</h5>
                        <Dyn.View id={'cidApprovalHis_' + itemId} name="sys_item_approval_history" dataSource={val} />
                    </div>
                </> : null}

            </div>
            <br />
        </>
        else return null;
    }

}