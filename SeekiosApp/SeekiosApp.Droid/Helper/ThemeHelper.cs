using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;

namespace SeekiosApp.Droid.Helper
{
    public class ThemeHelper
    {
        public static void ChangeToTheme(Activity activity, int theme)
        {
            App.ActualTheme = theme;
            activity.Finish();
            activity.StartActivity(new Intent(activity, activity.Class));
        }

        public static void OnActivityCreateSetTheme(Activity activity)
        {
            switch (App.ActualTheme)
            {
                default:
                case App.THEME_LIGHT:
                    activity.SetTheme(Resource.Style.Theme_Normal);
                    break;
                case App.THEME_BLACK:
                    activity.SetTheme(Resource.Style.Theme_Dark);
                    break;
                case App.THEME_COMMUNITY:
                    activity.SetTheme(Resource.Style.Theme_Community);
                    break;
            }
        }
    }
}