import React, { useState } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';

export class RuleAction extends Dyn.BaseComponent {

    protected renderComponent(): React.ReactNode {
        const parentView = this.getView() as Dyn.View;
        const ruleActions = parentView.props.linkingObjects.ruleActions as any[];
        ruleActions[this.props.dataIndex] = React.createRef<Dyn.View>();
        return <>
            <div key={'cidRuleAction' + this.props.dataIndex} className="list-group-item">
                <Dyn.View name="sys_rule_action" readonly={parentView.props.readonly} ref={ruleActions[this.props.dataIndex]} id={'cidRuleAction' + this.props.dataIndex} dataSource={this.props.data} linkingObjects={{ view: parentView }} options={{ dataIndex: this.props.dataIndex }} />
            </div>
        </>;
    }

}

export class ActionAddButton extends Dyn.BaseComponent {
    private buttonClickHandler = () => {
        const view = this.getView();
        const val = view.props.linkingObjects.buildValue();
        const act = val.actions as any[];
        act.push({});
        view.bindData(val);
    };
    protected renderComponent(): React.ReactNode {
        const Button = Dyn.DataPool.allControls["button"];
        return <>
            <Button id={this.props.id + 'submit'} options={{ htmlProps: { className: 'btn btn-success fas fa-plus-circle' } }} valueChangedFunc={() => { this.buttonClickHandler(); }} />
        </>
    }
}
export class ActionRemoveButton extends Dyn.BaseComponent {
    private buttonClickHandler = () => {
        const parentView = this.getView();
        const view = parentView.props.linkingObjects.view as Dyn.View;
        //console.log(view);
        const val = view.props.linkingObjects.buildValue();
        const act = val.actions as any[];
        act.splice(parentView.props.options.dataIndex, 1);
        view.bindData(val);

    };
    protected renderComponent(): React.ReactNode {
        const Button = Dyn.DataPool.allControls["button"];
        let readonly = this.props.readonly;

        const parentView = this.getView();
        //console.log(view);
        const index = parentView.props.options.dataIndex;

        return <>
            {(readonly) ? null : <>

                <Button id={this.props.id + 'ruleremove' + index} key={this.props.id + 'ruleremove' + index} options={{ htmlProps: { className: 'btn-outline-danger fas fa-times' } }} valueChangedFunc={() => { this.buttonClickHandler(); }} />

            </>}
        </>;

    }
}
