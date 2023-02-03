using BlueMoon.Business;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;
namespace BlueMoon.MVC.Controls
{
    public static partial class UIExtension
    {
        public static MvcHtmlString SingleChoice(this HtmlHelper html, string name, object value)
        {
            return null;
        }
        public static MvcHtmlString MultiChoice(this HtmlHelper html, string name, object value)
        {
            return null;
        }
        public class Column
        {
            public string HeaderText { get; set; }
            public string HeaderCssClass { get; set; }
            public string DataField { get; set; }
            public string ItemCssClass { get; set; }
            public string ItemDataFormat { get; set; }
            public string Width { get; set; }
            public Func<object, HelperResult> ItemTemplate { get; set; }
        }
        public static MvcHtmlString DivTable(this HtmlHelper html, IEnumerable data, IEnumerable<Column> columns,string noDataMsg = "No data")
        {
            html.RegisterScript(ExtractEmbeddedResource("div-table.js"));
            html.RegisterStyle(ExtractEmbeddedResource("div-table.css"));
            StringBuilder outHtml = new StringBuilder();
            outHtml.Append("<div class=\"l-table\">");
            //header
            outHtml.Append("<div class=\"l-head\"><div class=\"l-row\">");
            foreach (Column col in columns)
            {
                if (col.Width.IsNullOrEmpty()) col.Width = "200px";
                outHtml.Append(string.Format("<div class=\"l-col\" style=\"width:{1};\"><label>{0}</label></div>", col.HeaderText, col.Width));
            }
            outHtml.Append("</div></div>");
            //end header

            //data
            outHtml.Append("<div class=\"l-body\">");
            if (data == null)
            {
                outHtml.Append(string.Format("<div class=\"l-row\"><div class=\"l-no-item\">{0}.</div></div>", noDataMsg));
            }
            else foreach (var obj in data)
            {
                //row
                outHtml.Append("<div class=\"l-row\">");
                foreach (Column col in columns)
                {
                    string displayValue ="";

                    
                    if (col.ItemTemplate == null)
                    {
                        string dataFormat = "{0}";
                        if (!string.IsNullOrEmpty(col.ItemDataFormat))
                        {
                            dataFormat = col.ItemDataFormat;
                        }
                        displayValue = string.Format(dataFormat, obj.BindData(col.DataField));        
                    }
                    else
                    {
                        displayValue = col.ItemTemplate.Invoke(obj).ToHtmlString();
                    }
                    outHtml.Append(string.Format("<div class=\"l-col\"  style=\"width:{2};\"><label>{1}</label><div>{0}</div></div>", displayValue, col.HeaderText, col.Width));
                }
                outHtml.Append("</div>");
                //end row
            }
            outHtml.Append("</div>");
            //end data

            outHtml.Append("</div>");
            return MvcHtmlString.Create(outHtml.ToString());
        }
        public static MvcHtmlString Pager(this HtmlHelper html,string name, int page, int pageSize, int totalRows, string text = "Page", int maxDisplayedPage = 7)
        {
            string url = html.ViewContext.HttpContext.Request.RawUrl;
            NameValueCollection queryString = null;
            int pos = url.IndexOf('?');
            if (pos >= 0)
            {
                queryString = HttpUtility.ParseQueryString(url.Substring(pos + 1));
                url = url.Substring(0, pos);
                if (queryString.Count > 0)
                {
                    url += "?";
                    for (int i = 0; i < queryString.Count; i++)
                    {
                        if (queryString.AllKeys[i] != name)
                        {
                            url += queryString.AllKeys[i] + "=" + HttpUtility.UrlEncode(queryString[i]) + "&";
                        }
                    }
                }
            }
            else
            {
                url += "?";
            }
            url += name + "=";




            bool _ajaxEnable = false;
            int centerPage = maxDisplayedPage / 2;
            int totalPage = totalRows / pageSize;
            if (totalRows % pageSize != 0) totalPage++;

            if (totalPage <= 1) return MvcHtmlString.Empty;

            int totalGroup = totalPage / maxDisplayedPage;
            if (totalPage % maxDisplayedPage != 0) totalGroup++;

            int currentGroup = page / maxDisplayedPage;
            if (page % maxDisplayedPage != 0) currentGroup++;
            StringBuilder output = new StringBuilder("<ul class=\"pager\">");
            if (!string.IsNullOrEmpty(text)) output.Append("<li>" + text + "</li>");
            if (page <= centerPage + 1 || totalPage <= maxDisplayedPage)
            {
                for (int i = 1; i <= maxDisplayedPage && i <= totalPage; i++)
                {
                    if (i == page)
                    {
                        output.Append("<li class=\"current\"><a href=\"#\" onclick=\"return false;\">" + i + "</a></li>");
                    }
                    else
                    {
                        output.Append("<li><a href=\"" + url + i + "\"{0}>" + i + "</a></li>");
                    }

                }
                if (totalPage > maxDisplayedPage) output.Append("<li>...</li><li><a href=\"" + url + totalPage + "\"{0}>" + totalPage + "</a></li>");
            }
            else if (page > totalPage - centerPage - 1)
            {
                if (totalPage > maxDisplayedPage) output.Append("<li><a href=\"" + url + "1\"{0}>1</a></li><li>...</li>");
                for (int i = totalPage - maxDisplayedPage + 1; i <= totalPage; i++)
                {

                    if (i == page)
                    {
                        output.Append("<li class=\"current\"><a href=\"#\" onclick=\"return false;\">" + i + "</a></li>");
                    }
                    else
                    {
                        output.Append("<li><a href=\"" + url + i + "\"{0}>" + i + "</a></li>");
                    }
                }
            }
            else
            {
                output.Append("<li><a href=\"" + url + "1\"{0}>1</a></li><li>...</li>");
                for (int i = page - centerPage; i <= page + centerPage && i <= totalPage; i++)
                {

                    if (i == page)
                    {
                        output.Append("<li class=\"current\"><a href=\"#\" onclick=\"return false;\">" + i + "</a></li>");
                    }
                    else
                    {
                        output.Append("<li><a href=\"" + url + i + "\"{0}>" + i + "</a></li>");
                    }
                }
                output.Append("<li>...</li><li><a href=\"" + url + totalPage + "\"{0}>" + totalPage + "</a></li>");
            }
            output.Append("</ul><br style=\"clear:both;\"/>");
            return MvcHtmlString.Create(string.Format(output.ToString(), _ajaxEnable ? " onclick=\"return dhn.applyAjaxForPagerLink(this);\"" : ""));
        }
        public static MvcHtmlString SecuredHiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.HiddenFor(expression, new { @readonly = "true", @protected = "true" });
            
        }
        public static MvcHtmlString SecuredHidden(this HtmlHelper htmlHelper, string name,object value)
        {
            return htmlHelper.Hidden(name, value, new { @readonly = "true", @protected = "true" });
        }
        public static MvcHtmlString RadioList(this HtmlHelper html, string name, IEnumerable<SelectListItem> datasource)
        {
            StringBuilder radioButton = new StringBuilder();
            TagBuilder radio = null;
            TagBuilder label = null;
            foreach (SelectListItem item in datasource)
            {
                radio = new TagBuilder("input");
                label = new TagBuilder("label");
                label.InnerHtml = item.Text;
                radio.Attributes.Add("type", "radio");
                radio.Attributes.Add("name", name);
                radio.Attributes.Add("value", item.Value);
                radioButton.Append(label.ToString());
                radioButton.Append(radio.ToString(TagRenderMode.SelfClosing));
            }
            return MvcHtmlString.Create(radioButton.ToString());
        }
        
