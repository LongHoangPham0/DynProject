import React, { useEffect } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
import { Modal } from '../controls/Modal';

export const ValidationDef = (p: { type: string, modelproperty: string, innerData: string, dependant: string }) => {
    const dependantEl = document.getElementById(p.dependant) as HTMLInputElement;
    

    const modal = React.createRef<Modal>();
    const valueContainer = React.createRef<HTMLInputElement>();
    const view = React.createRef<Dyn.View>();
    const [uiValue, setUIValue] = React.useState({});
    const [value, setValue] = React.useState(p.innerData);
    
    const openModalHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        const dataType = dependantEl.value;        
        const data = window.utilities.parseJSON(value);
        const ds = { dataType, rules: {} };
        for (var r of data) {
            if (r.options) {
                let attName = '';
                for (let o in r.options) attName = o;
                ds.rules[r.rule] = [r.msg, r.options[attName]];
            }
            else {
                ds.rules[r.rule] = [r.msg];
            }
        }
        setUIValue(ds);
        modal.current.open();
        evt.preventDefault();
    };
    const doneHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>): boolean => {
        const val = view.current.getValues();
        setUIValue(val);
        const dss = [];
        for (let n in val.rules) {

            if (val.rules[n] != null) {
                let opt = null;
                let msg = '';
                if(typeof val.rules[n] == 'string'){
                    msg = val.rules[n];
                }
                else{
                    msg = val.rules[n][0];   
                    if (val.rules[n][2]) {
                    
                        opt = {};
                        let optVal = val.rules[n][1];
                        if(isNaN(optVal)){
                            opt[val.rules[n][2]] = optVal;
                        }
                        else{
                            opt[val.rules[n][2]] = parseFloat(optVal);
                        }
                    }
                } 
                dss.push({
                    rule: n,
                    msg: msg,
                    options: opt
                });
            }
        }
        let js = JSON.stringify(dss);
        js = js.replace(/"(\w+?)":/ig,'$1:').replace(/"/ig, '\'');
        
        setValue(js);
        return true;
    }
    
    return (
        <>
            <div>
                <button type="button" className="btn btn-secondary" onClick={(evt) => { openModalHandler(evt); }}>Select validation rules</button>
                <input ref={valueContainer} type="hidden" name={p.modelproperty} value={value}/>
            </div>
            
            <Modal title="Validation rules" ref={modal} saveOpt={{ text: 'Done', actionHandler: doneHandler }} cancelOpt={{ text: 'Cancel', actionHandler: null }}>{
                <Dyn.View ref={view} id={'cidValidationDef'} name="sys_validationDef" dataSource={uiValue} />    
            }</Modal>
        </>
    );
};
