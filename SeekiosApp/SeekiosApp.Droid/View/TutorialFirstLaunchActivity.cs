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
using Android.Support.V4.App;
using Android.Support.V4.View;
using SeekiosApp.Droid.View.FragmentView;
using Android.Util;
using Android.Support.Design.Widget;
using static Android.Support.V4.View.ViewPager;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class TutorialFirstLaunchActivity : AppCompatActivityBase, IOnPageChangeListener
    {
        private static int NUM_PAGES;
        private TutoFragmentPagerAdapter tutoPagerAdapter;
        private ViewPager tutoViewPager;
        private TabLayout tutoTabLayout;

        public XamSvg.SvgImageView QuitButton { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (!App.Locator.Login.GetSavedFirstLaunchTuto()) NUM_PAGES = 7;
            else NUM_PAGES = 6;

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TutoLayout);

            QuitButton = FindViewById<XamSvg.SvgImageView>(Resource.Id.tuto_quitButton);
            tutoViewPager = FindViewById<ViewPager>(Resource.Id.pager);
            tutoTabLayout = FindViewById<TabLayout>(Resource.Id.tabDots);

            if (!App.Locator.Login.GetSavedFirstLaunchTuto()) QuitButton.Visibility = ViewStates.Gone;
            else QuitButton.Visibility = ViewStates.Visible;

            // ViewPager and its adapters use support library
            //fragments, so use getSupportFragmentManager.
            tutoPagerAdapter = new TutoFragmentPagerAdapter(this, SupportFragmentManager);
            tutoViewPager.Adapter = tutoPagerAdapter;
            //Crash aléatoire ici !!!! ???
            tutoViewPager.AddOnPageChangeListener(this);
            tutoTabLayout.SetupWithViewPager(tutoViewPager);
        }

        protected override void OnResume()
        {
            base.OnResume();
            QuitButton.Click += OnQuitButtonClick;
        }

        protected override void OnPause()
        {
            base.OnPause();
            QuitButton.Click -= OnQuitButtonClick;
        }

        public override void OnBackPressed()
        {
            if (tutoViewPager.CurrentItem > 0) tutoViewPager.SetCurrentItem(tutoViewPager.CurrentItem - 1, true);
            else Finish();
        }

        private void OnQuitButtonClick(object sender, EventArgs e)
        {
            Finish();
        }

        public void OnPageScrollStateChanged(int state)
        {
            //throw new NotImplementedException();
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            //throw new NotImplementedException();
        }

        public void OnPageSelected(int position)
        {
            if (position == NUM_PAGES - 1) tutoTabLayout.Visibility = ViewStates.Gone;
            else tutoTabLayout.Visibility = ViewStates.Visible;
        }

        public class TutoFragmentPagerAdapter : FragmentStatePagerAdapter
        {
            private Context _context;

            public TutoFragmentPagerAdapter(Context context, Android.Support.V4.App.FragmentManager fragmentManager) : base(fragmentManager)
            {
                _context = context;
            }

            public override int Count
            {
                get
                {
                    return NUM_PAGES;
                }
            }

            public override Android.Support.V4.App.Fragment GetItem(int position)
            {

                switch (position)
                {
                    default:
                        return new TutoFragment();
                    case 0:
                        return new TutoFragment(
                   _context.Resources.GetString(Resource.String.tutoAddSeekiosTitle),
                   _context.Resources.GetString(Resource.String.tutoAddSeekiosFirstContent),
                   1,
                   Resource.Drawable.tuto_addseekios_first_image,
                   Resource.Color.primary);
                    case 1:
                        return new TutoFragment(
                    _context.Resources.GetString(Resource.String.tutoAddSeekiosTitle),
                    _context.Resources.GetString(Resource.String.tutoAddSeekiosSecondContent),
                    2,
                    Resource.Drawable.tuto_addseekios_second_image,
                    Resource.Color.primary);
                    case 2:
                        return new TutoFragment(
                    _context.Resources.GetString(Resource.String.tutoStartSeekiosTitle),
                    _context.Resources.GetString(Resource.String.tutoStartSeekiosFirstContent),
                    1,
                    Resource.Drawable.tuto_useseekios_first_image,
                    Resource.Color.tuto_background_grennblue);
                    case 3:
                        return new TutoFragment(
                    _context.Resources.GetString(Resource.String.tutoStartSeekiosTitle),
                    _context.Resources.GetString(Resource.String.tutoStartSeekiosSecondContent),
                    2,
                    Resource.Drawable.tuto_useseekios_second_image,
                    Resource.Color.tuto_background_grennblue);
                    case 4:
                        return new TutoFragment(
                    _context.Resources.GetString(Resource.String.tutoFirstLocationTitle),
                    _context.Resources.GetString(Resource.String.tutoFirstLocationFirstContent),
                    1,
                    Resource.Drawable.tuto_myseekios_first_image,
                    Resource.Color.tuto_background_blue);
                    case 5:
                        return new TutoFragment(
                    _context.Resources.GetString(Resource.String.tutoFirstLocationTitle),
                    _context.Resources.GetString(Resource.String.tutoFirstLocationSecondContent),
                    2,
                    Resource.Drawable.tuto_myseekios_second_image,
                    Resource.Color.tuto_background_blue);
                    case 6:
                        return new LastTutoFragment();
                }
            }
        }
    }
}