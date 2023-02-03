import React, { ChangeEvent, useState } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';

export const AttachmentList = (p: { itemId: any, editable: boolean }) => {
    const [, forceUpdate] = useState(null);
    const storageName = 'storageName_attachment' + p.itemId;
    Dyn.DataStorage.register(storageName, forceUpdate);
    const view = React.createRef<Dyn.View>();
    const handleSubmission = () => {
        if (!view.current.isValidData()) return;
        const val = view.current.getValues();
        const formData = new FormData();
        formData.append('itemId', p.itemId);
        formData.append('description', val.description);
        formData.append('file', val.file);
        Dyn.execApiAsync(view.current.submitApi, formData).then(() => Dyn.DataStorage.set(storageName, { dts: new Date().getTime() }));

    };

    return (<>
        {p.editable?<div className="row comment-panel">
            <div className="col-sm-12">
                <div className="form-group">
                    <Dyn.View id={'cidUploadAttachment' + p.itemId} ref={view} key={'detail_' + JSON.stringify(Dyn.DataStorage.get(storageName))} name="sys_attachment" options={{ itemId: p.itemId }} />
                </div>
                <div>
                    <button onClick={handleSubmission} className="btn btn-primary fas fa-upload"> Upload</button>
                </div>
            </div>
        </div>
        :null}
        
        <div className="row">
            <div className="col-sm-12">
                <Dyn.View id={'cidAttachmentList' + p.itemId} key={'list_' + JSON.stringify(Dyn.DataStorage.get(storageName))} name="sys_item_attachments" options={{ itemId: p.itemId, storageName, editable: p.editable }} />
            </div>
        </div>
    </>
    )
}
window.utilities.deleteAttachment = (fieldId:any, storageName: string) => {
    const viewDef = Dyn.DataPool.allViews['sys_attachment'];
    const url = viewDef.deleteApi;
    let postData = { fileId: fieldId };
    if(viewDef.deleteApiParamsFunc) postData = viewDef.deleteApiParamsFunc(null, postData);
    Dyn.execApiAsync(url, postData).then(() => {
        Dyn.DataStorage.set(storageName, { dts: new Date().getTime() });
    });
};
export const AttachmentDetail = (p: { data: any, parent: Dyn.View, dataListInstance: Dyn.BaseComponent }) => {
    if (!p.data) return null;
    let tpl = p.dataListInstance.props.options.htmlItem as string;
    if(!p.parent.props.options.editable){
        tpl = tpl.replace(/<button .*?<\/button>/ig,'');
    }
    
    const Template = Dyn.DataPool.allControls["htmltemplate"];
    p.data.storageName = p.parent.props.options.storageName;
    return (<>
        <Template options={{html: tpl}} value={p.data} />
        
    </>);
};