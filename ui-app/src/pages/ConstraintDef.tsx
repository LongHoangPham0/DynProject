import React from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
import { Modal } from '../controls/Modal';


export const ConstraintDef = (p: { modelproperty: string, innerData: string, type: number }) => {

    const modal = React.createRef<Modal>();
    const valueContainer = React.createRef<HTMLInputElement>();
    const view = React.createRef<Dyn.View>();
    const [uiValue, setUIValue] = React.useState({});
    const [value, setValue] = React.useState(p.innerData);
    const openModalHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        const data = window.utilities.parseJSON(value);
        const ds = { conditions: data };
        setUIValue(ds);
        modal.current.open();
        evt.preventDefault();
    };
    const buildValue = () => {
        const val = view.current.getValues();
        const result = { uiVal: val, jsVal: null as any };


        let js = JSON.stringify(val.conditions);

        js = js.replace(/"(\w+?)":/ig, '$1:');//replace attributes

        //replace for values
        js = js.replace(/(".*?")/ig, v => {
            let vv: number = v.replace(/"/ig, '') as any;
            if (v === '""' || isNaN(vv)) {
                return v.replace(/"/ig, '\'');
            }
            else {
                return vv + '';
            }
        });

        //js = js.replace(/"/ig, '\'');

        result.jsVal = js;
        return result;
    }
    const doneHandler = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>): boolean => {
        if(!view.current.isValidData()) return false;
        const val = buildValue();
        setUIValue(val.uiVal);

        setValue(val.jsVal);
        return true;
    }

    return (
        <>
            <div>
                <button type="button" className="btn btn-secondary" onClick={(evt) => { openModalHandler(evt); }}>Constraints</button>
                <input ref={valueContainer} type="hidden" name={p.modelproperty} value={value} />
            </div>
            <Modal title="Constraint list" ref={modal} saveOpt={{ text: 'Done', actionHandler: doneHandler }} cancelOpt={{ text: 'Cancel', actionHandler: null }}>{
                <div className="constraint-panel">
                <Dyn.View ref={view} id={'cidConstraintDef'} name="sys_constraintActionDef" dataSource={uiValue} options={{ typeId: p.type }} />
                </div>
            }</Modal>
        </>
    );
};


export class FieldConditions extends Dyn.BaseComponent {
    private views = [] as React.RefObject<Dyn.View>[];
    public isValid(): boolean{
        console.log('check me!!');
        for (let i = 0; i < this.views.length; i++){ 
            if(this.views[i] && !this.views[i].current.isValidData()) return false;
        }
        return true;
    }
    public getValue(): any {
        
        const vals = [];
        for (let i = 0; i < this.views.length; i++) {
            
            if(this.views[i]){
                vals[i] = this.views[i].current.getValues();
                let ds = vals[i].actions.dataSource;
                if (ds && typeof ds === 'string') {
                    vals[i].actions.dataSource = JSON.parse(ds);
                }
            }
            
        }

        return vals;
    }
    protected renderComponent(): React.ReactNode {
        const ds = this.state.value as any[];
        if (ds) {
            this.views = [];
            const constraints = [];
            for (let i = 0; i < ds.length; i++) {
                if(ds[i]==null) continue;
                this.views[i] = React.createRef<Dyn.View>();
                constraints.push(<Dyn.View ref={this.views[i]} key={'fieldConditions_' + i} id={'cidFieldConditions_' + i} name="fieldConstraints" dataSource={ds[i]} linkingObjects={{ parent: this.props.parent }} options={{ dataIndex: i }} />);
            }
            return (<>
                {constraints}
            </>);
        }
        else return null

    }
}
export class FieldConditionAddButton extends Dyn.BaseComponent {
    private buttonClickHandler = () => {
        const view = this.getView();
        const val = view.getValues();
        if (val.conditions == null) val.conditions = [];
        const con = val.conditions;
        con.push({
            match: { operator: '==', value: '' },
            fields: [],
            actions: null
        });
        view.bindData(val);
    };
    protected renderComponent(): React.ReactNode {
        const Button = Dyn.DataPool.allControls["button"];
        return <>
            <Button id={this.props.id + 'submit'} options={{ htmlProps: { className: 'btn btn-outline-success fas fa-plus-circle' } }} valueChangedFunc={() => { this.buttonClickHandler(); }} />
        </>
    }
}
export class FieldConditionRemoveButton extends Dyn.BaseComponent {
    private buttonClickHandler = () => {
        const parentView = this.getView();
        const view = parentView.props.linkingObjects.parent as Dyn.View;
        const val = view.getValues();
        delete val.conditions[parentView.props.options.dataIndex]; //must use delete, splice won't work
        view.bindData(val);

    };
    protected renderComponent(): React.ReactNode {
        const Button = Dyn.DataPool.allControls["button"];
        return <>
            <Button id={this.props.id + 'remove'} options={{ htmlProps: { className: 'btn btn-outline-danger fas fa-times' } }} valueChangedFunc={() => { this.buttonClickHandler(); }} />
        </>
    }
}