        public static MvcHtmlString CheckboxList(this HtmlHelper html, string name, IEnumerable<SelectListItem> datasource)
        {
            StringBuilder checkboxButton = new StringBuilder();
            TagBuilder checkbox = null;
            TagBuilder label = null;
            foreach (SelectListItem item in datasource)
            {
                checkbox = new TagBuilder("input");
                label = new TagBuilder("label");
                checkbox.InnerHtml = item.Text;
                checkbox.Attributes.Add("type", "checkbox");
                checkbox.Attributes.Add("name", name);
                checkbox.Attributes.Add("value", item.Value);
                if (item.Selected)
                    checkbox.Attributes.Add("checked", "checked");
                checkboxButton.Append(checkbox.ToString(TagRenderMode.SelfClosing));
                checkboxButton.Append(label.ToString());
            }
            return MvcHtmlString.Create(checkboxButton.ToString());
        }
        //public static MvcHtmlString DatePicker(this HtmlHelper html, string name, object value)
        //{
        //    //return html.InputFor()
        //    html.RegisterScript(ExtractEmbeddedResource("jquery-ui.js"));
        //    html.RegisterStyle(ExtractEmbeddedResource("jquery-ui.css"));
        //    string id = name.Replace('.', '_');
        //    StringBuilder datepicker = new StringBuilder();
        //    TagBuilder tag = new TagBuilder("input");
        //    tag.Attributes.Add("name", name);
        //    tag.Attributes.Add("id", id);
        //    if (value == System.DBNull.Value)
        //    {
        //        //tag.Attributes.Add("value", "01-01-1990");
        //    }
        //    else
        //    {
        //        tag.Attributes.Add("value", (Convert.ToDateTime(value)).ToString("MM-dd-yyyy"));

