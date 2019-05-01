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

namespace SeekiosApp.Droid.View.FragmentView
{
    public class LastTutoFragment : Android.Support.V4.App.Fragment
    {
        public TextView FinishButton { get; set; }

        #region ===== Life Cycle ==================================================================

        /// <summary>
        /// View creation
        /// </summary>
        public override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
        }

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.LastTutoFragmentLayout, container, false);
            GetObjectsFromView(view);

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
            FinishButton.Click += OnFinishButtonClick;
        }

        public override void OnPause()
        {
            base.OnPause();
            FinishButton.Click -= OnFinishButtonClick;
        }


        private void OnFinishButtonClick(object sender, EventArgs e)
        {
            App.Locator.Login.SaveFirstLaunchTuto();
            Activity.Finish();
            App.Locator.ListSeekios.GoToSeekios();
        }

        #endregion

        #region ===== Constructors ================================================================

        public LastTutoFragment() { }

        #endregion

        #region ===== Initialisation Vue ==========================================================

        /// <summary>
        /// Récupère les objets de la vue
        /// </summary>
        private void GetObjectsFromView(Android.Views.View view)
        {
            FinishButton = view.FindViewById<TextView>(Resource.Id.tuto_finishButton);
        }

        #endregion
    }
}