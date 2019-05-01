//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Java.Lang;
//using Android.Support.V4.App;

//namespace SeekiosApp.Droid.CustomComponents.Adapter
//{
//    public class PagerAdapter : FragmentStatePagerAdapter
//    {
//        private readonly Android.Support.V4.App.Fragment[] fragments;

//        private readonly ICharSequence[] titles;

//        public PagerAdapter(Android.Support.V4.App.FragmentManager fm, Android.Support.V4.App.Fragment[] fragments, ICharSequence[] titles) : base(fm)
//        {
//            this.fragments = fragments;
//            this.titles = titles;
//        }

//        public override int Count
//        {
//            get
//            {
//                return 1;
//            }
//        }

//        public override Android.Support.V4.App.Fragment GetItem(int position)
//        {
//            return fragments[position];
//        }

//        public override ICharSequence GetPageTitleFormatted(int position)
//        {
//            return titles[position];
//        }
//    }
//}