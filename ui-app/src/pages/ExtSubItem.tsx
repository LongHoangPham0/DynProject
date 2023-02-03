import React, { useState } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';

export const ExtSubItem = (p: { itemName: string, itemId: string, editable: boolean }) => {
    const view = React.createRef<Dyn.View>();
    const [readonly, setReadonly] = useState(true);
    const saveClickHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        if (!readonly) {
            const pm = view.current.submitData();
            if (pm == null) return;
        }
        view.current.setReadonly(!readonly);
        setReadonly(!readonly);
    };
    const cancelClickHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        view.current.bindData(view.current.getData());
        view.current.setReadonly(!readonly);
        setReadonly(!readonly);
    }
    return (
        <>
            <div className="row">
                <div className="col-sm-12">

                    <Dyn.View ref={view} readonly={readonly} name={p.itemName + '_DetailView'} options={{ itemId: p.itemId }} />
                    {p.editable ?
                        <div>
                            <button type="button" className={'btn btn-primary ' + (readonly ? 'fas fa-pen' : 'fas fa-save')} onClick={(evt) => { saveClickHandler(evt) }}> {readonly ? 'Edit' : 'Save'}</button>
                            {readonly ? null :
                                <button type="button" className="btn btn-secondary fas fa-undo" onClick={(evt) => { cancelClickHandler(evt) }}> Cancel</button>
                            }
                        </div>
                        : null}

                </div>
            </div>
        </>
    );
};
