using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace BlueMoon.DynWeb.Common
{
    public class Validation
    {
        public static Dictionary<string, Func<string, string, dynamic, string>> AllRules { get; private set; }
        static Validation()
        {
            AllRules = new Dictionary<string, Func<string, string, dynamic, string>>();
            AllRules.Add("required", (val, msg, opt) =>
            {
                return string.IsNullOrEmpty(val) ? msg : null;
            });
            AllRules.Add("max", (val, msg, opt) =>
            {
                return decimal.Parse(val) > opt["maxValue"] ? msg : null;
            });
            AllRules.Add("min", (val, msg, opt) =>
            {
                return decimal.Parse(val) < opt["minValue"] ? msg : null;
            });
            AllRules.Add("maxLength", (val, msg, opt) =>
            {
                return val.Length > opt["maxLength"] ? msg : null;
            });
            AllRules.Add("minLength", (val, msg, opt) =>
            {
                return val.Length > opt["minLength"] ? msg : null;
            });
            AllRules.Add("regExp", (val, msg, opt) =>
            {
                Regex reg = new Regex('^' + opt["pattern"] + '$');
                return reg.IsMatch(val) ? null : msg;
            });
            AllRules.Add("email", (val, msg, opt) =>
            {
                opt = new Dictionary<string, string>() { { "pattern", "[a-zA-Z0-9.!#$%&\'*+\\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*" } };
                return AllRules["regExp"](val, msg, opt);
            });
            AllRules.Add("url", (val, msg, opt) =>
            {
                opt = new Dictionary<string, string>() { { "pattern", "(?:(?:(?:https?|ftp):)?//)(?:\\S+(?::\\S*)?@)?(?:(?!(?:10|127)(?:\\.\\d{1,3}){3})(?!(?:169\\.254|192\\.168)(?:\\.\\d{1,3}){2})(?!172\\.(?:1[6-9]|2\\d|3[0-1])(?:\\.\\d{1,3}){2})(?:[1-9]\\d?|1\\d\\d|2[01]\\d|22[0-3])(?:\\.(?:1?\\d{1,2}|2[0-4]\\d|25[0-5])){2}(?:\\.(?:[1-9]\\d?|1\\d\\d|2[0-4]\\d|25[0-4]))|(?:(?:[a-z\\u00a1-\\uffff0-9]-*)*[a-z\\u00a1-\\uffff0-9]+)(?:\\.(?:[a-z\\u00a1-\\uffff0-9]-*)*[a-z\\u00a1-\\uffff0-9]+)*(?:\\.(?:[a-z\\u00a1-\\uffff]{2,})).?)(?::\\d{2,5})?(?:[/?#]\\S*)?" } };
                return AllRules["regExp"](val, msg, opt);
            });
            AllRules.Add("digits", (val, msg, opt) =>
            {
                opt = new Dictionary<string, string>() { { "pattern", "\\d+" } };
                return AllRules["regExp"](val, msg, opt);
            });
            AllRules.Add("number", (val, msg, opt) =>
            {
                opt = new  Dictionary<string,string>(){ { "pattern", "(?:-?\\d+|-?\\d{1,3}(?:,\\d{3})+)?(?:\\.\\d+)?" } };
                return AllRules["regExp"](val, msg, opt);
            });
        }
        public static string Validate(string attemptedValue, object[] rules)
        {
            
            foreach (Dictionary<string, object> rule in rules)
            {
                string msg = null;
                if (string.IsNullOrEmpty(attemptedValue))
                {
                    if (Validation.AllRules.ContainsKey("required"))
                    {
                        msg = Validation.AllRules[rule["rule"] as string](attemptedValue, rule["msg"] as string, rule.ContainsKey("options") ? rule["options"] : null);
                    }
                }
                else
                {
                    msg = Validation.AllRules[rule["rule"] as string](attemptedValue, rule["msg"] as string, rule.ContainsKey("options") ? rule["options"] : null);

                }
                if (msg != null) return msg;
            }
            return null;
        }
        public bool RequiredValid(string Content)
        {
            return !string.IsNullOrEmpty(Content);
        }
        public bool EmailValid(string Content)
        {
            Regex regex = new Regex(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$");
            Match match = regex.Match(Content);
            return match.Success;
            
        }
        public bool MoneyValid(string Content)
        {
            Regex regex = new Regex(@"^\d+(\.{0,1}\d{0,2})$");
            Match match = regex.Match(Content);
            return match.Success;
        }
        public bool CountValid(string Content)
        {
            Regex regex = new Regex(@"^\d+$");
            Match match = regex.Match(Content);
             return match.Success;
        }
    }
}