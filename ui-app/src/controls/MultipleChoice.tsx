import React, { MouseEvent } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
export default class MultipleChoice extends Dyn.BaseComponent {
    private _selectedContainer = React.createRef<HTMLSelectElement>();
    private _sourceContainer = React.createRef<HTMLSelectElement>();
    private _txtField = this.props.options?.textField || 'text';
    private _valField = this.props.options?.valueField || 'valueId';
    private onRemoveSelectedItem(e: MouseEvent<HTMLButtonElement>): void {
        let data = this.getValue() as any[];
        const ctrl = this._selectedContainer.current;
        for (let i = 0; i < ctrl.options.length; i++) {
            if (ctrl.options[i].selected) {
                data[i] = null;
            }
        }
        data = data.filter(n => n);
        this.setValue(data);

        e.preventDefault();
    }
    private onSelectItem(e: MouseEvent<HTMLButtonElement>): void {
        let data = this.getValue() as any[];
        const ctrl = this._sourceContainer.current;
        for (let i = 0; i < ctrl.options.length; i++) {
            if (ctrl.options[i].selected) {
                data.push({});
                data[data.length - 1][this._valField] = ctrl.options[i].value;
                data[data.length - 1][this._txtField] = ctrl.options[i].text;
            }
        }
        
        this.setValue(data);
        e.preventDefault();
    }

    protected renderComponent(): React.ReactNode {
        const selectedItems = [], sourceItems = [];


        let data = this.getValue() as any[];
        if (data != null) {
            if (this.state.readonly) {
                for (let i = 0; i < data.length; i++) {
                    if (this.getValue() == data[i][this._valField]) {
                        selectedItems.push(<span key={this.props.id + 'val' + i}>{data[i][this._txtField]}</span>);
                        break;
                    }
                }
            }
            else for (let i = 0; i < data.length; i++) {
                if (i === 0) this.defaultValue = data[i][this._valField];
                selectedItems.push(<option key={this.props.id + 'val' + i} value={data[i][this._valField]}>{data[i][this._txtField]}</option>);
            }
        }

        const ds = this.getDataSource() as any;
        if (ds != null) {
            data = ds.items.data;
            for (let i = 0; i < data.length; i++) {
                if (i === 0) this.defaultValue = data[i][this._valField];
                sourceItems.push(<option key={this.props.id + 'src' + i} value={data[i][this._valField]}>{data[i][this._txtField]}</option>);
            }
        }

        return <span{...this.props.options?.htmlProps}>

            {this.props.label ? (<><label>{this.props.label}</label><br /></>) : null}
            <span>
                {this.state.enable && !this.state.readonly ? <>
                    <select ref={this._sourceContainer} multiple>
                        {sourceItems}
                    </select>
                </> : null}
                <span>
                <button onClick={(event) => this.onSelectItem(event)} type={'button'}>Sel</button>
                <button onClick={(event) => this.onRemoveSelectedItem(event)} type={'button'}>Del</button>
                </span>
                {this.state.readonly ? <>{selectedItems[0]}</> :
                    <select ref={this._selectedContainer} multiple>
                        {selectedItems}
                    </select>
                }
            </span>
        </span>

    }

}