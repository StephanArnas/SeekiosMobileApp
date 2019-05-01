using System;
using System.Drawing;
using SeekiosApp.iOS.Services;
using UIKit;
using SeekiosApp.iOS.Helper;

namespace SeekiosApp.iOS
{
    public class ConnectivityView : UIView
    {
        #region ===== Attributs ===================================================================

        private float _width;
        private InternetConnectionService _connectionService = new InternetConnectionService();

        #endregion

        #region ===== Constructor =================================================================

        public ConnectivityView()
        {
            BackgroundColor = UIColor.FromRGB(224, 224, 224);
            _width = (float)UIScreen.MainScreen.Bounds.Width;

            var contactNameLable = new UILabel();
            contactNameLable.Frame = new RectangleF(20f, 10f, _width - 40f, 20f);
            contactNameLable.Text = Application.LocalizedString("InternetConnectionNotAvailable");
            contactNameLable.Font = UIFont.FromName("Helvetica", 15f);
            contactNameLable.TextColor = UIColor.Red;
            contactNameLable.Layer.BackgroundColor = UIColor.Clear.CGColor;

            var deleteButton = new UIButton(UIButtonType.RoundedRect);
            deleteButton.Frame = new RectangleF(_width - 30f, 10f, 15f, 15f); ;
            deleteButton.SetTitle("X", UIControlState.Normal);
            deleteButton.SetTitleColor(UIColor.FromRGB(255, 76, 57), UIControlState.Normal);
            deleteButton.Layer.BackgroundColor = UIColor.FromRGB(224, 224, 224).CGColor;

            deleteButton.TouchUpInside += (s, e) =>
            {
                AnimationView.StartAnimatinon(0.3);
                Frame = new RectangleF(0f, 24f, _width, 0f);
                AnimationView.StopAnimatinon();
            };

            AddSubview(contactNameLable);
            AddSubview(deleteButton);
        }

        #endregion

        #region ===== Public Methods ==============================================================

        public bool DataConnectivity()
        {
            // No internet
            if (!_connectionService.IsDeviceConnectedToInternet())
            {
                // We show the rubban displayed no internet
                Hidden = false;
                AnimationView.StartAnimatinon(0.3);
                Frame = new RectangleF(0f, 64f, _width, 40f);
                AnimationView.StopAnimatinon();
                return true;
            }
            else
            {
                Frame = new RectangleF(0f, 24f, _width, 0f);
                return false;
            }
        }

        #endregion
    }
}