window.utilities.importFieldDefs({

    anyItem_itemId: {
        type: 'label',
        dataField: 'itemId',
        label: 'Item id'

    },
    anyItem_itemName: {
        type: 'label',
        dataField: 'itemName',
        label: 'Item name'
    },
    anyField:{
        type: 'textbox',
        label: 'Field 1'
    },
    anyField2:{
        type: 'textbox',
        options: { textType: 'date'},
        label: 'Field 1'
    },
    OverviewInfo_State: {
        type: 'dropdownlist',
        dataField: 'state',
        label: 'State',

        dataSource: [{ "text": "Texas", "value": "TX" }, { "text": "North Carolina", "value": "NC" }],

        validationRules: [],
        valueChangedFunc:
            function (s, n, o, c) {
                s.props.parent.toggleFields(n, [
                    {
                        match: v => v != null && v == 'TX',
                        fields: [
                            {
                                names: ["OverviewInfo_City"],
                                action: (f,ctrl) => { 
                                    console.log(f);
                                    ctrl.setDataSource([{ "text": "City 1", "value": "c1" }]); 
                            }
                            }
                        ]
                    }, {
                        match: v => v != null && v == 'NC',
                        fields: [
                            {
                                names: ["OverviewInfo_City"],
                                action: (f,ctrl) => { 
                                    ctrl.setDataSource([{ "text": "City 2", "value": "c2" }]); 
                                }
                            }
                        ]
                    }
                ])
            }
        ,
        options: null

    },
    OverviewInfo_City: {
        type: 'dropdownlist',
        dataField: 'city',
        label: 'City',

        validationRules: [],
        valueChangedFunc:
            function (s, n, o, c) {
                s.props.parent.toggleFields(n, [

                ])
            }
        ,
        options: null

    }
});