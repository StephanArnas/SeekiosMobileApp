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
    public class ListAlertsViewHolder : Java.Lang.Object
    {
        public TextView TypeAlertTextView { get; set; }
        public RelativeLayout TypeAlertLayout { get; set; }
        public RelativeLayout RowLayout { get; set; }
        public TextView SeekiosNameAlertTextView { get; set; }
        public TextView ContentAlertTextView { get; set; }
        public TextView NumberOfRecipient { get; set; }
    }
}