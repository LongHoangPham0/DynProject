using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BlueMoon.DynWeb.Common
{
    public static class Encryptor
    {
        public static string MD5Hash(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }
        static Random s_random = new Random();
        public static string SimpleEncrypt(string s)
        {
            string randomKey = s_random.Next(10000000, 99999999).ToString();
            byte[] d = s.GetBytes();
            CryptByte(d, randomKey.GetBytes());
            return Convert.ToBase64String(d) + "$" + Convert.ToBase64String(randomKey.GetBytes());
        }
        public static string SimpleDecrypt(string s)
        {
            string[] pt = s.Split('$');
            byte[] d = Convert.FromBase64String(pt[0]);
            CryptByte(d, Convert.FromBase64String(pt[1]));
            return Encoding.UTF8.GetString(d);
            
        }
        static void CryptByte(byte[] data, byte[] key)
        {
            int pwdLength = key.Length;
            int lng = data.Length;
            int j = 0;
            for (int i = 0; i <= lng - 1; i++)
            {
                if (data[i] != 0 && key[j] != data[i])
                {
                    data[i] = (byte)(key[j] ^ data[i]);
                }

                j++;
                if (j == pwdLength) j = 0;
            }

        }
        static byte[] GetBytes(this string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }
    }
}