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
    public class FavoriteAreaViewHolder : Java.Lang.Object
    {
        public TextView AreaName { get; set; }
        public TextView PointsCount { get; set; }
        public TextView AreaGeodesic { get; set; }
        public SvgImageView DeleteAlerte { get; set; }
        public GridLayout RootGrid { get; internal set; }
    }
}