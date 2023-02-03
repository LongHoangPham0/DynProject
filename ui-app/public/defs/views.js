window.utilities.importViewDefs({

    anyItem_DetailView: {
        fields: [
            {
                name: 'title',
                label: 'Any item detail'
            },
            {
                name: 'anyField',
                label: 'Field 1',
                dataField: 'itemName',
                valueChangedFunc: function (s, n, o, c) {
                    s.props.parent.toggleFields(n, [
                        {
                            match: (v) => { return v != null && v > 0; },
                            fields: [
                                {
                                    names: [
                                        'anyField2'
                                    ],
                                    action: (f,ctrl) => { ctrl.setEnable(false); }
                                }
                            ]
                        }
                    ])
                }
            },
            {
                name: 'anyField2',
                label: 'Field 2',
                dataField: 'f2'
            },
            {
                name: 'anyField',
                label: 'Field 3',
                dataField: 'f3'
            },
            {
                name: 'OverviewInfo_State'
            },
            {
                name: 'OverviewInfo_City'
            }
        ],
        layout: {
            name: 'gridlayout',
            options: {
                columns: 1
            }
        },
        dataApi: 'data/anyItemDetail.json',
        dataApiParamsFunc: function (sender, url) {
            return { type: 'item1', itemId: sender.props.options.itemId };
        },
        submitApi: 'data/submitItemDetail.json',
        submitApiParamsFunc: function (s, d) {
            d = {
                type: 'item1',
                data: d,
                parentID: s.props.options ? s.props.options.linkedItemId : null
            };
            return d;
        },
        deleteApi: 'data/deleteAnyItem',
        deleteApiParamsFunc: function (s, d) {
            d = {
                type: 'item1',
                itemId: d.itemId
            }
            console.log('delete:');
            console.log(d);
            return d;
        }
    },
    anyItem_ItemView: {
        fields: [
            {
                name: 'anyItem_itemId'
            },
            {
                name: 'anyItem_itemName'
            }
        ],
        layout: {
            name: 'flowlayout'
        }
    },
    item1_DetailView: {
        fields: [
            {
                name: 'anyField',
                label: 'f1',
                dataField: 'f1'
            },
            {
                name: 'anyField',
                label: 'f2',
                dataField: 'f2'
            },
            {
                name: 'anyField',
                label: 'f3',
                dataField: 'f3'
            }
        ],
        layout: {
            name: 'gridlayout',
            options: {
                columns: 1
            }
        },
        dataApi: 'data/item1Detail.json',
        dataApiParamsFunc: function (sender, url) {
            return { itemId: sender.props.options.itemId };
        },
        submitApi: 'data/submitItemDetail.json',
        submitApiParamsFunc: function (s, d) {
            console.log(s.props.options);
            d = {
                type: 'item1',
                data: d,
                parentID: s.props.options ? s.props.options.linkedItemId : null
            }
            return d;
        },
    },
    item1_ItemView: {
        fields: [
            {
                name: 'anyItem_itemName',
                label: 'f1',
                dataField: 'f1'
            },
            {
                name: 'anyItem_itemId',
                label: 'f2',
                dataField: 'f2'
            }
        ]
    },
    item2_DetailView: {
        fields: [
            {
                name: 'anyField',
                label: 'f1',
                dataField: 'f1'
            },
            {
                name: 'anyField',
                label: 'f2',
                dataField: 'f2'
            },
            {
                name: 'anyField',
                label: 'f3',
                dataField: 'f3'
            }
        ],
        layout: {
            name: 'gridlayout',
            options: {
                columns: 1
            }
        }
    },
    item2_ItemView: {
        fields: [
            {
                name: 'anyItem_itemId',
                label: null,
                dataField: 'f2'
            },
            {
                name: 'anyItem_itemName',
                label: null,
                dataField: 'f1'
            }

        ],
        layout: {
            name: 'gridlayout',
            options: {
                columns: 1
            }
        }
    },
    item3_DetailView: {
        fields: [
            {
                name: 'anyField',
                label: 'V3 F1',
                dataField: 'f1'
            },
            {
                name: 'anyField',
                label: 'V3 F2',
                dataField: 'f2'
            },
            {
                name: 'anyField',
                label: 'V3 F3',
                dataField: 'f3'
            },
            {
                name: 'anyField',
                label: 'V3 F4',
                dataField: 'f4'
            }
        ],
        layout: {
            name: 'gridlayout',
            options: {
                columns: 1
            }
        },
        dataApi: 'data/item3Detail.json',
        dataApiParamsFunc: function (sender, url) {
            return { itemId: sender.props.options.itemId };
        },
        submitApi: 'data/submitItemDetail.json',
        submitApiParamsFunc: function (s, d, m) {
            console.log(s.props.options);
            d = {
                type: 'item1',
                data: d,
                parentID: s.props.options ? s.props.options.linkedItemId : null
            }
            return d;
        }
    },


});