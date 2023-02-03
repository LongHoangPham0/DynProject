import React, { useState } from 'react';
import * as Dyn from 'bluemoon-dyn-lib';
import { Tab, TabRef } from '../controls/Tab';


export const ReportDetail = (p: { 
    reportId: number, 
    editable: boolean, 
    tabId: string, 
    storageName: string, 
    editMode?: boolean, 
    newMode?: boolean,
    tabRef: TabRef
}) => {
    const whereQueries = [] as React.RefObject<Dyn.View>[];
    const sortingList = [] as React.RefObject<Dyn.View>[];
    const refView = React.createRef<Dyn.View>();
    const [editMode, setEditMode] = useState(p.editMode|| false);
    const buildValue = function () {
        var whereQueries = refView.current.props.linkingObjects.whereQueries;
        const val = { ...refView.current.getValues() };
        val.queries = [];
        for (var q of whereQueries) {
            if (q.current) val.queries.push(q.current.getValues());
        }
        val.sorts = [];
        for (var s of sortingList) {
            if (s.current) val.sorts.push(s.current.getValues());
        }
        return val;
    };
    const buttonClickHandler = () => {
        setEditMode(!editMode);
    };
    const closeTab = (evt: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        p.tabRef.instance.closeTab(p.tabId);
    };
    p.tabRef.instance.closedTab[p.tabId] = ()=>{
        Dyn.DataStorage.delete('storage_ReportResult' + p.reportId);
    };
    let ds = null;
    if(p.newMode) ds = {name: '', dataSource: ''};
    return (<>
        <div className="row">
            <div className="col-sm-12 reporting center">
                {editMode ? <>
                    <Dyn.View name="sys_reporting_detail" key={'key' + editMode} ref={refView} id={'cidReporting' + p.reportId} linkingObjects={{ whereQueries, sortingList, buildValue, closeTab }} options={{ reportId: p.reportId, storageName: p.storageName }} dataSource={ds} />
                    <br />
                    <ReportResult reportId={p.reportId} />
                </> : <>
                    <Dyn.View name="sys_reporting_view"  key={'key' + p.reportId} readonly={true} id={'cidReportingView' + p.reportId} options={{ reportId: p.reportId }} />
                    <br />
                    <div>
                        {p.editable?<>
                            <button className="btn btn-primary fas fa-pen" onClick={buttonClickHandler}> Edit</button>
                        </>:null}
                    </div>
                </>}


            </div>
        </div>


    </>);
};
ReportDetail.defaultProps = {
    tabRef: { instance: null } as TabRef
};
const ReportResult = (p: { reportId: number }) => {
    const [, forceUpdate] = useState(null);
    Dyn.DataStorage.register('storage_ReportResult' + p.reportId, forceUpdate);

    return (
        <div className="row">
            <div className="col-sm-12 reporting-result">
                <Dyn.View name="sys_reporting_result" id={'cidReportingResult'} key={'rp.result' + new Date().getTime()} options={{ reportId: p.reportId }} />
            </div>
        </div>
    );
};
