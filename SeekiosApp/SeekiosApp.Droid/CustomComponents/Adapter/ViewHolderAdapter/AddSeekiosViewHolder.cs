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
    public class AddSeekiosViewHolder : Java.Lang.Object
    {
        public TextView NameSeekios { get; set; }
        public RelativeLayout SeekiosLayout { get; set; }
        public RelativeLayout ConnectedLayout { get; set; }
        public XamSvg.SvgImageView ButtonConnexionSvg { get; set; }
        public ProgressBar ProgressingBar { get; set; }
    }
}