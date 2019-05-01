using HockeyApp.iOS;
using SeekiosApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekiosApp.iOS.Views.Delegates
{
    public class HockeyAppCrashDelegate : BITCrashManagerDelegate
    {
        public override string ApplicationLogForCrashManager(BITCrashManager crashManager)
        {
            var localCredential = App.Locator.Login.GetSavedCredentials();
            if (localCredential != null)
            {
                return string.Format("EMAIL : {0}\n\n{1}"
                    , localCredential.Email
                    , crashManager.Description);
            }
            else
            {
                return string.Format("No User Data\n\n{0}", crashManager.Description);
            }
        }
    }
}