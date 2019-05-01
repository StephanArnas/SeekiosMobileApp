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
    public class FavoriteAlertViewHolder : Java.Lang.Object
    {
        public TextView AlertContains { get; set; }
        public TextView EmailObject { get; set; }
        public SvgImageView DeleteAlertButton { get; set; }
        public RelativeLayout AlertLayoutFavorite { get; set; }
    }
}