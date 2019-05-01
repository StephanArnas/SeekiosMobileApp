using Foundation;
using System;
using UIKit;
using SeekiosApp.iOS.Views;
using System.Drawing;
using CoreGraphics;
using ContactsUI;
using Contacts;
using System.Linq;
using SeekiosApp.Model.DTO;
using SeekiosApp.iOS.Helper;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight.Ioc;
using SeekiosApp.Extension;

namespace SeekiosApp.iOS
{
    public partial class AlertSOSView : BaseViewController
    {
        #region ====== Attributs ==================================================================

        private int _index = 0;
        private int _sizeFrame = 0;
        private bool _isComeFromContact = false;
        private CNContactPickerViewController contactPickerView = null;
        private NSObject _observer1 = null;
        private NSObject _observer2 = null;

        private static nfloat _heightOfThePage = 0;

        #endregion

        #region ====== Constructor ================================================================

        public AlertSOSView(IntPtr handle) : base(handle) { }

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
            SaveButton.Enabled = false;

            var toolbar = new UIToolbar();
            toolbar.BarStyle = UIBarStyle.Default;
            toolbar.Translucent = true;
            toolbar.SizeToFit();

            var doneButton = new UIBarButtonItem(Application.LocalizedString("Validate"), UIBarButtonItemStyle.Done, (s, e) =>
            {
                AlertMessageTextField.ResignFirstResponder();
            });

            toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);
            AlertMessageTextField.InputAccessoryView = toolbar;
            AddRecipientButton.TouchUpInside += AddRecipientButton_TouchUpInside;
            AddRecipientFromContactBookButton.TouchUpInside += AddRecipientFromContactButton_TouchUpInside;
            SaveButton.TouchUpInside += SaveButton_TouchUpInside;
            App.Locator.AlertSOS.OnAlertSosChanged += AlertSOS_OnAlertSosChanged;
            EmptyDataButton.AddGestureRecognizer(new UITapGestureRecognizer(EmptyDataButton_TouchUpInside));

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

            Title = Application.LocalizedString("SOSAlert");
            App.Locator.AddSeekios.UpdatingSeekios = App.Locator.DetailSeekios.SeekiosSelected;

            if (!_isComeFromContact)
            {
                UpdateScrollViewContentSize();
            }
            else _isComeFromContact = false;
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            if (_heightOfThePage == 0)
            {
                nfloat size = 0;
                var edtiView = ScrollAlertSOS.Subviews[14]; // keep the order of the elements in the view
                size = edtiView.Frame.Y + edtiView.Frame.Height + 50;
                size = new nfloat(size * 1.2);
                _heightOfThePage = size;
                ScrollAlertSOS.ContentSize = new CGSize(View.Frame.Size.Width, size);
            }
            else
            {
                ScrollAlertSOS.ContentSize = new CGSize(View.Frame.Size.Width, _heightOfThePage);
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (!_isComeFromContact)
            {
                App.Locator.AlertSOS.Dispose();
            }
        }

        #endregion

        #region ===== Initialisze View ============================================================

