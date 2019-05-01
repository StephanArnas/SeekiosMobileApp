using Android.Views;
using Android.Widget;
using XamSvg;
using System.Threading;
using SeekiosApp.Droid.Services;
using SeekiosApp.Interfaces;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Model.DTO;

namespace SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter
{
    public class ListSeekiosViewHolder : Java.Lang.Object
    {
        public SeekiosDTO Seekios { get; set; }
        public RoundedImageView ImageSeekiosRoundedImageView { get; set; }
        public XamSvg.SvgImageView PowerSavingImageView { get; set; }
        public XamSvg.SvgImageView FirmwareUpdateImageView { get; set; }
        public TextView LastPositionTextView { get; set; }
        public RelativeLayout RootLayout { get; set; }
        public TextView SeekiosNameTextView { get; set; }
        public LinearLayout HeaderLayout { get; set; }
        public TextView HeaderTitle { get; set; }
        public LinearLayout ModeLayout { get; set; }
        public LinearLayout AlertLayout { get; set; }
        //public LinearLayout ShareLayout { get; set; }
        public TextView ModeTextView { get; set; }
        public TextView AlertTextView { get; set; }
        //public TextView ShareTextView { get; set; }
        public SvgImageView ModeSvgImageView { get; set; }
        public SvgImageView AlertSvgImageView { get; set; }
        //public SvgImageView ShareSvgImageView { get; set; }
        public RelativeLayout ButtonRightLayout { get; set; }
    }
}
