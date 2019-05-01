using System;
using UIKit;
using GalaSoft.MvvmLight.Views;
using Xamarin.SWRevealViewController;
using SeekiosApp.iOS.Menu;
using SeekiosApp.iOS.Helper;
using Foundation;
using GalaSoft.MvvmLight.Ioc;
using System.Text.RegularExpressions;
using SeekiosApp.Extension;


namespace SeekiosApp.iOS
{
    public partial class LoginView : UIViewController
    {
        #region ===== Attributs ===================================================================

        //private List<string> _readPermissions = new List<string> { "public_profile", "user_friends", "email" };
        //private LoginButton /*_facebookButton*/ = null;
        // this permission is set by default, even if you don't add it, but FB recommends to add it anyway
        //private int code = 0;
        private NSObject _observer1 = null;
        private NSObject _observer2 = null;

        private float bottom = 0;
        private nfloat amountToScroll = 0;

        #endregion

        #region ===== Properties ==================================================================

        #endregion

        #region ===== Constructor =================================================================

        public LoginView(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetDataAndStyleToView();
            if (!App.Locator.Login.IsDeconnected)
            {
                AutoConnexion();
            }

            // Keyboard popup
            _observer1 = NSNotificationCenter.DefaultCenter.AddObserver
            (UIKeyboard.DidShowNotification, KeyBoardUpNotification);

            // Keyboard Down
            _observer2 = NSNotificationCenter.DefaultCenter.AddObserver
            (UIKeyboard.WillHideNotification, KeyBoardDownNotification);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (!string.IsNullOrEmpty(EmailTextField.Text)) App.Locator.Login.Email = EmailTextField.Text;
            if (!string.IsNullOrEmpty(PasswordTextField.Text)) App.Locator.Login.Password = PasswordTextField.Text;
            if (string.IsNullOrEmpty(EmailTextField.Text) && string.IsNullOrEmpty(PasswordTextField.Text))
            {
                LoginButton.Enabled = false;
            }

            App.Locator.Login.PropertyChanged += OnLoginPropertyChanged;
            LoginButton.TouchUpInside += LoginButton_TouchUpInside;
            EmailTextField.EditingChanged += EmailTextField_EditingChanged;
            PasswordTextField.EditingChanged += PasswordTextField_EditingChanged;
            WhatIsSeekiosButton.TouchUpInside += WhatIsSeekiosButton_TouchUpInside;

            FirstNameTextField.EditingChanged += FirstNameTextField_EditingChanged;
            LastNameTextField.EditingChanged += LastNameTextField_EditingChanged;
            EmailTextFieldOfRegistration.EditingChanged += EmailTextFieldOfRegistration_EditingChanged;
            PasswordTextFieldOfRegistration.EditingChanged += PasswordTextFieldOfRegistration_EditingChanged;
            ConfirmPasswordTextField.EditingChanged += ConfirmPasswordTextField_EditingChanged;

            EmailTextFieldOfForgetPassword.EditingChanged += EmailTextFieldOfForgetPassword_EditingChanged;

            CreateAccountButton.TouchUpInside += CreateAccountButton_TouchUpInside;
            ForgetPasswordButton.TouchUpInside += ForgetPasswordButton_TouchUpInside;
            BackButton.TouchUpInside += BackButton_TouchUpInside;

            RegistrationButton.TouchUpInside += RegistrationButton_TouchUpInside;
            ResetPasswordButton.TouchUpInside += ResetPasswordButton_TouchUpInside;
        }

        public override void ViewWillDisappear(bool animated)
        {
            App.Locator.Login.PropertyChanged -= OnLoginPropertyChanged;
            LoginButton.TouchUpInside -= LoginButton_TouchUpInside;
            EmailTextField.EditingChanged -= EmailTextField_EditingChanged;
            PasswordTextField.EditingChanged -= PasswordTextField_EditingChanged;

            FirstNameTextField.EditingChanged -= FirstNameTextField_EditingChanged;
            LastNameTextField.EditingChanged -= LastNameTextField_EditingChanged;
            EmailTextFieldOfRegistration.EditingChanged -= EmailTextFieldOfRegistration_EditingChanged;
            PasswordTextFieldOfRegistration.EditingChanged -= PasswordTextFieldOfRegistration_EditingChanged;
            ConfirmPasswordTextField.EditingChanged -= ConfirmPasswordTextField_EditingChanged;

            EmailTextFieldOfForgetPassword.EditingChanged -= EmailTextFieldOfForgetPassword_EditingChanged;

            CreateAccountButton.TouchUpInside -= CreateAccountButton_TouchUpInside;
            ForgetPasswordButton.TouchUpInside -= ForgetPasswordButton_TouchUpInside;
            BackButton.TouchUpInside -= BackButton_TouchUpInside;

            RegistrationButton.TouchUpInside -= RegistrationButton_TouchUpInside;
            ResetPasswordButton.TouchUpInside -= ResetPasswordButton_TouchUpInside;

            base.ViewWillDisappear(animated);
            App.Locator.Login.IsLoading = false;
        }

