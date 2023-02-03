using BlueMoon.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.WebPages;

namespace BlueMoon.MVC.Controls
{
    
    public enum ControlType
    {
        None,
        Auto,
        Custom,
        Hidden,
        Textbox,
        Checkbox,
        Dropdown,
        DatePicker,
        FileUpload,
        TextArea,
        RadioList,
        CheckboxList
    }
    public class InputAttribute : Attribute
    {
        public DataType DataType { get; set; }
        public ControlType InputType { get; private set; }
        public string Label { get; private set; }
        public string ListDataSourceField { get; private set; }
        public string Validation { get; set; }

        public InputAttribute(string label = null, ControlType inputType = ControlType.Auto, string listDataSourceField = null)
        {
            InputType = inputType;
            Label = label;
            ListDataSourceField = listDataSourceField;
        }
        public InputAttribute(ControlType inputType, string listDataSourceField = null)
            : this(null, inputType, listDataSourceField)
        { }
    }

    public class ItemHtml
    {
        public string Label { get; set; }
        public string Input { get; set; }
    }

    public class InputItem
    {
        public InputItem(string name, object data)
        {
            Name = name;
            Data = data;
        }
        public string Name { get; private set; }
        public object Data { get; private set; }
    }
    public class InputItem<T> : InputItem
    {
        public InputItem(string name, T data)
            : base(name, data) { }
        public new T Data { get { return (T)base.Data; } }
    }

    public class PropertyDefinition
    {
        public DataType DataType { get; private set; }
        public string Name { get; private set; }
        public ControlType InputType { get; private set; }
        public string Label { get; private set; }
        public string DataSourceField { get; private set; }
        public string Validation { get; private set; }
        public string DisplayFormat { get; private set; }
        public bool AllowShowGrid { get; private set; }
        public bool ReadOnly { get; private set; }
        public string OnChanged { get; private set; }
        public bool AllowSearch { get; private set; }
        public Func<InputItem, HelperResult> InputTemplate { get; private set; }
        protected PropertyDefinition()
        {
        }
        public PropertyDefinition Clone()
        {
            return new PropertyDefinition()
            {
                Name = this.Name,
                DataType = this.DataType,
                AllowShowGrid = this.AllowShowGrid,
                InputType = this.InputType,
                Label = this.Label,
                DataSourceField = this.DataSourceField,
                Validation = this.Name,
                ReadOnly = this.ReadOnly,
                OnChanged = this.OnChanged,
                AllowSearch = this.AllowSearch,
                DisplayFormat = this.DisplayFormat,
                InputTemplate = this.InputTemplate
            };
        }
        
        public PropertyDefinition(string name, DataType dataType, bool allowShowGrid = false, ControlType input = ControlType.Auto, string label = null, string dataSourceField = null, string validation = null, bool readOnly = false, string onChanged = null, bool allowSearch = false, string displayFormat = null): this()
        {
            Name = name;
            DataType = dataType;
            AllowShowGrid = allowShowGrid;
            InputType = input;
            Label = label;
            DataSourceField = dataSourceField;
            Validation = validation;
            ReadOnly = readOnly;
            OnChanged = onChanged;
            AllowSearch = allowSearch;
            DisplayFormat = displayFormat;

        }

        public PropertyDefinition(string name, DataType dataType, bool allowShowGrid, Func<InputItem, HelperResult> template = null, string label = null, string dataSourceField = null, string validation = null, bool readOnly = false, string onChanged = null, bool allowSearch = false, string displayFormat = null)
            : this(name, dataType, allowShowGrid, ControlType.Custom, label, dataSourceField, validation, readOnly, onChanged, allowSearch, displayFormat)
        {
            InputTemplate = template;
        }
        public MvcHtmlString Display(DataItem data)
        {
            string format = DisplayFormat ?? "{0}";
            if (DataType == DataType.File)
            {
                if (data[Name] != DBNull.Value)
                {
                    format = "<a href=\"{0}\">{1}</a>";
                    FileDataInfo info = FileDataInfo.LoadInfo((Guid)data[Name]);
                    return MvcHtmlString.Create(string.Format(format, string.Format(Controller.DownloadUrl, info.ID, info.FileName), info.FileName));
                }
                else
                {
                    format = "{0}";
                }
            }
            return MvcHtmlString.Create(string.Format(format, data[Name]));
        }

    }

