using Foundation;
using System.Linq;
using System;
using UIKit;
using System.Collections.Generic;
using SeekiosApp.iOS.Helper;
using System.Drawing;
using System.Text.RegularExpressions;
using CoreGraphics;
using SeekiosApp.iOS.Views;
using HockeyApp.iOS;
using SeekiosApp.Interfaces;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.iOS.Services;
using SeekiosApp.ViewModel;
using SeekiosApp.Services;

namespace SeekiosApp.iOS
{
    public partial class ParameterUserView : BaseViewController
    {
        #region ===== Attributs ===================================================================

        private CountryCodePickerView _picker_model = null;
        private UIPickerView _picker = null;
        private bool _hasDataChanged = false, _isFromGallery = false;

        private static nfloat _heightOfThePage = 0;

        #endregion

        #region ===== Constructor =================================================================

        public ParameterUserView(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetPickerToView();

            App.Locator.Parameter.OnUserChanged += Parameter_OnUserChanged;
            AboutButton.TouchUpInside += AboutButton_TouchUpInside;
            LogOutButton.TouchUpInside += LogOutButton_TouchUpInside;
            ModifyUserImageButton.TouchUpInside += ModifyUserImageButton_TouchUpInside;
            UserImageView.AddGestureRecognizer(new UITapGestureRecognizer(() => { ModifyUserImageButton_TouchUpInside(null, null); }));
            SaveButton.TouchUpInside += SaveButton_TouchUpInside;
            UpdatePasswordButton.TouchUpInside += UpdatePasswordButton_TouchUpInside;
            App.RemainingRequestChanged += OnRemainingRequestChanged;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            if (_heightOfThePage == 0)
            {
                nfloat size = 0;
                foreach (var view in ScrollView.Subviews)
                {
                    size += view.Frame.Size.Height;
                }
                _heightOfThePage = size;
                ScrollView.ContentSize = new CGSize(ScrollView.Frame.Size.Width, size * 1.1);
            }
            else ScrollView.ContentSize = new CGSize(ScrollView.Frame.Size.Width, _heightOfThePage * 1.1);
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public override void SetDataAndStyleToView()
        {
            Title = Application.LocalizedString("ParameterTitle");
            InitialiseAllStrings();

            // round corner on the button modify the image of the user
            ModifyUserImageButton.Layer.CornerRadius = 4;
            ModifyUserImageButton.Layer.BorderWidth = 1;
            ModifyUserImageButton.Layer.BorderColor = UIColor.FromRGBA(98, 218, 115, 255).CGColor;
            ModifyUserImageButton.ContentEdgeInsets = new UIEdgeInsets(5, 10, 5, 10);

            SaveButton.Layer.CornerRadius = 4;
            SaveButton.Layer.MasksToBounds = true;
            AboutButton.Enabled = true;
            LogOutButton.Enabled = true;
            EmailTextField.Enabled = false;

            // Free Credits Details
            RefreshDisplayedCreditCount();

            FirstNameTextField.EditingChanged += (o, e) => { _hasDataChanged = true; SaveButton.Enabled = true; };
            FirstNameTextField.ShouldReturn += (textField) =>
            {
                FirstNameTextField.ResignFirstResponder();
                return true;
            };

            LastNameTextField.EditingChanged += (o, e) => { _hasDataChanged = true; SaveButton.Enabled = true; };
            LastNameTextField.ShouldReturn += (textField) =>
            {
                LastNameTextField.ResignFirstResponder();
                _hasDataChanged = true;
                return true;
            };

            EmailTextField.EditingChanged += (o, e) =>
            {
                _hasDataChanged = true;
                SaveButton.Enabled = true;
                EmailTextField.AutocorrectionType = UITextAutocorrectionType.No;
            };

            EmailTextField.ShouldReturn += (textField) =>
            {
                EmailTextField.ResignFirstResponder();
                _hasDataChanged = true;
                return true;
            };

            CountryCodeTextField.EditingChanged += (o, e) => { _hasDataChanged = true; SaveButton.Enabled = true; };
            CountryCodeTextField.ShouldReturn += (textField) =>
            {
                CountryCodeTextField.ResignFirstResponder();
                _hasDataChanged = true;
                return true;
            };

            PhoneNumberTextField.EditingChanged += (o, e) => { _hasDataChanged = true; SaveButton.Enabled = true; };
            PhoneNumberTextField.ShouldReturn += (textField) =>
            {
                PhoneNumberTextField.ResignFirstResponder();
                _hasDataChanged = true;
                return true;
            };

            var toolbar = new UIToolbar();
            toolbar.BarStyle = UIBarStyle.Default;
            toolbar.Translucent = true;
            toolbar.SizeToFit();

            // Done button
            var doneButton = new UIBarButtonItem(Application.LocalizedString("Validate"), UIBarButtonItemStyle.Done, (s, e) =>
            {
                PhoneNumberTextField.ResignFirstResponder();
            });
            toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);
            PhoneNumberTextField.InputAccessoryView = toolbar;
            PhoneNumberTextField.InputAccessoryView = toolbar;
            PhoneNumberTextField.InputAccessoryView = toolbar;

            FirstNameTextField.Text = App.CurrentUserEnvironment.User.FirstName;
            LastNameTextField.Text = App.CurrentUserEnvironment.User.LastName;
            EmailTextField.Text = App.CurrentUserEnvironment.User.Email;
            PhoneNumberTextField.KeyboardType = UIKeyboardType.DecimalPad;
            //if (!string.IsNullOrEmpty(App.CurrentUserEnvironment.User.PhoneNumber))
            //{
            //    var phoneNumber = App.CurrentUserEnvironment.User.PhoneNumber.Split('|');
            //    if (phoneNumber != null && phoneNumber.Length == 2)
            //    {
            //        if (phoneNumber[0].Contains("+"))
            //        {
            //            phoneNumber[0] = phoneNumber[0].Substring(1, phoneNumber[0].Length - 1);
            //        }
            //        CountryCodeTextField.Text = phoneNumber[0];
            //        PhoneNumberTextField.Text = phoneNumber[1];
            //    }
            //}
            UserImageView.Layer.CornerRadius = UserImageView.Frame.Size.Width / 2;
            UserImageView.ClipsToBounds = true;
            if (!string.IsNullOrEmpty(App.CurrentUserEnvironment.User.UserPicture))
            {
                using (var dataDecoded = new NSData(App.CurrentUserEnvironment.User.UserPicture
                        , NSDataBase64DecodingOptions.IgnoreUnknownCharacters))
                {
                    UserImageView.Image = new UIImage(dataDecoded);
                }
            }
            SaveButton.Enabled = false;
        }

