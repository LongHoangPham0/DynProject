import React, { useState } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';

export class SortingList extends Dyn.BaseComponent {

    protected renderComponent(): React.ReactNode {
        
        const parentView = this.getView() as Dyn.View;
        const sortingList = parentView.props.linkingObjects.sortingList as any[];
        sortingList[this.props.dataIndex] = React.createRef<Dyn.View>();
        return <>
            <div key={'cidSortingQuery' + this.props.dataIndex} className="list-group-item">
                <Dyn.View name="sys_sorting" readonly={parentView.props.readonly} ref={sortingList[this.props.dataIndex]} id={'cidSortingQuery' + this.props.dataIndex} dataSource={this.props.data} linkingObjects={{ view: parentView }} options={{ dataIndex: this.props.dataIndex }} />
            </div>
        </>;
    }

}

export class SortingAddButton extends Dyn.BaseComponent {
    private buttonClickHandler = () => {
        const view = this.getView();
        const val = view.props.linkingObjects.buildValue();
        const sorts = val.sorts as any;
        sorts.push({direction: 'asc'});
        view.bindData(val);
    };
    protected renderComponent(): React.ReactNode {
        const Button = Dyn.DataPool.allControls["button"];
        return <>
            <Button id={this.props.id + 'submit'} options={{ htmlProps: { className: 'btn btn-success fas fa-plus-circle' } }} valueChangedFunc={() => { this.buttonClickHandler(); }} />
        </>
    }
}
export class SortingRemoveButton extends Dyn.BaseComponent {
    private buttonClickHandler = () => {
        const parentView = this.getView();
        const view = parentView.props.linkingObjects.view as Dyn.View;
        //console.log(view);
        const val = view.props.linkingObjects.buildValue();
        const quer = val.sorts as any[];
        quer.splice(parentView.props.options.dataIndex, 1);
        view.bindData(val);

    };
    protected renderComponent(): React.ReactNode {
        const Button = Dyn.DataPool.allControls["button"];
        let readonly = this.props.readonly;

        const parentView = this.getView();
        const view = parentView.props.linkingObjects.view as Dyn.View;
        //console.log(view);

        const index = parentView.props.options.dataIndex;

        return <>
            {(readonly) ? null : <>

                <Button id={this.props.id + 'sortingremove' + index} key={this.props.id + 'sortingremove' + index} options={{ htmlProps: { className: 'btn-outline-danger fas fa-times' } }} valueChangedFunc={() => { this.buttonClickHandler(); }} />

            </>}
        </>;

    }
}
