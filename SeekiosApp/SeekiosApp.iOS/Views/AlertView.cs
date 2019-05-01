using Foundation;
using System;
using UIKit;
using ContactsUI;
using Contacts;
using System.Drawing;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using CoreAnimation;
using SeekiosApp.iOS.Helper;
using SeekiosApp.iOS.Views.TableSources;
using SeekiosApp.Model.DTO;
using SeekiosApp.Model.APP;
using SeekiosApp.iOS.Views;
using SeekiosApp.Extension;
using SeekiosApp.Services;

namespace SeekiosApp.iOS
{
    public partial class AlertView : BaseViewController
    {
        #region ====== Attributs ==================================================================

        private string _priviousMessage = string.Empty;
        private int _index = 0;
        private int _sizeFrame = 0;
        private bool _isComeFromContact = false;
        private CNContactPickerViewController contactPickerView = null;

        #endregion

        #region ====== Constructor ================================================================

        public AlertView(IntPtr handle) : base(handle) { }

        #endregion

        #region ====== Life Cycle =================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var width = (float)UIScreen.MainScreen.Bounds.Width;
            ScrollViewContact.Frame = new RectangleF(0f, 0f, width, 46.0f);
            ScrollViewContact.ScrollEnabled = true;
            ScrollViewContact.ShowsVerticalScrollIndicator = false;
            ScrollViewContact.ShowsHorizontalScrollIndicator = true;
            ScrollViewContact.ContentSize = new CGSize(width, 46.0f);

            var toolbar = new UIToolbar();
            toolbar.BarStyle = UIBarStyle.Default;
            toolbar.Translucent = true;
            toolbar.SizeToFit();

            var doneButton = new UIBarButtonItem(Application.LocalizedString("Validate"), UIBarButtonItemStyle.Done, (s, e) =>
            {
                foreach (var view in View.Subviews)
                {
                    if (view.IsFirstResponder)
                    {
                        AlertMessageTextField.ResignFirstResponder();
                    }
                }
            });

            toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);
            AlertMessageTextField.InputAccessoryView = toolbar;
            AddRecipientButton.TouchUpInside += AddRecipientButton_TouchUpInside;
            AddRecipientFromContactBookButton.TouchUpInside += AddRecipientFromContactButton_TouchUpInside;
            EmptyDataButton.AddGestureRecognizer(new UITapGestureRecognizer(EmptyDataButton_TouchUpInside));
            NavigationItem.SetRightBarButtonItem(
                new UIBarButtonItem(Application.LocalizedString("OK"), UIBarButtonItemStyle.Bordered
                , (sender, args) => { SaveBarButton_Clicked(); })
                , true);
        }

        public override void ViewWillAppear(bool animated)
        {
            Title = Application.LocalizedString("AlertEmailTitle");
            if (!_isComeFromContact)
            {
                base.ViewWillAppear(animated);
            }
            else _isComeFromContact = false;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (!_isComeFromContact)
            {
                App.Locator.Alert.Dispose();
            }
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            nfloat size = 0;
            foreach (var view in ScrollViewContact.Subviews)
            {
                size += view.Frame.Size.Height;
            }
            ScrollViewContact.ContentSize = new CGSize(ScrollViewContact.Frame.Size.Width, size * 1.1);
        }

        #endregion

        #region ===== Initialisze View ============================================================

        /// <summary>
        /// Set the data and style to the view
        /// </summary>
        public override void SetDataAndStyleToView()
        {
            //Initialise all strings
            InitialiseAllStrings();

            // set up title and content
            if (!App.Locator.Alert.IsNew)
            {
                AlertTitleTextField.Text = App.Locator.Alert.TitleAlert;
                AlertMessageTextField.Text = App.Locator.Alert.ContentAlert;
                MaxSizeMessageLabel.Text = string.Format(Application.LocalizedString("MessageLength"), AlertMessageTextField.Text.Length);
            }
            else
            {
                var nav = (NavigationService)GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<GalaSoft.MvvmLight.Views.INavigationService>();
                
                //if (nav.GetLastEntry() is ModeDontMoveSecondView)
                //{
                //    AlertTitleTextField.Text = string.Format(Application.LocalizedString("AlertSeekiosMovedTitle"), App.Locator.DetailSeekios.SeekiosSelected.SeekiosName);
                //    AlertMessageTextField.Text = string.Format(Application.LocalizedString("AlertSeekiosMovedContent"), App.Locator.DetailSeekios.SeekiosSelected.SeekiosName);
                //}
                //else if (nav.GetLastEntry() is ModeZoneSecondView)
                //{
                //    AlertTitleTextField.Text = string.Format(Application.LocalizedString("AlertSeekiosOutOfZoneTitle"), App.Locator.DetailSeekios.SeekiosSelected.SeekiosName);
                //    AlertMessageTextField.Text = string.Format(Application.LocalizedString("AlertSeekiosOutOfZoneContent"), App.Locator.DetailSeekios.SeekiosSelected.SeekiosName);
                //}
            }

            // set up list of recipient
            if (App.Locator.ModeZone.EditingAlerts)
            {
                foreach (var recipient in App.Locator.Alert.LsRecipients)
                {
                    CreateContactContainerUI(recipient);
                }
            }

            // round buttons for the contacts container
            var profileImageCircle = AddRecipientButton.Layer;
            profileImageCircle.CornerRadius = AddRecipientButton.Frame.Size.Width / 2;
            AddRecipientButton.ClipsToBounds = true;

            var profileImageCircle1 = AddRecipientFromContactBookButton.Layer;
            profileImageCircle1.CornerRadius = AddRecipientFromContactBookButton.Frame.Size.Width / 2;
            AddRecipientFromContactBookButton.ClipsToBounds = true;

            // display empty list if the list is empty
            if (App.Locator.Alert.LsRecipients.Count == 0)
            {
                EmptyDataButton.Hidden = false;
            }
            else EmptyDataButton.Hidden = true;

            // set up the border of the contact container
            ViewBehindScrollView.Layer.BorderWidth = 1f;
            ViewBehindScrollView.Layer.CornerRadius = 4;
            ViewBehindScrollView.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;

            // set up the border of the alert title
            AlertTitleTextField.Layer.BorderWidth = 1f;
            AlertTitleTextField.Layer.CornerRadius = 4;
            AlertTitleTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            AlertTitleTextField.ShouldReturn += (textField) => { AlertTitleTextField.ResignFirstResponder(); return true; };
            AlertTitleTextField.EditingChanged += (o, s) =>
            {
                AlertTitleTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            };

            // set up the border of the alert body
            AlertMessageTextField.Layer.BorderWidth = 1f;
            AlertMessageTextField.Layer.CornerRadius = 4;
            AlertMessageTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            AlertMessageTextField.ShouldBeginEditing += (UITextView) =>
            {
                // set up the scroll the view up or down
                AnimationView.StartAnimatinon(0.3);
                var frame = (RectangleF)View.Frame;
                frame.Y -= 150;
                View.Frame = frame;
                AnimationView.StopAnimatinon();
                return true;
            };

            AlertMessageTextField.ShouldChangeText += (UITextView textView, NSRange range, string text) =>
            {
                AlertMessageTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
                // set up the Maximum Character Length for Alert Message
                if (text.Length > 1000)
                {
                    AlertMessageTextField.Text = _priviousMessage;
                }
                else
                {
                    _priviousMessage = textView.Text;
                    MaxSizeMessageLabel.Text = string.Format(Application.LocalizedString("MessageLength"), textView.Text.Length);
                    return true;
                }
                return false;
            };

            AlertMessageTextField.ShouldEndEditing += (UITextView) =>
            {
                // set up the scroll the view up or down
                AnimationView.StartAnimatinon(0.3);
                var frame = (RectangleF)View.Frame;
                frame.Y += 150;
                View.Frame = frame;
                AnimationView.StopAnimatinon();
                return true;
            };
        }

        #endregion

        #region ====== Private Methodes ===========================================================

        private void InitialiseAllStrings()
        {
            AlertTitleLabel.Text = Application.LocalizedString("AlertTitle");
            AlertTitleTextField.Placeholder = Application.LocalizedString("TitleAlertPlaceholder");
            MessageTitleLabel.Text = Application.LocalizedString("MessageAlert");
            RecipientsTitleLabel.Text = Application.LocalizedString("RecipientsAlert");
            EmptyDataButton.SetTitle(Application.LocalizedString("AddRecipientsAlert"), UIControlState.Normal);
        }

        /// <summary>
        /// Method for Add and Update a contact to ScrollView
        /// </summary>
        private void AddRecipientToRecipientContainer(AlertRecipientDTO contact)
        {
            App.Locator.Alert.LsRecipients.Add(contact);
            CreateContactContainerUI(contact);
        }

        /// <summary>
        /// Validation for Textfields of AlertController
        /// </summary>
        private bool VerificationFields(/*UITextField displayName, */UITextField adresseEmail)
        {
            var isCorrectData = true;

            //// contact name verification
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

        /// <summary>
        /// Updates the refresh user interface
        /// </summary>
        private void UpdateScrollViewUI()
        {
            foreach (var subView in ScrollViewContact.Subviews)
            {
                subView.RemoveFromSuperview();
            }
            _index = 0;
            _sizeFrame = 0;

            foreach (var alert in App.Locator.Alert.LsRecipients)
            {
                CreateContactContainerUI(alert);
            }

            if (App.Locator.Alert.LsRecipients.Count == 0)
            {
                EmptyDataButton.Hidden = false;
            }
            else EmptyDataButton.Hidden = true;
        }

        /// <summary>
        /// Create a contact container
        /// </summary>
        private void CreateContactContainerUI(AlertRecipientDTO contact)
        {
            if (App.Locator.Alert.LsRecipients.Count == 0)
            {
                EmptyDataButton.Hidden = false;
                return;
            }
            else EmptyDataButton.Hidden = true;

            var customView = new UIView();
            var sizeUI = (contact.Email.Length > contact.DisplayName.Length ? contact.Email.Length : contact.DisplayName.Length) * 7;

            var contactNameLable = new UILabel();
            contactNameLable.Frame = new RectangleF(3f, 7f, sizeUI, 20f);
            contactNameLable.Text = contact.DisplayName;
            contactNameLable.Font = UIFont.FromName("Helvetica", 15f);
            contactNameLable.TextColor = UIColor.FromRGB(102, 102, 102);

            var emailLabel = new UILabel();
            emailLabel.Frame = new RectangleF(3f, 30f, sizeUI, 20f);
            emailLabel.Text = contact.Email;
            emailLabel.Font = UIFont.FromName("Helvetica", 12f);
            emailLabel.TextColor = UIColor.FromRGB(153, 153, 153);

            var deleteButton = new UIButton(UIButtonType.RoundedRect);
            deleteButton.Frame = new RectangleF(sizeUI + 6f, 16f, 25f, 25f);
            deleteButton.SetImage(UIImage.FromBundle("Trash"), UIControlState.Normal);
            deleteButton.Tag = _index;

            // Delete the content of ScrollView.
            deleteButton.TouchUpInside += (s, e) =>
            {
                var index = (int)deleteButton.Tag;
                App.Locator.Alert.LsRecipients.RemoveAt(index);
                UpdateScrollViewUI();
            };

            customView.AddSubview(contactNameLable);
            customView.AddSubview(emailLabel);
            customView.AddSubview(deleteButton);
            customView.Frame = new RectangleF(_sizeFrame, 0f, sizeUI + 30f, 46f);
            _sizeFrame += (sizeUI + 30);

            ScrollViewContact.AddSubview(customView);
            ScrollViewContact.ContentSize = new CGSize(_sizeFrame, 46f);
            ScrollViewContact.SetNeedsDisplay();

            _index++;
        }
        #endregion

        #region ====== Event ======================================================================

        /// <summary>
        /// Event for adding a contact from the popup
        /// </summary>
        private void AddRecipientButton_TouchUpInside(object sender, EventArgs e)
        {
            var alertView = AlertControllerHelper.ShowAlertToAddContact((displayNameTextField, emailTextField) =>
            {
                if (App.Locator.Alert.LsRecipients.Any((arg) => arg.Email.ToLower() == emailTextField.Text.Trim().ToLower()))
                {
                    return false;
                }
                // adding contact in the contact container
                if (VerificationFields(emailTextField))
                {
                    var recipient = new AlertRecipientDTO();
                    recipient.DisplayName = displayNameTextField.Text;
                    recipient.Email = emailTextField.Text.Trim();
                    AddRecipientToRecipientContainer(recipient);
                    return true;
                }
                return false;
            });
            PresentViewController(alertView, true, null);
        }

        /// <summary>
        /// Event for adding a contact from the contact book
        /// </summary>
        private void AddRecipientFromContactButton_TouchUpInside(object sender, EventArgs e)
        {
            _isComeFromContact = true;

            if (contactPickerView == null)
            {
                contactPickerView = new CNContactPickerViewController();
                contactPickerView.DisplayedPropertyKeys = new NSString[] { CNContactKey.EmailAddresses };
                contactPickerView.PredicateForEnablingContact = NSPredicate.FromFormat("emailAddresses.@count > 0");
                contactPickerView.PredicateForSelectionOfContact = NSPredicate.FromFormat("emailAddresses.@count == 1");

                var contactPickerDelegate = new ContactPickerDelegate();
                contactPickerView.Delegate = contactPickerDelegate;
                contactPickerDelegate.SelectionCanceled += () => { };
                contactPickerDelegate.ContactPropertySelected += (property) => { };


                contactPickerDelegate.ContactsSelected += (CNContactPickerViewController picker, CNContact[] contacts) =>
                {
                    foreach (var contact in contacts)
                    {
                        if (App.Locator.Alert.LsRecipients.Any((arg) => arg.Email.ToLower() == contact.EmailAddresses[0].Value.ToString().Trim().ToLower()))
                        {
                            continue;
                        }
                        var recipient = new AlertRecipientDTO();
                        recipient.DisplayName = string.Format("{0} {1}", contact.GivenName, contact.FamilyName);
                        recipient.Email = contact.EmailAddresses[0].Value.ToString().Trim();
                        AddRecipientToRecipientContainer(recipient);
                    }
                };
            }

            PresentViewController(contactPickerView, true, null);
        }

        /// <summary>
        /// Event for adding a contact from the popup when no data
        /// </summary>
        private void EmptyDataButton_TouchUpInside()
        {
            var alertView = AlertControllerHelper.ShowAlertToAddContact((displayNameTextField, emailTextField) =>
            {
                if (App.Locator.Alert.LsRecipients.Any((arg) => arg.Email.ToLower() == emailTextField.Text.Trim().ToLower()))
                {
                    return false;
                }
                // adding contact in the contact container
                if (VerificationFields(emailTextField))
                {
                    var recipient = new AlertRecipientDTO();
                    recipient.DisplayName = displayNameTextField.Text;
                    recipient.Email = emailTextField.Text.Trim();
                    AddRecipientToRecipientContainer(recipient);
                    return true;
                }
                return false;
            });

            PresentViewController(alertView, true, null);
        }

        /// <summary>
        /// Event to save the alert
        /// </summary>
        private void SaveBarButton_Clicked()
        {
            var isFieldsValid = true;
            // verification title alert
            if (string.IsNullOrEmpty(AlertTitleTextField.Text))
            {
                AlertTitleTextField.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
                isFieldsValid = false;
            }
            else
            {
                AlertTitleTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            }
            // verification message alert
            if (string.IsNullOrEmpty(AlertMessageTextField.Text))
            {
                AlertMessageTextField.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
                isFieldsValid = false;
            }
            else
            {
                AlertMessageTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            }
            // verification recipients
            if (App.Locator.Alert.LsRecipients.Count == 0)
            {
                ViewBehindScrollView.Layer.BorderColor = UIColor.FromRGB(255, 76, 57).CGColor;
                isFieldsValid = false;
            }
            else
            {
                ViewBehindScrollView.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            }
            // add the alert in the view model
            if (isFieldsValid)
            {
                if (App.Locator.Alert.IdAlert == 0)
                {
                    if (App.Locator.ModeZone.WaitingForAlerts)
                    {
                        if (!App.Locator.ModeZone.EditingAlerts)
                        {
                            App.Locator.Alert.InsertAlert(AlertTitleTextField.Text, AlertMessageTextField.Text);
                        }
                        else App.Locator.Alert.UpdateAlert(AlertTitleTextField.Text, AlertMessageTextField.Text);
                    }
                    else App.Locator.Alert.InsertAlert(AlertTitleTextField.Text, AlertMessageTextField.Text);
                }
                else
                {
                    if (App.Locator.ModeZone.WaitingForAlerts && App.Locator.ModeZone.EditingAlerts)
                    {
                        App.Locator.Alert.UpdateAlert(AlertTitleTextField.Text, AlertMessageTextField.Text);
                    }
                    else App.Locator.Alert.UpdateAlert(AlertTitleTextField.Text, AlertMessageTextField.Text);
                }
                NavigationController.PopViewController(true);
            }
        }

        #endregion
    }
}