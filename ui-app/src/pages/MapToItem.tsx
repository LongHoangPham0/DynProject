import React, { ChangeEvent, useEffect } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
import { Modal } from '../controls/Modal';
const SAPERATOR = '|';
export const MapToItem = (p: { type: string, modelproperty: string, innerData: string }) => {
    const modal = React.createRef<Modal>();
    const valueContainer = React.createRef<HTMLInputElement>();
    const view = React.createRef<Dyn.View>();
    const [value, setValue] = React.useState(p.innerData.length > 0 ? p.innerData.split(',') : []);

    const openModalHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        modal.current.open();
        evt.preventDefault();
    };
    const doneHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>): boolean => {
        let data = view.current.getData();
        if (data && data.selectedItems) setValue(value.concat(data.selectedItems));
        return true;
    }
    const removeItem = (k: string) => {
        let index = value.indexOf(k);
        console.log(k, index);
        if (index >= 0) {
            value.splice(index, 1);
            setValue(value.concat());
        }
    };
    const ds = [];

    for (const r of value) {
        const rr = r.split(SAPERATOR);
        ds.push(<div key={'items' + rr[0]}><button type="button" className="btn btn-danger fas fa-trash" onClick={e => { removeItem(r); }}></button><span>{rr[1]}</span></div>);//push object and assign value later
    }
    return (
        <>
            <div>
                <button type="button" className="btn btn-secondary" onClick={(evt) => { openModalHandler(evt); }}>Add more...</button>
                <input ref={valueContainer} type="hidden" name={p.modelproperty} value={value} />
            </div>
            {ds}
            <Modal title="Select items to bind" ref={modal} saveOpt={{ text: 'Done', actionHandler: doneHandler }} cancelOpt={{ text: 'Cancel', actionHandler: null }}>{
                <Dyn.View ref={view} name="sys_anyItem_selectionView" />
            }</Modal>
        </>
    );
};
export const MapItem = (p: { data: any, parent: Dyn.View }) => {
    const container = p.parent.find('sys_anyItem_list', true);
    const idField = container.props.options.valueField;
    const txtField = container.props.options.textField;
    const onChangedHandler = (e: ChangeEvent<HTMLInputElement>) => {
        const el = e.target;
        const val = p.parent.getData();
        if (!val.selectedItems) val.selectedItems = [];
        const vals = val.selectedItems as string[];
        const selVal = el.value + SAPERATOR + el.dataset.text;
        if (el.checked) {
            vals.push(selVal);
        }
        else {
            vals.splice(vals.indexOf(selVal), 1);
        }
    }
    return (
        <>
            <div className="card">
                <div className="card-body">
                    {p.data[txtField]}
                </div>
                <div className="card-footer center">
                    <input type="checkbox" className="btn-check" autoComplete="off" value={p.data[idField]} data-text={p.data[txtField]} onChange={(event) => onChangedHandler(event)} />
                </div>
            </div>

        </>
    );
}