        public override void UpdateViewConstraints()
        {
            base.UpdateViewConstraints();
            //LoginContainer.AddConstraints(new NSLayoutConstraint[] {
            //	  NSLayoutConstraint.Create(_facebookButton, NSLayoutAttribute.Top, NSLayoutRelation.Equal, LoginButton, NSLayoutAttribute.Bottom, 1.0f, 10.0f)
            //	  , NSLayoutConstraint.Create(_facebookButton, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, LoginContainer, NSLayoutAttribute.Trailing, 1.0f, 0.0f)
            //    , NSLayoutConstraint.Create(_facebookButton, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, LoginContainer, NSLayoutAttribute.Leading, 1.0f, 0.0f)
            //    , NSLayoutConstraint.Create(_facebookButton, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1.0f, 30.0f)
            //});
        }

        #endregion

        #region ===== Initialisze View ============================================================

        private void InitializeAllStrings()
        {
            // login
            EmailTextField.Placeholder = Application.LocalizedString("EmailAddress");
            PasswordTextField.Placeholder = Application.LocalizedString("Password");
            LoginButton.SetTitle(Application.LocalizedString("Login"), UIControlState.Normal);
            WhatIsSeekiosButton.SetTitle(Application.LocalizedString("WhatIsSeekios"), UIControlState.Normal);

            // double action forget/create
            ForgetPasswordButton.SetTitle(Application.LocalizedString("ForgetPassword"), UIControlState.Normal);
            CreateAccountButton.SetTitle(Application.LocalizedString("CreateAccount"), UIControlState.Normal);
            BackButton.SetTitle(Application.LocalizedString("Back"), UIControlState.Normal);

            // createAccount
            FirstNameTextField.Placeholder = Application.LocalizedString("FirstName");
            LastNameTextField.Placeholder = Application.LocalizedString("LastName");
            PasswordTextFieldOfRegistration.Placeholder = Application.LocalizedString("Password");
            ConfirmPasswordTextField.Placeholder = Application.LocalizedString("ConfirmPassword");
            RegistrationButton.SetTitle(Application.LocalizedString("CreateMyAccount"), UIControlState.Normal);
            EmailTextFieldOfRegistration.Placeholder = Application.LocalizedString("EmailAddress");
            ErrorMessageLabel.Text = Application.LocalizedString("PasswordErrorLength");

            // forget Password
            EmailTextFieldOfForgetPassword.Placeholder = Application.LocalizedString("EmailAddress");
            ResetPasswordButton.SetTitle(Application.LocalizedString("ResetPassword"), UIControlState.Normal);
        }

