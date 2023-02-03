import React, { ChangeEvent, useState } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';

export class SelectionList extends React.Component<{ title: string, itemName: string, idField: string, displayField: string, linkedItemId: string }> {
    public view = React.createRef<Dyn.View>();
    getValues() {
        return this.view.current.getData().selectedValues || null;
    }
    render() {
        const listName = this.props.itemName + '_ItemView';
        const options = {
            listName: listName,
            idField: this.props.idField,
            titleField: this.props.displayField,
            itemName: this.props.itemName,
            linkedItemId: this.props.linkedItemId
        };

        return (
            <>
                <div className="row">
                    <div className="col-sm-12">
                            <Dyn.View ref={this.view} name="sys_anyItem_sellistView" id={'cidList_' + listName} options={options} />
                    </div>
                </div>
            </>
        );
    }
};


export const SelectionItem = (p: { data: any, parent: Dyn.View, dataIndex: number }) => {
    const opt = p.parent.props.options;
    const listViewName = opt.listName;
    const idField = opt.idField;
    const onChangedHandler = (event: ChangeEvent<HTMLInputElement>) => {
        const val = p.parent.getData();
        if (!val.selectedValues) val.selectedValues = [];
        const vals = val.selectedValues as string[];
        if (event.target.checked) {
            vals.push(event.target.value);
        }
        else {
            vals.splice(vals.indexOf(event.target.value), 1);
        }
    }
    return (
        <>
            <div className="card">
                <div className="card-body">

                    <Dyn.View name={listViewName} readonly={true} id={'cidItem_' + listViewName + p.dataIndex} key={'cidItem_' + listViewName + p.dataIndex} dataSource={p.data} />

                </div>
                <div className="card-footer center">
                    <input type="checkbox" className="btn-check" autoComplete="off" value={p.data[idField]} onChange={(event) => onChangedHandler(event)} />
                </div>
            </div>

        </>
    );
}
