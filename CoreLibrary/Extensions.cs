using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BlueMoon.MVC.Controls
{
    public static partial class UIExtension
    {
        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider descriptor, bool inherit)
        {
            var atts = descriptor.GetCustomAttributes(inherit);
            var result  = atts.Where(p => p is T).Select(p => (T)p).ToArray();
            return result.Length > 0 ? result: null;
        }
        public static bool HasCustomAttribute<T>(this ICustomAttributeProvider descriptor, bool inherit)
        {
            var atts = descriptor.GetCustomAttributes(inherit);
            return atts.Any(p => p is T);
        }
        static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
        public static string ExtractEmbeddedResource(string resourceName)
        {
            
            return ExtractEmbeddedResource<BlueMoon.MVC.Controller>("BlueMoon.EmbedResources." + resourceName);
        }
        public static string ExtractEmbeddedResource<T>(string resourceName)
        {

            string resFolder = HttpContext.Current.Server.MapPath("~/EmbeddedResources");
            string resFile = resFolder + "\\" + resourceName;
            if (!File.Exists(resFile))
            {
                Directory.CreateDirectory(resFolder);
                using (Stream s = typeof(T).Assembly.GetManifestResourceStream(resourceName))
                {
                    using (var w = new FileStream(resFile, FileMode.OpenOrCreate))
                    {
                        int readBytes = 0;
                        byte[] buffers = new byte[1024];
                        do
                        {
                            readBytes = s.Read(buffers, 0, buffers.Length);
                            if (readBytes > 0) w.Write(buffers, 0, readBytes);
                        } while (readBytes > 0);
                    }
                }
                if (resFile.EndsWith(".css") || resFile.EndsWith(".js"))
                {
                    StreamReader rd = new StreamReader(resFile);
                    string content = rd.ReadToEnd();
                    rd.Close();
                    content = Regex.Replace(content, @"res::[\w.]+", m =>
                    {
                        return VirtualPathUtility.ToAbsolute(ExtractEmbeddedResource<T>(m.Value.Replace("res::", "")));
                    });
                    StreamWriter wt = new StreamWriter(resFile, false);
                    wt.Write(content);
                    wt.Close();
                }
            }
            //~/EmbeddedResources
            return VirtualPathUtility.ToAbsolute("~/EmbeddedResources/" + resourceName);
        }
        static void RegisterResource(this HtmlHelper html, bool isScript, string url)
        {
            HttpContextBase context = html.ViewContext.HttpContext;
            List<string> lst = null;
            string key = "RegisteredStyles";
            if (isScript) key = "RegisteredScripts";
            lst = (List<string>)context.Items[key];
            if (lst == null)
            {
                lst = new List<string>();
                context.Items[key] = lst;
            }
            if (!lst.Contains(url)) lst.Add(url);
        }
        static MvcHtmlString RenderResources(this HtmlHelper html, bool isScript)
        {
            HttpContextBase context = html.ViewContext.HttpContext;
            StringBuilder outHtml = new StringBuilder();
            IEnumerable<string> lst = null;

            if (isScript)
            {
                lst = (IEnumerable<string>)context.Items["RegisteredScripts"];
                if (lst != null)
                {
                    foreach (string f in lst)
                    {
                        outHtml.Append(string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", f));
                    }
                }
            }
            else
            {
                lst = (IEnumerable<string>)context.Items["RegisteredStyles"];
                if (lst != null)
                {
                    foreach (string f in lst)
                    {
                        outHtml.Append(string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", f));
                    }
                }
            }
            return MvcHtmlString.Create(outHtml.ToString());
        }
        public static void RegisterStyle(this HtmlHelper html,string url)
        {
            html.RegisterResource(false, url);
        }
        public static void RegisterScript(this HtmlHelper html, string url)
        {
            html.RegisterResource(true, url);
        }
        internal static MvcHtmlString RenderRegisteredScripts(this HtmlHelper html)
        {
            return html.RenderResources(true);
        }
        internal static MvcHtmlString RenderRegisteredStyles(this HtmlHelper html)
        {
            return html.RenderResources(false);
        }
        public static MvcHtmlString Version(this HtmlHelper html, string url)
        {
            string filePath = html.ViewContext.HttpContext.Server.MapPath(url);
            if (!File.Exists(filePath)) return new MvcHtmlString(url);
            string version = "";
            using (var md5 = MD5.Create())
            {
                using(var stream = File.OpenRead(filePath))
                {
                    version = BitConverter.ToString(md5.ComputeHash(stream));
                }
            }
            url = new UrlHelper(html.ViewContext.RequestContext).Content(url);
            return MvcHtmlString.Create(url + "?v="+ version);
        }



    }
    public static class HelperExtension
    {
        const string ENC_PRE = "secured:";
        const string ENC_SUR = ":deruces";
        static Regex s_encryptedValues = new Regex(ENC_PRE + ".+?" + ENC_SUR, RegexOptions.Compiled);
        public static string Encrypt(this string val)
        {

            val = Encoding.UTF8.GetBytes(val).ToBase32String();//encrypt here
            return ENC_PRE + val + ENC_SUR;
        }
        public static string DecryptAll(this string val)
        {
            val = s_encryptedValues.Replace(val, m =>
            {
                return m.Value.Decrypt();
            });
            return val;
        }
        public static string Decrypt(this string val)
        {
            if (!IsEncrypted(val)) return val;
            val = val.Substring(ENC_PRE.Length, val.Length - ENC_PRE.Length - ENC_SUR.Length);
            val = Encoding.UTF8.GetString(val.FromBase32String());//decrypt here
            return val;
        }
        public static bool IsEncrypted(this string val)
        {
            if (string.IsNullOrEmpty(val)) return false;
            return val.StartsWith(ENC_PRE) && val.EndsWith(ENC_SUR);
        }

        internal const string CODE_CHARS = "abcdefghijklmnopqrstuvwxyz012345";
        internal const byte BIT5 = 0x5;
        internal const byte BIT8 = 0x8;
        private static char[] char_table;
        static string ToBase32String(this byte[] data)
        {
            if (data == null) return "";
            StringBuilder binaryString = new StringBuilder();

            foreach (byte b in data)
            {
                binaryString.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            int blocks = binaryString.Length % BIT5 == 0 ? binaryString.Length / BIT5 : binaryString.Length / BIT5 + 1;
            string[] bit5Array = new string[blocks];
            int readSize = 0;
            for (int i = 0; i < blocks; i++)
            {
                readSize = BIT5;
                if (i * BIT5 + BIT5 > binaryString.Length)
                {
                    readSize = binaryString.Length - i * BIT5;
                }
                bit5Array[i] = binaryString.ToString(i * BIT5, readSize);
                bit5Array[i] = bit5Array[i].PadRight(BIT5, '0');
                bit5Array[i] = bit5Array[i].PadLeft(BIT8, '0');
            }
            byte[] encodedBytes = new byte[bit5Array.Length];
            for (int i = 0; i < bit5Array.Length; i++)
            {
                encodedBytes[i] = Convert.ToByte(bit5Array[i], 2);
            }
            if (char_table == null)
            {
                char_table = CODE_CHARS.ToCharArray();
            }
            StringBuilder encodedString = new StringBuilder();
            foreach (byte b in encodedBytes)
            {
                encodedString.Append(char_table[b]);
            }
            return encodedString.ToString();
        }
        static byte[] FromBase32String(this string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return null;
            }
            str = str.ToLower();
            StringBuilder binaryString = new StringBuilder();
            foreach (char c in str.ToCharArray())
            {
                binaryString.Append(Convert.ToString(CODE_CHARS.IndexOf(c), 2).PadLeft(BIT8, '0'));
            }
            int n = binaryString.Length / BIT8;
            string[] bit8Array = new string[n];
            for (int i = 0; i < n; i++)
            {
                bit8Array[i] = binaryString.ToString(i * BIT8, BIT8).Substring(BIT8 - BIT5);
            }
            string bit8String = String.Join("", bit8Array);
            bit8Array = new string[bit8String.Length / BIT8];
            for (int i = 0; i < bit8Array.Length; i++)
            {
                bit8Array[i] = bit8String.Substring(i * BIT8, BIT8);
            }
            byte[] decodedBytes = new byte[bit8Array.Length];
            for (int i = 0; i < decodedBytes.Length; i++)
            {
                decodedBytes[i] = Convert.ToByte(bit8Array[i], 2);
            }
            return decodedBytes;
        }
    }
}
