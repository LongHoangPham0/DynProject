using BlueMoon.Business;
using BlueMoon.MVC.Controls;
using BlueMoon.DynWeb.Common;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Linq;
using KeyValue = System.Collections.Generic.Dictionary<string, object>;
using System.Text.RegularExpressions;
using BlueMoon.DynWeb.Entities;
using System.IO;
using System.Web;

namespace BlueMoon.DynWeb.Common
{
    public static class DofGenerator
    {
        static object s_locker = new object();
        static void DeleteFile(string virtualPath)
        {
            string filePath = HttpContext.Current.Server.MapPath(virtualPath);
            if (File.Exists(filePath))
            {
                lock (s_locker)
                {
                    File.Delete(filePath);
                }
                
            }
        }
        static MvcHtmlString WriteFile(this HtmlHelper html, string virtualPath, string content)
        {
            string filePath = html.ViewContext.HttpContext.Server.MapPath(virtualPath);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, content);
            }
            virtualPath = new UrlHelper(html.ViewContext.RequestContext).Content(virtualPath);
            return MvcHtmlString.Create(virtualPath);
        }
        public static void DeleteFieldDefs(string virtualPath = "~/res/defs/dyn/fields.js")
        {
            DeleteFile(virtualPath);
        }
        public static MvcHtmlString GenDynFieldDefs(this HtmlHelper html, string url = "~/res/defs/dyn/fields.js")
        {
            return html.WriteFile(url, GetFields());
        }
        public static void DeleteViewDefs(string virtualPath = "~/res/defs/dyn/views.js")
        {
            DeleteFile(virtualPath);
        }
        public static MvcHtmlString GenDynViewDefs(this HtmlHelper html, string url = "~/res/defs/dyn/views.js")
        {
            return html.WriteFile(url, GetViews());
        }

        static string MapToReactControl(ControlType ctrlType, DataType dataType)
        {
            if (ctrlType == ControlType.Auto)
            {
                switch (dataType)
                {
                    case DataType.Bool: ctrlType = ControlType.Checkbox; break;
                    case DataType.DateTime: ctrlType = ControlType.DatePicker; break;
                    default: ctrlType = ControlType.Textbox; break;
                }
            }
            switch (ctrlType)
            {
                case ControlType.Hidden: return "hidden";
                case ControlType.TextArea: return "textarea";
                case ControlType.Dropdown: return "dropdownlist";
                case ControlType.CheckboxList: return "checkboxlist";
                case ControlType.Checkbox: return "checkbox";
                case ControlType.RadioList: return "radiolist";
                case ControlType.DatePicker: return "datepicker";

            }
            return "textbox";
        }
        public static string GetFields()
        {
            const string FIELD_TEMPLATE = @"
{TYPE}_{FIELD_NAME}: {
    type: '{FIELD_TYPE}',
    dataField: '{FIELD_NAME}',
    label: '{FIELD_LABEL}',
    {DATA_SOURCE}
    validationRules: {VALIDATION_RULES},
    valueChangedFunc: {VALUE_CHANGED},
    {FIELD_OPTION}

}";
            const string FIELD_TEMPLATE_DS = @"
    dataSourceApi: window.rootUrl + 'api/item/refList',
    dataApiParamsFunc: function(s,u,c){
        var opt = s.props.parent.props.options;
        var ret = {type: '{REF_TYPE}'};
        if(opt){
            ret.parentId = opt.linkedItemId;
            ret.itemId = opt.itemId;
        }
        {FILTER_BY}
        return ret;
    },
";
            const string FIELD_TEMPLATE_DS_2 = @"
    dataSource: {DATA_LIST},
";
            const string FIELD_DEF = @"
Object.assign(bluemoon.reactjs.staticFieldDefs, {
    {ALL_FIELDS}
});
";
            StringBuilder fields = new StringBuilder();
            foreach (var type in CacheManager.AllItemTypes)
            {
                string typeName = type.Name;
                ItemType itemType = CacheManager.AllItemTypes[typeName];
                //ModelDefinition colDef = CacheManager.ModelInputs[itemType.ID];

                ModelEntity item = CacheManager.CreateModelEntity(itemType.ID);
                List<string> readOnlyFields = new List<string>();
                item.Properties.ForEach(p => {
                    if (p.Name != "Type" && p.Name != "ID")
                    {
                        string reactCtrl = MapToReactControl(p.InputType, p.DataType);
                        string ctrOptions = "options: null";
                        if (reactCtrl == "datepicker")
                        {
                            reactCtrl = "textbox";
                            ctrOptions = "options: { textType: 'date'}";
                        }
                        string field = FIELD_TEMPLATE.Replace("{TYPE}", typeName)
                            .Replace("{FIELD_NAME}", p.Name)
                            .Replace("{FIELD_LABEL}", p.Label)
                            .Replace("{FIELD_TYPE}", reactCtrl)
                            .Replace("{FIELD_OPTION}", ctrOptions)
                        ;
                        if (string.IsNullOrEmpty(p.DataSourceField))
                        {
                            field = field.Replace("{DATA_SOURCE}", "");
                        }
                        else
                        {
                            string ds = p.DataSourceField;
                            string filterBy = "";
                            if (ds.StartsWith("[") && ds.EndsWith("]"))
                            {
                                ds = ds.Replace("[", "[{},");
                                field = field.Replace("{DATA_SOURCE}", FIELD_TEMPLATE_DS_2.Replace("{DATA_LIST}", ds));
                            }
                            else
                            {
                                if (ds.IndexOf(":") > 0)
                                {
                                    string[] ps = ds.Split(':');
                                    field = field.Replace("{DATA_SOURCE}", FIELD_TEMPLATE_DS.Replace("{REF_TYPE}", ps[0]));
                                    filterBy = "ret.filterBy = {{field_by}: 0};".Replace("{field_by}", ps[1]);
                                }
                                else field = field.Replace("{DATA_SOURCE}", FIELD_TEMPLATE_DS.Replace("{REF_TYPE}", ds));
                            }
                            field = field.Replace("{FILTER_BY}", filterBy);
                        }
                        if (string.IsNullOrEmpty(p.Validation))
                        {
                            field = field.Replace("{VALIDATION_RULES}", "null");
                        }
                        else
                        {
                            field = field.Replace("{VALIDATION_RULES}", p.Validation);
                        }

                        if (p.ReadOnly)
                        {
                            readOnlyFields.Add(typeName + "_" + p.Name);
                        }

                        if (string.IsNullOrEmpty(p.OnChanged))
                        {
                            field = field.Replace("{VALUE_CHANGED}", "null");
                        }
                        else
                        {
                            field = field.Replace("{VALUE_CHANGED}", ParseOnChangedConditions(p.OnChanged, typeName));
                        }
                        fields.Append(field + ",");
                    }

                });
                if (readOnlyFields.Count > 0)
                {
                    fields.Append(@"{TYPE}_ID: {type: 'hidden', dataField: 'ID', valueChangedFunc: {VALUE_CHANGED}}"
                            .Replace("{TYPE}", typeName)
                            .Replace("{VALUE_CHANGED}", @"function (s, n, o, c) {
s.props.parent.toggleFields(n, [
                {
                    match:function(v) {return v!=null && v>0;}, 
                    fields: [
                        {
                            names:[" + "'" + string.Join("','", readOnlyFields) + "'" + @"],
                            action: function(f,ctrl) {ctrl.setEnable(false);}
                        }
                    ]
                }
            ])
        }") + ",");
                }
                else
                {
                    fields.Append(@"{TYPE}_ID: {type: 'hidden', dataField: 'ID'}".Replace("{TYPE}", typeName) + ",");
                }

            }
            return FIELD_DEF.Replace("{ALL_FIELDS}", fields.ToString().Trim(','));
        }
        static readonly Regex reg_PPP = new Regex(@"\{"".*?"":""\[v\]""\}", RegexOptions.Compiled);
        private static  string ParseOnChangedConditions(string condition, string typeName)
        {
            const string FIELD_TEMPLATE_VALUECHANGED_ENABLE = @"
function (s, n, o, c) {
    s.props.parent.toggleFields(n, [
        {MATCHED_OPTIONS}
    ])
}
";
            const string OPTION_TEMPLATE = @"{
            match:function(v){ return {MATCHED_CONDITION};}, 
            fields: [
                {
                    names:{MATCHED_FIELDS},
                    action: function(f,ctrl,v){{MATCHED_ACTIONS}}
                }
            ]
        }";
            StringBuilder sb = new StringBuilder();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            object[] options = (object[])serializer.DeserializeObject(condition);
            foreach (KeyValue opt in options)
            {
                KeyValue matchOpt = opt["match"] as KeyValue;
                string operatorCompare = string.Format("{0}", matchOpt["operator"]);
                //if (operatorCompare == "=") operatorCompare = "==";
                //if (operatorCompare == "=") operatorCompare = "==";
                string match = string.Format("v != null && v {0} '{1}'", operatorCompare, matchOpt["value"]);
                object[] fieldNames = (object[])opt["fields"];

                string fields = serializer.Serialize(fieldNames.Select(v => typeName + "_" + (string)v));
                KeyValue actionOpt = opt["actions"] as KeyValue;
                List<string> actions = new List<string>();
                foreach (var act in actionOpt)
                {
                    switch (act.Key)
                    {
                        case "visibility":
                            if (act.Value != null) actions.Add(string.Format("ctrl.setVisible({0});", act.Value.ToString() == "visible" ? "true" : "false"));
                            break;
                        case "ability":
                            if (act.Value != null) actions.Add(string.Format("ctrl.setEnable({0});", act.Value.ToString() == "enable" ? "true" : "false"));
                            break;
                        case "dataSource":
                            if (act.Value != null)
                            {
                                string ds = serializer.Serialize(act.Value);
                                if (reg_PPP.IsMatch(ds))
                                {
                                    actions.Add(string.Format("f.rebind(null, {{filterBy:{0}}});", ds.Replace("\"[v]\"", "v")));
                                }
                                else if (ds.StartsWith("{"))//expect to be a single object
                                {
                                    actions.Add(string.Format("f.rebind(null, {0});", ds.Replace("\"[v]\"", "v")));
                                }
                                else //expect to be an array
                                {
                                    actions.Add(string.Format("ctrl.setDataSource({0});", ds));
                                }

                            }

                            break;
                    }
                }
                sb.Append(OPTION_TEMPLATE.Replace("{MATCHED_CONDITION}", match)
                    .Replace("{MATCHED_FIELDS}", fields)
                    .Replace("{MATCHED_ACTIONS}", string.Join("", actions)) + ",");
            }
            return FIELD_TEMPLATE_VALUECHANGED_ENABLE.Replace("{MATCHED_OPTIONS}", sb.ToString().Trim(','));
        }
        
        public static string GetViews()
        {
            const string FIELD_TEMPLATE = @"
{ name: '{TYPE}_{FIELD_NAME}' }";
            const string ITEMVIEW_TEMPLATE = @"
{TYPE}_ItemView:{
    fields:[
        {ALL_FIELDS},
        {
            name: 'createdBy'
        }
    ],
    layout: {
        name: 'flowlayout'
    }
}
";
            const string DETAILVIEW_TEMPLATE = @"
{TYPE}_DetailView:{
    fields:[
        {ALL_FIELDS}
    ],
    layout: {
        name: 'gridlayout',
        options:{
            columns: 1
        }
    },
    dataApi: window.rootUrl + 'api/item/get',
    dataApiParamsFunc: function(sender, url){
        return { type: '{TYPE}', ID: sender.props.options.itemId, t: new Date().getTime()};
    },
    submitApi: window.rootUrl + 'api/item/save',
    submitApiParamsFunc: function(s,d){
        if(d.ID==null) d.ID = 0;
        return {type:'{TYPE}', data:d, parentID: s.props.options ? s.props.options.linkedItemId : null};
    },
    deleteApi: window.rootUrl + 'api/item/delete',
    deleteApiParamsFunc: function(s,d){
        return {type:'{TYPE}', ID:d.itemId};
    }
    
}
";
            const string VIEW_DEF = @"
Object.assign(bluemoon.reactjs.staticViewDefs, {
    {ALL_VIEWS}
});
";
            StringBuilder views = new StringBuilder();
            foreach (var type in CacheManager.AllItemTypes)
            {
                string typeName = type.Name;
                ItemType itemType = CacheManager.AllItemTypes[typeName];
                StringBuilder fields = new StringBuilder();
                ModelDefinition colDef = CacheManager.GetModelDef(itemType.ID);
                //item view fields
                fields.Clear();
                colDef.ForEach(p => {
                    if (p.Name != "Type" && (p.AllowShowGrid || p.Name == itemType.DisplayProperty))
                        fields.Append(FIELD_TEMPLATE.Replace("{TYPE}", typeName)
                            .Replace("{FIELD_NAME}", p.Name) + ",");
                });
                views.Append(ITEMVIEW_TEMPLATE.Replace("{TYPE}", typeName).Replace("{ALL_FIELDS}", fields.ToString().Trim(',')) + ",");

                //detail view fields
                fields.Clear();
                colDef.ForEach(p => {
                    if (p.Name != "Type")
                        fields.Append(FIELD_TEMPLATE.Replace("{TYPE}", typeName)
                            .Replace("{FIELD_NAME}", p.Name) + ",");
                });
                views.Append(DETAILVIEW_TEMPLATE.Replace("{TYPE}", typeName).Replace("{ALL_FIELDS}", fields.ToString().Trim(',')) + ",");
            }
            return VIEW_DEF.Replace("{ALL_VIEWS}", views.ToString().Trim(','));
        }
    }

}