import React, { ChangeEvent } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';


export class CheckAny extends Dyn.BaseComponent {
    private checkbox = React.createRef<Dyn.BaseComponent>();
    private input = React.createRef<Dyn.BaseComponent>();
    private controlType  = null as string;
    constructor(controlType: string, p: Dyn.IBaseComponentProps) {
        super(p);
        this.controlType = controlType;
    }
    public getValue(): any {
        if (this.checkbox.current?.getValue()) {
            return this.input.current.getValue();
        }
        else {
            return null;
        }
    }
    private checkBoxChanged(sender: Dyn.BaseComponent) {
        const checked = sender.getValue();
        this.input.current.setEnable(checked);
        if (!checked) this.input.current.setValue(null);
        else if (this.input.current.getValue() == null) {
            if (this.state.dataSource) this.input.current.setValue(this.state.dataSource[0].value);
            else this.input.current.setValue(null);
        }
    }
    protected renderComponent(): React.ReactNode {
        const CheckBox = Dyn.DataPool.allControls['checkbox'];
        const Input = Dyn.DataPool.allControls[this.controlType];
        const val = this.state.value;//do NOT change to this.getValues() for getValues is overridden
        const enable = this.state.enable;
        const checked = val != null;
        return <span>
            <CheckBox ref={this.checkbox} key={this.props.id + 'chk' + enable + checked} id={this.props.id + 'chk'} enable={enable} value={checked} valueChangedFunc={(s: Dyn.BaseComponent) => { this.checkBoxChanged(s) }} label={this.props.label} />
            <Input ref={this.input} key={this.props.id + this.controlType + enable + val} id={this.props.id + this.controlType} enable={enable && checked} value={val} dataSource={this.state.dataSource} options={this.props.options} />
        </span>;
    }
}