        //    }
        //    tag.Attributes.Add("type", "text");

        //    string scriptValue = string.Format("<script type=\"text/javascript\">$('#{0}').datepicker({{changeYear: true,changeMonth: true}});</script>", id);
        //    datepicker.Append(tag.ToString(TagRenderMode.SelfClosing));
        //    datepicker.Append(scriptValue);
        //    return MvcHtmlString.Create(datepicker.ToString());
        //}
        public static MvcHtmlString FileUpload(this HtmlHelper html, string name, object value)
        {
            string id = name.Replace('.', '_');
            html.RegisterScript(ExtractEmbeddedResource("fileupload.js"));
            if (value == null || value is DBNull)
            {
                TagBuilder tagFile = new TagBuilder("input");
                tagFile.Attributes.Add("name", name);
                tagFile.Attributes.Add("id", id);
                tagFile.Attributes.Add("type", "file");
                return MvcHtmlString.Create(tagFile.ToString(TagRenderMode.SelfClosing));
            }
            else
            {
                FileDataInfo fileInfo = FileDataInfo.LoadInfo((Guid)value);
                StringBuilder tagFile = new StringBuilder();
                tagFile.AppendLine("<div>");
                TagBuilder tagBuilder = new TagBuilder("a");
                tagBuilder.InnerHtml = fileInfo.FileName;
                tagBuilder.Attributes.Add("href", string.Format(Controller.DownloadUrl, fileInfo.ID, fileInfo.FileName));
                tagFile.Append(tagBuilder);
                tagBuilder = new TagBuilder("input");
                tagBuilder.Attributes.Add("name", name);
                tagBuilder.Attributes.Add("type", "hidden");
                tagBuilder.Attributes.Add("value", value.ToString());
                tagFile.Append(tagBuilder.ToString(TagRenderMode.SelfClosing));

                tagBuilder = new TagBuilder("button");
                tagBuilder.Attributes.Add("name", name);
                tagBuilder.Attributes.Add("type", "button");
                tagBuilder.Attributes.Add("onclick", "removeUploadFile(this)");
                tagBuilder.InnerHtml = "Delete";
                tagFile.Append(tagBuilder);

                tagFile.AppendLine("</div>");

                return MvcHtmlString.Create(tagFile.ToString());
            }
        }
    }
}
