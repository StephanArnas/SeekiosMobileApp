using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using SeekiosApp.Droid.CustomComponents;
using SeekiosApp.Droid.Services;
using System.Threading;
using Android.Views;
using SeekiosApp.Model.DTO;
using System.Collections.Generic;
using SeekiosApp.Model.APP;
using Android.Views.InputMethods;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class AlertActivity : AppCompatActivityBase
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

        /// <summary>Delete button</summary>
        public TextView DeleteButton { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle bundle)
        {
            SetContentView(Resource.Layout.AlertLayout);
            base.OnCreate(bundle);

            GetObjectsFromView();
            SetDataToView();

            if (ToolbarPage != null)
            {
                ToolbarPage.SetTitle(Resource.String.alert_title);
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            if (App.Locator.ModeZone.EditingAlerts)
            {
                DeleteButton.Visibility = ViewStates.Visible;
            }
        }

        protected override void OnResume()
        {
            AddRecipientFromPhonebooksButton.Click += AddRecipientFromPhonebooksButton_Click;
            AddRecipientFromPopupButton.Click += AddRecipientFromPopupButton_Click;
            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            AlertRecipientLayout.EmptyClick += AlertRecipientLayout_EmptyClick;
            base.OnResume();
        }

        protected override void OnPause()
        {
            AddRecipientFromPhonebooksButton.Click -= AddRecipientFromPhonebooksButton_Click;
            AddRecipientFromPopupButton.Click -= AddRecipientFromPopupButton_Click;
            SaveButton.Click -= SaveButton_Click;
            DeleteButton.Click -= DeleteButton_Click;
            AlertRecipientLayout.EmptyClick -= AlertRecipientLayout_EmptyClick;
            base.OnPause();
        }

        protected override void OnDestroy()
        {
            App.Locator.Alert.Dispose();
            base.OnDestroy();
        }

        #endregion

        #region ===== Initializes View ============================================================

        /// <summary>
        /// Initialise les objets de la vue
        /// </summary>
        private void GetObjectsFromView()
        {
            AlertBodyEditText = FindViewById<EditText>(Resource.Id.alert_alertBody);
            AlertTitleEditText = FindViewById<EditText>(Resource.Id.alert_title);
            AlertRecipientLayout = FindViewById<ContactLayout>(Resource.Id.alert_recipient);
            AddRecipientFromPhonebooksButton = FindViewById<TextView>(Resource.Id.alert_addRecipientFromPhonebooks);
            AddRecipientFromPopupButton = FindViewById<TextView>(Resource.Id.alert_addRecipientFromPopup);
            SaveButton = FindViewById<TextView>(Resource.Id.alert_save);
            DeleteButton = FindViewById<TextView>(Resource.Id.alert_delete);
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
        }

        /// <summary>
        /// Initialise les objets de la vue avec les données
        /// </summary>
        private void SetDataToView()
        {
            if (!App.Locator.Alert.IsNew)
            {
                AlertTitleEditText.Text = App.Locator.Alert.TitleAlert;
                AlertBodyEditText.Text = App.Locator.Alert.ContentAlert;
            }

            if (App.Locator.ModeZone.EditingAlerts)
            {
                //var lsRecipientsCopy = new AlertRecipientDTO[App.Locator.Alert.LsRecipients.Count()];
                //App.Locator.Alert.LsRecipients.CopyTo(lsRecipientsCopy);
                //App.Locator.Alert.LsRecipients.Clear();
                foreach (AlertRecipientDTO recipient in App.Locator.Alert.LsRecipients)
                {
                    AlertRecipientLayout.AddChild(new Contact
                    {
                        DisplayName = recipient.DisplayName,
                        Email = recipient.Email,
                        EmailType = recipient.EmailType,
                        PhoneNumber = recipient.PhoneNumber,
                        PhoneType = recipient.PhoneNumberType
                    });
                }
            }
        }

        #endregion

        #region ===== ActionBar ===================================================================

        /// <summary>
        /// Sélection d'un bouton de l'action bar
        /// </summary>
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

        #region ===== Private Methods =============================================================

        #endregion

        #region ===== Events ======================================================================

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

                    // empty display name
                    if (string.IsNullOrEmpty(displayNameEditText.Text))
                    {
                        displayNameEditText.Error = Resources.GetString(Resource.String.alertSOS_popupEmptyField);
                    }
                    // empty email
                    else if (string.IsNullOrEmpty(emailEditText.Text))
                    {
                        emailEditText.Error = Resources.GetString(Resource.String.alertSOS_popupEmptyField);
                    }
                    // invalide email
                    else if (!Android.Util.Patterns.EmailAddress.Matcher(emailEditText.Text).Matches())
                    {
                        emailEditText.Error = Resources.GetString(Resource.String.alertSOS_popupEmail);
                    }
                    // add the contact
                    else
                    {
                        displayNameEditText.Error = null;
                        emailEditText.Error = null;

                        App.Locator.Alert.LsRecipients.Add(new AlertRecipientDTO
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
        /// Remove the alert
        /// </summary>
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            //int id = App.Locator.Alert.IdAlert;
            int pos = App.Locator.Alert.IdAlert;
            if (App.Locator.Alert.ModeDefinition == Enum.ModeDefinitionEnum.ModeZone)
            {
                App.Locator.ModeZone.InvalidViews = true;
                App.Locator.ModeZone.LsAlertsModeZone.RemoveAt(pos);
                for (int i = 0; i != App.Locator.ModeZone.LsAlertsModeZone.Count(); ++i)
                {
                    App.Locator.ModeZone.LsAlertsModeZone[i].IdAlert = i;
                }
            }
            else if (App.Locator.Alert.ModeDefinition == Enum.ModeDefinitionEnum.ModeDontMove)
            {
                App.Locator.ModeDontMove.LsAlertsModeDontMove.RemoveAt(pos);
                for (int i = 0; i != App.Locator.ModeDontMove.LsAlertsModeDontMove.Count(); ++i)
                {
                    App.Locator.ModeDontMove.LsAlertsModeDontMove[i].IdAlert = i;
                }
            }
            App.Locator.Alert.Dispose();
            Finish();
        }

        /// <summary>
        /// Save the alert
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
            else if (App.Locator.Alert.LsRecipients.Count < 1)
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

            var result = false;
            if (App.Locator.Alert.IdAlert == 0)
            {
                if (App.Locator.ModeZone.WaitingForAlerts)
                {
                    if (!App.Locator.ModeZone.EditingAlerts)
                    {
                        result = await App.Locator.Alert.InsertAlert(AlertTitleEditText.Text, AlertBodyEditText.Text);
                    }
                    else result = await App.Locator.Alert.UpdateAlert(AlertTitleEditText.Text, AlertBodyEditText.Text);
                }
                else result = await App.Locator.Alert.InsertAlert(AlertTitleEditText.Text, AlertBodyEditText.Text);
            }
            else
            {
                if (App.Locator.ModeZone.WaitingForAlerts && App.Locator.ModeZone.EditingAlerts)
                {
                    result = await App.Locator.Alert.UpdateAlert(AlertTitleEditText.Text, AlertBodyEditText.Text);
                }
                else result = await App.Locator.Alert.UpdateAlert(AlertTitleEditText.Text, AlertBodyEditText.Text);
            }
            if (result) Finish();
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
                        App.Locator.Alert.LsRecipients.Add(new AlertRecipientDTO { DisplayName = _selectedContact.DisplayName, Email = _selectedContact.Email });
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
    }
}
