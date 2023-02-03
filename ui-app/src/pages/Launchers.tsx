import React, { useState } from 'react';
import { Tab, TabRef } from '../controls/Tab';
import { ItemDetail } from './ItemDetail';
import { ItemList } from './ItemList';
import { ReportList } from './ReportList';
import { RuleList } from './RuleList';

export const ItemLauncher = (p: { tabname: string, itemtitle: string, itemname: string, idfield: string, displayfield: string, showcomment: string, authenticated: string, editable: string, showhistory: string, showapproval: string, approver: string, itemid: string }) => {

  let tabs = null;
  if(p.itemid){
    const detailName = p.itemname + '_DetailView';
    const listName = p.itemname + '_ItemView';
    const storageName = 'storage_' + listName + 'sys_anyItem_listView';
    tabs = {
      mainTab: {
        title: p.tabname,
        content: <ItemDetail
        displayField={p.displayfield}
        detailName={detailName}
        listStorage={storageName}
        tabId="mainTab"
        showComment={p.showcomment == 'true'}
        showHistory={p.showhistory == 'true'}
        editable={p.editable == 'true'}
        authenticated={p.authenticated == 'true'}
        itemId={p.itemid}
        listName={listName}
        />
      }
    }
  }
  else{
    tabs = {
      mainTab: {
        title: p.tabname,
        content: <ItemList
          id="cidList"
          title={p.itemtitle}
          itemName={p.itemname}
          idField={p.idfield}
          displayField={p.displayfield}
          showComment={p.showcomment == 'true'}
          showHistory={p.showhistory == 'true'}
          editable={p.editable == 'true'}
          authenticated={p.authenticated == 'true'}
          approvalList={false}
        />
      }
  
    };
  
    if (p.showapproval == 'true') {
      if (p.approver == 'true')
        tabs['apprTab'] = {
          title: 'Waiting',
          content: <ItemList
            id="cidApprList"
            title={p.itemtitle}
            itemName={p.itemname}
            idField={p.idfield}
            displayField={p.displayfield}
            showComment={p.showcomment == 'true'}
            showHistory={p.showhistory == 'true'}
            editable={false}
            approvalList={true}
            authenticated={p.authenticated == 'true'}
          />
        };
      tabs['submittedTab'] = {
        title: 'Submitted',
        content: <ItemList
          id="cidSubmittedList"
          title={p.itemtitle}
          itemName={p.itemname}
          idField={p.idfield}
          displayField={p.displayfield}
          showComment={p.showcomment == 'true'}
          showHistory={p.showhistory == 'true'}
          editable={false}
          approvalList={true}
          listViewName="sys_anyItem_submittedList"
          authenticated={p.authenticated == 'true'}
        />
      };
      tabs['approvedTab'] = {
        title: 'Approved',
        content: <ItemList
          id="cidApprovedList"
          title={p.itemtitle}
          itemName={p.itemname}
          idField={p.idfield}
          displayField={p.displayfield}
          showComment={p.showcomment == 'true'}
          showHistory={p.showhistory == 'true'}
          editable={false}
          approvalList={true}
          listViewName="sys_anyItem_approvedList"
          authenticated={p.authenticated == 'true'}
        />
      };
      tabs['rejectedTab'] = {
        title: 'Rejected',
        content: <ItemList
          id="cidRejectedList"
          title={p.itemtitle}
          itemName={p.itemname}
          idField={p.idfield}
          displayField={p.displayfield}
          showComment={p.showcomment == 'true'}
          showHistory={p.showhistory == 'true'}
          editable={false}
          approvalList={true}
          listViewName="sys_anyItem_rejectedList"
          authenticated={p.authenticated == 'true'}
        />
      };
  
    }
  }
  

  return (<Tab dataSource={tabs} primary={true} />);
}

export const ReportLauncher = (p: { editable: string }) => {
  const tabs = {
    mainTab: {
      title: 'Report list',
      content: <ReportList
        editable={p.editable == 'true'} />
    }

  };

  return (<Tab dataSource={tabs} primary={true} />);
}
export const RuleLauncher = (p: { editable: string }) => {
  const tabs = {
    mainTab: {
      title: 'Approval rules',
      content: <RuleList />
    }

  };

  return (<Tab dataSource={tabs} primary={true} />);
}