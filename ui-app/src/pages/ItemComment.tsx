import React, { useState } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';

export const ItemComment = (p: { itemId: string, authenticated: boolean }) => {
    const view = React.createRef<Dyn.View>();
    const [update, forceUpdate] = useState(Math.random());
    const submitHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        const pm = view.current.submitData();
        if (pm) {
            pm.then(() => {
                forceUpdate(Math.random());
            });
        }
    };
    return (<>
        <div className="row comment-panel">
            <Dyn.View ref={view} name={p.authenticated ? 'sys_item_comment' : 'sys_item_comment_view'} key={'ukey' + update} id={'cidCommentPanel_' + p.itemId} options={{ itemId: p.itemId }} />
        </div>
        {p.authenticated?
        <div className="center">
            <button type="button" className="btn btn-primary fas fa-save" onClick={(evt) => { submitHandler(evt); }}> Add comment</button>
        </div>
        :null}
        
    </>);
};