        private void SetDataAndStyleToView()
        {
            // init strings in view
            InitializeAllStrings();

            // set up the background 
            //BackgroundImageView2.Image = UIImage.FromBundle("LoginBackground");

            // dissmiss the keyboard when i press return key on email TextField
            EmailTextField.ShouldReturn += (textField) =>
            {
                EmailTextField.ResignFirstResponder();
                PasswordTextField.BecomeFirstResponder();
                return true;
            };

            // dissmiss the keyboard when i press return key on password TextField
            PasswordTextField.ShouldReturn += (textField) =>
            {
                PasswordTextField.ResignFirstResponder();
                LoginButton_TouchUpInside(null, null);
                return true;
            };

            // dissmiss the keyboard when i press return key on Name TextField
            FirstNameTextField.ShouldReturn += (textField) =>
            {
                FirstNameTextField.ResignFirstResponder();
                LastNameTextField.BecomeFirstResponder();
                return true;
            };

            // dissmiss the keyboard when i press return key on FirstName TextField
            LastNameTextField.ShouldReturn += (textField) =>
            {
                LastNameTextField.ResignFirstResponder();
                EmailTextFieldOfRegistration.BecomeFirstResponder();
                return true;
            };

            // dissmiss the keyboard when i press return key on email TextField
            EmailTextFieldOfRegistration.ShouldReturn += (textField) =>
            {
                EmailTextFieldOfRegistration.ResignFirstResponder();
                PasswordTextFieldOfRegistration.BecomeFirstResponder();
                return true;
            };

            // dissmiss the keyboard when i press return key on password TextField
            PasswordTextFieldOfRegistration.ShouldReturn += (textField) =>
            {
                PasswordTextFieldOfRegistration.ResignFirstResponder();
                ConfirmPasswordTextField.BecomeFirstResponder();
                return true;
            };
            PasswordTextFieldOfRegistration.EditingChanged += (o, e) =>
            {
                ErrorMessageLabel.Hidden = true;
            };
            PasswordTextFieldOfRegistration.ShouldBeginEditing += (textField) =>
            {
                if (View.Frame.Y >= 0)
                {
                    bottom = ((float)(RegistrationContainer.Frame.Y + PasswordTextFieldOfRegistration.Frame.Y + PasswordTextFieldOfRegistration.Frame.Height + 10));
                }
                return true;
            };

            // dissmiss the keyboard when i press return key on Confirm Password TextField
            ConfirmPasswordTextField.ShouldReturn += (textField) =>
            {
                ConfirmPasswordTextField.ResignFirstResponder();
                return true;
            };
            ConfirmPasswordTextField.EditingChanged += (o, e) =>
            {
                ErrorMessageLabel.Hidden = true;
            };
            ConfirmPasswordTextField.ShouldBeginEditing += (textField) =>
            {
                if (View.Frame.Y >= 0)
                {
                    bottom = ((float)(RegistrationContainer.Frame.Y + ConfirmPasswordTextField.Frame.Y + ConfirmPasswordTextField.Frame.Height + 10));
                }
                return true;
            };

            // dissmiss the keyboard when i press return key on Confirm Password TextField
            EmailTextFieldOfForgetPassword.ShouldReturn += (textField) =>
            {
                EmailTextFieldOfForgetPassword.ResignFirstResponder();
                return true;
            };

            EmailTextField.KeyboardType = UIKeyboardType.EmailAddress;
            EmailTextField.AutocorrectionType = UITextAutocorrectionType.No;
            PasswordTextField.AutocorrectionType = UITextAutocorrectionType.No;
            FirstNameTextField.ReturnKeyType = UIReturnKeyType.Next;
            LastNameTextField.AutocorrectionType = UITextAutocorrectionType.No;
            LastNameTextField.ReturnKeyType = UIReturnKeyType.Next;
            EmailTextFieldOfRegistration.KeyboardType = UIKeyboardType.EmailAddress;
            EmailTextFieldOfRegistration.AutocorrectionType = UITextAutocorrectionType.No;
            EmailTextFieldOfRegistration.ReturnKeyType = UIReturnKeyType.Next;
            PasswordTextFieldOfRegistration.ReturnKeyType = UIReturnKeyType.Next;
            ConfirmPasswordTextField.ReturnKeyType = UIReturnKeyType.Next;
            EmailTextFieldOfForgetPassword.KeyboardType = UIKeyboardType.EmailAddress;
            EmailTextFieldOfForgetPassword.AutocorrectionType = UITextAutocorrectionType.No;

            // set up the rounded corner fot the button
            LoadingIndicator.Hidden = true;
            LoginButton.Layer.CornerRadius = 4;
            LoginButton.Layer.MasksToBounds = true;

            RegistrationButton.Layer.CornerRadius = 4;
            RegistrationButton.Layer.MasksToBounds = true;
            RegistrationButton.Enabled = false;

            ResetPasswordButton.Layer.CornerRadius = 4;
            ResetPasswordButton.Layer.MasksToBounds = true;
            ResetPasswordButton.Enabled = false;

            //DisplayFacebookButton();
            //_facebookButton.Layer.CornerRadius = 4;
            //_facebookButton.Layer.MasksToBounds = true;

            CreateAccountButton.ContentEdgeInsets = new UIEdgeInsets(5, 0, 5, 5);
            ForgetPasswordButton.ContentEdgeInsets = new UIEdgeInsets(5, 5, 5, 0);

            LoginContainer.Hidden = false;
            RegistrationContainer.Hidden = true;
            ForgetPasswordContainer.Hidden = true;
            BackButton.Hidden = true;
            CreateAccountButton.Hidden = false;
            ForgetPasswordButton.Hidden = false;
            ErrorMessageLabel.Hidden = true;
            ErrorMessageLabel.TextColor = UIColor.FromRGB(255, 76, 56);
        }

        #endregion

        #region ===== Private Methodes ============================================================

