import React, { useState } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
export const ItemHistory = (p: { itemId: string }) => {
    const storageName = 'storage_History_' + p.itemId;
    const [, forceUpdate] = useState(null);
    Dyn.DataStorage.register(storageName, forceUpdate);
    return (<>
        <div className="row comment-panel">
            <Dyn.View name="sys_item_history" key={'cidHistoryPanel_' + JSON.stringify(Dyn.DataStorage.get(storageName))} id={'cidHistoryPanel_' + p.itemId} options={{ itemId: p.itemId }} />
        </div>
    </>);
};
export const HistoryDetail = (p: { data: any, dataListInstance: Dyn.BaseComponent, dataIndex: number }) => {
    if (!p.data) return null;
    const itemId = p.dataListInstance.getView().props.options.itemId;
    const children = <Dyn.View id={'cidHisChange_' + itemId + '_' + p.dataIndex} name="sys_item_changes" dataSource={p.data} />;
    const tpl = p.dataListInstance.props.options.htmlItem;
    const HtmlTemplate = Dyn.DataPool.allControls['htmltemplate'];
    return (
        <>
            <HtmlTemplate id={'cidHis_' + itemId + '_' + p.dataIndex} options={{ html: tpl }} value={p.data} >{children}</HtmlTemplate>


        </>
    );
};