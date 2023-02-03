import React, { useState } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';

export class Query extends Dyn.BaseComponent {

    protected renderComponent(): React.ReactNode {
        
        const parentView = this.getView() as Dyn.View;
        const whereQueries = parentView.props.linkingObjects.whereQueries as any[];
        
        whereQueries[this.props.dataIndex] = React.createRef<Dyn.View>();
        return <>
            <div key={'cidReportingQuery' + this.props.dataIndex} className="list-group-item">
                <Dyn.View name="sys_query" readonly={parentView.props.readonly} ref={whereQueries[this.props.dataIndex]} id={'cidReportingQuery' + this.props.dataIndex} dataSource={this.props.data} linkingObjects={{ view: parentView }} options={{ dataIndex: this.props.dataIndex, optional: this.props.dataListInstance.props.options.optional }} />
            </div>
        </>;
    }

}

export class QueryAddButton extends Dyn.BaseComponent {
    private buttonClickHandler = () => {
        const view = this.getView();
        const val = view.props.linkingObjects.buildValue();
        const quer = val.queries as any[];
        quer.push({});
        view.bindData(val);
    };
    protected renderComponent(): React.ReactNode {
        const Button = Dyn.DataPool.allControls["button"];
        return <>
            <Button id={this.props.id + 'submit'} options={{ htmlProps: { className: 'btn btn-success fas fa-plus-circle' } }} valueChangedFunc={() => { this.buttonClickHandler(); }} />
        </>
    }
}
export class QueryRemoveButton extends Dyn.BaseComponent {
    private buttonClickHandler = () => {
        const parentView = this.getView();
        const view = parentView.props.linkingObjects.view as Dyn.View;
        //console.log(view);
        const val = view.props.linkingObjects.buildValue();
        const quer = val.queries as any[];
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
        const val = view.getData();
        
        if (!parentView.props.options.optional || parentView.props.options.optional !== true) {
            const quer = val.queries as any[];
            if (quer.length == 1) readonly = true;
        }

        return <>
            {(readonly) ? null : <>

                <Button id={this.props.id + 'queryremove' + index} key={this.props.id + 'queryremove' + index} options={{ htmlProps: { className: 'btn-outline-danger fas fa-times' } }} valueChangedFunc={() => { this.buttonClickHandler(); }} />

            </>}
        </>;

    }
}