        /// <summary>
        /// If the user have credential store in local
        /// Launch the auto connection
        /// </summary>
        private async void AutoConnexion()
        {
            LoadingIndicator.Hidden = false;
            LoadingIndicator.StartAnimating();
            EmailTextField.Hidden = true;
            PasswordTextField.Hidden = true;
            WhatIsSeekiosButton.Hidden = true;
            LoginButton.Hidden = true;
            //_facebookButton.Hidden = true;
            CreateAccountButton.Hidden = true;
            ForgetPasswordButton.Hidden = true;

            await App.Locator.Login.AutoConnect(DeviceInfoHelper.DeviceModel
                , DeviceInfoHelper.Platform
                , DeviceInfoHelper.Version
                , DeviceInfoHelper.GetDeviceUniqueId
                , DeviceInfoHelper.CountryCode);

            LoadingIndicator.StopAnimating();
            LoadingIndicator.Hidden = true;
            EmailTextField.Hidden = false;
            PasswordTextField.Hidden = false;
            WhatIsSeekiosButton.Hidden = false;
            LoginButton.Hidden = false;
            //_facebookButton.Hidden = false;
            CreateAccountButton.Hidden = false;
            ForgetPasswordButton.Hidden = false;
        }

        private void KeyBoardDownNotification(NSNotification notification)
        {
            if (View.Frame.Y < 0)
            {
                SetViewMovedUp(false, new nfloat(amountToScroll * -1));
            }
        }

        private void KeyBoardUpNotification(NSNotification notification)
        {
            if (bottom != 0)
            {
                amountToScroll = ScrollHelper.ScrollOnHiddenUiElement(View, bottom, notification, InterfaceOrientation);
                if (amountToScroll < 0)
                {
                    SetViewMovedUp(true, amountToScroll);
                }
            }
            bottom = 0;
        }

        private void SetViewMovedUp(bool movedUp, nfloat amountToScroll)
        {
            UIView.BeginAnimations(string.Empty, IntPtr.Zero);
            UIView.SetAnimationDuration(0);

            var rect = View.Frame;
            rect.Y += amountToScroll;
            rect.Size = new CoreGraphics.CGSize(rect.Width, rect.Size.Height - amountToScroll);
            View.Frame = rect;

            UIView.CommitAnimations();
        }

        #region FACEBOOK

        ///// <summary>
        ///// Display the Facebook button on the screen
        ///// </summary>
        //private void DisplayFacebookButton()
        //{
        //    //// Localization for French Language
        //    //var userLang = NSLocale.CurrentLocale.LocaleIdentifier;
        //    //if (userLang.Equals("fr_US"))  //en_US for english
        //    //{
        //    //    CreateAccountButton.SetTitle(StringHelper.Translate("GL2-LF-8rE"), UIControlState.Normal);
        //    //    ForgetPasswordButton.SetTitle(StringHelper.Translate("RGo-JK-taY"), UIControlState.Normal);
        //    //}

        //    //// If was send true to Profile.EnableUpdatesOnAccessTokenChange method
        //    //// this notification will be called after the user is logged in and
        //    //// after the AccessToken is gotten
        //    //Profile.Notifications.ObserveDidChange((sender, e) =>
        //    //{
        //    //    if (e.NewProfile == null)
        //    //        return;
        //    //    string nameLabel = e.NewProfile.Name;
        //    //});

        //    // Set the Read and Publish permissions you want to get
        //    //_facebookButton = new LoginButton(new CGRect(0, 150, 256, 30))
        //    //{
        //    //    LoginBehavior = LoginBehavior.Native,
        //    //    ReadPermissions = _readPermissions.ToArray()
        //    //};

        //    //_facebookButton.TranslatesAutoresizingMaskIntoConstraints = false;
        //    //_facebookButton.Alpha = new nfloat(0.8);


        //    //// Handle actions once the user is logged in
        //    //_facebookButton.Completed += (sender, e) =>
        //    //{
        //    //    if (e.Error != null)
        //    //    {
        //    //        // Handle if there was an error
        //    //        new UIAlertView("Login", e.Error.Description, null, "Ok", null).Show();
        //    //        return;
        //    //    }

        //    //    if (e.Result.IsCancelled)
        //    //    {
        //    //        // Handle if the user cancelled the login request
        //    //        new UIAlertView("Login", "The user cancelled the login", null, "Ok", null).Show();
        //    //        return;
        //    //    }

        //    //    // Handle your successful login
        //    //    new UIAlertView("Login", "Success!!", null, "Ok", null).Show();
        //    //    CallFacebookLogin();
        //    //};

        //    //// Handle actions once the user is logged out
        //    //_facebookButton.LoggedOut += (sender, e) =>
        //    //{
        //    //    // Handle your logout
        //    //};

