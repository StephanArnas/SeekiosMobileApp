using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SeekiosApp.Helper
{
    public class ParserHelper
    {
        public static Dictionary<string, object> ConvertToDictionaryParser2(string input)
        {
            input = input.Substring(input.IndexOf('{', input.IndexOf('{') + 1) + 1).Trim();

            var key = new StringBuilder();
            var value = new StringBuilder();
            var json = new StringReader(input);
            var result = new Dictionary<string, object>();

            var keyEnter = true;
            var endCharFirst = true;
            var parsing = true;
            var endChar = ';';

            char c;

            while (parsing)
            {
                if (json.Peek() == -1)
                {
                    parsing = false;
                    break;
                }

                c = System.Convert.ToChar(json.Read());

                // key
                if (keyEnter)
                {
                    if (c == '=')
                    {
                        keyEnter = false;
                        result.Add(key.Replace("\"", string.Empty).Replace(";", string.Empty).ToString(), string.Empty);
                        endCharFirst = true;
                    }
                    else
                    {
                        key.Append(c);
                    }
                }
                // value
                else
                {
                    if (endCharFirst && c == '{')
                    {
                        endChar = '}';
                    }
                    else if (endCharFirst)
                    {
                        endChar = ';';
                    }
                    if (c == endChar)
                    {
                        if (endChar == '}') value.Append(c);
                        keyEnter = true;
                        result[key.ToString()] = value.Replace("\"", string.Empty).ToString();
                        value.Clear();
                        key.Clear();
                    }
                    else
                    {
                        value.Append(c);
                        endCharFirst = false;
                    }
                }
            }

            json.Dispose();
            return result;
        }

        public static Dictionary<string, object> ConvertToDictionaryParser(string input)
        {
            var outputString = new StringBuilder();
            var json = new StringReader(input);

            var result = new Dictionary<string, object>();
            var buildId = string.Empty;

            var keyEnter = false;
            var valueEnter = false;
            var parsing = true;

            var isInt = 0;
            var index = 0;

            char c;

            while (parsing)
            {
                if (json.Peek() == -1)
                {
                    parsing = false;
                    break;
                }

                c = System.Convert.ToChar(json.Read());

                // key
                if (!valueEnter && int.TryParse(c.ToString(), out isInt))
                {
                    buildId += c;
                    keyEnter = true;
                }
                else if (keyEnter)
                {
                    buildId += c;
                    keyEnter = false;
                    result.Add(buildId, string.Empty);
                    isInt = 0;
                }

                // value
                if (c == '"')
                {
                    valueEnter = !valueEnter;
                    if (!valueEnter)
                    {
                        result[buildId] = outputString.ToString();
                        index++;
                        buildId = string.Empty;
                        outputString.Clear();
                    }
                    continue;
                }
                if (valueEnter)
                {
                    outputString.Append(c);
                }
            }

            json.Dispose();
            return result;
        }

        public static int ParseCountryCode(string countryCode)
        {
            switch (countryCode)
            {
                default:
                case "fr":
                    return 1;
                case "en":
                    return 2;
            }
        }
    }
}
