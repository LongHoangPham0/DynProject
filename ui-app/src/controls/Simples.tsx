import React from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
export class Seperator extends Dyn.BaseComponent {
    protected renderComponent(): React.ReactNode {
        return (<>
            <hr/>
        </>);
    }
}
export class Title extends Dyn.BaseComponent {
    protected renderComponent(): React.ReactNode {
        return (<>
            <h3>{this.props.label || this.getValue() || ''}</h3>
        </>);
    }
}
export class SubTitle extends Dyn.BaseComponent {
    protected renderComponent(): React.ReactNode {
        return (<>
            <h4>{this.props.label || this.getValue() || ''}</h4>
        </>);
    }
}
export class SectionTitle extends Dyn.BaseComponent {
    protected renderComponent(): React.ReactNode {
        return (<>
            <h5>{this.props.label || this.getValue() || ''}</h5>
        </>);
    }
}
