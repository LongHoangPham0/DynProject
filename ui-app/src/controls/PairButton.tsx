import React from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
export class PairButton extends Dyn.BaseComponent {
    private buttonClickHandler = (action: string) => {
        //console.log(action);
        this.setValue({action});
    };
    protected renderComponent(): React.ReactNode {
        const Button = Dyn.DataPool.allControls["button"];
        const options = this.props.options || {} as {[name: string]: {action?: string, text: string,  htmlProps?: any}};
        
        const items = [] as React.ReactElement[];
        for(const name in options){
            if(typeof options[name] !== 'object') continue;
            options[name].action = options[name].action || options[name].text;
            options[name].htmlProps = options[name].htmlProps || { className: 'btn btn-warning' };
            items.push(<Button id={this.props.id + name} key={this.props.id + name} label={options[name].text} options={{ htmlProps: options[name].htmlProps}} valueChangedFunc={() => { this.buttonClickHandler(options[name].action); }} />)
        }
        return <span className="pair-button">
                {items}
            </span>
    }
}