        /// <summary>
        /// Set the data and style to the view
        /// </summary>
        public override void SetDataAndStyleToView()
        {
            InitialiseAllStrings();

            // Display empty list if the list is empty
            if (App.Locator.AlertSOS.LsRecipients.Count == 0)
            {
                EmptyDataButton.Hidden = false;
            }
            else
            {
                EmptyDataButton.Hidden = true;
            }

            // Save button corner
            SaveButton.Layer.CornerRadius = 4;
            SaveButton.Layer.MasksToBounds = true;

            // Set up the border of the contact container
            ViewBehindScrollView.Layer.BorderWidth = 1f;
            ViewBehindScrollView.Layer.CornerRadius = 4;
            ViewBehindScrollView.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;

            // Set up the border of the alert title
            AlertTitleTextField.Layer.BorderWidth = 1f;
            AlertTitleTextField.Layer.CornerRadius = 4;
            AlertTitleTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            AlertTitleTextField.ShouldReturn += (textField) => 
            {
                AlertTitleTextField.ResignFirstResponder();
                return true;
            };
            AlertTitleTextField.EditingChanged += (o, s) =>
            {
                AlertTitleTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
                SaveButton.Enabled = true;
            };

            // Set up the border of the alert body
            AlertMessageTextField.Layer.BorderWidth = 1f;
            AlertMessageTextField.Layer.CornerRadius = 4;
            AlertMessageTextField.Layer.BorderColor = UIColor.FromRGB(229, 229, 229).CGColor;
            AlertMessageTextField.Changed += (o, s) =>
            {
                SaveButton.Enabled = true;
            };

            // Set up list of recipientt
            var alert = App.Locator.AlertSOS.CurrentAlertSOS;
            if (alert != null)
            {
                UpdateScrollViewUI();
                AlertTitleTextField.Text = alert.Title;
                AlertMessageTextField.Text = alert.Content;
                foreach (var contact in App.CurrentUserEnvironment.LsAlertRecipient.Where(w => w.IdAlert == alert.IdAlert))
                {
                    App.Locator.AlertSOS.LsRecipients.Add(new Model.DTO.AlertRecipientDTO
                    {
                        DisplayName = contact.DisplayName,
                        Email = contact.Email,
                        EmailType = contact.EmailType,
                        PhoneNumber = contact.PhoneNumber,
                        PhoneNumberType = contact.PhoneNumberType,
                        IdAlert = contact.IdAlert,
                        IdRecipient = contact.IdRecipient
                    });
                    CreateContactContainerUI(contact);
                }
            }
        }

        #endregion

        #region ====== Private Methods ============================================================

        private void InitialiseAllStrings()
        {
            Title1Label.Text = Application.LocalizedString("Title1AlertSOS");
            SubTitle11Label.Text = Application.LocalizedString("SubTitle11AlertSOS");
            SubTitle12Label.Text = Application.LocalizedString("SubTitle12AlertSOS");
            SubTitle13Label.Text = Application.LocalizedString("SubTitle13AlertSOS");
            Title2Label.Text = Application.LocalizedString("Title2AlertSOS");
            AlertTitleLabel.Text = Application.LocalizedString("TitleAlertSOS");
            AlertTitleTextField.Placeholder = Application.LocalizedString("PlaceholderAlertSOS");
            RecipientsTitleLabel.Text = Application.LocalizedString("RecipientsAlertSOS");
            EmptyDataButton.Text = Application.LocalizedString("AddRecipientsAlertSOS");
            MessageTitleLabel.Text = Application.LocalizedString("MessageAlertSOS");
        }

        /// <summary>
        /// Method for Add and Update a contact to ScrollView
        /// </summary>
        private void AddRecipientToRecipientContainer(AlertRecipientDTO contact)
        {
            App.Locator.AlertSOS.LsRecipients.Add(contact);
            UpdateScrollViewContentSize();
            CreateContactContainerUI(contact);
        }

        /// <summary>
        /// Validation for Textfields of AlertController
        /// </summary>
        private bool VerificationFields(/*UITextField displayName, */UITextField adresseEmail)
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

