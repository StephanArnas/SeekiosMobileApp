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

namespace SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter
{
    public class ListPackCreditViewHolder : Java.Lang.Object
    {
        public TextView PriceTextView { get; set; }
        public TextView RewardingCreditTextView { get; set; }
        public TextView TitleTextView { get; set; }
        public TextView DescriptionTextView { get; set; }
        public LinearLayout ContainerLayout { get; set; }
        public RelativeLayout SecondLayout { get; set; }
        public XamSvg.SvgImageView ArcCircleSvgImage { get; set; }
    }
}