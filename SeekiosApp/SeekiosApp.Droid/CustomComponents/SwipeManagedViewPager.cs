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
using Android.Support.V4.View;
using Android.Util;

namespace SeekiosApp.Droid.CustomComponents
{
    public class SwipeManagedViewPager : ViewPager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public SwipeManagedViewPager(Context context) : base(context)
        {
            CanSwipe = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attrs"></param>
        public SwipeManagedViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            CanSwipe = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (CanSwipe) return base.OnInterceptTouchEvent(ev);

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        public override bool OnTouchEvent(MotionEvent ev)
        {
            if (CanSwipe) return base.OnTouchEvent(ev);

            return false;
        }

        /// <summary>
        /// Vrai si le viewpager est scrollable
        /// </summary>
        public bool CanSwipe { get; set; }
    }
}