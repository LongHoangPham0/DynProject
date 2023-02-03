import React, { ChangeEvent } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
import { CheckAny } from './CheckAny';

export class CheckTextBox extends CheckAny {
    constructor(p: Dyn.IBaseComponentProps) {
        super('textbox', p);
    }
}
export class CheckDblTextBox extends CheckAny {
    constructor(p: Dyn.IBaseComponentProps) {
        super('dbltextbox', p);
    }
}
export class DblTextBox extends Dyn.BaseComponent {
    private textbox1 = React.createRef<Dyn.BaseComponent>();
    private textbox2 = React.createRef<Dyn.BaseComponent>();
    public getValue(): any {
        return [this.textbox1.current.getValue(), this.textbox2.current?this.textbox2.current.getValue():null, this.props.options.optAtt];
    }
    public setEnable(enable: boolean){
        this.textbox1.current.setEnable(enable);
        this.textbox2.current.setEnable(enable);
    }

    protected renderComponent(): React.ReactNode {
        const TextBox = Dyn.DataPool.allControls['textbox'];
        const val = this.state.value || ['',''];
        return <span className="dbl-text">
            <TextBox ref={this.textbox1} key={this.props.id + 'txt1' + val[0]} enable={this.state.enable} id={this.props.id + 'txt1'} value={val[0]}/>
            <TextBox ref={this.textbox2} key={this.props.id + 'txt2' + val[1]} enable={this.state.enable} id={this.props.id + 'txt2'} value={val[1]}/>
            
        </span>;
    }
}