        #endregion

        #region ===== Private Methodes ============================================================

        private void SetPickerToView()
        {
            _picker_model = new CountryCodePickerView();

            _picker = new UIPickerView();
            _picker.Model = _picker_model;
            _picker.ShowSelectionIndicator = true;
            _picker.BackgroundColor = UIColor.White;

            var toolbar = new UIToolbar();
            toolbar.BarStyle = UIBarStyle.Default;
            toolbar.Translucent = true;
            toolbar.SizeToFit();

            var doneButton = new UIBarButtonItem(Application.LocalizedString("Validate"), UIBarButtonItemStyle.Done, (s, e) =>
            {
                CountryCodeTextField.Text = _picker_model.ValuesPicker[(int)_picker.SelectedRowInComponent(0)].Item1.ToString();
                CountryCodeLabel.Text = _picker_model.ValuesPicker[(int)_picker.SelectedRowInComponent(0)].Item2.ToString();
                CountryCodeTextField.ResignFirstResponder();
            });
            toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);

            CountryCodeTextField.TouchDown += SetPicker;
            CountryCodeTextField.InputView = _picker;
            CountryCodeTextField.InputAccessoryView = toolbar;
        }

        private void PictureSelected(NSDictionary pictureDict)
        {
            float maxWidth = 100;
            UIImage Picture;

            if ((pictureDict[UIImagePickerController.MediaType] as NSString) == "public.image")
            {
                Picture = pictureDict[UIImagePickerController.EditedImage] as UIImage;
                if (Picture == null)
                {
                    Picture = pictureDict[UIImagePickerController.OriginalImage] as UIImage;
                }
                // save a copy of the original picture
                if (!_isFromGallery)
                {
                    Picture.SaveToPhotosAlbum(delegate { });
                }
                var size = Picture.Size;
                // show the UI, and on a callback, do the scaling, so the user gets an animation
                NSTimer.CreateScheduledTimer(TimeSpan.FromSeconds(0), delegate
                {
                    if (size.Width > maxWidth || size.Height > maxWidth)
                    {
                        Picture = Scale(Picture, new SizeF(maxWidth, (float)(maxWidth * size.Height / size.Width)));
                    }
                    UserImageView.Image = Picture;
                    App.Locator.Parameter.UserPicture = ImageHelper.ImageToByte(Picture);
                    SaveButton.Enabled = true;
                    _hasDataChanged = true;
                });
            }
            else
            {
                //NSUrl movieUrl = pictureDict [UIImagePickerController.MediaURL] as NSUrl;
                // Future use, when we find a video host that does not require your Twitter login/password
            }
            pictureDict.Dispose();
        }

