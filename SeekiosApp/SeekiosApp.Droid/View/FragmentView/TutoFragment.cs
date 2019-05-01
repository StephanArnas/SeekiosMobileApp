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
using Android.Util;

namespace SeekiosApp.Droid.View.FragmentView
{
    public class TutoFragment : Android.Support.V4.App.Fragment
    {
        #region ===== Attributs ===================================================================

        private string _title = string.Empty;

        private string _content = string.Empty;

        private int _slideNumber;

        private int _imageId;

        private int _backgroundColorId;

        #endregion

        #region ===== Properties ==================================================================

        public TextView TitleTextView { get; set; }
        public TextView ContentTextView { get; set; }
        public TextView SlideNumberTextView { get; set; }
        public ImageView TutoImage { get; set; }
        public LinearLayout TutoTopLayout { get; set; }

        #endregion

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
            var view = inflater.Inflate(Resource.Layout.TutoFragmentLayout, container, false);
            GetObjectsFromView(view);
            SetDataToView();

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        #endregion

        #region ===== Constructors ================================================================

        public TutoFragment() { }

        public TutoFragment(string title, string content, int slideNumber, int imageId, int backgroundColorId)
        {
            _title = title;
            _slideNumber = slideNumber;
            _imageId = imageId;
            _content = content;
            _backgroundColorId = backgroundColorId;
        }

        #endregion

        #region ===== Initialisation Vue ==========================================================

        /// <summary>
        /// Récupère les objets de la vue
        /// </summary>
        private void GetObjectsFromView(Android.Views.View view)
        {
            TitleTextView = view.FindViewById<TextView>(Resource.Id.tutoTitle);
            ContentTextView = view.FindViewById<TextView>(Resource.Id.tutoContent);
            TutoImage = view.FindViewById<ImageView>(Resource.Id.tutoImageView);
            SlideNumberTextView = view.FindViewById<TextView>(Resource.Id.tutoSlideNumber);
            TutoTopLayout = view.FindViewById<LinearLayout>(Resource.Id.tutoTopLayout);
        }

        /// <summary>
        /// Initialise la vue avec les données
        /// </summary>
        private void SetDataToView()
        {
            TitleTextView.Text = _title;
            ContentTextView.Text = _content;
            TutoImage.SetImageResource(_imageId);
            if(_imageId == Resource.Drawable.tuto_useseekios_first_image || _imageId == Resource.Drawable.tuto_useseekios_second_image)
            {
                var layoutParams = TutoImage.LayoutParameters;
                layoutParams.Width = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 120, Resources.DisplayMetrics);
                TutoImage.LayoutParameters = layoutParams;
            }
            SlideNumberTextView.Text = string.Format(Resources.GetString(Resource.String.tutoPageNumber), _slideNumber);
            TutoTopLayout.SetBackgroundResource(_backgroundColorId);
        }

        #endregion
    }
}