            foreach (var alert in App.Locator.AlertSOS.LsRecipients)
            {
                CreateContactContainerUI(alert);
            }
            UpdateScrollViewContentSize();
        }

        /// <summary>
        /// Updates the size of the content
        /// </summary>
        private void UpdateScrollViewContentSize()
        {
            var newContentSize = (App.Locator.AlertSOS.LsRecipients.Count) * 150;
            var scrollContentSize = ScrollViewContact.ContentSize;
            scrollContentSize.Width = newContentSize + 20;
            ScrollViewContact.ContentSize = scrollContentSize;
        }

        /// <summary>
        /// Create a contact container
        /// </summary>
        private void CreateContactContainerUI(AlertRecipientDTO contact)
        {
            if (App.Locator.AlertSOS.LsRecipients.Count == 0)
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
            deleteButton.Frame = new RectangleF(sizeUI + 6f, 16f, 25f, 25f); ;
            deleteButton.SetImage(UIImage.FromBundle("Trash"), UIControlState.Normal);
            deleteButton.Tag = _index;

            // Delete the content of ScrollView.
            deleteButton.TouchUpInside += (s, e) =>
            {
                var index = (int)deleteButton.Tag;
                App.Locator.AlertSOS.LsRecipients.RemoveAt(index);
                UpdateScrollViewUI();
                SaveButton.Enabled = true;
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

        private CGPoint offset;

        private void KeyBoardDownNotification(NSNotification notification)
        {
            ScrollAlertSOS.ContentOffset = offset;
        }

        private void KeyBoardUpNotification(NSNotification notification)
        {
            // Find what opened the keyboard
            foreach (UIView view in ScrollAlertSOS.Subviews)
            {
                if (view.IsFirstResponder)
                {
                    offset = ScrollAlertSOS.ContentOffset;
                    ScrollHelper.ScrollOnHiddenUIElement(View
                        , ScrollAlertSOS
                        , ((float)(view.Frame.Y + view.Frame.Height + 10)) // 10 is extra offset
                        , notification
                        , InterfaceOrientation);
                    break;
                }
            }
        }

        private void AlertSOS_OnAlertSosChanged(object sender, EventArgs e)
        {
            SetDataAndStyleToView();
        }

        /// <summary>
        /// Event for adding a contact from the popup
        /// </summary>
        private void AddRecipientButton_TouchUpInside(object sender, EventArgs e)
        {
            var alertView = AlertControllerHelper.ShowAlertToAddContact((displayNameTextField, emailTextField) =>
            {
                if (App.Locator.AlertSOS.LsRecipients.Any((arg) => arg.Email.ToLower() == emailTextField.Text.Trim().ToLower()))
                {
                    return false;
                }
                // adding contact in the contact container
                if (VerificationFields(/*displayNameTextField, */emailTextField))
                {
                    var recipient = new AlertRecipientDTO();
                    recipient.DisplayName = displayNameTextField.Text;
                    recipient.Email = emailTextField.Text.Trim();
                    AddRecipientToRecipientContainer(recipient);
                    SaveButton.Enabled = true;
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
                        if (App.Locator.AlertSOS.LsRecipients.Any((arg) => arg.Email.ToLower() == contact.EmailAddresses[0].Value.ToString().Trim().ToLower()))
                        {
                            continue;
                        }
                        var recipient = new AlertRecipientDTO();
                        recipient.DisplayName = string.Format("{0} {1}", contact.GivenName, contact.FamilyName);
                        recipient.Email = contact.EmailAddresses[0].Value.ToString().Trim();
                        AddRecipientToRecipientContainer(recipient);
                        SaveButton.Enabled = true;
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
                if (App.Locator.AlertSOS.LsRecipients.Any((arg) => arg.Email.ToLower() == emailTextField.Text.Trim().ToLower()))
                {
                    return false;
                }
                // adding contact in the contact container
                if (VerificationFields(/*displayNameTextField, */emailTextField))
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
        private async void SaveButton_TouchUpInside(object sender, EventArgs e)
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
            if (App.Locator.AlertSOS.LsRecipients.Count == 0)
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
                SaveButton.Enabled = false;
                // display the loading layout
                var alertLoading = AlertControllerHelper.ShowAlertLoading();
                PresentViewController(alertLoading, true, null);
                if (await App.Locator.AlertSOS.InsertOrUpdateAlertSOS(AlertTitleTextField.Text, AlertMessageTextField.Text))
                {
                    alertLoading.DismissViewController(true, null);
                }
                else
                {
                    alertLoading.DismissViewController(true, null);
                    SaveButton.Enabled = true;
                }
            }
        }

        #endregion
    }
}