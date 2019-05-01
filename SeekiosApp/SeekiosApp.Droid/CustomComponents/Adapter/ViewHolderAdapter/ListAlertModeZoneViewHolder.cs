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
    public class ListAlertModeZoneViewHolder : Java.Lang.Object
    {
        public TextView titre { get; set; }
        public TextView description { get; set; }

        public TextView nbrRecipients { get; set; }
    }
}