        //    //// The user image profile is set automatically once is logged in
        //    ////	pictureView = new ProfilePictureView(new CGRect(80, 100, 220, 220));

        //    //// If you have been logged into the app before, ask for the your profile namee
        //    //if (AccessToken.CurrentAccessToken != null)
        //    //{
        //    //    CallFacebookLogin();
        //    //}

        //    //// Add views to main view
        //    //LoginContainer.Add(_facebookButton);
        //}

        ///// <summary>
        ///// Get the facebook picture
        ///// </summary>
        //private UIImage GetFacebookPicture(string userID)
        //{
        //    string urlString = "https://graph.facebook.com/" + userID + "/picture?type=large";
        //    return (ImageHelper.FromUrl(urlString));
        //}

        ///// <summary>
        ///// Facebook connection
        ///// </summary>
        //private void CallFacebookLogin()
        //{
        //    // If you have been logged into the app before, ask for the your profile name
        //    if (AccessToken.CurrentAccessToken != null)
        //    {
        //        var request = new GraphRequest("/me?fields=id,first_name,last_name,email", null, AccessToken.CurrentAccessToken.TokenString, null, "GET");
        //        request.Start((connection, result, error) =>
        //        {
        //            // Handle if something went wrong with the reques
        //            if (error != null)
        //            {
        //                new UIAlertView("Error...", error.Description, null, "Ok", null).Show();
        //                return;
        //            }

        //            // Get your profile
        //            var userInfo = result as NSDictionary;
        //            var id = userInfo["id"].ToString();
        //            var first_name = userInfo["first_name"].ToString();
        //            var last_name = userInfo["last_name"].ToString();
        //            var email = userInfo["email"].ToString();
        //            var picture = GetFacebookPicture(id);

        //            if (!string.IsNullOrEmpty(email))
        //            {
        //                try
        //                {
        //                    // This method allows to connect to Facebook with email, password, first_name and last_name
        //                    //if (!await App.Locator.Login.ConnectWithFacebook(email
        //                    //    , firstname
        //                    //    , lastname
        //                    //    , id
        //                    //    , picture
        //                    //    , DeviceInfoHelper.DeviceModel
        //                    //    , DeviceInfoHelper.Platform
        //                    //    , DeviceInfoHelper.Version
        //                    //    , DeviceInfoHelper.GetDeviceUniqueId(this)
        //                    //    , DeviceInfoHelper.GetDeviceIp(this)
        //                    //    , RegistrationIntentService.GCMRegistrationToken))
        //                    //{
        //                    //    PopupErrorFacebook();
        //                    //}
        //                }
        //                catch (TimeoutException)
        //                {

        //                }
        //            }
        //        });
        //    }
        //}

        #endregion

        #region VERIFICATIONS

