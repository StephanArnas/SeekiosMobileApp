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
    public class TransactionHistoricViewHolder : Java.Lang.Object
    {
        public XamSvg.SvgImageView OperationSvgImageView { get; set; }
        public TextView OperationTitle { get; set; }
        public TextView OperationSubtitle { get; set; }
        public TextView OperationDate { get; set; }
        public TextView OperationCreditAmount { get; set; }
    }
}