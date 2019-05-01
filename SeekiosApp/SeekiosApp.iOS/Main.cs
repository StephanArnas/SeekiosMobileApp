using Foundation;
using UIKit;

namespace SeekiosApp.iOS
{
    public class Application
	{
        public static string LocalizedString(string key, string comment = "")
        {
            return NSBundle.MainBundle.LocalizedString(key, comment);
        }

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
