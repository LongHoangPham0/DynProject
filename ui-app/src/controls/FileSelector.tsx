import React, { ChangeEvent, ReactNode } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
export class FileSelector extends Dyn.BaseComponent {
    private onChangedHandler(event: ChangeEvent<HTMLInputElement>) {
        this.setValue(event.target.files[0]);
    }

    protected renderComponent(): ReactNode {
        return <span>
            {this.props.label ? (<label htmlFor={this.props.id}>{this.props.label}</label>) : null}
            {this.state.readonly ? <span>{this.getValue() || ''}</span>
                : <input type="file" id={this.props.id} disabled={!this.state.enable} onChange={(event) => this.onChangedHandler(event)} />
            }
        </span>
    }
}