using System;
using System.Linq;
using HockeyApp.iOS;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Interfaces;
using SeekiosApp.iOS.Helper;
using SeekiosApp.iOS.Services;
using SeekiosApp.ViewModel;
using UIKit;
using Xamarin.SWRevealViewController;
using SeekiosApp.Services;
using Foundation;

namespace SeekiosApp.iOS.Menu
{
    public partial class MenuController : UIViewController
    {
        #region ===== Properties ==================================================================

        public SWRevealViewController Controller { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public MenuController(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // button click seekios tutorial
            SeekiosTutorialButton.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NavigationService.LeftMenuView.RevealViewController().RightRevealToggleAnimated(true);
                App.Locator.LeftMenu.GoToListTutorial();
            }));

            // button click my consomation
            HistoriqueConsommationButton.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NavigationService.LeftMenuView.RevealViewController().RightRevealToggleAnimated(true);
                App.Locator.Credits.GoToCreditHistoric();
            }));

            // button click on add a seekios
            AddSeekiosButton.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NavigationService.LeftMenuView.RevealViewController().RightRevealToggleAnimated(true);
                App.Locator.LeftMenu.GoToAddSeekios();
            }));

            // button click on map all seekios
            MapButton.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (App.CurrentUserEnvironment.LsSeekios.Count == 0)
                {
                    AlertControllerHelper.ShowAlert(Application.LocalizedString("ZeroSeekios")
                        , Application.LocalizedString("NeedAtLeastOneSeekios")
                        , Application.LocalizedString("Close"));
                }
                else
                {
                    if (App.CurrentUserEnvironment.LsSeekios.All(a => a.LastKnownLocation_latitude == App.DefaultLatitude
                    && a.LastKnownLocation_longitude == App.DefaultLongitude))
                    {
                        if (App.CurrentUserEnvironment.LsSeekios.Count == 1)
                        {
                            AlertControllerHelper.ShowAlert(Application.LocalizedString("NoPosition")
                                , Application.LocalizedString("OneSeekiosNewlyAdded")
                                , Application.LocalizedString("Close"));
                        }
                        else
                        {
                            AlertControllerHelper.ShowAlert(Application.LocalizedString("NoPosition")
                                , Application.LocalizedString("PluralSeekiosNewlyAdded")
                                , Application.LocalizedString("Close"));
                        }
                    }
                    else
                    {
                        NavigationService.LeftMenuView.RevealViewController().RightRevealToggleAnimated(true);
                        App.Locator.LeftMenu.GoToSeekiosMapAllSeekios();
                    }
                }
            }));

            // button click on feedback
            FeedbackButton.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                var feedbackManager = BITHockeyManager.SharedHockeyManager.FeedbackManager;
                feedbackManager.ShowFeedbackListView();
                feedbackManager.ShowFeedbackComposeView();
                NavigationService.LeftMenuView.RevealViewController().RightRevealToggleAnimated(true);
            }));

            // button click on Parameter
            UserImageView.UserInteractionEnabled = true;
            UserImageView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NavigationService.LeftMenuView.RevealViewController().RightRevealToggleAnimated(true);
                App.Locator.LeftMenu.GoToParameter();
            }));
            EmailUser.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NavigationService.LeftMenuView.RevealViewController().RightRevealToggleAnimated(true);
                App.Locator.LeftMenu.GoToParameter();
            }));
            NameUser.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NavigationService.LeftMenuView.RevealViewController().RightRevealToggleAnimated(true);
                App.Locator.LeftMenu.GoToParameter();
            }));
            SettingsImageView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NavigationService.LeftMenuView.RevealViewController().RightRevealToggleAnimated(true);
                App.Locator.LeftMenu.GoToParameter();
            }));
            CreditsTitleLabel.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NavigationService.LeftMenuView.RevealViewController().RightRevealToggleAnimated(true);
                App.Locator.LeftMenu.GoToParameter();
            }));
            CreditsLabel.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NavigationService.LeftMenuView.RevealViewController().RightRevealToggleAnimated(true);
                App.Locator.LeftMenu.GoToParameter();
            }));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SetDataAndStyleToView();
            App.RemainingRequestChanged += OnRemainingRequestChanged;
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            App.RemainingRequestChanged -= OnRemainingRequestChanged;
        }

        #endregion

        #region ===== Privates Methodes ===========================================================

        private void SetDataAndStyleToView()
        {
            // Initialize all strings
            CreditsTitleLabel.Text = Application.LocalizedString("Credits");
            UserGuideLabel.Text = Application.LocalizedString("UserGuide");
            MyConsoLabel.Text = Application.LocalizedString("MyConso");
            AddSeekiosLabel.Text = Application.LocalizedString("AddSeekios");
            AllSeekiosMapLabel.Text = Application.LocalizedString("AllSeekiosMap");
            FeedbackLabel.Text = Application.LocalizedString("Feedback");
            ParametersLabel.Text = Application.LocalizedString("UserSettings");
            SettingsImageView.Image = UIImage.FromBundle("Parameter");

            NameUser.Text = string.Format("{0} {1}"
                , App.CurrentUserEnvironment.User.FirstName
                , App.CurrentUserEnvironment.User.LastName);
            EmailUser.Text = App.CurrentUserEnvironment.User.Email;
            CreditsLabel.Text = SeekiosApp.Helper.CreditHelper.TotalCredits;
            CreditsLabel.AdjustsFontSizeToFitWidth = true;
            UserImageView.Layer.CornerRadius = UserImageView.Frame.Width / 2;
            UserImageView.ClipsToBounds = true;
            if (!string.IsNullOrEmpty(App.CurrentUserEnvironment.User.UserPicture))
            {
                using (var dataDecoded = new NSData(App.CurrentUserEnvironment.User.UserPicture
                        , NSDataBase64DecodingOptions.IgnoreUnknownCharacters))
                {
                    UserImageView.Image = new UIImage(dataDecoded);
                }
            }
            else UserImageView.Image = UIImage.FromBundle("DefaultUser");
        }

        #endregion

        #region ===== Credit Seekios ==============================================================

        private void OnRemainingRequestChanged(object sender, EventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                if (CreditsLabel == null) return;
                RefreshDisplayedCreditCount();
            });
        }

        public void RefreshDisplayedCreditCount()
        {
            if (CreditsLabel != null)
            {
                CreditsLabel.Text = SeekiosApp.Helper.CreditHelper.TotalCredits;
            }
        }

        #endregion
    }
}
