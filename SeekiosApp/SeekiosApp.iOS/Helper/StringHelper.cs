using System;
using Foundation;

namespace SeekiosApp.iOS.Helper
{
    public static class StringHelper
    {
        public static string Translate(string translateVal)
        {
            return NSBundle.MainBundle.LocalizedString(translateVal, string.Empty);
        }
    }
}