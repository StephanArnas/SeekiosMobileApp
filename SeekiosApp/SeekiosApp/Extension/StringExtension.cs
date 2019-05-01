using SeekiosApp.Properties;
using System.Text.RegularExpressions;

namespace SeekiosApp.Extension
{
    public static class StringExtension
    {
        public static string ToUpperCaseFirst(this string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return string.Empty;
            }
            char[] a = word.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static bool IsEmail(this string email)
        {
            return new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$").Match(email).Success;
        }

        public static string FormatTrackingRate(this string refreshRate)
        {
            int rate = 0;
            if (!int.TryParse(refreshRate, out rate)) return string.Empty;
            string hourRate = string.Empty;

            if (rate >= 60)
            {
                switch (rate)
                {
                    case 60:
                        hourRate = "1h";
                        break;
                    case 120:
                        hourRate = "2h";
                        break;
                    case 180:
                        hourRate = "3h";
                        break;
                    case 240:
                        hourRate = "4h";
                        break;
                    case 300:
                        hourRate = "5h";
                        break;
                    case 360:
                        hourRate = "6h";
                        break;
                    case 720:
                        hourRate = "12h";
                        break;
                    case 1440:
                        hourRate = "24h";
                        break;
                    default:
                        break;
                }
            }
            return (rate < 60 ? string.Format(Resources.Every, (rate + "min")) : string.Format(Resources.Every, hourRate));
        }
    }
}
