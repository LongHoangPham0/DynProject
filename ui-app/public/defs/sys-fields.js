bluemoon.reactjs.staticFieldDefs = {
    title: {
        type: 'title',
        label: '???',
        dataField: 'title'
    },
    sectiontitle: {
        type: 'sectiontitle',
        label: '???',
        dataField: 'sectiontitle'
    },
    subtitle: {
        type: 'subtitle',
        label: '???',
        dataField: 'subtitle'
    },
    viewloader: {
        type: 'viewloader',
        dataField: 'viewName'
    },
    seperator: {
        type: 'seperator'
    },
    submitbutton: {
        type: 'button',
        label: 'Save',
        options: {
            htmlProps: {
                className: 'btn btn-primary'
            }
        },
        valueChangedFunc: function () {
            alert('Me clicked');
        }

    },
    pairButton: {
        type: 'pairbutton'
    },
    sys_anyItem_list: {
        type: 'datalist',
        options: {
            htmlProps: {
                className: 'list-item-block'
            },
            pagingHandler: function (s, i, p) {
                s.props.parent.pageIndex = i;
                s.props.parent.rebind();
            }
        },
        dataField: 'items'
    },

    createdBy: {
        type: 'label',
        label: 'Creator',
        dataField: 'CreatedBy'
    },
    fieldDataType: {
        type: 'hidden',
        dataField: 'dataType',
        valueChangedFunc: function (s, n, o, c) {
            s.props.parent.toggleFields(n, [
                {
                    match: function (v) { return v == dataType.TypeOfInt; },
                    fields: [
                        {
                            names: ['fieldRuleMaxValue', 'fieldRuleMinValue', 'fieldRuleDigits'],
                            action: function (f, ctrl) { ctrl.setVisible(true); }
                        }
                    ]

                },
                {
                    match: function (v) { return v == dataType.TypeOfDecimal; },
                    fields: [
                        {
                            names: ['fieldRuleMaxValue', 'fieldRuleMinValue', 'fieldRuleNumber'],
                            action: function (f, ctrl) { ctrl.setVisible(true); }
                        }
                    ]

                },
                {
                    match: function (v) { return v == dataType.TypeOfString; },
                    fields: [
                        {
                            names: ['fieldRuleMaxLength', 'fieldRuleMinLength', 'fieldRuleEmail', 'fieldRuleUrl', 'fieldRuleDigits', 'fieldRuleRegExp'],
                            action: function (f, ctrl) { ctrl.setVisible(true); }
                        }
                    ]
                }
            ])
        }
    },
    fieldRuleRequire: {
        type: 'checktextbox',
        label: 'Required',
        dataField: 'rules.required'
    },
    fieldRuleMaxValue: {
        type: 'checkdbltextbox',
        label: 'Max value',
        dataField: 'rules.max',
        options: {
            optAtt: 'maxValue'
        },
        visible: false
    },
    fieldRuleMinValue: {
        type: 'checkdbltextbox',
        label: 'Min value',
        dataField: 'rules.min',
        options: {
            optAtt: 'minValue'
        },
        visible: false
    },
    fieldRuleMaxLength: {
        type: 'checkdbltextbox',
        label: 'Max length',
        dataField: 'rules.maxLength',
        options: {
            optAtt: 'maxLength'
        },
        visible: false
    },
    fieldRuleMinLength: {
        type: 'checkdbltextbox',
        label: 'Min length',
        dataField: 'rules.minLength',
        options: {
            optAtt: 'minLength'
        },
        visible: false
    },
    fieldRuleRegExp: {
        type: 'checkdbltextbox',
        label: 'Reg exp',
        dataField: 'rules.regExp',
        options: {
            optAtt: 'pattern'
        },
        visible: false
    },
    fieldRuleEmail: {
        type: 'checktextbox',
        label: 'Email',
        dataField: 'rules.email',
        visible: false
    },
    fieldRuleUrl: {
        type: 'checktextbox',
        label: 'Url',
        dataField: 'rules.url',
        visible: false
    },
    fieldRuleDigits: {
        type: 'checktextbox',
        label: 'Digits',
        dataField: 'rules.digits',
        visible: false
    },
    fieldRuleNumber: {
        type: 'checktextbox',
        label: 'Number',
        dataField: 'rules.number',
        visible: false
    },
    conditionAdd: {
        type: 'FieldConditionAddButton'
    },
    conditionRemove: {
        type: 'FieldConditionRemoveButton'
    },
    conditions: {
        type: 'FieldConditions',
        dataField: 'conditions'
    },
    valueMatch: {
        type: 'combotextbox',
        dataSource: [
            { text: 'Equal', valueId: '==' },
            { text: 'Not equal', valueId: '!=' },
            { text: 'Greater than', valueId: '>' },
            { text: 'Less than', valueId: '<' },
        ],
        dataField: 'match'
    },
    appliedFields: {
        type: 'checkboxlist',
        label: 'Apply for:',
        dataField: 'fields',
        validationRules: [
            {
                rule: 'required',
                msg: 'Please select a field'
            }
        ],
        dataSourceApi: window.rootUrl + 'system/itemtype/properties.json',
        dataApiParamsFunc: function (s, url) {
            var view = s.getView();
            return { typeId: view.props.linkingObjects.parent.props.options.typeId };
        },
        options: {
            htmlProps: {
                className: 'check-fields'
            }
        }


    },
    fieldAction_visible: {
        type: 'checkradio',
        label: 'Visibility',
        dataField: 'actions.visibility',
        dataSource: [
            {
                text: 'Visible',
                valueId: 'visible'
            },
            {
                text: 'Invisible',
                valueId: 'invisible'
            }
        ]
    },
    fieldAction_enable: {
        type: 'checkradio',
        label: 'Ability',
        dataField: 'actions.ability',
        dataSource: [
            {
                text: 'Enable',
                valueId: 'enable'
            },
            {
                text: 'Disable',
                valueId: 'disable'
            }
        ]
    },
    fieldAction_dataSource: {
        type: 'checkarea',
        label: 'Data ({\"filter_by_prop\":"[v]"};[{\"text\":string, \"value\":string}])',
        dataField: 'actions.dataSource'
    },

    tabContent: {
        type: 'Tab'
    },
    comment_content: {
        type: 'textarea',
        label: 'Your comment',
        dataField: 'content',
        validationRules: [
            {
                rule: 'required',
                msg: 'Please enter your comment'
            }
        ]
    },
    comment_list: {
        inherit: 'sys_anyItem_list',
        options: {
            htmlItem: '\
                <div class="row">\
                    <div class="col-sm-12 comment-item">\
                        <div class="fas fa-user-circle"></div>\
                        <div class="popover fade show bs-popover-right" role="tooltip" x-placement="right">\
                            <div class="arrow" style="top: 34px"></div>\
                            <h3 class="popover-header"><b>{Username}</b> ({CreatedDate})</h3>\
                            <div class="popover-body">{Content}</div>\
                        </div>\
                    </div>\
                </div>',
            htmlProps: {
                className: 'comments'
            }
        }
    },
    reporting_id: {
        type: 'hidden',
        dataField: 'id'
    },
    reporting_name: {
        type: 'textbox',
        dataField: 'name',
        validationRules: [
            {
                rule: 'required',
                msg: 'Report name is required'
            }
        ]
    },
    reporting_dataSource: {
        type: 'dropdownlist',
        dataField: 'dataSourceId',
        label: 'Data source',
        validationRules: [
            {
                rule: 'required',
                msg: 'Data source is required'
            }
        ],
        dataSourceApi: window.rootUrl + 'api/report/datasource.json',
        valueChangedFunc: function (s, n, o, c) {

            var view = s.getView();

            var pp = view.find('reporting_visibleColumns', true);
            pp.rebind();

            var val = {};
            val.queries = [];
            val.queries.push({ t: new Date() });
            val.cols = [];
            val.sorts = [];
            //val.dataSource = view.getValues().dataSource;
            view.bindData(val);

        }
    },
    reporting_visibleColumns: {
        type: 'checkboxlist',
        dataField: 'colIds',
        dataSourceApi: window.rootUrl + 'api/item/properties.json',
        dataApiParamsFunc: function (s, url) {
            var view = s.getView();
            var pp = view.find('reporting_dataSource');
            var typeId = pp.getValue();
            if (typeId == null) return { shouldCancel: true };
            else return { typeId: typeId };
        },
        validationRules: [
            {
                rule: 'required',
                msg: 'Please select a column'
            }
        ],
        valueChangedFunc: function (s, v, o, c) {
            var view = s.getView();
            var val = view.getValues();
            if (s.first) {
                val.sorts = [];
            }
            else {
                if (val.sorts && val.sorts[0]) val.sorts[0].t = new Date().getTime();
                s.first = true;
            }

            view.bindData(val);
        }
    },
    reporting_table: {
        type: 'table',
        options: {
            htmlProps: {
                className: 'table'
            },
            pagingHandler: function (s, i, p) {
                s.props.parent.pageIndex = i;
                s.props.parent.rebind();
            }

        },
        dataField: 'result'
    },

    query_properties: {
        type: 'dropdownlist',
        dataField: 'propId',
        dataSourceApi: window.rootUrl + 'api/item/properties.json',
        dataApiParamsFunc: function (s, url) {
            var view = s.getView().props.linkingObjects.view;
            var pp = view.find('reporting_dataSource') || view.find('rule_item');
            var typeId = pp.getValue();

            if (typeId == null) return { shouldCancel: true };
            else {
                var mode = null;
                if (s.props.options) mode = s.props.options.mode;
                if (mode) return { typeId: typeId, mode: mode };
                else return { typeId: typeId };
            }
        }
    },
    query_operator: {
        type: 'dropdownlist',
        dataField: 'operator',
        dataSource: [
            {
                text: 'Contains',
                valueId: 'like'
            },
            {
                text: 'Equal',
                valueId: '='
            },
            {
                text: 'Not Equal',
                valueId: '<>'
            },
            {
                text: 'Greater than',
                valueId: '>'
            },
            {
                text: 'Greater or equal than',
                valueId: '>='
            },
            {
                text: 'Less than',
                valueId: '<'
            },
            {
                text: 'Less or equal than',
                valueId: '<='
            }
        ]
    },
    query_value: {
        type: 'textbox',
        dataField: 'value'
    },

    queryAdd: {
        type: 'QueryAddButton'
    },
    queryRemove: {
        type: 'QueryRemoveButton'
    },
    sorting_properties: {
        type: 'dropdownlist',
        dataField: 'propId',
        dataSourceApi: window.rootUrl + 'api/item/properties.json',
        dataApiParamsFunc: function (s, url) {
            /*
            var view = s.getView().props.linkingObjects.view;
            var pp = view.find('reporting_dataSource');
            var typeId = pp.getValue();

            if (typeId == null) return { shouldCancel: true };
            else {
                var mode = null;
                if (s.props.options) mode = s.props.options.mode;
                if (mode) return { typeId, mode };
                else return { typeId };
            }
            //*/
            var view = s.getView().props.linkingObjects.view;
            var ctrl = view.find('reporting_visibleColumns');
            var vs = ctrl.getValue() || [];
            var ds = ctrl.getDataSource();
            var result = null;
            if (ds && ds.filter) {
                result = ds.filter(function (obj) {
                    return vs.indexOf(obj.valueId) >= 0;
                });
            }

            return {
                shouldCancel: true,
                localData: result
            };
        }
    },
    sort_direction: {
        type: 'radiolist',
        dataField: 'direction',
        dataSource: [
            {
                text: 'ASC',
                valueId: 'asc'
            },
            {
                text: 'DESC',
                valueId: 'desc'
            }
        ]
    },
    sortingAdd: {
        type: 'SortingAddButton'
    },
    sortingRemove: {
        type: 'SortingRemoveButton'
    },
    rule_id: {
        type: 'hidden',
        dataField: 'id'
    },
    rule_name: {
        type: 'textbox',
        dataField: 'name',
        validationRules: [
            {
                rule: 'required',
                msg: 'Rule name is required'
            }
        ]
    },
    rule_permission: {
        type: 'textbox',
        dataField: 'permissions',
        label: 'Permissions',
        validationRules: [
            {
                rule: 'required',
                msg: 'Permission is required'
            }
        ]
    },
    rule_queryState: {
        type: 'textbox',
        dataField: 'queryState',
        label: 'State',
        validationRules: [
            {
                rule: 'required',
                msg: 'Query state is required'
            }
        ]
    },
    rule_nextState: {
        type: 'textbox',
        dataField: 'nextState',
        label: 'Approval state',
        validationRules: [
            {
                rule: 'required',
                msg: 'Next state is required'
            }
        ]
    },
    rule_item: {
        type: 'dropdownlist',
        dataField: 'itemTypeId',
        label: 'Applied item',
        validationRules: [
            {
                rule: 'required',
                msg: 'Applied item is required'
            }
        ],
        dataSourceApi: window.rootUrl + 'api/rule/itemType.json',
        valueChangedFunc: function (s, n, o, c) {
            var view = s.getView();

            if (!view.firstLoad) {
                view.firstLoad = true;
                return;
            }
            var val = {};
            val.queries = [];
            val.actions = [];
            view.bindData(val);


        }
    },
    actionAdd: {
        type: 'ActionAddButton'
    },
    actionRemove: {
        type: 'ActionRemoveButton'
    },
    approval_panel: {
        type: 'ItemApproval',
        dataField: 'approval'
    },
    fileAttachment: {
        type: 'fileselector',
        label1: 'Select a file',
        dataField: 'file',
        validationRules: [
            {
                rule: 'required',
                msg: 'Please select a file'
            }
        ]
    },
    fileDescription: {
        type: 'textarea',
        label: 'Description',
        dataField: 'description'
    },
    sys_itemType: {
        type: 'dropdownlist',
        label: 'Item type',
        dataSourceApi: window.rootUrl + 'api/item/types.json',
    }
};