        private bool VerificationFieldCreateAccount()
        {
            var isFieldsValid = true;
            // check if both password are the same
            if (PasswordTextFieldOfRegistration.Text != ConfirmPasswordTextField.Text)
            {
                PasswordTextFieldOfRegistration.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
                PasswordTextFieldOfRegistration.Layer.BorderWidth = 1;
                PasswordTextFieldOfRegistration.Layer.CornerRadius = 4;
                ConfirmPasswordTextField.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
                ConfirmPasswordTextField.Layer.BorderWidth = 1;
                ConfirmPasswordTextField.Layer.CornerRadius = 4;
                ErrorMessageLabel.Hidden = false;
                ErrorMessageLabel.Text = Application.LocalizedString("ErrorNotTheSamePassword");
                isFieldsValid = false;
            }
            else
            {
                PasswordTextFieldOfRegistration.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
                PasswordTextFieldOfRegistration.Layer.BorderWidth = 1;
                PasswordTextFieldOfRegistration.Layer.CornerRadius = 4;
                ConfirmPasswordTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
                ConfirmPasswordTextField.Layer.BorderWidth = 1;
                ConfirmPasswordTextField.Layer.CornerRadius = 4;
            }
            // check lenght confirm password
            if (string.IsNullOrEmpty(ConfirmPasswordTextField.Text) || ConfirmPasswordTextField.Text.Length < 8)
            {
                ConfirmPasswordTextField.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
                ConfirmPasswordTextField.Layer.BorderWidth = 1;
                ConfirmPasswordTextField.Layer.CornerRadius = 4;
                ErrorMessageLabel.Hidden = false;
                ErrorMessageLabel.Text = Application.LocalizedString("ErrorWeakPassword");
                isFieldsValid = false;
            }
            else
            {
                if (PasswordTextFieldOfRegistration.Text == ConfirmPasswordTextField.Text)
                {
                    ConfirmPasswordTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
                    ConfirmPasswordTextField.Layer.BorderWidth = 1;
                    ConfirmPasswordTextField.Layer.CornerRadius = 4;
                }
            }
            // check lenght password
            if (string.IsNullOrEmpty(PasswordTextFieldOfRegistration.Text) || PasswordTextFieldOfRegistration.Text.Length < 8)
            {
                PasswordTextFieldOfRegistration.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
                PasswordTextFieldOfRegistration.Layer.BorderWidth = 1;
                PasswordTextFieldOfRegistration.Layer.CornerRadius = 4;
                ErrorMessageLabel.Hidden = false;
                ErrorMessageLabel.Text = Application.LocalizedString("ErrorWeakPassword");
                isFieldsValid = false;
            }
            else
            {
                if (PasswordTextFieldOfRegistration.Text == ConfirmPasswordTextField.Text)
                {
                    PasswordTextFieldOfRegistration.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
                    PasswordTextFieldOfRegistration.Layer.BorderWidth = 1;
                    PasswordTextFieldOfRegistration.Layer.CornerRadius = 4;
                }
            }
            // check email
            if (string.IsNullOrEmpty(EmailTextFieldOfRegistration.Text) || !EmailTextFieldOfRegistration.Text.IsEmail())
            {
                EmailTextFieldOfRegistration.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
                EmailTextFieldOfRegistration.Layer.BorderWidth = 1;
                EmailTextFieldOfRegistration.Layer.CornerRadius = 4;
                ErrorMessageLabel.Hidden = false;
                ErrorMessageLabel.Text = Application.LocalizedString("ErrorEmail");
                isFieldsValid = false;
            }
            else
            {
                EmailTextFieldOfRegistration.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
                EmailTextFieldOfRegistration.Layer.BorderWidth = 1;
                EmailTextFieldOfRegistration.Layer.CornerRadius = 4;
            }
            // check last name
            if (string.IsNullOrEmpty(LastNameTextField.Text))
            {
                LastNameTextField.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
                LastNameTextField.Layer.BorderWidth = 1;
                LastNameTextField.Layer.CornerRadius = 4;
                ErrorMessageLabel.Hidden = false;
                ErrorMessageLabel.Text = Application.LocalizedString("ErrorLastName");
                isFieldsValid = false;
            }
            else
            {
                LastNameTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
                LastNameTextField.Layer.BorderWidth = 1;
                LastNameTextField.Layer.CornerRadius = 4;
            }
            // check first name
            if (string.IsNullOrEmpty(FirstNameTextField.Text))
            {
                FirstNameTextField.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
                FirstNameTextField.Layer.BorderWidth = 1;
                FirstNameTextField.Layer.CornerRadius = 4;
                ErrorMessageLabel.Hidden = false;
                ErrorMessageLabel.Text = Application.LocalizedString("ErrorFirstName");
                isFieldsValid = false;
            }
            else
            {
                FirstNameTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
                FirstNameTextField.Layer.BorderWidth = 1;
                FirstNameTextField.Layer.CornerRadius = 4;
            }
            return isFieldsValid;
        }

        #endregion

        #endregion

        #region ===== Event =======================================================================

        /// <summary>
        /// Raised when boolean changed in the LoginViewModel
        /// </summary>
        private void OnLoginPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsLoading")
            {
                var valueEnable = !App.Locator.Login.IsLoading;
                var valueOpacity = new nfloat(App.Locator.Login.IsLoading ? 0.3 : 0.8);

                //_facebookButton.Enabled = valueEnable;
                EmailTextField.Enabled = valueEnable;
                PasswordTextField.Enabled = valueEnable;
                WhatIsSeekiosButton.Enabled = valueEnable;
                LoginButton.UserInteractionEnabled = valueEnable;
                CreateAccountButton.UserInteractionEnabled = valueEnable;
                ForgetPasswordButton.UserInteractionEnabled = valueEnable;

                //_facebookButton.Alpha = valueOpacity;
                EmailTextField.Alpha = valueOpacity;
                PasswordTextField.Alpha = valueOpacity;
                LoginButton.Alpha = valueOpacity;
                CreateAccountButton.Alpha = valueOpacity;
                ForgetPasswordButton.Alpha = valueOpacity;

                EmailTextFieldOfForgetPassword.Enabled = valueEnable;
                ResetPasswordButton.Enabled = valueEnable;
                BackButton.Enabled = valueEnable;
                EmailTextFieldOfForgetPassword.Alpha = valueOpacity;
                ResetPasswordButton.Alpha = valueOpacity;
                BackButton.Alpha = valueOpacity;

                LoadingIndicator.Hidden = valueEnable;
            }
            if (e.PropertyName == "CanConnect")
            {
                LoginButton.Enabled = App.Locator.Login.CanConnect;
            }
            if (e.PropertyName == "CanCreateAccount")
            {
                //CreateAccountButton.Enabled = App.Locator.Login.CanCreateAccount;
            }
            if (e.PropertyName == "CanForgetPassword")
            {
                //ForgetPasswordButton.Enabled = App.Locator.Login.CanForgetPassword;
            }
            if (e.PropertyName == "Password" && App.Locator.Login.Password == string.Empty)
            {
                PasswordTextField.Text = App.Locator.Login.Password;
            }
        }

