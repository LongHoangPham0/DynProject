import React, { ChangeEvent } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
export class ComboTextBox extends Dyn.BaseComponent {
    private dropdown = React.createRef<Dyn.BaseComponent>();
    private textbox = React.createRef<Dyn.BaseComponent>();
    public getValue(): any {
        return {operator: this.dropdown.current.getValue(), value: this.textbox.current.getValue()};
    }

    protected renderComponent(): React.ReactNode {
        const DropdownList = Dyn.DataPool.allControls['dropdownlist'];
        const TextBox = Dyn.DataPool.allControls['textbox'];
        const val = this.state.value || { operator: '=', value: '' };
        return <span>
            <DropdownList ref={this.dropdown} key={this.props.id + 'cb' + val.operator} id={this.props.id + 'cb'} dataSource={this.state.dataSource} value={val.operator} />
            <TextBox ref={this.textbox} key={this.props.id + 'txt' + val.value} id={this.props.id + 'txt'} value={val.value}/>
        </span>;
    }
}