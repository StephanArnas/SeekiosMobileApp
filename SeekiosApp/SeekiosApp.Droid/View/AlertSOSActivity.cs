using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SeekiosApp.Droid.Services;
using SeekiosApp.Droid.CustomComponents;
using SeekiosApp.Model.APP;
using Android.Views.InputMethods;
using static Android.Views.View;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class AlertSOSActivity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================

        private Contact _selectedContact = null;
        private InputMethodManager _imm = null;

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Titre de l'alerte</summary>
        public EditText AlertTitleEditText { get; set; }

        /// <summary>Représente le layout qui contient la liste des contacts</summary>
        public ContactLayout AlertRecipientLayout { get; set; }

        /// <summary>Contenu de l'alerte</summary>
        public EditText AlertBodyEditText { get; set; }

        /// <summary>Représente le layout de l'objet de l'email</summary>
        public TextView AddRecipientFromPhonebooksButton { get; set; }

        /// <summary>Représente le layout de l'objet de l'email</summary>
        public TextView AddRecipientFromPopupButton { get; set; }

        /// <summary>Save the alert</summary>
        public TextView SaveButton { get; set; }

        public LinearLayout ScrollLayout { get; set; }

        public ScrollView ScrollView { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle bundle)
        {
            SetContentView(Resource.Layout.AlertSOSLayout);
            base.OnCreate(bundle);

            GetObjectsFromView();
            SetDataToView();

            ToolbarPage.SetTitle(Resource.String.alertSOS_pagetitle);
            SetSupportActionBar(ToolbarPage);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        protected override void OnResume()
        {
            AddRecipientFromPopupButton.Click += AddRecipientFromPopupButton_Click;
            AddRecipientFromPhonebooksButton.Click += AddRecipientFromPhonebooksButton_Click;
            SaveButton.Click += SaveButton_Click;
            AlertRecipientLayout.EmptyClick += AlertRecipientLayout_EmptyClick1;
            App.Locator.AlertSOS.OnAlertSosChanged += AlertSOS_OnAlertSosChanged;
            App.Locator.AddSeekios.UpdatingSeekios = App.Locator.DetailSeekios.SeekiosSelected;

            base.OnResume();
        }

        protected override void OnPause()
        {
            AddRecipientFromPopupButton.Click -= AddRecipientFromPopupButton_Click;
            AddRecipientFromPhonebooksButton.Click -= AddRecipientFromPhonebooksButton_Click;
            SaveButton.Click -= SaveButton_Click;
            AlertRecipientLayout.EmptyClick -= AlertRecipientLayout_EmptyClick1;
            App.Locator.AlertSOS.OnAlertSosChanged -= AlertSOS_OnAlertSosChanged;

            base.OnPause();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            App.Locator.AlertSOS.Dispose();
        }

        #endregion

        #region ===== ActionBar ===================================================================

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.home:
                    Finish();
                    break;
                default:
                    Finish();
                    break;
            }
            return true;
        }

        #endregion

        #region ===== Initializes View ============================================================

        private void GetObjectsFromView()
        {
            AlertBodyEditText = FindViewById<EditText>(Resource.Id.alert_alertBody);
            AlertTitleEditText = FindViewById<EditText>(Resource.Id.alert_title);
            AlertRecipientLayout = FindViewById<ContactLayout>(Resource.Id.alert_recipient);
            AddRecipientFromPhonebooksButton = FindViewById<TextView>(Resource.Id.alert_addRecipientFromPhonebooks);
            AddRecipientFromPopupButton = FindViewById<TextView>(Resource.Id.alert_addRecipientFromPopup);
            SaveButton = FindViewById<TextView>(Resource.Id.alert_save);
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
            ScrollLayout = FindViewById<LinearLayout>(Resource.Id.scrollEmbedded_layout);
            ScrollView = FindViewById<ScrollView>(Resource.Id.scroll);
            //_imm = (InputMethodManager)GetSystemService(InputMethodService);
            //ScrollLayout.SetOnTouchListener(this);
            //ScrollLayout.RequestDisallowInterceptTouchEvent(true);
        }

        private void SetDataToView()
        {
            var alert = App.Locator.AlertSOS.CurrentAlertSOS;
            if (alert != null)
            {
                AlertTitleEditText.Text = alert.Title;
                AlertBodyEditText.Text = alert.Content;
                AlertRecipientLayout.RemoveAllViews();
                foreach (var contact in App.CurrentUserEnvironment.LsAlertRecipient.Where(w => w.IdAlert == alert.IdAlert))
                {
                    AlertRecipientLayout.AddChild(new Contact
                    {
                        DisplayName = contact.DisplayName,
                        Email = contact.Email,
                        EmailType = contact.EmailType,
                        PhoneNumber = contact.PhoneNumber,
                        PhoneType = contact.PhoneNumberType
                    });
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
                }
            }
        }

        #endregion

        #region ===== Private Methods =============================================================

        /// <summary>
        /// Methods used to HideKeyboard
        /// </summary>
        //private void HideKeyboard(Android.Views.View v)
        //{
        //    if (_imm.IsActive)
        //    {
        //        _imm.HideSoftInputFromWindow(v.WindowToken, HideSoftInputFlags.None);
        //    }
        //}

        #endregion

        #region ===== Events ======================================================================

        /// <summary>
        /// Save the sos alert
        /// </summary>
        private async void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(AlertTitleEditText.Text))
            {
                AlertTitleEditText.RequestFocus();
                if (_imm == null) _imm = (InputMethodManager)GetSystemService(InputMethodService);
                _imm.ShowSoftInput(AlertTitleEditText, ShowFlags.Implicit);
                Toast.MakeText(this, Resource.String.alertSOS_cannotSaveTitle, ToastLength.Long).Show();
                return;
            }
            else if (App.Locator.AlertSOS.LsRecipients.Count < 1)
            {
                Toast.MakeText(this, Resource.String.alertSOS_cannotSaveContact, ToastLength.Long).Show();
                return;
            }
            else if (string.IsNullOrEmpty(AlertBodyEditText.Text))
            {
                AlertBodyEditText.RequestFocus();
                if (_imm == null) _imm = (InputMethodManager)GetSystemService(InputMethodService);
                _imm.ShowSoftInput(AlertBodyEditText, ShowFlags.Implicit);
                Toast.MakeText(this, Resource.String.alertSOS_cannotSaveContent, ToastLength.Long).Show();
                return;
            }
            LoadingLayout.Visibility = ViewStates.Visible;
            if (await App.Locator.AlertSOS.InsertOrUpdateAlertSOS(AlertTitleEditText.Text, AlertBodyEditText.Text))
            {
                LoadingLayout.Visibility = ViewStates.Gone;
                Finish();
            }
            LoadingLayout.Visibility = ViewStates.Gone;
        }

        /// <summary>
        /// Open the contact intent
        /// </summary>
        private void AddRecipientFromPhonebooksButton_Click(object sender, EventArgs e)
        {
            StartIntentContact();
        }

        /// <summary>
        /// Open a popup to enter a new recipient
        /// </summary>
        private void AddRecipientFromPopupButton_Click(object sender, EventArgs e)
        {
            Android.Support.V7.App.AlertDialog addRecipientDialog = null;
            var addRecipientBuilder = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog);
            var view = LayoutInflater.Inflate(Resource.Drawable.AddRecipient, null);

            addRecipientBuilder.SetTitle(Resource.String.alertSOS_popupTitle);
            addRecipientBuilder.SetPositiveButton(Resource.String.alertSOS_popupPositive, new EventHandler<DialogClickEventArgs>((o, ev) => { }));
            addRecipientBuilder.SetNegativeButton(Resource.String.alertSOS_popupNegative, new EventHandler<DialogClickEventArgs>((o, ev) =>
            {
                addRecipientDialog.Dismiss();
            }));

            addRecipientBuilder.SetView(view);
            addRecipientDialog = addRecipientBuilder.Create();
            addRecipientDialog.ShowEvent += (ee, oo) =>
            {
                var button = addRecipientDialog.GetButton((int)Android.Content.DialogButtonType.Positive);
                button.Click += (eee, ooo) =>
                {
                    var displayNameEditText = view.FindViewById<EditText>(Resource.Id.addRecipient_contact);
                    var emailEditText = view.FindViewById<EditText>(Resource.Id.addRecipient_email);

                    // Empty display name
                    if (string.IsNullOrEmpty(displayNameEditText.Text))
                    {
                        displayNameEditText.Error = Resources.GetString(Resource.String.alertSOS_popupEmptyField);
                    }
                    // Empty email
                    else if (string.IsNullOrEmpty(emailEditText.Text))
                    {
                        emailEditText.Error = Resources.GetString(Resource.String.alertSOS_popupEmptyField);
                    }
                    // Invalide email
                    else if (!Android.Util.Patterns.EmailAddress.Matcher(emailEditText.Text).Matches())
                    {
                        emailEditText.Error = Resources.GetString(Resource.String.alertSOS_popupEmail);
                    }
                    // Add the contact
                    else
                    {
                        displayNameEditText.Error = null;
                        emailEditText.Error = null;

                        App.Locator.AlertSOS.LsRecipients.Add(new Model.DTO.AlertRecipientDTO
                        {
                            DisplayName = displayNameEditText.Text,
                            Email = emailEditText.Text
                        });

                        _selectedContact = new Contact { DisplayName = displayNameEditText.Text, Email = emailEditText.Text };
                        AlertRecipientLayout.AddChild(_selectedContact);
                        addRecipientDialog.Dismiss();
                    }
                };
            };
            addRecipientDialog.Show();
        }

        /// <summary>
        /// Open the contact intent
        /// </summary>
        private void AlertRecipientLayout_EmptyClick(object sender, EventArgs e)
        {
            StartIntentContact();
        }

        /// <summary>
        /// Open the contact intent
        /// </summary>
        private void AlertRecipientLayout_EmptyClick1(object sender, EventArgs e)
        {
            StartIntentContact();
        }


        /// <summary>
        /// Raised if the user has been broadcast by SignalR with new data for the alert SOS
        /// </summary>
        private void AlertSOS_OnAlertSosChanged(object sender, EventArgs e)
        {
            SetDataToView();
        }

        #endregion

        #region ===== Contact =====================================================================

        /// <summary>
        /// Select a contact
        /// </summary>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == 101 && resultCode == Result.Ok)
            {
                if (data == null || data.Data == null) return;
                string id = data.Data.PathSegments[1];
                if (string.IsNullOrEmpty(id)) return;

                _selectedContact = new Contact();

                using (Android.Database.ICursor cursor = ContentResolver.Query(data.Data, null, null, null, null))
                {
                    if (cursor.MoveToFirst())
                    {
                        _selectedContact.DisplayName = cursor.GetString(cursor.GetColumnIndex(Android.Provider.ContactsContract.PhoneLookup.InterfaceConsts.DisplayName));
                        _selectedContact.Email = cursor.GetString(cursor.GetColumnIndex(Android.Provider.ContactsContract.CommonDataKinds.Email.Address));
                    }
                    else
                    {
                        Toast.MakeText(ApplicationContext, Resource.String.alert_noEmailFound, ToastLength.Short).Show();
                    }
                }

                if (App.Locator.Alert.LsRecipients.Where(el => el.Email == _selectedContact.Email).FirstOrDefault() == null)
                {
                    if (App.Locator.Alert.LsRecipients.Count >= 3)
                    {
                        var errorMessage = Resources.GetString(Resource.String.alert_moreThanThreeRecipients);
                        Toast.MakeText(ApplicationContext, errorMessage, ToastLength.Short).Show();
                    }
                    else
                    {
                        AlertRecipientLayout.AddChild(_selectedContact);
                        App.Locator.AlertSOS.LsRecipients.Add(new Model.DTO.AlertRecipientDTO { DisplayName = _selectedContact.DisplayName, Email = _selectedContact.Email });
                    }
                }
            }
        }

        /// <summary>
        /// Open the contact intent
        /// </summary>
        private void StartIntentContact()
        {
            var contactPickerIntent = new Intent(Intent.ActionPick, Android.Provider.ContactsContract.Contacts.ContentUri);
            contactPickerIntent.SetType(Android.Provider.ContactsContract.CommonDataKinds.Email.ContentType);
            StartActivityForResult(contactPickerIntent, 101);
        }

        #endregion

        #region ===== Interface implementation ====================================================

        //public bool OnTouch(Android.Views.View v, MotionEvent e)
        //{
        //    try
        //    {
        //        bool isScrollLayout = v == ScrollLayout;
        //        ScrollLayout.RequestDisallowInterceptTouchEvent(!isScrollLayout);

        //        switch (e.Action & MotionEventActions.Mask)
        //        {
        //            case MotionEventActions.Down:
        //                break;
        //            case MotionEventActions.Up:
        //                break;
        //            case MotionEventActions.PointerDown:
        //                break;
        //            case MotionEventActions.PointerUp:
        //                break;
        //            case MotionEventActions.Move:
        //                if (v != AlertTitleEditText && v != AlertBodyEditText)
        //                {
        //                    HideKeyboard(v);
        //                }
        //                break;
        //        }
        //        return true;
        //    }
        //    catch (Exception) { return true; }
        //}

        #endregion
    }
}