        /// <summary>
        /// Connect the user to the app
        /// </summary>
        private async void LoginButton_TouchUpInside(object sender, EventArgs e)
        {
            // start the loading animation
            LoadingIndicator.StartAnimating();

            if (!EmailTextField.Text.IsEmail())
            {
                // stop the loading animation
                LoadingIndicator.StopAnimating();

                // set up the EmailTextField border in red
                EmailTextField.Layer.BorderColor = UIColor.FromRGBA(255, 76, 57, 255).CGColor;
                EmailTextField.Layer.BorderWidth = 1;
                EmailTextField.Layer.CornerRadius = 4;
            }
            else
            {
                await App.Locator.Login.Connect(DeviceInfoHelper.DeviceModel
                    , DeviceInfoHelper.Platform
                    , DeviceInfoHelper.Version
                    , DeviceInfoHelper.GetDeviceUniqueId
                    , DeviceInfoHelper.CountryCode);
            }
        }

        /// <summary>
        /// Create an account
        /// </summary>
        private void RegistrationButton_TouchUpInside(object sender, EventArgs e)
        {
            if (VerificationFieldCreateAccount())
            {
                var alert = AlertControllerHelper.CreatePopupCGU(async () =>
               {
                   RegistrationButton.Enabled = false;

                   if (await App.Locator.Login.CreateAccount(FirstNameTextField.Text
                   , LastNameTextField.Text
                   , DeviceInfoHelper.DeviceModel
                   , DeviceInfoHelper.Platform
                   , DeviceInfoHelper.Version
                   , DeviceInfoHelper.GetDeviceUniqueId
                   , DeviceInfoHelper.CountryCode))
                   {
                       RegistrationButton.Enabled = true;
                       BackButton_TouchUpInside(null, null);
                   }
                   else
                   {
                       BackButton_TouchUpInside(null, null);
                   }
               }, () =>
               {
                   UIApplication.SharedApplication.OpenUrl(new NSUrl("https://seekios.com/Home/CGU"));
               });
                PresentViewController(alert, true, null);
            }
        }

        /// <summary>
        /// Reset the password of the user
        /// </summary>
        private async void ResetPasswordButton_TouchUpInside(object sender, EventArgs e)
        {
            LoadingIndicator.StartAnimating();
            if (EmailTextFieldOfForgetPassword.Text.IsEmail())
            {
                await App.Locator.Login.ForgetPassword(EmailTextFieldOfForgetPassword.Text);
                EmailTextFieldOfForgetPassword.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
                EmailTextFieldOfForgetPassword.Layer.BorderWidth = 1;
                EmailTextFieldOfForgetPassword.Layer.CornerRadius = 4;
            }
            else
            {
                EmailTextFieldOfForgetPassword.Layer.BorderColor = UIColor.FromRGBA(255, 76, 57, 255).CGColor;
                EmailTextFieldOfForgetPassword.Layer.BorderWidth = 1;
                EmailTextFieldOfForgetPassword.Layer.CornerRadius = 4;
            }
        }

        /// <summary>
        /// Navigate to create account
        /// </summary>
        private void CreateAccountButton_TouchUpInside(object sender, EventArgs e)
        {
            RegistrationContainer.Hidden = false;
            BackButton.Hidden = false;
            LoginContainer.Hidden = true;
            ForgetPasswordContainer.Hidden = true;
            ForgetPasswordButton.Hidden = true;
            CreateAccountButton.Hidden = true;
        }

        /// <summary>
        /// Navigate to forget passwor
        /// </summary>
        private void ForgetPasswordButton_TouchUpInside(object sender, EventArgs e)
        {
            ForgetPasswordContainer.Hidden = false;
            BackButton.Hidden = false;
            LoginContainer.Hidden = true;
            RegistrationContainer.Hidden = true;
            CreateAccountButton.Hidden = true;
            ForgetPasswordButton.Hidden = true;
        }

