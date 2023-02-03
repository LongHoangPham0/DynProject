import React, { useState } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
import { Tab, TabRef } from '../controls/Tab';


export const RuleDetail = (p: { ruleId: number, tabId: string, storageName: string, newMode?: boolean, tabRef: TabRef }) => {
    const whereQueries = [] as React.RefObject<Dyn.View>[];
    const ruleActions = [] as React.RefObject<Dyn.View>[];
    const refView = React.createRef<Dyn.View>();
    const buildValue = function () {
        const linkingObj = refView.current.props.linkingObjects;
        let val = { ...refView.current.getValues(), actions: [], queries: [] };
        const whereQueries = linkingObj.whereQueries;

        //val.queries = [];
        for (const q of whereQueries) {
            if (q.current) val.queries.push(q.current.getValues());
        }
        const ruleActions = linkingObj.ruleActions;
        //val.actions = [];
        for (const a of ruleActions) {
            if (a.current) val.actions.push(a.current.getValues());
        }
        return val;
    };

    const closeTab = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        p.tabRef.instance.closeTab(p.tabId);
    };
    p.tabRef.instance.closedTab[p.tabId] = ()=>{
        Dyn.DataStorage.delete('storage_RuleResult' + p.ruleId);
        console.log('tab closed:'+p.tabId);
    };
    let ds = null;
    if (p.newMode) ds = { name: '', itemType: '', permissions: '', state: '' };
    return (<>
        <div className="row">
            <div className="col-sm-12 reporting center">
                <Dyn.View name="sys_rule_detail" ref={refView} id={'cidRule' + p.ruleId} linkingObjects={{ whereQueries, ruleActions, buildValue, closeTab }} options={{ ruleId: p.ruleId, storageName: p.storageName }} dataSource={ds} />


            </div>
        </div>


    </>);
};
RuleDetail.defaultProps = {
    tabRef: { instance: null } as TabRef
};
