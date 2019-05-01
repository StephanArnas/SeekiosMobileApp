using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLabs.Cryptography;

namespace SeekiosApp.Helper
{
    public class CryptographyHelper
    {
        public static string CalculatePasswordMD5Hash(string login, string password)
        {
            var hashTemplate = "{0}{1}fds#q!sdg'qsg;;àg;bd@gjk&";
            var input = string.Format(hashTemplate, login, password);

            return CalculateMD5Hash(input);
        }

        public static string CalculateMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Portable.Text.ASCIIEncoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