    public class ModelDefinition : List<PropertyDefinition>
    {
        public string Name { get; set; }
        public string KeyField { get; set; }
        public string DisplayField { get; set; }
        public ModelDefinition(string name, string keyField, string displayField)
        {
            Name = name;
            KeyField = keyField;
            DisplayField = displayField;
        }
        public ModelDefinition(string name, string keyField, string displayField, IEnumerable<PropertyDefinition> collection)
            : base(collection)
        {
            Name = name;
            KeyField = keyField;
            DisplayField = displayField;
        }
        public PropertyDefinition this[string name]
        {
            get
            {
                return this.Find(o => o.Name == name);
            }
        }
        public ModelDefinition Append(PropertyDefinition item)
        {
            base.Add(item);
            return this;
        }
    }
    public class ModelControlConfiguration
    {
        public string LabelClass { get; set; }
        public string InputClass { get; set; }
        public string ValidateFunction { get; set; }
        public Func<ItemHtml, HelperResult> ItemTemplate { get; set; }
    }
    public class TemplateCollection<T> : Dictionary<string, Func<InputItem<T>, HelperResult>>
    {
        public TemplateCollection<T> Append(string field, Func<InputItem<T>, HelperResult> template)
        {
            Add(field, template);
            return this;
        }
    }
    public class ModelControlConfiguration<T> : ModelControlConfiguration
    {
        public TemplateCollection<T> CustomInputs { get; set; }
    }
    public static partial class UIExtension
    {
        static object BindData(this object source, string field)
        {
            if (source == null) return null;
            if (source is DataItem)
            {
                DataItem data = (DataItem)source;
                if (data.ContainsKey(field)) return data[field];
                else return null;
            }
            else
            {
                return DataBinder.Eval(source, field);
            }
        }
        public static MvcHtmlString InputFor(this HtmlHelper html, ModelDefinition defi, object dataBind = null, ModelControlConfiguration config = null)
        {
            html.RegisterScript(ExtractEmbeddedResource("validation.js"));
            StringBuilder outHtml = new StringBuilder();
            ItemHtml itemHtml = new ItemHtml();
            Func<ItemHtml, HelperResult> itemTemplate = null;
            if (config == null) config = new ModelControlConfiguration();
            itemTemplate = config.ItemTemplate;
            foreach (var p in defi)
            {
                itemHtml.Input = itemHtml.Label = "";
                ControlType inputType = p.InputType;
                if (inputType == ControlType.Auto)
                {
                    //reassign inputType
                    switch (p.DataType)
                    {
                        case DataType.Bool:
                            inputType = ControlType.Checkbox;
                            break;
                        case DataType.DateTime:
                            inputType = ControlType.DatePicker;
                            break;
                        case DataType.Text:
                            inputType = ControlType.TextArea;
                            break;
                        case DataType.File:
                            inputType = ControlType.FileUpload;
                            break;
                        default:
                            inputType = ControlType.Textbox;
                            break;
                    }

                }
                if (inputType == ControlType.None) continue;
                string label = p.Label ?? p.Name;
                object attObj = new { };
                if (!config.LabelClass.IsNullOrEmpty()) attObj = new { @class = config.LabelClass };
                itemHtml.Label = html.Label(label, attObj).ToString();

                object value = dataBind == null ? null : dataBind.BindData(p.Name);
                string name = (string.IsNullOrEmpty(defi.Name) ? "" : defi.Name + ".") + p.Name;
                string validation = p.Validation;
                switch (inputType)
                {
                    
                    case ControlType.Custom:
                        {
                            if (p.InputTemplate != null)
                            {
                                itemHtml.Input = p.InputTemplate.Invoke(new InputItem(name, dataBind)).ToString();
                            }
                        }
                        break;

                    case ControlType.Textbox:
                        {
                            itemHtml.Input = html.TextBox(name, value).ToString();
                        }
                        break;
                    case ControlType.Checkbox:
                        {
                            string shtml = html.CheckBox(name, (bool)(value ?? (object)false)).ToString();
                            shtml = shtml.Substring(0, shtml.IndexOf("/>") + 2);
                            itemHtml.Input = shtml;

                        }
                        break;
                    case ControlType.Hidden:
                        {
                            itemHtml.Input = html.SecuredHidden(name, value).ToString();
                        }
                        break;
                    case ControlType.DatePicker:
                        {
                            itemHtml.Input = html.TextBox(name, value, new { type = "date" }).ToString();
                        }
                        break;
                    case ControlType.TextArea:
                        {
                            itemHtml.Input = html.TextArea(name, value).ToString();
                        }
                        break;
                    case ControlType.FileUpload:
                        {
                            itemHtml.Input = html.FileUpload(name, value).ToString();
                        }
                        break;
                    case ControlType.Dropdown:
                        {
                            IEnumerable<SelectListItem> ds = (IEnumerable<SelectListItem>)dataBind.BindData(p.DataSourceField);
                            if (ds != null)
                            {
                                foreach (var i in ds)
                                {
                                    i.Selected = (value != null && i.Value == value.ToString());
                                }
                            }
                            itemHtml.Input = html.DropDownList(name, ds).ToString();
                        }
                        break;
                    case ControlType.RadioList:
                        {
                            IEnumerable<SelectListItem> ds = (IEnumerable<SelectListItem>)dataBind.BindData(p.DataSourceField);
                            if (ds != null)
                            {
                                foreach (var i in ds)
                                {
                                    i.Selected = (value != null && i.Value == value.ToString());
                                }
                                itemHtml.Input = html.RadioList(name, ds).ToString();
                            }
                        }
                        break;
                    case ControlType.CheckboxList:
                        {
                            IEnumerable<SelectListItem> ds = (IEnumerable<SelectListItem>)dataBind.BindData(p.DataSourceField);
                            if (ds != null)
                            {
                                foreach (var i in ds)
                                {
                                    i.Selected = (value != null && i.Value == value.ToString());
                                }
                                itemHtml.Input = html.CheckboxList(name, ds).ToString();
                            }
                        }
                        break;
                }
                
                itemHtml.Input = string.IsNullOrEmpty(validation) ? string.Format("<span class=\"model-input\">{0}</span>", itemHtml.Input) : string.Format("<span class=\"model-input\" data-validation=\"{1}\">{0}</span>", itemHtml.Input, HttpUtility.HtmlAttributeEncode(validation));
                if (inputType == ControlType.Hidden)
                {
                    outHtml.Append(itemHtml.Input);
                }
                else if (itemTemplate == null)
                {
                    outHtml.Append(string.Format("<div>{0}{1}</div>", itemHtml.Label, itemHtml.Input));
                }
                else
                {
                    outHtml.Append(HttpUtility.HtmlDecode(itemTemplate.Invoke(itemHtml).ToString()));
                }

            }
            string divID = "cid" + defi.Name + "Panel";
            string script = "";
            if (!string.IsNullOrEmpty(config.ValidateFunction))
            {
                script = string.Format("<script type=\"text/javascript\">function {0}(){{ return validateModel(document.getElementById('{1}')) }}</script>", config.ValidateFunction, divID);
            }
            return new MvcHtmlString("<div id=\""+ divID +"\">" + outHtml.ToString() + script + "</div>");
        }
        public static MvcHtmlString InputForModel<T>(this HtmlHelper html,string name, T dataBind = default(T), ModelControlConfiguration<T> args = null) where T : class
        {
            Type type = typeof(T);
            PropertyInfo[] props = type.GetProperties();
            ModelDefinition defi = new ModelDefinition(name, null, null);
            foreach (var p in props)
            {
                var att = p.GetCustomAttribute<InputAttribute>();
                if (att != null)
                {
                    if (p.PropertyType.IsAssignableFrom(typeof(bool)))
                    {
                        att.DataType = DataType.Bool;
                    }
                    else if (p.PropertyType.IsAssignableFrom(typeof(DateTime)))
                    {
                        att.DataType = DataType.DateTime;
                    }
                    else if (p.PropertyType.IsAssignableFrom(typeof(decimal)))
                    {
                        att.DataType = DataType.Decimal;
                    }
                    else if (p.PropertyType.IsAssignableFrom(typeof(int)))
                    {
                        att.DataType = DataType.Int;
                    }
                    
                    ControlType inputType = att.InputType;
                    if (inputType == ControlType.Custom)
                    {
                        defi.Add(new PropertyDefinition(p.Name, att.DataType, true, new Func<InputItem, HelperResult>(o =>
                        {
                            return args.CustomInputs[p.Name].Invoke(new InputItem<T>(o.Name, (T)o.Data));
                        }), att.Label, att.ListDataSourceField, att.Validation));
                    }
                    else
                    {
                        defi.Add(new PropertyDefinition(p.Name, att.DataType, true, att.InputType, att.Label, att.ListDataSourceField, att.Validation));
                    }
                }
            }
            return InputFor(html, defi, dataBind, args);
        }
    }

}