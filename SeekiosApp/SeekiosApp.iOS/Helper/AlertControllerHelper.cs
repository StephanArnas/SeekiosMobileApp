using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using System.Drawing;
using System.Text.RegularExpressions;
using Contacts;
using SeekiosApp.Model.DTO;
using System.Linq;
using SeekiosApp.Services;
using SeekiosApp.Extension;

namespace SeekiosApp.iOS.Helper
{
    public static class AlertControllerHelper
    {
        /// <summary>
		/// Create popup for selecting a picture between the camera and the library
		/// </summary>
		/// <param name="button1">callback for the first button</param>
		/// <param name="button2">callback for the second button</param>
		public static UIAlertController CreateActionSheetToPickPictureSeekios(Action callback1, Action callback2, Action callback3)
        {
            var actionSheetAlert = UIAlertController.Create(Application.LocalizedString("PhotographySeekios"), string.Empty, UIAlertControllerStyle.ActionSheet);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Camera")
                , UIAlertActionStyle.Default
                , (action) => callback1.Invoke()));

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("ImageGalery")
                , UIAlertActionStyle.Default
                , (action) => callback2.Invoke()));

            if (App.Locator.AddSeekios.UpdatingSeekios != null
                && !string.IsNullOrEmpty(App.Locator.AddSeekios.UpdatingSeekios.SeekiosPicture))
            {
                var buttonDeleteImage = UIAlertAction.Create(Application.LocalizedString("DeleteImage")
                    , UIAlertActionStyle.Default
                    , (action) => callback3.Invoke());
                buttonDeleteImage.SetValueForKey(UIColor.FromRGB(255, 76, 56), new NSString("titleTextColor"));
                actionSheetAlert.AddAction(buttonDeleteImage);
            }

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Close")
                , UIAlertActionStyle.Cancel
                , null));

            return actionSheetAlert;
        }

        public static UIAlertController CreateActionSheetToPickPictureUser(Action callback1, Action callback2, Action callback3)
        {
            var actionSheetAlert = UIAlertController.Create(Application.LocalizedString("PhotographyUser"), string.Empty, UIAlertControllerStyle.ActionSheet);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Camera")
                , UIAlertActionStyle.Default
                , (action) => callback1.Invoke()));

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("ImageGalery")
                , UIAlertActionStyle.Default
                , (action) => callback2.Invoke()));

            if (!string.IsNullOrEmpty(App.CurrentUserEnvironment.User.UserPicture))
            {
                var buttonDeleteImage = UIAlertAction.Create(Application.LocalizedString("DeleteImage")
                    , UIAlertActionStyle.Default
                    , (action) => callback3.Invoke());
                buttonDeleteImage.SetValueForKey(UIColor.FromRGB(255, 76, 56), new NSString("titleTextColor"));
                actionSheetAlert.AddAction(buttonDeleteImage);
            }

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Close")
                , UIAlertActionStyle.Cancel
                , null));

            return actionSheetAlert;
        }

        public static UIAlertController CreateAlertOnMarkerMap(Action callback1, Action callback2, Action callback3)
        {
            var actionSheetAlert = UIAlertController.Create(string.Empty, string.Empty, UIAlertControllerStyle.ActionSheet);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Navigate")
                , UIAlertActionStyle.Default
                , (action) => callback1.Invoke()));
            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Share")
                , UIAlertActionStyle.Default
                , (action) => callback2.Invoke()));
            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("CopyAdress")
                , UIAlertActionStyle.Default
                , (action) => callback3.Invoke()));
            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Close")
                , UIAlertActionStyle.Cancel
                , null));

            return actionSheetAlert;
        }

        public static UIAlertController CreateAlertOnMarkerMapNoAdress()
        {
            var actionSheetAlert = UIAlertController.Create(string.Empty
                , Application.LocalizedString("NoAdressSeekiosContent")
                , UIAlertControllerStyle.Alert);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Close")
                , UIAlertActionStyle.Cancel
                , null));

            return actionSheetAlert;
        }

        public static UIAlertController CreateAlertSeekiosInPowerSaving()
        {
            var actionSheetAlert = UIAlertController.Create(string.Empty
                , Application.LocalizedString("PowerSavingOn")
                , UIAlertControllerStyle.Alert);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Close")
                , UIAlertActionStyle.Cancel
                , null));

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("ShowPowerSavingTuto")
                , UIAlertActionStyle.Default
                , (action) => { App.Locator.Parameter.GoToTutorialPowerSaving(); }));

            return actionSheetAlert;
        }

        public static UIAlertController CreatePopupCGU(Action callback1, Action callback2)
        {
            var actionSheetAlert = UIAlertController.Create(Application.LocalizedString("ConditionsOfUseTitle")
                , Application.LocalizedString("ConditionsOfUseContent")
                , UIAlertControllerStyle.Alert);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("AcceptConditions")
                , UIAlertActionStyle.Default
                , (action) => callback1.Invoke()));

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("ReadConditions")
                , UIAlertActionStyle.Default
                , (action) => callback2.Invoke()));

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Cancel")
                , UIAlertActionStyle.Cancel, null));

            return actionSheetAlert;
        }

        public static UIAlertController CreatePopupInforBatteryCost(Action callback1, Action callback2)
        {
            var actionSheetAlert = UIAlertController.Create(Application.LocalizedString("UpdateCostTitle")
                , Application.LocalizedString("UpdateCostContent")
                , UIAlertControllerStyle.Alert);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Update")
                , UIAlertActionStyle.Default
                , (action) => callback1.Invoke()));

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("UpdateAndRemember")
                , UIAlertActionStyle.Default
                , (action) => callback2.Invoke()));

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Cancel")
                , UIAlertActionStyle.Cancel, null));

            return actionSheetAlert;
        }

        public static UIAlertController CreatePopupMaximumPointZone()
        {
            var actionSheetAlert = UIAlertController.Create(Application.LocalizedString("Max10Points")
                        , Application.LocalizedString("CantDefineZone")
                        , UIAlertControllerStyle.Alert);
            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Close")
                , UIAlertActionStyle.Default
                , null));

            return actionSheetAlert;
        }

        public static UIAlertController CreateAlertToInformeExistingMode(string seekios, string modeDisplay, Action callback1)
        {
            var actionSheetAlert = UIAlertController.Create(Application.LocalizedString("DeleteActiveModeTitle")
                , string.Format(Application.LocalizedString("DeleteActiveModeContent")
                , modeDisplay, seekios)
                , UIAlertControllerStyle.Alert);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Yes")
                , UIAlertActionStyle.Default
                , (action) => callback1.Invoke()));

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("No")
                , UIAlertActionStyle.Default
                , null));

            return actionSheetAlert;
        }

        public static UIAlertController CreateAlertToInformSeekiosPositionMoreThan1Hour(Action callback1)
        {
            var actionSheetAlert = UIAlertController.Create(Application.LocalizedString("OldSeekiosPositionTitlePopup")
                , Application.LocalizedString("OldSeekiosPositionMessagePopup")
                , UIAlertControllerStyle.Alert);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Continue")
                , UIAlertActionStyle.Default
                , (action) => callback1.Invoke()));

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Cancel")
                , UIAlertActionStyle.Default
                , null));

            return actionSheetAlert;
        }

        public static UIAlertController CreateAlertToInformAlertSOS(Action callback1)
        {
            var actionSheetAlert = UIAlertController.Create(Application.LocalizedString("SOSAlertReceivedTitle")
                , Application.LocalizedString("SOSAlertReceivedContent")
                , UIAlertControllerStyle.Alert);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Seen")
                , UIAlertActionStyle.Default
                , (action) => callback1.Invoke()));

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Cancel")
                , UIAlertActionStyle.Default
                , null));

            return actionSheetAlert;
        }

        /// <summary>
        /// Create popup for refreshing the seekios position
        /// </summary>
        /// <param name="callback1">callback for the first button</param>
        /// <param name="callback2">callback for the second button</param>
        /// <param name="callback3">callback for the third button</param>
        public static UIAlertController CreateAlertOnClickRefreshButton(string seekios)
        {
            var actionSheetAlert = UIAlertController.Create(Application.LocalizedString("RefreshInProgress")
                , Application.LocalizedString("ReceivingSeekiosLastPosition") + seekios + Application.LocalizedString("TimeTaken")
                , UIAlertControllerStyle.Alert);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Close")
                , UIAlertActionStyle.Cancel
                , null));

            return actionSheetAlert;
        }

        public static UIAlertController CreateBackgroundNotificationAreNotAvailable()
        {
            var actionSheetAlert = UIAlertController.Create(Application.LocalizedString("NotificationErrorTitle")
                , Application.LocalizedString("NotificationErrorDescr")
                , UIAlertControllerStyle.Alert);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Close")
                , UIAlertActionStyle.Cancel
                , null));

            return actionSheetAlert;
        }

        /// <summary>
        /// Create popup to select a mode
        /// </summary>
        /// <param name="callback1">callback for the first button</param>
        /// <param name="callback2">callback for the second button</param>
        /// <param name="callback3">callback for the third button</param>
        /// <returns></returns>
        public static UIAlertController CreateAlertToPickMode(Action callback1, Action callback2, Action callback3, Action callback4, ModeDTO mode)
        {
            var actionSheetAlert = UIAlertController.Create("Modes", "", UIAlertControllerStyle.Alert);
            int sizeImage = 40;
            var buttonModeZone = UIAlertAction.Create("Zone"
                , UIAlertActionStyle.Default
                , (action) => callback1.Invoke());
            var buttonModeDontMove = UIAlertAction.Create("Don't Move"
                , UIAlertActionStyle.Default
                , (action) => callback2.Invoke());
            var buttonModeTracking = UIAlertAction.Create("Tracking"
                , UIAlertActionStyle.Default
                , (action) => callback3.Invoke());

            var imageModeZone = UIImage.FromBundle("ModeZone").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
            var resizeImageModeZone = ImageHelper.ResizeImage(imageModeZone, sizeImage, sizeImage).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
            var imageModeDontMove = UIImage.FromBundle("ModeDontMove").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
            var resizeImageModeDontMove = ImageHelper.ResizeImage(imageModeDontMove, sizeImage, sizeImage).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
            var imageModeTracking = UIImage.FromBundle("ModeTracking").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
            var resizeImageModeTracking = ImageHelper.ResizeImage(imageModeTracking, sizeImage, sizeImage).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);

            buttonModeZone.SetValueForKey(resizeImageModeZone
                , new NSString("image"));
            buttonModeDontMove.SetValueForKey(resizeImageModeDontMove
                , new NSString("image"));
            buttonModeTracking.SetValueForKey(resizeImageModeTracking
                , new NSString("image"));

            actionSheetAlert.AddAction(buttonModeZone);
            actionSheetAlert.AddAction(buttonModeDontMove);
            actionSheetAlert.AddAction(buttonModeTracking);

            if (mode != null)
            {
                if (mode.ModeDefinition_idmodeDefinition == (int)Enum.ModeDefinitionEnum.ModeZone)
                {
                    buttonModeZone.SetValueForKey(UIColor.FromRGB(98, 218, 115), new NSString("titleTextColor"));
                }
                else if (mode.ModeDefinition_idmodeDefinition == (int)Enum.ModeDefinitionEnum.ModeDontMove)
                {
                    buttonModeDontMove.SetValueForKey(UIColor.FromRGB(98, 218, 115), new NSString("titleTextColor"));
                }
                else if (mode.ModeDefinition_idmodeDefinition == (int)Enum.ModeDefinitionEnum.ModeTracking)
                {
                    buttonModeTracking.SetValueForKey(UIColor.FromRGB(98, 218, 115), new NSString("titleTextColor"));
                }

                var buttonDeleteMode = UIAlertAction.Create("Delete Mode"
                    , UIAlertActionStyle.Default
                    , (action) => callback4.Invoke());
                buttonDeleteMode.SetValueForKey(UIColor.FromRGB(255, 76, 57), new NSString("titleTextColor"));

                var imageModeDelete = UIImage.FromBundle("Trash").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                var resizeImageModeDelete = ImageHelper.ResizeImage(imageModeDelete, sizeImage, sizeImage).ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                buttonDeleteMode.SetValueForKey(resizeImageModeDelete, new NSString("image"));
                actionSheetAlert.AddAction(buttonDeleteMode);

                imageModeDelete.Dispose();
                resizeImageModeDelete.Dispose();
            }

            actionSheetAlert.AddAction(UIAlertAction.Create("Fermer", UIAlertActionStyle.Cancel, null));

            imageModeZone.Dispose();
            resizeImageModeZone.Dispose();
            imageModeDontMove.Dispose();
            resizeImageModeDontMove.Dispose();
            imageModeTracking.Dispose();
            resizeImageModeTracking.Dispose();

            return actionSheetAlert;
        }

        public static UIAlertController CreatePopupSeekiosNeedUpdate(Action callback1)
        {
            NSError error = null;
            var content = new NSAttributedString(App.CurrentUserEnvironment.LastVersionEmbedded.ReleaseNotes
                , new NSAttributedStringDocumentAttributes { DocumentType = NSDocumentType.HTML, StringEncoding = NSStringEncoding.UTF8 }
                , ref error);

            var label = new UILabel();
            label.Frame = new CoreGraphics.CGRect(10, 80, 260, 900);
            label.AttributedText = content;
            label.Lines = 100;
            label.SizeToFit();

            var customView = new UIView();
            customView.Frame = new CoreGraphics.CGRect(0, 0, 300, label.Frame.Size.Height);
            customView.Add(label);

            string slashn = string.Empty;
            for (int i = 0; i < label.Frame.Size.Height / 23; i++)
            {
                slashn += "\n";
            }
            slashn += "\n";

            var actionSheetAlert = UIAlertController.Create(string.Format("{0} ({1}){2}"
                , Application.LocalizedString("SeekiosNeedUpdateTitlePopup")
                , App.CurrentUserEnvironment.LastVersionEmbedded.VersionName
                , slashn)
                , null
                , UIAlertControllerStyle.Alert);

            actionSheetAlert.Add(customView);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("SeekiosNeedUpdateGoWebsiteButton")
                , UIAlertActionStyle.Default
                , (action) => callback1.Invoke()));

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Cancel")
                , UIAlertActionStyle.Default
                , null));

            return actionSheetAlert;
        }

        /// <summary>
        /// Create popup 
        /// </summary>
        /// <param name="title">title of the popup</param>
        /// <param name="message">message of the popup</param>
        /// <param name="buttons">buttun in the popup</param>
        /// <returns></returns>
        public static Task<int> ShowAlert(string title, string message, params string[] buttons)
        {
            var tcs = new TaskCompletionSource<int>();

            var alert = new UIAlertView
            {
                Title = title,
                Message = message
            };

            foreach (var button in buttons)
            {
                alert.AddButton(button);
            }

            alert.Clicked += (s, e) => tcs.TrySetResult((int)e.ButtonIndex);
            alert.Show();

            return tcs.Task;
        }

        /// <summary>
        /// Create popup to show the something is under developpement
        /// </summary>
        public static void ShowAlertDeveloppementInProgress()
        {
            new UIAlertView(Application.LocalizedString("InDevelopment")
                , Application.LocalizedString("BeingDeveloped")
                , null
                , Application.LocalizedString("Close")
                , null).Show();
        }

        public static UIAlertController CreateAlertFailedSeekios(string seekios)
        {
            var actionSheetAlert = UIAlertController.Create(Application.LocalizedString("NoPositionFound")
                , Application.LocalizedString("TheRefreshOf") + seekios + Application.LocalizedString("RefreshFailed")
                , UIAlertControllerStyle.Alert);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Close")
                , UIAlertActionStyle.Cancel
                , null));

            return actionSheetAlert;
        }

        /// <summary>
        /// Create popup to show a progress bar for loading
        /// </summary>
        /// <returns></returns>
        public static UIAlertController ShowAlertLoading()
        {
            var actionSheetAlert = UIAlertController.Create(null
                , Application.LocalizedString("Loading_n")
                , UIAlertControllerStyle.Alert);

            var indicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            indicator.Center = new CoreGraphics.CGPoint(130.5, 65.5);
            indicator.Color = UIColor.Black;
            indicator.StartAnimating();
            actionSheetAlert.Add(indicator);

            return actionSheetAlert;
        }

        /// <summary>
        /// Create a popup to enter a contact 
        /// </summary>
        /// <param name="callback">call back to get the contact</param>
        public static UIAlertController ShowAlertToAddContact(Func<UITextField, UITextField, bool> callback)
        {
            var actionSheetAlert = UIAlertController.Create(null
                , Application.LocalizedString("AddContact_n")
                , UIAlertControllerStyle.Alert);

            UITextField emailAddressTextField = null;
            UITextField contactTextField = null;

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Add")
                , UIAlertActionStyle.Default
                , (action) => callback.Invoke(contactTextField, emailAddressTextField)));
            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Close")
                , UIAlertActionStyle.Cancel
                , null));

            (actionSheetAlert.Actions[0] as UIAlertAction).Enabled = false;

            var customView = new UIView();
            customView.Frame = new CoreGraphics.CGRect(0, 0, 270, 210);

            var contactNameLable = new UILabel();
            contactNameLable.Text = Application.LocalizedString("NameSurname");
            contactNameLable.Frame = new RectangleF(20f, 60f, 150f, 20f);
            contactNameLable.Font = UIFont.FromName("Helvetica Neue", 15f);
            contactNameLable.TextColor = UIColor.FromRGB(153, 153, 153);

            contactTextField = new UITextField();
            contactTextField.Frame = new RectangleF(20f, 90f, 230f, 35f);
            contactTextField.Placeholder = Application.LocalizedString("NameSurnamePH");
            contactTextField.Layer.BorderWidth = 1;
            contactTextField.Layer.CornerRadius = 4;
            contactTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            contactTextField.Font = UIFont.FromName("Helvetica Neue", 17f);
            contactTextField.AutocorrectionType = UITextAutocorrectionType.No;
            contactTextField.KeyboardType = UIKeyboardType.Default;
            contactTextField.ReturnKeyType = UIReturnKeyType.Done;
            contactTextField.ClearButtonMode = UITextFieldViewMode.WhileEditing;
            contactTextField.TextColor = UIColor.FromRGB(102, 102, 102);
            contactTextField.EditingChanged += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(emailAddressTextField.Text) && emailAddressTextField.Text.IsEmail() && actionSheetAlert.Actions[0] != null)
                {
                    (actionSheetAlert.Actions[0] as UIAlertAction).Enabled = true;
                }
                else if (actionSheetAlert.Actions[0] != null)
                {
                    (actionSheetAlert.Actions[0] as UIAlertAction).Enabled = false;
                }
            };

            var emailAddressLabel = new UILabel();
            emailAddressLabel.Text = Application.LocalizedString("EmailAddress");
            emailAddressLabel.Frame = new RectangleF(20f, 130f, 150f, 20f);
            emailAddressLabel.Font = UIFont.FromName("Helvetica Neue", 15f);
            emailAddressLabel.TextColor = UIColor.FromRGB(153, 153, 153);

            emailAddressTextField = new UITextField();
            emailAddressTextField.Frame = new RectangleF(20f, 160f, 230f, 35f);
            emailAddressTextField.Placeholder = Application.LocalizedString("EmailAddressPH");
            emailAddressTextField.Font = UIFont.FromName("Helvetica Neue", 17f);
            emailAddressTextField.AutocorrectionType = UITextAutocorrectionType.No;
            emailAddressTextField.Layer.BorderWidth = 1;
            emailAddressTextField.Layer.CornerRadius = 4;
            emailAddressTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            emailAddressTextField.AutocorrectionType = UITextAutocorrectionType.No;
            emailAddressTextField.AutocapitalizationType = UITextAutocapitalizationType.None;
            emailAddressTextField.KeyboardType = UIKeyboardType.EmailAddress;
            emailAddressTextField.ReturnKeyType = UIReturnKeyType.Done;
            emailAddressTextField.ClearButtonMode = UITextFieldViewMode.WhileEditing;
            emailAddressTextField.TextColor = UIColor.FromRGB(102, 102, 102);
            emailAddressTextField.ShouldReturn += (textField) =>
            {
                return textField.ResignFirstResponder();
            };
            emailAddressTextField.EditingChanged += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(emailAddressTextField.Text) && emailAddressTextField.Text.IsEmail() && actionSheetAlert.Actions[0] != null)
                {
                    (actionSheetAlert.Actions[0] as UIAlertAction).Enabled = true;
                }
                else if (actionSheetAlert.Actions[0] != null)
                {
                    (actionSheetAlert.Actions[0] as UIAlertAction).Enabled = false;
                }
            };

            customView.AddSubview(contactNameLable);
            customView.AddSubview(emailAddressLabel);
            customView.AddSubview(contactTextField);
            customView.AddSubview(emailAddressTextField);

            actionSheetAlert.Add(customView);

            return actionSheetAlert;
        }

        public static UIAlertController ShowPopupUpdatePassword(Func<UITextField, UITextField, UITextField, UILabel, bool> callback)
        {
            var actionSheetAlert = UIAlertController.Create(null
                , Application.LocalizedString("NewPassword_n")
                , UIAlertControllerStyle.Alert);

            UILabel errorLabel = null;
            UITextField passwordOld = null;
            UITextField password1 = null;
            UITextField password2 = null;

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Modify")
                , UIAlertActionStyle.Default
                , (action) => { callback.Invoke(passwordOld, password1, password2, errorLabel); }));

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Cancel")
                , UIAlertActionStyle.Cancel
                , null));

            (actionSheetAlert.Actions[0] as UIAlertAction).Enabled = false;

            var customView = new UIView();
            customView.Frame = new CoreGraphics.CGRect(0, 0, 270, 350);

            var passwordOldLabel = new UILabel();
            passwordOldLabel.Text = Application.LocalizedString("ActualPassword");
            passwordOldLabel.Frame = new RectangleF(20f, 60f, 230f, 20f);
            passwordOldLabel.Font = UIFont.FromName("Helvetica Neue", 15f);
            passwordOldLabel.TextColor = UIColor.FromRGB(153, 153, 153);

            passwordOld = new UITextField();
            passwordOld.Frame = new RectangleF(20f, 90f, 230f, 35f);
            passwordOld.Placeholder = Application.LocalizedString("ActualPH");
            passwordOld.Layer.BorderWidth = 1;
            passwordOld.Layer.CornerRadius = 4;
            passwordOld.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            passwordOld.Font = UIFont.FromName("Helvetica Neue", 17f);
            passwordOld.AutocorrectionType = UITextAutocorrectionType.No;
            passwordOld.AutocapitalizationType = UITextAutocapitalizationType.None;
            passwordOld.KeyboardType = UIKeyboardType.Default;
            passwordOld.SecureTextEntry = true;
            passwordOld.ReturnKeyType = UIReturnKeyType.Done;
            passwordOld.ClearButtonMode = UITextFieldViewMode.WhileEditing;
            passwordOld.TextColor = UIColor.FromRGB(102, 102, 102);
            passwordOld.EditingChanged += (sender, e) =>
            {
                VerificationFieldsPassword(passwordOld, password1, password2, errorLabel, actionSheetAlert);
            };
            passwordOld.ShouldReturn += (textField) =>
             {
                 passwordOld.ResignFirstResponder();
                 return true;
             };

            var password1Label = new UILabel();
            password1Label.Text = Application.LocalizedString("NewPassword");
            password1Label.Frame = new RectangleF(20f, 130f, 230f, 20f);
            password1Label.Font = UIFont.FromName("Helvetica Neue", 15f);
            password1Label.TextColor = UIColor.FromRGB(153, 153, 153);

            password1 = new UITextField();
            password1.Frame = new RectangleF(20f, 160f, 230f, 35f);
            password1.Placeholder = Application.LocalizedString("NewPH");
            password1.Layer.BorderWidth = 1;
            password1.Layer.CornerRadius = 4;
            password1.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            password1.Font = UIFont.FromName("Helvetica Neue", 17f);
            password1.AutocorrectionType = UITextAutocorrectionType.No;
            password1.AutocapitalizationType = UITextAutocapitalizationType.None;
            password1.KeyboardType = UIKeyboardType.Default;
            password1.SecureTextEntry = true;
            password1.ReturnKeyType = UIReturnKeyType.Done;
            password1.ClearButtonMode = UITextFieldViewMode.WhileEditing;
            password1.TextColor = UIColor.FromRGB(102, 102, 102);
            password1.EditingChanged += (sender, e) =>
            {
                VerificationFieldsPassword(passwordOld, password1, password2, errorLabel, actionSheetAlert);
            };
            password1.ShouldReturn += (textField) =>
            {
                password1.ResignFirstResponder();
                return true;
            };

            var password2Label = new UILabel();
            password2Label.Text = Application.LocalizedString("ConfirmPassword");
            password2Label.Frame = new RectangleF(20f, 200f, 230f, 20f);
            password2Label.Font = UIFont.FromName("Helvetica Neue", 15f);
            password2Label.TextColor = UIColor.FromRGB(153, 153, 153);

            password2 = new UITextField();
            password2.Frame = new RectangleF(20f, 230f, 230f, 35f);
            password2.Placeholder = Application.LocalizedString("Confirm");
            password2.Font = UIFont.FromName("Helvetica Neue", 17f);
            password2.AutocorrectionType = UITextAutocorrectionType.No;
            password2.Layer.BorderWidth = 1;
            password2.Layer.CornerRadius = 4;
            password2.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            password2.AutocorrectionType = UITextAutocorrectionType.No;
            password2.AutocapitalizationType = UITextAutocapitalizationType.None;
            password2.KeyboardType = UIKeyboardType.Default;
            password2.SecureTextEntry = true;
            password2.ReturnKeyType = UIReturnKeyType.Done;
            password2.ClearButtonMode = UITextFieldViewMode.WhileEditing;
            password2.TextColor = UIColor.FromRGB(102, 102, 102);
            password2.EditingChanged += (sender, e) =>
            {
                VerificationFieldsPassword(passwordOld, password1, password2, errorLabel, actionSheetAlert);
            };
            password2.ShouldReturn += (textField) =>
            {
                password2.ResignFirstResponder();
                return true;
            };

            errorLabel = new UILabel();
            errorLabel.Frame = new RectangleF(20f, 270f, 230f, 40f);
            errorLabel.Font = UIFont.FromName("Helvetica Neue", 15f);
            errorLabel.TextColor = UIColor.FromRGB(255, 76, 56);
            errorLabel.Lines = 3;

            customView.Frame = new CoreGraphics.CGRect(0, 0, 270, errorLabel.Frame.Y);

            customView.AddSubview(passwordOldLabel);
            customView.AddSubview(password1Label);
            customView.AddSubview(password2Label);
            customView.AddSubview(passwordOld);
            customView.AddSubview(password1);
            customView.AddSubview(password2);
            customView.AddSubview(errorLabel);

            actionSheetAlert.Add(customView);

            return actionSheetAlert;
        }

        private static bool VerificationFields(/*UITextField displayName, */UITextField adresseEmail)
        {
            var isCorrectData = true;

            // contact name verification
            //if (string.IsNullOrEmpty(displayName.Text))
            //{
            //    displayName.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
            //    isCorrectData = false;
            //}
            //else
            //{
            //    displayName.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            //}

            // email verification
            if (string.IsNullOrEmpty(adresseEmail.Text) || !adresseEmail.Text.IsEmail())
            {
                adresseEmail.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
                isCorrectData = false;
            }
            else
            {
                adresseEmail.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            }

            return isCorrectData;
        }

        private static void VerificationFieldsPassword(UITextField passwordOld
            , UITextField password1
            , UITextField password2
            , UILabel errorLabel
            , UIAlertController actionSheetAlert)
        {
            if (!string.IsNullOrEmpty(passwordOld.Text)
                && !string.IsNullOrEmpty(password1.Text)
                && !string.IsNullOrEmpty(password2.Text))
            {
                var isValid = true;
                if (App.Locator.Login.CalculatePassword(passwordOld.Text) != DataService.Pass)
                {
                    errorLabel.Text = Application.LocalizedString("WrongPassword");
                    isValid = false;
                }
                if (password1.Text.Length < 8 || password2.Text.Length < 8)
                {
                    errorLabel.Text = Application.LocalizedString("PasswordLengthError");
                    isValid = false;
                }
                if (password1.Text != password2.Text)
                {
                    errorLabel.Text = Application.LocalizedString("ConfirmError");
                    isValid = false;
                }
                if (isValid)
                {
                    errorLabel.Text = string.Empty;
                    if (actionSheetAlert.Actions[0] != null)
                    {
                        (actionSheetAlert.Actions[0] as UIAlertAction).Enabled = true;
                    }
                }
                else (actionSheetAlert.Actions[0] as UIAlertAction).Enabled = false;
            }
            else
            {
                errorLabel.Text = string.Empty;
                (actionSheetAlert.Actions[0] as UIAlertAction).Enabled = false;
            }
        }
    }
}
