import React, { ChangeEvent } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
import { CheckAny } from './CheckAny';
export class CheckRadio extends CheckAny{
    constructor(p:Dyn.IBaseComponentProps){
        super('radiolist', p);
    }
}
export class CheckRadio1 extends Dyn.BaseComponent {
    private checkbox = React.createRef<Dyn.BaseComponent>();
    private radiolist = React.createRef<Dyn.BaseComponent>();
    public getValue(): any {

        if (this.checkbox.current.getValue()) {
            return this.radiolist.current.getValue();
        }
        else {
            return null;
        }
    }
    private checkBoxChanged(sender: Dyn.BaseComponent) {
        const checked = sender.getValue();
        this.radiolist.current.setEnable(checked);
        if (!checked) this.radiolist.current.setValue(null);
        else if (this.radiolist.current.getValue() == null) this.radiolist.current.setValue(this.state.dataSource[0].value);
    }
    protected renderComponent(): React.ReactNode {
        const CheckBox = Dyn.DataPool.allControls['checkbox'];
        const RadioList = Dyn.DataPool.allControls['radiolist'];
        const val = this.state.value;
        return <span>
            <CheckBox ref={this.checkbox} key={this.props.id + 'chk' + val} id={this.props.id + 'chk'} value={val != null} valueChangedFunc={(s: Dyn.BaseComponent) => { this.checkBoxChanged(s) }} label={this.props.label} />
            <RadioList ref={this.radiolist} key={this.props.id + 'radio' + val} id={this.props.id + 'radio'} enable={val != null} dataSource={this.state.dataSource} value={val} />
        </span>;
    }
}