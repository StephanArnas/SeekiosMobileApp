using SeekiosApp.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SeekiosApp.Helper
{
    public static class StringHelper
    {
        /// <summary>
        /// Return a truncated string
        /// </summary>
        public static string SetMaxSizeText(string input, int value)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= value) return input;
            return string.Format("{0} ...", input.Substring(0, value));
        }

        /// <summary>
        /// Method used to change the color of an int in a string (see number of friends in FriendList - Community) 
        /// </summary>
        public static Tuple<int, int> GetStartAndEndIndexOfNumberInString(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            int temp = 0;
            int startIndex = 0;
            int endIndex = 0;
            bool change = true;

            for (int i = 0; i < value.Length; i++)
            {
                if (int.TryParse(value[i].ToString(), out temp))
                {
                    if (change)
                    {
                        startIndex = i;
                        change = false;
                    }
                    endIndex = i;
                }
            }

            endIndex++;

            return new Tuple<int, int>(startIndex, endIndex);
        }

        /// <summary>
        /// Method used to highlight the user name and firstname in the RequestTabFragment
        /// </summary>
        public static Tuple<int, int> GetStartAndEndIndexOfStringInString(string wholeString, string stringToModify)
        {
            if (string.IsNullOrEmpty(wholeString) || string.IsNullOrEmpty(stringToModify)) return null;

            int startIndex = wholeString.IndexOf("{");
            if (startIndex < 0) return null;
            int endIndex = startIndex + stringToModify.Length;

            return new Tuple<int, int>(startIndex, endIndex);
        }

        /// <summary>
        /// Generate a link Google Map to display the seekios position
        /// </summary>
        public static string GoogleMapLinkShare(double latitude, double longitude)
        {
            return "http://www.google.com/maps/place/" + string.Format(new CultureInfo("en-US"), "{0:0.00000000},{1:0.00000000}", latitude, longitude);
        }

        /// <summary>
        /// Remove white space in a string
        /// </summary>
        public static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
    }
}