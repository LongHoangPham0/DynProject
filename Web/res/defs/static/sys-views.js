bluemoon.reactjs.staticViewDefs = {
    fieldConstraints: {
        fields: [
            {
                name: 'valueMatch'
            },
            {
                name: 'appliedFields'
            },
            {
                name: 'fieldAction_visible'
            },
            {
                name: 'fieldAction_enable'
            },
            {
                name: 'fieldAction_dataSource'
            },
            {
                name: 'conditionRemove'
            },
            {
                name: 'seperator'
            }
        ],
        layout: {
            name: 'gridlayout',
            options: {
                columns: 1
            }
        }
    },
    sys_anyItem_listView: {
        fields: [
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'ListItem'
                }
            }
        ],
        dataApi: window.rootUrl + 'api/item/list',
        dataApiParamsFunc: function (sender, url) {
            var opt = sender.props.options;
            var val = window.storage.get(opt.listStorage);
            return { type: opt.itemName, searchFor: val ? val.searchValue : '', page: sender.pageIndex };
        }
    },
    sys_anyItem_approvalList: {
        fields: [
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'ListItem'
                }
            }
        ],
        dataApi: window.rootUrl + 'api/item/approvalList',
        dataApiParamsFunc: function (sender, url) {
            var opt = sender.props.options;
            return { type: opt.itemName, page: sender.pageIndex };
        }
    },
    sys_anyItem_submittedList: {
        fields: [
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'ListItem'
                }
            }
        ],
        dataApi: window.rootUrl + 'api/item/list/submitted',
        dataApiParamsFunc: function (sender, url) {
            var opt = sender.props.options;
            var val = window.storage.get(opt.listStorage);
            return { type: opt.itemName, searchFor: val ? val.searchValue : '', page: sender.pageIndex };
        }
    },
    sys_anyItem_approvedList: {
        fields: [
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'ListItem'
                }
            }
        ],
        dataApi: window.rootUrl + 'api/item/list/approved',
        dataApiParamsFunc: function (sender, url) {
            var opt = sender.props.options;
            var val = window.storage.get(opt.listStorage);
            return { type: opt.itemName, searchFor: val ? val.searchValue : '', page: sender.pageIndex };
        }
    },
    sys_anyItem_rejectedList: {
        fields: [
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'ListItem'
                }
            }
        ],
        dataApi: window.rootUrl + 'api/item/list/rejected',
        dataApiParamsFunc: function (sender, url) {
            var opt = sender.props.options;
            var val = window.storage.get(opt.listStorage);
            return { type: opt.itemName, searchFor: val ? val.searchValue : '', page: sender.pageIndex };
        }
    },
    sys_anyItem_sublistView: {
        fields: [
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'SubItem'
                }
            }
        ],
        dataApi: window.rootUrl + 'api/item/childList',
        dataApiParamsFunc: function (sender, url) {
            var opt = sender.props.options;
            return { parentID: opt.linkedItemId, type: opt.itemName, page: sender.pageIndex };
        }
    },
    sys_anyItem_sellistView: {
        fields: [
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'SelectionItem'
                }
            }
        ],
        dataApi: window.rootUrl + 'api/item/selectChildItems',
        dataApiParamsFunc: function (sender, url) {
            var opt = sender.props.options;
            return { parentID: opt.linkedItemId, type: opt.itemName, page: sender.pageIndex };
        },
        submitApi: window.rootUrl + 'api/item/addChildItems',
        submitApiParamsFunc: function (s, d) {
            var opt = s.props.options;
            d.parentID = opt.linkedItemId;
            return d;
        },
        deleteApi: window.rootUrl + 'api/item/removeChildItem',
        deleteApiParamsFunc: function (s, d) {
            return { parentID: d.parentID, childID: d.itemId };
        }
    },
    sys_anyItem_linkedView: {
        fields: [

            {
                name: 'tabContent',
                dataField: 'linkedItems'
            }
        ],
        dataApi: window.rootUrl + 'api/item/getChildItems',
        dataApiParamsFunc: function (sender, url) {
            return { parentID: sender.props.options.itemId };
        }
    },
    sys_item_history: {
        fields: [
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'HistoryDetail',
                    htmlItem: '\
                    <div class="row">\
                        <div class="col-sm-12 comment-item">\
                            <div class="fas fa-user-circle"></div>\
                            <div class="popover fade show bs-popover-right" role="tooltip" x-placement="right">\
                                <div class="arrow"></div>\
                                <h3 class="popover-header"><b>{UpdatedBy}</b> ({UpdatedTime})</h3>\
                                <div class="popover-body">\
                                    <ul role="children"></ul> \
                                </div>\
                            </div>\
                        </div>\
                    </div>\
                    ',
                    htmlProps: {
                        className: 'comments'
                    }
                },
                dataField: 'histories'
            }
        ],
        layout: {
            name: 'gridlayout',
            options: {
                columns: 1
            }
        },
        dataApi: window.rootUrl + 'api/history/list',
        dataApiParamsFunc: function (sender, url) {
            return { itemId: sender.props.options.itemId, page: sender.pageIndex };
        }
    },
    sys_item_changes: {
        fields: [
            {
                name: 'sys_anyItem_list',
                options: {
                    htmlItem: '<li><b>{Field}</b> was changed from <b>{OldValue}</b> to <b>{NewValue}</b></li>'
                },
                dataField: 'Changes'
            }
        ]

    },
    sys_item_comment_view: {
        fields: [
            {
                name: 'comment_list'
            }
        ],
        layout: {
            name: 'gridlayout',
            options: {
                columns: 1
            }
        },
        dataApi: window.rootUrl + 'api/comment/list',
        dataApiParamsFunc: function (sender, url) {
            return { itemId: sender.props.options.itemId, page: sender.pageIndex };
        }
    },
    sys_item_comment: {
        inherit: 'sys_item_comment_view',

        fields: [

            {
                name: 'seperator'
            },
            {
                name: 'comment_content'
            }
        ],
        submitApi: window.rootUrl + 'api/comment/add',
        submitApiParamsFunc: function (s, d, m) {
            var val = { itemId: s.props.options.itemId, content: d.content };
            return val;
        }
    },
    sys_item_attachments: {
        fields: [
            {
                name: 'subtitle',
                label: 'Attachment list'
            },
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'AttachmentDetail',
                    htmlItem: '\
                        <div class="list-group-item list-group-item-action attachment-item">\
                            <div>\
                                <span><a href={url} target="_blank" ><span class="fas fa-file-download"></span> {fileName}</a> was uploaded by {userName} on {time}</span>\
                            </div>\
                            <div><i>{description}</i></div>\
                            <button class="btn btn-danger fas fa-trash" onclick="window.utilities.deleteAttachment(\'{fileId}\',\'{storageName}\')"> Delete</button>\
                        </div>\
                    ',
                    htmlProps: {
                        className: 'list-group'
                    }
                },
                dataField: 'attachments'
            }
        ],
        layout: {
            name: 'gridlayout',
            options: {
                columns: 1
            }
        },
        dataApi: window.rootUrl + 'api/attachment/list',
        dataApiParamsFunc: function (sender, url) {
            return { itemId: sender.props.options.itemId, page: sender.pageIndex };
        }

    },
    sys_attachment: {
        fields: [
            {
                name: 'fileAttachment'
            },
            {
                name: 'fileDescription'
            }
        ],
        layout: {
            name: 'gridlayout',
            options: {
                columns: 1
            }
        },
        submitApi: window.rootUrl + 'api/attachment/upload',
        deleteApi: window.rootUrl + 'api/attachment/delete'
    },
    sys_validationDef: {
        fields: [
            {
                name: 'fieldDataType'
            },
            {
                name: 'sectiontitle',
                label: 'Validation rules'
            },
            {
                name: 'fieldRuleRequire'
            },
            {
                name: 'fieldRuleEmail'
            },
            {
                name: 'fieldRuleUrl'
            },
            {
                name: 'fieldRuleDigits'
            },
            {
                name: 'fieldRuleNumber'
            },
            {
                name: 'fieldRuleMaxValue'
            },
            {
                name: 'fieldRuleMinValue'
            },
            {
                name: 'fieldRuleMinLength'
            },
            {
                name: 'fieldRuleMaxLength'
            },

            {
                name: 'fieldRuleRegExp'
            }

        ],
        layout: {
            name: 'gridlayout',
            options: {
                columns: 1
            }
        }
    },
    sys_constraintActionDef: {
        fields: [
            {
                name: 'sectiontitle',
                label: 'Contraints'
            },
            {
                name: 'conditions'
            },
            {
                name: 'conditionAdd'
            }

        ],
        layout: {
            name: 'gridlayout',
            options: {
                columns: 1
            }
        }
    },
    sys_reporting_view: {
        fields: [
            {
                name: 'reporting_id'
            },
            {
                name: 'reporting_name'
            },
            {
                name: 'reporting_table'
            }
        ],
        dataApi: window.rootUrl + 'api/report/data',
        dataApiParamsFunc: function (sender, url) {
            return { reportId: sender.props.options.reportId, page: sender.pageIndex };
        }

    },
    sys_reporting_detail: {
        fields: [
            {
                name: 'reporting_id'
            },
            {
                name: 'reporting_name'
            },
            {
                name: 'reporting_dataSource'
            },
            {
                name: 'seperator'
            },
            {
                name: 'subtitle',
                label: 'Query conditions'
            },
            {
                name: 'queryAdd'
            },
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'Query',
                    htmlProps: {
                        className: 'list-group list-group-flush'
                    }
                },
                dataField: 'queries'
            },
            {
                name: 'subtitle',
                label: 'Display columns'
            },
            {
                name: 'reporting_visibleColumns'
            },
            {
                name: 'subtitle',
                label: 'Order by'
            },
            {
                name: 'sortingAdd'
            },
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'SortingList',
                    htmlProps: {
                        className: 'list-group list-group-flush'
                    }
                },
                dataField: 'sorts'
            },
            {
                name: 'pairButton',
                options: {
                    save: {
                        text: 'Save',
                        action: 'save',
                        htmlProps: { className: 'btn btn-primary' }
                    },
                    preview: {
                        text: 'Preview',
                        action: 'preview',
                    },
                    cancel: {
                        text: 'Clear',
                        action: 'clear',
                        htmlProps: { className: 'btn btn-secondary' }
                    }
                },
                valueChangedFunc: function (s, v, o, c) {
                    var view = s.getView();
                    var reportDetail = view.props.linkingObjects.buildValue();
                    for (var n in reportDetail) if (['name', 'dataSourceId', 'colIds', 'queries', 'sorts'].indexOf(n) < 0) delete reportDetail[n];
                    switch (v.action) {
                        case 'save':
                            view.reportDetail = reportDetail;//assign to submit
                            var ok = view.submitData();
                            if (ok) ok.then(function () {
                                window.storage.set(view.props.options.storageName, { dts: new Date().getTime() });
                                view.props.linkingObjects.closeTab();
                            });
                            break;
                        case 'preview':
                            window.storage.set('storage_ReportResult' + view.props.options.reportId, null);
                            if (reportDetail.dataSourceId && reportDetail.colIds && reportDetail.colIds.length > 0) setTimeout(function (pp) {
                                window.storage.set('storage_ReportResult' + pp.view.props.options.reportId, pp.reportDetail);
                            }, 100, { view: view, reportDetail: reportDetail });

                            break;
                        case 'clear':
                            window.storage.set('storage_ReportResult' + view.props.options.reportId, null);
                            break;
                    }
                }
            }
        ],
        layout: {
            name: 'gridlayout',
            options: {
                columns: 1
            }
        },
        dataApi: window.rootUrl + 'api/report/detail',
        dataApiParamsFunc: function (s, url) {
            return { reportId: s.props.options.reportId };
        },
        deleteApi: window.rootUrl + 'api/report/delete',
        deleteApiParamsFunc: function (s, d) {
            return { reportId: d.id };
        },
        submitApi: window.rootUrl + 'api/report/save',
        submitApiParamsFunc: function (s, d) {
            return window.utilities.merge({ reportId: s.props.options.reportId }, s.reportDetail);
        }

    },
    sys_query: {
        fields: [
            {
                name: 'query_properties'
            },
            {
                name: 'query_operator'
            },
            {
                name: 'query_value'
            },
            {
                name: 'queryRemove'
            },
        ]
    },
    sys_sorting: {
        fields: [
            {
                name: 'sorting_properties'
            },
            {
                name: 'sort_direction'
            },
            {
                name: 'sortingRemove'
            }
        ]
    },
    sys_reporting_result: {
        fields: [
            {
                name: 'reporting_table'
            }
        ],
        dataApi: window.rootUrl + 'api/report/preview',
        dataApiParamsFunc: function (sender, url) {
            var reporting = window.storage.get('storage_ReportResult' + sender.props.options.reportId);
            if (reporting) return window.utilities.merge(reporting, { page: sender.pageIndex });
            else return { shouldCancel: true };
        }
    },
    sys_reporting_list: {
        fields: [
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'ReportItem'
                },
                dataField: 'reports'
            }
        ],
        dataApi: window.rootUrl + 'api/report/list',
        dataApiParamsFunc: function (sender, url) {
            var opt = sender.props.options;
            var val = window.storage.get(opt.listStorage);
            return { searchFor: val ? val.searchValue : '', pageIndex: sender.pageIndex, t: new Date().getTime() };
        }
    },
    sys_reporting_item: {
        fields: [
            {
                name: 'reporting_name'
            }

        ]
    },
    sys_item_approval: {
        fields: [
            {
                name: 'comment_content',
                label: 'The below item is required an approval process. Please review and give your comment'
            }
        ],
        layout: {
            name: 'gridlayout',
            options: {
                columns: 1
            }
        },
        submitApi: window.rootUrl + 'api/approval/submit',
        submitApiParamsFunc: function (s, d, m) {
            return { itemId: s.props.options.itemId, comment: d.content };
        },
        deleteApi: window.rootUrl + 'api/approval/reject',
        deleteApiParamsFunc: function (s, d, m) {
            return { itemId: s.props.options.itemId, comment: d.content };
        }
    },
    sys_item_approval_history: {
        fields: [
            {
                name: 'sys_anyItem_list',
                options: {
                    htmlItem: '\
                    <div>\
                    <div>On {time}, {approver}</div>\
                    <li>...set state: <strong>{state}</strong> </li>\
                    <li>...put comment: <i>{comment}</i></li>\
                </div>'
                },
                dataField: 'histories'
            }
        ]

    },
    sys_item_approval_loader: {
        fields: [
            {
                name: 'approval_panel'
            }
        ],
        dataApi: window.rootUrl + 'api/approval/check',
        dataApiParamsFunc: function (s, d, m) {
            return { itemId: s.props.options.itemId };
        },
    },
    sys_rule_list: {
        fields: [
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'RuleItem'
                },
                dataField: 'rules'
            }
        ],
        dataApi: window.rootUrl + 'api/rule/list',
        dataApiParamsFunc: function (sender, url) {
            var opt = sender.props.options;
            var val = window.storage.get(opt.listStorage);
            return { searchFor: val ? val.searchValue : '', pageIndex: sender.pageIndex, t: new Date().getTime() };
        }
    },
    sys_rule_item: {
        fields: [
            {
                name: 'rule_name'
            }

        ]
    },
    sys_rule_detail: {
        fields: [
            {
                name: 'rule_id'
            },
            {
                name: 'rule_name'
            },
            {
                name: 'rule_permission'
            },
            {
                name: 'rule_item'
            },
            {
                name: 'seperator'
            },
            {
                name: 'subtitle',
                label: 'Query conditions'
            },
            {
                name: 'queryAdd'
            },
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'Query',
                    htmlProps: {
                        className: 'list-group list-group-flush'
                    },
                    optional: true

                },
                dataField: 'queries'
            },
            {
                name: 'rule_queryState'
            },
            {
                name: 'subtitle',
                label: 'Actions'
            },
            {
                name: 'actionAdd'
            },
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'RuleAction',
                    htmlProps: {
                        className: 'list-group list-group-flush'
                    }
                },
                dataField: 'actions'
            },
            {
                name: 'rule_nextState'
            },
            {
                name: 'pairButton',
                options: {
                    save: {
                        text: ' Save',
                        action: 'save',
                        htmlProps: { className: 'btn btn-primary fas fa-save' }
                    },
                    cancel: {
                        text: ' Close',
                        action: 'close',
                        htmlProps: { className: 'btn btn-secondary fas fa-window-close' }
                    }
                },
                valueChangedFunc: function (s, v, o, c) {
                    var view = s.getView();
                    var rule = view.props.linkingObjects.buildValue();
                    for (var n in rule) if (['name', 'itemTypeId', 'queries', 'actions', 'nextState', 'queryState', 'permissions'].indexOf(n) < 0) delete rule[n];
                    switch (v.action) {
                        case 'save':
                            view.ruleDetail = rule;
                            var ok = view.submitData();
                            if (ok) ok.then(function () {
                                window.storage.set(view.props.options.storageName, { dts: new Date().getTime() });
                                view.props.linkingObjects.closeTab();
                            });
                            break;
                        case 'close':
                            view.props.linkingObjects.closeTab();
                            break;
                    }
                }
            }
        ],
        layout: {
            name: 'gridlayout',
            options: {
                columns: 1
            }
        },
        dataApi: window.rootUrl + 'api/rule/detail',
        dataApiParamsFunc: function (s, url) {
            return { ruleId: s.props.options.ruleId };
        },
        deleteApi: window.rootUrl + 'api/rule/delete',
        deleteApiParamsFunc: function (s, d) {
            return { ruleId: d.id };
        },
        submitApi: window.rootUrl + 'api/rule/save',
        submitApiParamsFunc: function (s, url) {
            return window.utilities.merge({ ruleId: s.props.options.ruleId }, s.ruleDetail);
        },

    },
    sys_rule_action: {
        fields: [
            {
                name: 'query_properties',
                options: {
                    mode: 1
                }
            },
            {
                name: 'query_value'
            },
            {
                name: 'actionRemove'
            },
        ]
    },
    sys_anyItem_selectionView: {
        fields: [
            {
                name: 'sys_itemType',
                valueChangedFunc: function (s, v, o, c) {
                    var view = s.getView();
                    if (s.getValue()) view.rebind();
                    else view.bindData({ items: null });
                }
            },
            {
                name: 'sys_anyItem_list',
                options: {
                    component: 'MapItem',
                    textField: 'itemName',
                    valueField: 'itemId',
                }
            }
        ],
        dataApi: window.rootUrl + 'api/item/selectionList',
        dataApiParamsFunc: function (v, url) {
            var ctrl = v.find('sys_itemType');
            var typeId = ctrl.getValue();
            if (typeId == null) return { shouldCancel: true };
            else return { typeId };
        },
    }
};