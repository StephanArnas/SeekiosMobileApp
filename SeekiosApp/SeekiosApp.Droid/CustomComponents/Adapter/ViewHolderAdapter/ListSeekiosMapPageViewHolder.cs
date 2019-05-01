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
using XamSvg;

namespace SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter
{
     public class ListSeekiosMapPageViewHolder : Java.Lang.Object
    {
        public RoundedImageView ImageSeekiosRoundedImageView { get; set; }
        public TextView LastPositionTextView { get; set; }
        public TextView SeekiosNameTextView { get; set; }
        public TextView BatteryTextView { get; set; }
        public TextView SignalTextView { get; set; }
        public SvgImageView LogoModeSvgImageView { get; set; }
        public SvgImageView LogoBatterySvgImageView { get; set; }
        public SvgImageView LogoSignalSvgImageView { get; set; }
        public SvgImageView LogoBatteryProgressSvgImageView { get; set; }
        public SvgImageView LogoSignalProgressSvgImageView { get; set; }
        public RelativeLayout BatteryProgressRelativeLayout { get; set; }
        public RelativeLayout SignalProgressRelativeLayout { get; set; }
    }
}