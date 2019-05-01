using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SeekiosApp.Droid.CustomComponents;
using Android.Graphics;
using Android.Text;
using SeekiosApp.Droid.Services;
using Com.OneSignal.Android;
using Android.Graphics.Drawables;
using SeekiosApp.Droid.Helper;
using Android.Text.Style;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class ParameterActivity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================

        private CameraService _cameraService = null;
        private InternetConnectionService _connectionService = new InternetConnectionService();
        private bool _isFromGallery = false;
        private bool _hasDataChanged = false;

        private static MyTimer _timer;

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Picture image</summary>
        public RoundedImageView UserImageView { get; set; }

        /// <summary>Button take a picture from camera</summary>
        public Button UpdateUserImageButton { get; set; }

        /// <summary>First name</summary>
        public EditText FirstNameEditText { get; private set; }

        /// <summary>Last name</summary>
        public EditText LastNameEditText { get; private set; }

        /// <summary>Email</summary>
        public EditText EmailEditText { get; private set; }

        /// <summary>Phone number</summary>
        //public EditText PhoneNumberEditText { get; private set; }

        /// <summary>Code country for the phone number</summary>
        //public CustomComponents.CountryCode.CountryCodePicker CodeCountryLayout { get; set; }

        /// <summary>Button save</summary>
        public Android.Support.V7.Widget.AppCompatTextView SaveButton { get; set; }

        /// <summary>Button change password</summary>
        public RelativeLayout ChangePasswordLayout { get; set; }

        /// <summary>Button about</summary>
        public RelativeLayout AboutLayout { get; set; }

        /// <summary>Button credit<summary>
        public RelativeLayout ReloadCreditLayout { get; set; }

        /// <summary>Button deconnection<summary>
        public RelativeLayout DeconnectionLayout { get; set; }


        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ParameterLayout);

            GetObjectsFromView();
            SetDataToView();

            _cameraService = new CameraService(this);
            _connectionService.Initialize(this, ConnectivityService);

            if (ToolbarPage != null)
            {
                ToolbarPage.SetTitle(Resource.String.parameter_actionBarTitle);
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            OnConnectionStateChanged += UpdateUIOnConnectionStateChanged;
            App.Locator.Parameter.OnUserChanged += Parameter_OnUserChanged;

            UserImageView.Click += PickImageFromCameraOrGallery_Click;
            UpdateUserImageButton.Click += PickImageFromCameraOrGallery_Click;
            FirstNameEditText.TextChanged += FirstNameEditText_TextChanged;
            LastNameEditText.TextChanged += LastNameEditText_TextChanged;
            EmailEditText.TextChanged += EmailEditText_TextChanged;
            SaveButton.Click += SaveButton_Click;

            ChangePasswordLayout.Click += PopupChangePassword_Click;
            AboutLayout.Click += OnAbout_Click;
            DeconnectionLayout.Click += DeconnectionLayout_Click;
        }

        protected override void OnPause()
        {
            base.OnPause();
            OnConnectionStateChanged -= UpdateUIOnConnectionStateChanged;
            App.Locator.Parameter.OnUserChanged += Parameter_OnUserChanged;

            UserImageView.Click -= PickImageFromCameraOrGallery_Click;
            UpdateUserImageButton.Click -= PickImageFromCameraOrGallery_Click;
            FirstNameEditText.TextChanged -= FirstNameEditText_TextChanged;
            LastNameEditText.TextChanged -= LastNameEditText_TextChanged;
            EmailEditText.TextChanged -= EmailEditText_TextChanged;
            SaveButton.Click -= SaveButton_Click;

            ChangePasswordLayout.Click -= PopupChangePassword_Click;
            AboutLayout.Click -= OnAbout_Click;
            DeconnectionLayout.Click -= DeconnectionLayout_Click;
        }

        #endregion

        #region ===== ActionBar ===================================================================

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                default:
                    Finish();
                    break;
            }
            return true;
        }

        #endregion

        #region ===== Initialize View =============================================================

        private void GetObjectsFromView()
        {
            // User information
            UserImageView = FindViewById<RoundedImageView>(Resource.Id.parameter_imageUser);
            UpdateUserImageButton = FindViewById<Button>(Resource.Id.button_updatePicture);
            FirstNameEditText = FindViewById<EditText>(Resource.Id.parameter_firstname);
            LastNameEditText = FindViewById<EditText>(Resource.Id.parameter_lastname);
            EmailEditText = FindViewById<EditText>(Resource.Id.parameter_emailAdress);
            SaveButton = FindViewById<Android.Support.V7.Widget.AppCompatTextView>(Resource.Id.parameter_save);

            // Buttons
            ChangePasswordLayout = FindViewById<RelativeLayout>(Resource.Id.parameter_changePassword);
            AboutLayout = FindViewById<RelativeLayout>(Resource.Id.parameter_about);
            DeconnectionLayout = FindViewById<RelativeLayout>(Resource.Id.parameter_deconnection);

            // Loading and navbar
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
        }

        private void SetDataToView()
        {
            // user information
            if (!string.IsNullOrEmpty(App.CurrentUserEnvironment.User.UserPicture))
            {
                var bytes = Convert.FromBase64String(App.CurrentUserEnvironment.User.UserPicture);
                UserImageView.SetImageBitmap(BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length));
            }
            else UserImageView.SetImageResource(Resource.Drawable.DefaultUser);
            FirstNameEditText.Text = App.CurrentUserEnvironment.User.FirstName;
            LastNameEditText.Text = App.CurrentUserEnvironment.User.LastName;
            EmailEditText.Text = App.CurrentUserEnvironment.User.Email;
            SaveButton.Enabled = false;

            /*if (!string.IsNullOrEmpty(App.CurrentUserEnvironment.User.PhoneNumber))
            {
                var phoneNumber = App.CurrentUserEnvironment.User.PhoneNumber.Split('|');
                if (phoneNumber != null && phoneNumber.Length == 2)
                {
                    var countries = CustomComponents.CountryCode.Country.GetCustomMasterCountryList(CodeCountryLayout);
                    if (phoneNumber[0].Contains("+")) phoneNumber[0] = phoneNumber[0].Substring(1, phoneNumber[0].Length - 1);
                    CodeCountryLayout.SetSelectedCountry(countries.Find(f => f.PhoneCode == phoneNumber[0]));
                    PhoneNumberEditText.Text = phoneNumber[1];
                }
            }*/
            //PhoneNumberEditText.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(NumberOfDigitPhoneNumber()) });
        }

        #endregion

        #region ===== Private Methodes ============================================================

        private void EnableSaveButton()
        {
            _hasDataChanged = true;
            SaveButton.Enabled = true;
        }

        #endregion

        #region ===== Event =======================================================================

        /// <summary>
        /// Update user 
        /// </summary>
        private async void SaveButton_Click(object sender, EventArgs e)
        {
            var isFieldsValid = true;
            if (string.IsNullOrEmpty(FirstNameEditText.Text))
            {
                FirstNameEditText.Error = Resources.GetString(Resource.String.parameter_emptyField);
                isFieldsValid = false;
            }
            if (string.IsNullOrEmpty(LastNameEditText.Text))
            {
                LastNameEditText.Error = Resources.GetString(Resource.String.parameter_emptyField);
                isFieldsValid = false;
            }
            if (!Android.Util.Patterns.EmailAddress.Matcher(EmailEditText.Text).Matches())
            {
                EmailEditText.Error = Resources.GetString(Resource.String.parameter_emailInvalid);
                isFieldsValid = false;
            }
            /*if (string.IsNullOrEmpty(PhoneNumberEditText.Text))
            {
                PhoneNumberEditText.Error = Resources.GetString(Resource.String.parameter_emptyField);
                isFieldsValid = false;
            }*/
            /*var phone = string.Format("{0}{1}", CodeCountryLayout.SelectedCountry.PhoneCode, PhoneNumberEditText.Text);
            if (!Android.Telephony.PhoneNumberUtils.IsGlobalPhoneNumber(phone) && PhoneNumberEditText.Text.Length == NumberOfDigitPhoneNumber())
            {
                PhoneNumberEditText.Error = Resources.GetString(Resource.String.parameter_phonelInvalid);
                isFieldsValid = false;
            }*/
            if (isFieldsValid && _hasDataChanged)
            {
                if (await App.Locator.Parameter.UpdateUser(EmailEditText.Text
                , string.Format("{0}|{1}", /*CodeCountryLayout.SelectedCountry.PhoneCode*/ string.Empty, /*PhoneNumberEditText.Text*/string.Empty)
                , FirstNameEditText.Text
                , LastNameEditText.Text) != 1)
                {
                    SaveButton.Enabled = true;
                }
                else SaveButton.Enabled = false;
            }
        }


        /// <summary>
        /// Update user
        /// </summary>
        private void Parameter_OnUserChanged(object sender, EventArgs e)
        {
            SetDataToView();
        }

        /// <summary>
        /// Take a picture from the camera or the library
        /// </summary>
        private void PickImageFromCameraOrGallery_Click(object sender, EventArgs e)
        {
            var lastImage = UserImageView.GetHashCode();
            var options = new[] {
                Resources.GetString(Resource.String.addSeekios_takePicture),
                Resources.GetString(Resource.String.addSeekios_gallery),
                Resources.GetString(Resource.String.addSeekios_clearImage)
            };

            var builder = new AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog);
            builder.SetTitle(Resource.String.addSeekios_modifyPicture);
            builder.SetItems(options, (innersender, innerargs) =>
            {
                if (innerargs.Which == 0)
                {
                    _isFromGallery = false;
                    if (_cameraService != null) _cameraService.TakePictureFromCamera();
                    EnableSaveButton();
                }
                else if (innerargs.Which == 1)
                {
                    _isFromGallery = true;
                    if (_cameraService != null) _cameraService.TakePictureFromGallery(Resources.GetString(Resource.String.addSeekios_addPicture));
                    EnableSaveButton();
                }
                else if (innerargs.Which == 2)
                {
                    var image = Resource.Drawable.DefaultUser;
                    var drawable = Resources.GetDrawable(image);
                    using (Bitmap bitmap = ((BitmapDrawable)drawable).Bitmap)
                    using (var stream = new System.IO.MemoryStream())
                    {
                        bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                        var bitMapData = stream.ToArray();
                        App.Locator.Parameter.UserPicture = bitMapData;
                    }
                    UserImageView.SetImageResource(image);
                    EnableSaveButton();
                }
            });
            builder.Show();
        }

        /// <summary>
        /// Évènement déclanché lors de la modification d'information de l'utilisateur
        /// </summary>
        private void EmailEditText_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableSaveButton();
        }

        /// <summary>
        /// Évènement déclanché lors de la modification d'information de l'utilisateur
        /// </summary>
        private void FirstNameEditText_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableSaveButton();
        }

        /// <summary>
        /// Évènement déclanché lors de la modification d'information de l'utilisateur
        /// </summary>
        private void LastNameEditText_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableSaveButton();
        }

        /// <summary>
        /// Display the page about
        /// </summary>
        private void OnAbout_Click(object sender, EventArgs e)
        {
            App.Locator.Parameter.GoToAboutPage();
        }

        /// <summary>
        /// Display the popup for changing the password
        /// </summary>
        /// 
        private void PopupChangePassword_Click(object sender, EventArgs e)
        {
            // Popup password
            var popupDialogBuilder = new AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog_Alert);
            var inflater = (LayoutInflater)GetSystemService(LayoutInflaterService);
            var view = inflater.Inflate(Resource.Drawable.PopupChangePassword, null);

            // Get elements in the view
            EditText OldPasswordTextView = view.FindViewById<EditText>(Resource.Id.parameter_oldPassword);
            EditText NewPasswordTextView = view.FindViewById<EditText>(Resource.Id.parameter_newPassword);
            EditText NewPassword2TextView = view.FindViewById<EditText>(Resource.Id.parameter_newPassword2);

            NewPasswordTextView.TextChanged += NewPasswordTextView_TextChanged;
            NewPasswordTextView.FocusChange += NewPasswordTextView_FocusChange;

            popupDialogBuilder.SetView(view);
            var alertDialog = popupDialogBuilder.Create();
            alertDialog.SetTitle(Resources.GetString(Resource.String.parameter_changePassword));
            alertDialog.SetButton2(Resources.GetString(Resource.String.parameter_cancelPopup), (senderAlert, args) =>
            {
                NewPasswordTextView.TextChanged -= NewPasswordTextView_TextChanged;
                NewPasswordTextView.FocusChange -= NewPasswordTextView_FocusChange;
                alertDialog.Dismiss();
            });
            alertDialog.SetButton(Resources.GetString(Resource.String.parameter_savePopup), (senderAlert, args) =>
            {
                // Prepare for update
                App.Locator.Parameter.OldPassword = OldPasswordTextView.Text;
                App.Locator.Parameter.NewPassword = NewPasswordTextView.Text;
                App.Locator.Parameter.NewPasswordReenter = NewPassword2TextView.Text;
                NewPasswordTextView.TextChanged -= NewPasswordTextView_TextChanged;
                NewPasswordTextView.FocusChange -= NewPasswordTextView_FocusChange;

                // Update password
                RunOnUiThread(async () =>
                {
                    await App.Locator.Parameter.UpdateNewPasswordChanged();
                });
            });
            alertDialog.Show();
        }

        /// <summary>
        /// Stop the timer and hide the SetError overlay
        /// </summary>
        private void NewPasswordTextView_FocusChange(object sender, Android.Views.View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus && _timer != null) _timer.Cancel();
        }

        /// <summary>
        /// Handle the display of the SetError overlay for the popup change password
        /// </summary>
        private void NewPasswordTextView_TextChanged(object sender, TextChangedEventArgs e)
        {
            var editText = sender as EditText;
            if (editText.Text.Length < 8)
            {
                editText.SetError(Resources.GetString(Resource.String.login_password_error), null);
                if (_timer != null) _timer.Cancel();
            }
            else
            {
                editText.SetError(string.Empty, null);
                switch ((int)PasswordComplexityHelper.CheckStrength(editText.Text))
                {
                    case (int)PasswordComplexityHelper.PasswordScore.Weak:
                        SetPasswordError(editText
                            , Resources.GetString(Resource.String.login_password_weak)
                            , Resources.GetString(Resource.String.login_weak), App.Red);
                        break;
                    case (int)PasswordComplexityHelper.PasswordScore.Medium:
                        SetPasswordError(editText
                            , Resources.GetString(Resource.String.login_password_medium)
                            , Resources.GetString(Resource.String.login_medium), App.Orange);
                        break;
                    case (int)PasswordComplexityHelper.PasswordScore.Strong:
                    case (int)PasswordComplexityHelper.PasswordScore.VeryStrong:
                        SetPasswordError(editText
                            , Resources.GetString(Resource.String.login_password_strong)
                            , Resources.GetString(Resource.String.login_strong), App.MainColor);
                        break;
                }
                if (_timer == null) _timer = new MyTimer(2000, 2000, editText);
                _timer.Cancel();
                _timer.Start();
            }
            if (string.IsNullOrEmpty(editText.Text)) editText.Error = null;
        }

        /// <summary>
        /// Define the UI for the SetError and display it
        /// </summary>
        private void SetPasswordError(EditText editText, string spanString, string protectForceStr, string textColor)
        {
            var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfStringInString(spanString, protectForceStr);
            var formattedinfoText = new SpannableString(string.Format(spanString, protectForceStr));
            formattedinfoText.SetSpan(new ForegroundColorSpan(Color.ParseColor(textColor)), resultTuple.Item1, resultTuple.Item2, 0);
            editText.SetError(formattedinfoText, null);
        }

        /// <summary>
        /// Deconnection of the user
        /// </summary>
        private void DeconnectionLayout_Click(object sender, EventArgs e)
        {
            RunOnUiThread(async () =>
            {
                LoadingLayout.Visibility = ViewStates.Visible;
                // Delete the device from the database
                if (App.CurrentUserEnvironment.Device != null)
                {
                    if (await App.Locator.Login.Disconnect(App.CurrentUserEnvironment.Device.UidDevice) == 1)
                    {
                        // Stop notification from OneSignal
                        OneSignal.SetSubscription(false);
                        // Go to login page
                        App.Locator.LeftMenu.GoToLogin();
                        if (!IsFinishing) //cf https://rink.hockeyapp.net/manage/apps/323214/app_versions/27/crash_reasons/151001492
                        {
                            //LoginManager.Instance.LogOut(); //facebook disconnection
                            Finish();
                        }
                    }
                }
                LoadingLayout.Visibility = ViewStates.Gone;
            });
        }

        /// <summary>
        /// Enable or disable the save button
        /// </summary>
        private void UpdateUIOnConnectionStateChanged(bool isConnected)
        {
            SaveButton.Enabled = isConnected;
        }

        #endregion

        #region ===== CallBack ====================================================================

        /// <summary>
        /// Callback to pick the picture between the camera or the gallery
        /// </summary>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            Bitmap bitmap = null;
            if (resultCode == Result.Ok
                && _isFromGallery
                && data != null
                && data.Data != null)
            {
                if (_cameraService != null) bitmap = _cameraService.GetPictureFromGallery(data.Data);
            }
            else if (resultCode == Result.Ok)
            {
                if (_cameraService != null) bitmap = _cameraService.GetPictureFromCamera();
            }

            if (bitmap != null)
            {
                // Update the seekios image
                UserImageView.SetImageBitmap(bitmap);
                bitmap.Dispose();

                // Put the image in the ViewModel for the update in bdd
                if (_cameraService != null) App.Locator.Parameter.UserPicture = _cameraService.PictureBinary;
                GC.Collect();
            }
        }

        #endregion
    }
}