        private UIImage Scale(UIImage image, SizeF size)
        {
            UIGraphics.BeginImageContext(size);
            image.Draw(new RectangleF(new PointF(0, 0), size));
            var ret = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return ret;
        }

        private void InitialiseAllStrings()
        {
            FirstNameTextField.Placeholder = Application.LocalizedString("FirstName");
            FirstNameLabel.Text = Application.LocalizedString("FirstName");
            LastNameTextField.Placeholder = Application.LocalizedString("LastName");
            LastNameLabel.Text = Application.LocalizedString("LastName");
            EmailTextField.Placeholder = Application.LocalizedString("EmailAddress");
            EmailTitleLabel.Text = Application.LocalizedString("EmailParam");
            SaveButton.SetTitle(Application.LocalizedString("Save"), UIControlState.Normal);
            AboutButton.SetTitle(Application.LocalizedString("AboutParam"), UIControlState.Normal);
            LogOutButton.SetTitle(Application.LocalizedString("Logout"), UIControlState.Normal);
            ModifyUserImageButton.SetTitle(Application.LocalizedString("ModifyImage"), UIControlState.Normal);
            PhoneNumberLabel.Text = Application.LocalizedString("PhoneNumber");
            UpdatePasswordButton.SetTitle(Application.LocalizedString("PasswordParam"), UIControlState.Normal);
        }

        #endregion

        #region ==== Credit Seekios Update ========================================================

