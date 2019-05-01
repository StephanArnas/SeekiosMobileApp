using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SeekiosApp.Droid.Services;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class TutorialBackgroundActivity : AppCompatActivityBase
    {
        public TextView StartButton { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FirstLaunchLayout);

            StartButton = FindViewById<TextView>(Resource.Id.tuto_startButton);
        }

        protected override void OnResume()
        {
            base.OnResume();
            StartButton.Click += OnStartButtonClick;
        }

        protected override void OnPause()
        {
            base.OnPause();
            StartButton.Click -= OnStartButtonClick;
        }

        public override void OnBackPressed()
        {
        }

        private void OnStartButtonClick(object sender, EventArgs e)
        {
            App.Locator.Parameter.GoToTutorial();
        }
    }
}