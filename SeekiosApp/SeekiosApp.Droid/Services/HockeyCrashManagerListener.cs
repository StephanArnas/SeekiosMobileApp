using Android.App;
using Android.Widget;
using HockeyApp.Android;

namespace SeekiosApp.Droid.Services
{
    public class HockeyCrashManagerListener : CrashManagerListener
    {
        private Activity _context;
        
        public HockeyCrashManagerListener(Activity context)
        {
            _context = context;
        }

        public override bool ShouldAutoUploadCrashes()
        {
            return true;
        }

        public override string Description
        {
            get
            {
                //string description = string.Empty;

                //try
                //{
                //    Java.Lang.Process process = Runtime.GetRuntime().Exec("logcat -d HockeyApp:D *:S");

                //    BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(process.InputStream));

                //    Java.Lang.StringBuilder log = new Java.Lang.StringBuilder();
                //    string line;
                //    while ((line = bufferedReader.ReadLine()) != null)
                //    {
                //        log.Append(line);
                //        log.Append(System.Environment.NewLine);
                //    }
                //    bufferedReader.Close();
                //    if (App.CurrentUserEnvironment != null)
                //    {
                //        description = string.Format("Id : {0} " + System.Environment.NewLine + "Name : {1} {2}" + System.Environment.NewLine + " {3}"
                //          , App.CurrentUserEnvironment.User.IdUser
                //          , App.CurrentUserEnvironment.User.FirstName
                //          , App.CurrentUserEnvironment.User.LastName
                //          , log.ToString());
                //    }
                //}
                //catch (IOException e)
                //{
                //}
                var credentials = App.Locator.Login.GetSavedCredentials();
                if (credentials != null)
                {
                    return string.Format("EMAIL : {0}\n\n {1}"
                        , credentials.Email
                        , Description);
                }
                else
                {
                    return string.Format("No User Data\n\n{0}", Description);
                }
            }
        }

        public override void OnCrashesSent()
        {
            base.OnCrashesSent();
            _context.RunOnUiThread(() =>
            {
                Toast.MakeText(_context, Resource.String.crash_data_sent, ToastLength.Long).Show();
            });
        }

        public override void OnCrashesNotSent()
        {
            base.OnCrashesNotSent();
            _context.RunOnUiThread(() =>
            {
                Toast.MakeText(_context, "Crash data failed to sent.", ToastLength.Long).Show();
            });
        }
    }
}