        private void OnRemainingRequestChanged(object sender, EventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                if (CreditsUserLabel == null) return;
                RefreshDisplayedCreditCount();
            });
        }

        private void RefreshDisplayedCreditCount()
        {
            if (CreditsUserLabel != null)
            {
                CreditsUserLabel.Text = SeekiosApp.Helper.CreditHelper.TotalCredits;
                var firstAttributes = new UIStringAttributes
                {
                    ForegroundColor = UIColor.FromRGB(98, 218, 115)
                };
                var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfStringInString(Application.LocalizedString("CreditOfferedAmount"), SeekiosApp.Helper.CreditHelper.CreditsOffered);
                var offeredCreditsText = new NSMutableAttributedString(string.Format(Application.LocalizedString("CreditOfferedAmount")
                    , SeekiosApp.Helper.CreditHelper.CreditsOffered));
                offeredCreditsText.SetAttributes(firstAttributes.Dictionary
                    , new NSRange(resultTuple.Item1, SeekiosApp.Helper.CreditHelper.CreditsOffered.Length));
                CreditsFreeLabel.AttributedText = offeredCreditsText;
            }
        }

        #endregion

        #region ===== Events ======================================================================

        private async void SaveButton_TouchUpInside(object sender, EventArgs e)
        {
            var isFieldsValid = true;
            //int n;
            //var isNumeric = int.TryParse(PhoneNumberTextField.Text, out n);

            if (string.IsNullOrEmpty(FirstNameTextField.Text))
            {
                FirstNameTextField.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
                FirstNameTextField.Layer.BorderWidth = 1;
                FirstNameTextField.Layer.CornerRadius = 4;
                isFieldsValid = false;
            }
            else
            {
                FirstNameTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
                FirstNameTextField.Layer.BorderWidth = 1;
                FirstNameTextField.Layer.CornerRadius = 4;
            }
            if (string.IsNullOrEmpty(LastNameTextField.Text))
            {
                LastNameTextField.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
                LastNameTextField.Layer.BorderWidth = 1;
                LastNameTextField.Layer.CornerRadius = 4;
                isFieldsValid = false;
            }
            else
            {
                LastNameTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
                LastNameTextField.Layer.BorderWidth = 1;
                LastNameTextField.Layer.CornerRadius = 4;
            }
            if (string.IsNullOrEmpty(EmailTextField.Text)
                || !new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Match(EmailTextField.Text).Success)
            {
                EmailTextField.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
                EmailTextField.Layer.BorderWidth = 1;
                EmailTextField.Layer.CornerRadius = 4;
                isFieldsValid = false;
            }
            else
            {
                EmailTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
                EmailTextField.Layer.BorderWidth = 1;
                EmailTextField.Layer.CornerRadius = 4;
            }
            //if (string.IsNullOrEmpty(PhoneNumberTextField.Text) && (!isNumeric))
            //{
            //    PhoneNumberTextField.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
            //    PhoneNumberTextField.Layer.BorderWidth = 1;
            //    PhoneNumberTextField.Layer.CornerRadius = 4;
            //    isFieldsValid = false;
            //}
            //else
            //{
            //    PhoneNumberTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            //    PhoneNumberTextField.Layer.BorderWidth = 1;
            //    PhoneNumberTextField.Layer.CornerRadius = 4;
            //}
            //if (string.IsNullOrEmpty(CountryCodeTextField.Text) && !string.IsNullOrEmpty(PhoneNumberTextField.Text))
            //{
            //    CountryCodeTextField.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
            //    CountryCodeTextField.Layer.BorderWidth = 1;
            //    CountryCodeTextField.Layer.CornerRadius = 4;
            //    isFieldsValid = false;
            //}
            //else
            //{
            //    CountryCodeTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            //    CountryCodeTextField.Layer.BorderWidth = 1;
            //    CountryCodeTextField.Layer.CornerRadius = 4;
            //}
            //var phone = string.Format("{0}{1}", CodeCountryLayout.SelectedCountry.PhoneCode, PhoneNumberEditText.Text);
            //if (!Android.Telephony.PhoneNumberUtils.IsGlobalPhoneNumber(phone) && PhoneNumberEditText.Text.Length == NumberOfDigitPhoneNumber())
            //{
            //    PhoneNumberEditText.Error = Resources.GetString(Resource.String.parameter_phonelInvalid);
            //    isFieldsValid = false;UserPicture
            //}
            if (isFieldsValid && _hasDataChanged)
            {
                if (await App.Locator.Parameter.UpdateUser(EmailTextField.Text
                , string.Format("{0}|{1}", CountryCodeTextField.Text, PhoneNumberTextField.Text)
                , FirstNameTextField.Text
                , LastNameTextField.Text) != 1)
                {
                    SaveButton.Enabled = true;
                }
                else SaveButton.Enabled = false;
            }
        }

        private void Parameter_OnUserChanged(object sender, EventArgs e)
        {
            FirstNameTextField.Text = App.CurrentUserEnvironment.User.FirstName;
            LastNameTextField.Text = App.CurrentUserEnvironment.User.LastName;
            EmailTextField.Text = App.CurrentUserEnvironment.User.Email;
            UserImageView.Layer.CornerRadius = UserImageView.Frame.Size.Width / 2;
            UserImageView.ClipsToBounds = true;
            if (!string.IsNullOrEmpty(App.CurrentUserEnvironment.User.UserPicture))
            {
                using (var dataDecoded = new NSData(App.CurrentUserEnvironment.User.UserPicture
                        , NSDataBase64DecodingOptions.IgnoreUnknownCharacters))
                {
                    UserImageView.Image = new UIImage(dataDecoded);
                }
            }
        }

        private void ModifyUserImageButton_TouchUpInside(object sender, EventArgs e)
        {
            var alert = AlertControllerHelper.CreateActionSheetToPickPictureUser(() =>
            {
                if (UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
                {
                    CameraHelper.TakePicture(this, PictureSelected);
                }
                else CameraHelper.SelectPicture(this, PictureSelected);
            }
            , () =>
             {
                 CameraHelper.SelectPicture(this, PictureSelected);
             }
            , () =>
            {
                if (!string.IsNullOrEmpty(App.CurrentUserEnvironment.User.UserPicture))
                {
                    UserImageView.Image = UIImage.FromBundle("DefaultUser");
                    App.Locator.Parameter.UserPicture = new byte[0];
                    SaveButton.Enabled = true;
                    _hasDataChanged = true;
                }
            });
            PresentViewController(alert, true, null);
        }

        private void SetPicker(object sender, EventArgs e)
        {
            _hasDataChanged = true;
            SaveButton.Enabled = true;
            _picker.Select(_picker_model.ValuesPicker.IndexOf(new Tuple<string, string>(CountryCodeTextField.Text, CountryCodeLabel.Text)), 0, true);
        }

        private void UpdatePasswordButton_TouchUpInside(object sender, EventArgs e)
        {
            var popup = AlertControllerHelper.ShowPopupUpdatePassword((passwordOld, password1, password2, errorLabel) =>
           {
               App.Locator.Parameter.OldPassword = passwordOld.Text;
               App.Locator.Parameter.NewPassword = password1.Text;
               App.Locator.Parameter.NewPasswordReenter = password2.Text;
               UpdatePassword();
               return true;
           });
            PresentViewController(popup, true, null);
        }

        private async void UpdatePassword()
        {
            await App.Locator.Parameter.UpdateNewPasswordChanged();
        }

        private void AboutButton_TouchUpInside(object sender, EventArgs e)
        {
            AboutButton.Enabled = false;
            App.Locator.Parameter.GoToAboutPage();
        }

        private void LogOutButton_TouchUpInside(object sender, EventArgs e)
        {
            LogOutButton.Enabled = false;
            var alertLoading = AlertControllerHelper.ShowAlertLoading();
            PresentViewController(alertLoading, true, async () =>
            {
                if (App.CurrentUserEnvironment.Device != null)
                {
                    if (await App.Locator.Login.Disconnect(App.CurrentUserEnvironment.Device.UidDevice) == 1)
                    {
                        var oneSignal = (ServiceLocator.Current.GetInstance<IOneSignal>());
                        oneSignal.DeleteTag(App.CurrentUserEnvironment.User.IdUser.ToString() + (DataService.UseStaging ? "s" : "p"));
                        oneSignal.SetSubscription(false);
                        DismissViewController(false, () =>
                        {
                            UIApplication.SharedApplication.KeyWindow.RootViewController = UIStoryboard.FromName("Main", null).InstantiateViewController("LoginView");
                        });
                    }
                }
                else DismissViewController(false, null);
            });
        }

        #endregion
    }
}