using BlueMoon.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BlueMoon.MVC.Controls;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BlueMoon.MVC
{
    public abstract class Controller : System.Web.Mvc.Controller
    {
        public static string UploadFolder { get; set; }
        public static string DownloadUrl { get; set; }
        public static IDataItemConverter DataItemConverter { get; set; }
        static Controller()
        {

            ModelBinders.Binders.DefaultBinder = new SpecialBinder();
        }
        protected ActionResult Download(string id, string name)
        {
            string uploadFolder = Controller.UploadFolder;
            Directory.CreateDirectory(uploadFolder);
            string filePath = uploadFolder + id + ".dat";
            return new FilePathResult(filePath, "application/octet-stream") { FileDownloadName = name };
        }
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new CustomJsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
        class CustomJsonResult : JsonResult
        {
            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }
                if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    //throw new InvalidOperationException("Not allow");
                }
                HttpResponseBase response = context.HttpContext.Response;
                if (!string.IsNullOrEmpty(this.ContentType))
                {
                    response.ContentType = this.ContentType;
                }
                else
                {
                    response.ContentType = "application/json";
                }
                if (this.ContentEncoding != null)
                {
                    response.ContentEncoding = this.ContentEncoding;
                }
                if (this.Data != null)
                {
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    javaScriptSerializer.RegisterConverters(new JavaScriptConverter[] { new DateTimeConverter() });
                    if (this.MaxJsonLength.HasValue)
                    {
                        javaScriptSerializer.MaxJsonLength = this.MaxJsonLength.Value;
                    }
                    if (this.RecursionLimit.HasValue)
                    {
                        javaScriptSerializer.RecursionLimit = this.RecursionLimit.Value;
                    }
                    string json = javaScriptSerializer.Serialize(this.Data);
                    //process data for DateTimeConverter: revert "{ServerDateTime}": "2021-06-08 21:37:43" to "2021-06-08 21:37:43"
                    json = s_regDateTime.Replace(json, m =>
                    {
                        return m.Groups["v"].Value;
                    });
                    //*
                    //encrypt Ids
                    List<string> sVal = new List<string>();
                    json = s_regJsonStrings.Replace(json, m =>
                    {
                        if (json[m.Index + m.Length] == ':') return m.Value;//ignore properties
                        string v = m.Groups["v"].Value;
                        int index = 0;
                        index = sVal.IndexOf(v);
                        if (index < 0)
                        {
                            index = sVal.Count;
                            sVal.Add(v);
                        }
                        return m.Value.Replace(v, S_IND + index);
                    });

                    json = s_regIDs.Replace(json, m =>
                    {
                        string v = m.Groups["v"].Value;
                        var vs = v.Split(',');
                        for (int i = 0; i < vs.Length; i++)
                        {
                            v = vs[i].Trim('"');
                            if (v.StartsWith(S_IND)) v = sVal[int.Parse(v.Replace(S_IND, ""))];
                            vs[i] = v.Trim('"').Encrypt();
                        }
                        v = "\"" + string.Join("\",\"", vs) +"\"";
                        return m.Value.Replace(m.Groups["v"].Value, string.Format("{0}", v));
                    });

                    json = s_regID.Replace(json, m =>
                    {
                        string v = m.Groups["v"].Value;
                        v = v.Trim('"');
                        if (v.StartsWith(S_IND)) v = sVal[int.Parse(v.Replace(S_IND, ""))];
                        v = v.Encrypt();
                        return m.Value.Replace(m.Groups["v"].Value, string.Format("\"{0}\"", v));
                    });

                    json = s_regReturnStrings.Replace(json, m =>
                    {
                        string v = m.Groups["v"].Value;
                        return "\""+ sVal[int.Parse(m.Groups["v"].Value)] +"\"";
                    });
                    //*/
                    response.Write(json);
                }
            }
            const string S_IND = "sss";
            static Regex s_regDateTime = new Regex(@"{""{ServerDateTime}"":(?<v>.*?)}", RegexOptions.Compiled);
            static Regex s_regID = new Regex(@"""\w*?id"":(?<v>.*?)[,}]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            static Regex s_regIDs = new Regex(@"""\w*?ids"":\[(?<v>.*?)\]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            static Regex s_regJsonStrings = new Regex(@"[^\\]""(?<v>.*?[^\\])""", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            static Regex s_regReturnStrings = new Regex(@"""sss(?<v>\d*?)""", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

            //static Regex s_regValue = new Regex(@"""value"":(?<v>.*?)[,}]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
        class DateTimeConverter : JavaScriptConverter
        {
            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                string sDate = ((DateTime)obj).ToString("yyyy-MM-dd HH:mm:ss");
                if (sDate.EndsWith("00:00:00")) sDate = sDate.Replace("00:00:00", "").Trim();
                result.Add("{ServerDateTime}", sDate);
                return result;
            }

            public override IEnumerable<Type> SupportedTypes
            {
                get { return new Type[] { typeof(DateTime) }; }
            }
        }
    }
    
}