        /// <summary>
        /// Handle the back button
        /// </summary>
        private void BackButton_TouchUpInside(object sender, EventArgs e)
        {
            LoginContainer.Hidden = false;
            RegistrationContainer.Hidden = true;
            ForgetPasswordContainer.Hidden = true;
            CreateAccountButton.Hidden = false;
            ForgetPasswordButton.Hidden = false;
            BackButton.Hidden = true;
        }

        /// <summary>
        /// When the password fiel is modified it's updating the password variable in the ViewModel
        /// </summary>
        private void PasswordTextField_EditingChanged(object sender, EventArgs e)
        {
            App.Locator.Login.Password = PasswordTextField.Text;
        }

        /// <summary>
        /// When the email fiel is modified it's updating the email variable in the ViewModel
        /// </summary>
        private void EmailTextField_EditingChanged(object sender, EventArgs e)
        {
            App.Locator.Login.Email = EmailTextField.Text;
        }


        /// <summary>
        /// Open a webbrowser to explain what is seekios
        /// </summary>
        private void WhatIsSeekiosButton_TouchUpInside(object sender, EventArgs e)
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl("https://seekios.com/"));
        }

        /// <summary>
        /// When the first name fiel is modified it's updating the firstname variable in the ViewModel
        /// </summary>
        private void FirstNameTextField_EditingChanged(object sender, EventArgs e)
        {
            App.Locator.Login.UserFirstName = FirstNameTextField.Text;
            FirstNameTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            FirstNameTextField.Layer.BorderWidth = 1;
            FirstNameTextField.Layer.CornerRadius = 4;
            RegistrationButton.Enabled = true;
        }

        /// <summary>
        /// When the Lastname fiel is modified it's updating the Lastname variable in the ViewModel
        /// </summary>
        private void LastNameTextField_EditingChanged(object sender, EventArgs e)
        {
            App.Locator.Login.UserLastName = LastNameTextField.Text;
            LastNameTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            LastNameTextField.Layer.BorderWidth = 1;
            LastNameTextField.Layer.CornerRadius = 4;
            RegistrationButton.Enabled = true;
        }

        /// <summary>
        /// When the emailaddress fiel is modified it's updating the emailaddress variable in the ViewModel
        /// </summary>
        private void EmailTextFieldOfRegistration_EditingChanged(object sender, EventArgs e)
        {
            App.Locator.Login.UserEmail = EmailTextFieldOfRegistration.Text;
            EmailTextFieldOfRegistration.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            EmailTextFieldOfRegistration.Layer.BorderWidth = 1;
            EmailTextFieldOfRegistration.Layer.CornerRadius = 4;
            RegistrationButton.Enabled = true;
        }

        /// <summary>
        /// When the password fiel is modified it's updating the password variable in the ViewModel
        /// </summary>
        private void PasswordTextFieldOfRegistration_EditingChanged(object sender, EventArgs e)
        {
            App.Locator.Login.UserPassword = PasswordTextFieldOfRegistration.Text;
            PasswordTextFieldOfRegistration.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            PasswordTextFieldOfRegistration.Layer.BorderWidth = 1;
            PasswordTextFieldOfRegistration.Layer.CornerRadius = 4;
            RegistrationButton.Enabled = true;
        }

        /// <summary>
        /// When the confirmpassword fiel is modified it's updating the confirmpassword variable in the ViewModel
        /// </summary>
        private void ConfirmPasswordTextField_EditingChanged(object sender, EventArgs e)
        {
            App.Locator.Login.UserConfirmedPassword = ConfirmPasswordTextField.Text;
            ConfirmPasswordTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            ConfirmPasswordTextField.Layer.BorderWidth = 1;
            ConfirmPasswordTextField.Layer.CornerRadius = 4;
            RegistrationButton.Enabled = true;
        }

        /// <summary>
        /// When the emailaddressfor fiel is modified it's updating the password variable in the ViewModel
        /// </summary>
        private void EmailTextFieldOfForgetPassword_EditingChanged(object sender, EventArgs e)
        {
            App.Locator.Login.ForgetPasswordEmail = EmailTextFieldOfForgetPassword.Text;
            if (EmailTextFieldOfForgetPassword.Text.IsEmail())
            {
                ResetPasswordButton.Enabled = true;
            }
            else
            {
                ResetPasswordButton.Enabled = false;
            }
            EmailTextFieldOfForgetPassword.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            EmailTextFieldOfForgetPassword.Layer.BorderWidth = 1;
            EmailTextFieldOfForgetPassword.Layer.CornerRadius = 4;
        }

        #endregion
    }
}