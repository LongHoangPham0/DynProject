import React, { MouseEvent } from 'react';
type ActionHandler = (evt: MouseEvent<HTMLButtonElement>, sender: Modal) => boolean;
type ActionOption = { text?: string, actionHandler?: ActionHandler };
export class Modal extends React.Component<{ title: string, saveOpt?: ActionOption, cancelOpt?: ActionOption }, { visible: boolean, children?: any, title: string }> {
    private saveOpt: ActionOption = null;
    private cancelOpt: ActionOption = null;
    public childRef = React.createRef<any>();
    constructor(props: { title: string, visible?: boolean }) {
        super(props);
        this.state = {
            visible: false,
            title: this.props.title
        };
        this.childRef = React.createRef();
    }
    public open(title?: string, saveOpt?: ActionOption, cancelOpt?: ActionOption, children?: any) {
        this.saveOpt = saveOpt;
        this.cancelOpt = cancelOpt;

        this.setState({ title: title || this.props.title, visible: true, children });
    }
    private close() {
        this.setState({ visible: false });
    }
    private onAction(evt: MouseEvent<HTMLButtonElement>, action: ActionHandler) {
        let shouldClose = true;
        if (action) shouldClose = action(evt, this);
        if (shouldClose) this.close();
        evt.preventDefault();

    }
    public render(): React.ReactNode {
        if (!this.state.visible) return null;
        const saveOpt = { ...this.props.saveOpt, ...this.saveOpt };
        const cancelOpt = { ...this.props.cancelOpt, ...this.cancelOpt };
        const children = this.state.children || this.props.children;
        const mynewChildren = [] as React.ReactElement[];
        React.Children.forEach(children, (child, index) => {
            //child.ref = this.childRef;
            //React.
            if (child.ref) {
                mynewChildren.push(child);
            }
            else {
                let p = React.cloneElement(child, { ...child.props, ...{ ref: this.childRef }, ...{ key: 'mc' + index } });

                mynewChildren.push(p);
            }

        });
        return (<>
            <div className="modal" tabIndex={-1} role="dialog" style={{ display: (this.state.visible ? 'block' : 'none') }}>
                <div className="modal-dialog  modal-dialog-centered" role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h5 className="modal-title">{this.state.title}</h5>
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close" onClick={(evt) => { this.onAction(evt, cancelOpt?.actionHandler); }}>
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div className="modal-body">
                            {mynewChildren}
                        </div>
                        {saveOpt || cancelOpt ? (
                            <div className="modal-footer">
                                {saveOpt ? <button type="button" className="btn btn-primary fas fa-save" onClick={(evt) => { this.onAction(evt, saveOpt.actionHandler); }}> {saveOpt.text}</button> : null}
                                {cancelOpt ? <button type="button" className="btn btn-secondary fas fa-undo" data-dismiss="modal" onClick={(evt) => { this.onAction(evt, cancelOpt.actionHandler); }}> {cancelOpt.text}</button> : null}
                            </div>
                        ) : null}

                    </div>
                </div>
            </div>
            <div className="modal-backdrop fade show"></div>
        </>
        );
    }

};