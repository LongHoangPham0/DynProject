import React from 'react';
import ReactDOM from 'react-dom';
import * as Dyn from 'bluemoon-dyn-lib';
import './controls/index';
import './pages/index';

Dyn.DynConfig.customValidationMessage = (msg: string, sender: Dyn.IComponent, newVal: any, oldVal: any, context: Dyn.IAppContext) => {
    return <>
        <div className="alert alert-danger">{msg}</div>
    </>;

};


Dyn.DynConfig.appDOM = ReactDOM;
let root = document.getElementsByTagName('app-loader')[0];
Dyn.DynConfig.apiCache = (root.getAttribute('cache') ?? 'true') === 'true';
Dyn.DynConfig.debug = (root.getAttribute('debug') ?? 'false') === 'true';;
if (root) ReactDOM.render(<Dyn.ConfigLoader viewUrls={root.getAttribute('viewUrls')?.split('|')} fieldUrls={root.getAttribute('fieldUrls')?.split('|')} />, root);