using Foundation;
using SeekiosApp.iOS.Helper;
using System;
using System.Drawing;
using UIKit;
using SeekiosApp.iOS.Views;
using CoreGraphics;
using SeekiosApp.Enum;

namespace SeekiosApp.iOS
{
    public partial class AddSeekiosView : BaseViewController
    {
        #region ===== Attributs ===================================================================

        private bool _isSelectedImage = false;
        private UIImage Picture;
        private string _priviousSeekiosName = string.Empty;
        private string _priviousIMEI = string.Empty;
        private string _priviousPinCode = string.Empty;
        private NSObject _observer1 = null;
        private NSObject _observer2 = null;
        private static nfloat _heightOfThePage = 0;

        #endregion

        #region ===== Constructor =================================================================

        public AddSeekiosView(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            // Keyboard popup
            _observer1 = NSNotificationCenter.DefaultCenter.AddObserver
            (UIKeyboard.DidShowNotification, KeyBoardUpNotification);

            // Keyboard Down
            _observer2 = NSNotificationCenter.DefaultCenter.AddObserver
            (UIKeyboard.WillHideNotification, KeyBoardDownNotification);

            if (App.Locator.AddSeekios.IsAdding)
            {
                // Seekios Name Toolbar (next -> go to IMEI)
                var toolbarName = new UIToolbar();
                toolbarName.BarStyle = UIBarStyle.Default;
                toolbarName.Translucent = true;
                toolbarName.SizeToFit();
                var nextNameButton = new UIBarButtonItem(Application.LocalizedString("Next"), UIBarButtonItemStyle.Done, (s, e) =>
                {
                    SeekiosNameTextField.ResignFirstResponder();
                    SeekiosIMEITextField.BecomeFirstResponder();
                });
                toolbarName.SetItems(new UIBarButtonItem[] { nextNameButton }, true);
                SeekiosNameTextField.InputAccessoryView = toolbarName;

                // Seekios IMEI Toolbar (next -> go to Pin Code)
                var toolbarIMEI = new UIToolbar();
                toolbarIMEI.BarStyle = UIBarStyle.Default;
                toolbarIMEI.Translucent = true;
                toolbarIMEI.SizeToFit();
                var nextIMEIButton = new UIBarButtonItem(Application.LocalizedString("Next"), UIBarButtonItemStyle.Done, (s, e) =>
                {
                    SeekiosIMEITextField.ResignFirstResponder();
                    SeekiosPinCodeTextField.BecomeFirstResponder();
                });
                toolbarIMEI.SetItems(new UIBarButtonItem[] { nextIMEIButton }, true);
                SeekiosIMEITextField.InputAccessoryView = toolbarIMEI;

                // Seekios Pin Code Toolbar (Done -> save the seekios)
                var toolbarPinCode = new UIToolbar();
                toolbarPinCode.BarStyle = UIBarStyle.Default;
                toolbarPinCode.Translucent = true;
                toolbarPinCode.SizeToFit();
                var nextPinCodeButton = new UIBarButtonItem(Application.LocalizedString("DoneButton"), UIBarButtonItemStyle.Done, (s, e) =>
                {
                    SeekiosPinCodeTextField.ResignFirstResponder();
                    SaveButton_TouchUpInside(null, null);
                });
                toolbarPinCode.SetItems(new UIBarButtonItem[] { nextPinCodeButton }, true);
                SeekiosPinCodeTextField.InputAccessoryView = toolbarPinCode;
            }

            SeekiosImageView.AddGestureRecognizer(new UITapGestureRecognizer(() => { ModifySeekiosImageButton_TouchDown(null, null); }));
            ModifySeekiosImageButton.TouchDown += ModifySeekiosImageButton_TouchDown;
            DeleteButton.TouchUpInside += DeleteButton_TouchUpInside;
            BuySeekiosButton.TouchDown += BuySeekiosButton_TouchDown;
            SaveButton.TouchUpInside += SaveButton_TouchUpInside;
            NeedUpdateButton.TouchDown += NeedUpdateButton_TouchDown;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (_isSelectedImage) _isSelectedImage = false;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            if (!_isSelectedImage)
            {
                if (_observer1 != null)
                {
                    NSNotificationCenter.DefaultCenter.RemoveObserver(_observer1);
                    _observer1 = null;
                }
                if (_observer2 != null)
                {
                    NSNotificationCenter.DefaultCenter.RemoveObserver(_observer2);
                    _observer2 = null;
                }
                App.Locator.AddSeekios.UpdateNotificationSetting(NotificationTrackingSwitch.On
                    , NotificationZoneSwitch.On
                    , NotificationDontMoveSwitch.On);
            }
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            nfloat size = 0;
            if (App.Locator.AddSeekios.IsAdding)
            {
                size = DeleteButton.Frame.Y + DeleteButton.Frame.Height + 50;
            }
            else size = NotificationDontMoveSwitch.Frame.Y + DeleteButton.Frame.Height + 50;
            size = new nfloat(size * 1.1);
            _heightOfThePage = size;
            ScrollView.ContentSize = new CGSize(View.Frame.Size.Width, _heightOfThePage);
            ScrollView.DirectionalLockEnabled = true;
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public override void SetDataAndStyleToView()
        {
            InitialiseAllStrings();

            if (App.Locator.AddSeekios.IsAdding)
            {
                NavigationItem.Title = Application.LocalizedString("AddSeekiosTitle");
                NotificationTrackingSwitch.Hidden = true;
                NotificationZoneSwitch.Hidden = true;
                NotificationDontMoveSwitch.Hidden = true;
                NotificationTrackingLabel.Hidden = true;
                NotificationZoneLabel.Hidden = true;
                NotificationDontMoveLabel.Hidden = true;
                DeleteButton.Hidden = true;
            }
            else
            {
                NotificationTrackingSwitch.On = App.Locator.DetailSeekios.SeekiosSelected.SendNotificationOnNewTrackingLocation;
                NotificationZoneSwitch.On = App.Locator.DetailSeekios.SeekiosSelected.SendNotificationOnNewOutOfZoneLocation;
                NotificationDontMoveSwitch.On = App.Locator.DetailSeekios.SeekiosSelected.SendNotificationOnNewDontMoveLocation;

                NavigationItem.Title = Application.LocalizedString("Parameters");
                try
                {
                    // Sometimes : System.Exception: Could not initialize an instance of the type 'UIKit.UIImage': the native 'initWithData:' method returned nil.
                    // Solution : Clean & Rebuild https://forums.xamarin.com/discussion/25248/how-to-find-the-image-that-is-not-found-on-ios
                    SetSeekiosDataToView();
                }
                catch (Exception) { }
            }

            // Round seekios image
            SeekiosImageView.Layer.CornerRadius = SeekiosImageView.Frame.Size.Width / 2;
            SeekiosImageView.ClipsToBounds = true;

            // Round corner on the button modify the image of the seekios
            ModifySeekiosImageButton.Layer.CornerRadius = 4;
            ModifySeekiosImageButton.Layer.BorderWidth = 1;
            ModifySeekiosImageButton.Layer.BorderColor = UIColor.FromRGBA(98, 218, 115, 255).CGColor;
            ModifySeekiosImageButton.ContentEdgeInsets = new UIEdgeInsets(5, 10, 5, 10);

            // Round corner on the button save data
            SaveButton.Layer.CornerRadius = 4;
            SaveButton.Layer.MasksToBounds = true;
            BuySeekiosButton.Layer.CornerRadius = 4;
            BuySeekiosButton.Layer.MasksToBounds = true;
            DeleteButton.Layer.CornerRadius = 4;
            DeleteButton.Layer.MasksToBounds = true;

            // Set up the max of caractere, only digit and the type of the keyboard sould be numeric
            SeekiosNameTextField.EditingChanged += SeekiosNameTextField_EditingChanged;
            SeekiosIMEITextField.EditingChanged += SeekiosIMEITextField_EditingChanged;
            SeekiosPinCodeTextField.EditingChanged += SeekiosPinCodeTextField_EditingChanged;
            SeekiosNameTextField.ShouldReturn += (textField) =>
            {
                SeekiosNameTextField.ResignFirstResponder();
                if (App.Locator.AddSeekios.IsAdding) SeekiosIMEITextField.BecomeFirstResponder();
                return true;
            };
            SeekiosIMEITextField.ShouldReturn += (textField) =>
            {
                SeekiosIMEITextField.ResignFirstResponder();
                return true;
            };
            SeekiosPinCodeTextField.ShouldReturn += (textField) =>
            {
                SeekiosPinCodeTextField.ResignFirstResponder();
                return true;
            };
            SeekiosPinCodeTextField.ShouldChangeCharacters = (textField, range, replacement) =>
            {
                int number;
                return replacement.Length == 0 || int.TryParse(replacement, out number);
            };
            SeekiosIMEITextField.ShouldChangeCharacters = (textField, range, replacement) =>
            {
                int number;
                return replacement.Length == 0 || int.TryParse(replacement, out number);
            };

            if (App.Locator.AddSeekios.IsAdding)
            {
                SeekiosNameTextField.ReturnKeyType = UIReturnKeyType.Next;
                SeekiosIMEITextField.ReturnKeyType = UIReturnKeyType.Next;
                SeekiosPinCodeTextField.ReturnKeyType = UIReturnKeyType.Done;
            }
            else SeekiosNameTextField.ReturnKeyType = UIReturnKeyType.Done;

            SeekiosIMEITextField.KeyboardType = UIKeyboardType.DecimalPad;
            SeekiosPinCodeTextField.KeyboardType = UIKeyboardType.DecimalPad;
        }

        #endregion

        #region ===== Privates Methodes ===========================================================

        private void InitialiseAllStrings()
        {
            ModifySeekiosImageButton.SetTitle(Application.LocalizedString("ModifyImage"), UIControlState.Normal);
            SeekiosNameLabel.Text = Application.LocalizedString("SeekiosName");
            SeekiosNameTextField.Placeholder = Application.LocalizedString("MyNameIs");
            IMEILabel.Text = Application.LocalizedString("IMEINumber");
            SeekiosIMEITextField.Placeholder = Application.LocalizedString("IMEIPlaceholder");
            PinLabel.Text = Application.LocalizedString("PinCode");
            SeekiosPinCodeTextField.Placeholder = Application.LocalizedString("PinPlaceholder");
            SaveButton.SetTitle(Application.LocalizedString("Save"), UIControlState.Normal);
            DeleteButton.SetTitle(Application.LocalizedString("Delete"), UIControlState.Normal);
            NotificationTrackingLabel.Text = Application.LocalizedString("NotificationNewLocationTracking");
            NotificationZoneLabel.Text = Application.LocalizedString("NotificationNewLocationOutOfZone");
            NotificationDontMoveLabel.Text = Application.LocalizedString("NotificationNewLocationMoved");
        }

        private void KeyBoardUpNotification(NSNotification notification)
        {
            // Find what opened the keyboard
            foreach (UIView view in ScrollView.Subviews)
            {
                if (view.IsFirstResponder)
                {
                    ScrollHelper.ScrollOnHiddenUIElement(View
                        , ScrollView
                        , ((float)(view.Frame.Y + view.Frame.Height + 10)) // 10 is extra offset
                        , notification
                        , InterfaceOrientation);
                    break;
                }
            }
        }

        private void KeyBoardDownNotification(NSNotification notification)
        {
            ScrollView.ContentOffset = new CGPoint(0, -64);
        }

        private void SetSeekiosDataToView()
        {
            if (App.Locator.AddSeekios.SeekiosImage != null && App.Locator.AddSeekios.SeekiosImage.Length > 0)
            {
                using (var dataDecoded = new NSData(App.Locator.DetailSeekios.SeekiosSelected.SeekiosPicture
                            , NSDataBase64DecodingOptions.IgnoreUnknownCharacters))
                {
                    SeekiosImageView.Image = new UIImage(dataDecoded);
                }
            }

            SeekiosNameTextField.Text = App.Locator.AddSeekios.SeekiosName;
            SeekiosIMEITextField.Text = App.Locator.AddSeekios.SeekiosIMEI;
            PinLabel.Text = Application.LocalizedString("VersionEmbeddedLabel");
            if (App.CurrentUserEnvironment.LastVersionEmbedded != null
                && App.Locator.DetailSeekios.SeekiosSelected.VersionEmbedded_idversionEmbedded != App.CurrentUserEnvironment.LastVersionEmbedded?.IdVersionEmbedded
                && !App.CurrentUserEnvironment.LastVersionEmbedded.IsBetaVersion)
            {
                SeekiosPinCodeTextField.Text = string.Format("{0} {1}"
                    , App.Locator.DetailSeekios.SeekiosEmbeddedVersion
                    , string.Format(Application.LocalizedString("VersionEmbeddedUpdateLabel")
                    , App.CurrentUserEnvironment.LastVersionEmbedded.VersionName));
                NeedUpdateButton.Hidden = false;
            }
            else SeekiosPinCodeTextField.Text = App.Locator.DetailSeekios.SeekiosEmbeddedVersion;
            MaxSizeSeekiosNameLabel.Text = App.Locator.AddSeekios.SeekiosName.Length + "/30";
            MaxSizeSeekiosIMEILabel.Text = "15/15";
            MaxSizeSeekiosPinCodeLabel.Hidden = true;
            SeekiosIMEITextField.Enabled = false;
            SeekiosPinCodeTextField.Enabled = false;
            SaveButton.Enabled = false;
        }

        private void EnableSaveButton()
        {
            if (SeekiosNameTextField.Text.Length > 0 && SeekiosPinCodeTextField.Text.Length == 4 && SeekiosIMEITextField.Text.Length == 15)
            {
                SaveButton.Enabled = true;
            }
            else SaveButton.Enabled = false;
        }

        private void PictureSelected(NSDictionary pictureDict)
        {
            float maxWidth = 100;
            if ((pictureDict[UIImagePickerController.MediaType] as NSString) == "public.image")
            {
                Picture = pictureDict[UIImagePickerController.EditedImage] as UIImage;
                if (Picture == null)
                {
                    Picture = pictureDict[UIImagePickerController.OriginalImage] as UIImage;
                }
                var size = Picture.Size;
                // Show the UI, and on a callback, do the scaling, so the user gets an animation
                NSTimer.CreateScheduledTimer(TimeSpan.FromSeconds(0), delegate
                {
                    if (size.Width > maxWidth || size.Height > maxWidth)
                    {
                        Picture = Scale(Picture, new SizeF(maxWidth, (float)(maxWidth * size.Height / size.Width)));
                    }
                    SeekiosImageView.Image = Picture;
                    App.Locator.AddSeekios.SeekiosImage = ImageHelper.ImageToByte(Picture);
                    if (App.Locator.AddSeekios.IsAdding) EnableSaveButton();
                    else SaveButton.Enabled = true;
                });
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

        #endregion

        #region ===== Event =======================================================================

        /// <summary>
        /// Open the webbrowser to buy a seekios
        /// </summary>

        private void BuySeekiosButton_TouchDown(object sender, EventArgs e)
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(App.BuySeekiosLink));
        }

        /// <summary>
        /// Display a popup with information about the seekios update
        /// </summary>
        private void NeedUpdateButton_TouchDown(object sender, EventArgs e)
        {
            var popup = AlertControllerHelper.CreatePopupSeekiosNeedUpdate(() =>
            {
                UIApplication.SharedApplication.OpenUrl(new NSUrl(App.TutorialHelpLink));
            });
            PresentViewController(popup, true, null);
        }

        /// <summary>
        /// Save the Seekios in the database
        /// </summary>
        private async void SaveButton_TouchUpInside(object sender, EventArgs e)
        {
            // Insert or update the seekios in database
            if (await App.Locator.AddSeekios.InsertOrUpdateSeekios(SeekiosNameTextField.Text
                , SeekiosIMEITextField.Text
                , SeekiosPinCodeTextField.Text
                , App.Locator.AddSeekios.SeekiosImage))
            {
                if (App.Locator.AddSeekios.IsAdding)
                {
                    App.Locator.AddSeekios.IsGoingBack = true;
                    App.Locator.AddSeekios.IsAdding = true;
                    GoBack(true);
                }
            }
        }

        /// <summary>
        /// On seekios text name changed
        /// </summary>
        private void SeekiosNameTextField_EditingChanged(object sender, EventArgs e)
        {
            if (SeekiosNameTextField.Text.Length > 30)
            {
                SeekiosNameTextField.Text = _priviousSeekiosName;
            }
            else
            {
                _priviousSeekiosName = SeekiosNameTextField.Text;
                MaxSizeSeekiosNameLabel.Text = string.Format(Application.LocalizedString("MaxSizeSeekiosName"), _priviousSeekiosName.Length);
                if (!App.Locator.AddSeekios.IsAdding)
                {
                    if (!SaveButton.Enabled) SaveButton.Enabled = true;
                }
                else EnableSaveButton();
            }
        }

        /// <summary>
        /// On seekios IMEI text changed
        /// </summary>
        private void SeekiosIMEITextField_EditingChanged(object sender, EventArgs e)
        {
            if (SeekiosIMEITextField.Text.Length > 15)
            {
                SeekiosIMEITextField.Text = _priviousIMEI;
            }
            else
            {
                _priviousIMEI = SeekiosIMEITextField.Text;
                MaxSizeSeekiosIMEILabel.Text = string.Format(Application.LocalizedString("MaxSizeIMEI"), _priviousIMEI.Length);
                App.Locator.AddSeekios.SeekiosIMEI = _priviousIMEI;
                EnableSaveButton();
            }
        }

        /// <summary>
        /// On seekios pin code changed
        /// </summary>
        private void SeekiosPinCodeTextField_EditingChanged(object sender, EventArgs e)
        {
            if (SeekiosPinCodeTextField.Text.Length > 4)
            {
                SeekiosPinCodeTextField.Text = _priviousPinCode;
            }
            else
            {
                _priviousPinCode = SeekiosPinCodeTextField.Text;
                MaxSizeSeekiosPinCodeLabel.Text = string.Format(Application.LocalizedString("MaxSizePin"), _priviousPinCode.Length);
                App.Locator.AddSeekios.SeekiosPIN = _priviousPinCode;
                EnableSaveButton();
            }
        }

        /// <summary>
        /// Display the popup to choose between camera and librairy
        /// </summary>
        private void ModifySeekiosImageButton_TouchDown(object sender, EventArgs e)
        {
            var alert = AlertControllerHelper.CreateActionSheetToPickPictureSeekios(() =>
           {
               _isSelectedImage = true;
               if (UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
               {
                   CameraHelper.TakePicture(this, PictureSelected);
               }
               else CameraHelper.SelectPicture(this, PictureSelected);
           }
           , () =>
           {
               _isSelectedImage = true;
               CameraHelper.SelectPicture(this, PictureSelected);
           }
           , () =>
           {
               if (App.Locator.AddSeekios.UpdatingSeekios != null
                    && !string.IsNullOrEmpty(App.Locator.AddSeekios.UpdatingSeekios.SeekiosPicture))
               {
                   _isSelectedImage = true;
                   SeekiosImageView.Image = UIImage.FromBundle("DefaultSeekios");
                   App.Locator.AddSeekios.SeekiosImage = null;
                   SaveButton.Enabled = true;
               }
           });
            PresentViewController(alert, true, null);
        }

        /// <summary>
        /// Delete the seekios
        /// </summary>
        private async void DeleteButton_TouchUpInside(object sender, EventArgs e)
        {
            if (await AlertControllerHelper.ShowAlert(Application.LocalizedString("Delete")
                , Application.LocalizedString("SureToDeleteSeekios")
                , Application.LocalizedString("Yes")
                , Application.LocalizedString("No")) == 0)
            {
                // display the loading layout
                var alertLoading = AlertControllerHelper.ShowAlertLoading();
                PresentViewController(alertLoading, true, async () =>
                {
                    if (await App.Locator.DetailSeekios.DeleteSeekios() == 1)
                    {
                        DismissViewController(false, () =>
                        {
                            App.Locator.AddSeekios.IsGoingBack = true;
                            App.Locator.AddSeekios.IsAdding = true;
                            GoBack(true);
                        });
                    }
                    else DismissViewController(false, null);
                });
            }
        }

        #endregion
    }
}