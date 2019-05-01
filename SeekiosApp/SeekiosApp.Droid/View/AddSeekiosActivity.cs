using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using SeekiosApp.Droid.Helper;
using SeekiosApp.Droid.CustomComponents;
using Android.Graphics;
using SeekiosApp.Droid.Services;
using SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter;
using System.Threading.Tasks;
using SeekiosApp.Droid.View.FragmentView;
using Android.Views.InputMethods;
using Android.Graphics.Drawables;
using SeekiosApp.Enum;
using Android.Support.Design.Widget;
using Android.Text;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class AddSeekiosActivity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================

        private CameraService _cameraService = null;
        private bool _isFromGallery = false;
        private double _longitude = 0;
        private double _latitude = 0;

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Button for updating the seekios's picture</summary>
        public Button UpdateSeekiosPictureButton { get; set; }

        /// <summary>Seekios picture</summary>
        public RoundedImageView SeekiosImageView { get; set; }

        /// <summary>The "update" image showing at the bottom right corner of the seekios picture</summary>
        public XamSvg.SvgImageView FirmwareUpdateImageView { get; set; }

        /// <summary>Seekios name</summary>
        public EditText SeekiosNameEditText { get; set; }

        /// <summary>Seekios IMEI number (unique id of the seekios)</summary>
        public EditText IMEIEditText { get; set; }

        /// <summary>Seekios PIN code</summary>
        public EditText PINEditText { get; set; }

        /// <summary>Save Button</summary>
        public TextView SaveButton { get; set; }

        /// <summary>Delete button</summary>
        public TextView DeleteButton { get; set; }

        /// <summary>Buy a seekios button</summary>
        public TextView BuySeekiosButton { get; set; }

        /// <summary>Switch to enable or disable notification when a new tracking location is comming</summary>
        public Switch NotificationTrackingSwitch { get; set; }

        /// <summary>Switch to enable or disable notification when a new out of zone location is comming</summary>
        public Switch NotificationZoneSwitch { get; set; }

        /// <summary>Switch to enable or disable notification when a new don't move location is comming</summary>
        public Switch NotificationDontModeSwitch { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle bundle)
        {
            SetContentView(Resource.Layout.AddSeekiosLayout);
            base.OnCreate(bundle);

            GetObjectsFromView();
            SetDataToView();
            _cameraService = new CameraService(this);

            SetSupportActionBar(ToolbarPage);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        protected override void OnResume()
        {
            base.OnResume();
            OnConnectionStateChanged += UpdateUIOnConnectionStateChanged;
            UpdateSeekiosPictureButton.Click += PickImageFromCameraOrGallery_Click;
            SeekiosImageView.Click += PickImageFromCameraOrGallery_Click;
            SeekiosNameEditText.TextChanged += SeekiosNameEditText_TextChanged;
            IMEIEditText.TextChanged += OnImeiNumberTextView_TextChanged;
            PINEditText.TextChanged += PINEditText_TextChanged;
            FirmwareUpdateImageView.Click += FirmwareUpdateImageView_Click;
            SaveButton.Click += SaveSeekios_Click;
            DeleteButton.Click += DeleteSeekiosButton_Click;
            BuySeekiosButton.Click += BuySeekiosButton_Click;
        }

        protected override void OnPause()
        {
            base.OnPause();
            OnConnectionStateChanged -= UpdateUIOnConnectionStateChanged;
            UpdateSeekiosPictureButton.Click -= PickImageFromCameraOrGallery_Click;
            SeekiosImageView.Click -= PickImageFromCameraOrGallery_Click;
            SeekiosNameEditText.TextChanged -= SeekiosNameEditText_TextChanged;
            IMEIEditText.TextChanged -= OnImeiNumberTextView_TextChanged;
            PINEditText.TextChanged -= PINEditText_TextChanged;
            FirmwareUpdateImageView.Click -= FirmwareUpdateImageView_Click;
            SaveButton.Click -= SaveSeekios_Click;
            DeleteButton.Click -= DeleteSeekiosButton_Click;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _cameraService.Dispose();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            App.Locator.AddSeekios.UpdateNotificationSetting(NotificationTrackingSwitch.Checked
                , NotificationZoneSwitch.Checked
                , NotificationDontModeSwitch.Checked);
            Finish();
        }

        #endregion

        #region ===== ActionBar ===================================================================

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.home:
                    Finish();
                    App.Locator.AddSeekios.UpdateNotificationSetting(NotificationTrackingSwitch.Checked
                        , NotificationZoneSwitch.Checked
                        , NotificationDontModeSwitch.Checked);
                    break;
                default:
                    Finish();
                    App.Locator.AddSeekios.UpdateNotificationSetting(NotificationTrackingSwitch.Checked
                        , NotificationZoneSwitch.Checked
                        , NotificationDontModeSwitch.Checked);
                    break;
            }
            return true;
        }

        #endregion

        #region ===== Initialize View =============================================================

        private void GetObjectsFromView()
        {
            SeekiosImageView = FindViewById<RoundedImageView>(Resource.Id.addSeekios_imageSeekios);
            FirmwareUpdateImageView = FindViewById<XamSvg.SvgImageView>(Resource.Id.addSeekios_firmwareUpdate);
            UpdateSeekiosPictureButton = FindViewById<Button>(Resource.Id.button_updatePicture);
            SeekiosNameEditText = FindViewById<EditText>(Resource.Id.seekiosname);
            IMEIEditText = FindViewById<EditText>(Resource.Id.seekios_imei);
            PINEditText = FindViewById<EditText>(Resource.Id.seekios_pin);
            SaveButton = FindViewById<TextView>(Resource.Id.save);
            DeleteButton = FindViewById<TextView>(Resource.Id.addSeekios_deleteSeekiosButton);
            BuySeekiosButton = FindViewById<TextView>(Resource.Id.addSeekios_buyASeekiosButton);
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
            NotificationTrackingSwitch = FindViewById<Switch>(Resource.Id.notificationNewLocationTracking_switch);
            NotificationZoneSwitch = FindViewById<Switch>(Resource.Id.notificationNewLocationOutOfZone_switch);
            NotificationDontModeSwitch = FindViewById<Switch>(Resource.Id.notificationNewLocationMoved_switch); 
        }

        private void SetDataToView()
        {
            if (App.Locator.AddSeekios.IsAdding)
            {
                ToolbarPage.SetTitle(Resource.String.addseekios_addSeekios);
                DeleteButton.Visibility = ViewStates.Gone;
                BuySeekiosButton.Visibility = ViewStates.Visible;
                NotificationTrackingSwitch.Visibility = ViewStates.Gone;
                NotificationZoneSwitch.Visibility = ViewStates.Gone;
                NotificationDontModeSwitch.Visibility = ViewStates.Gone;
                (FindViewById<TextView>(Resource.Id.notificationNewLocationTracking_textView)).Visibility = ViewStates.Gone;
                (FindViewById<TextView>(Resource.Id.notificationNewLocationOutOfZone_textView)).Visibility = ViewStates.Gone;
                (FindViewById<TextView>(Resource.Id.notificationNewLocationMoved_textView)).Visibility = ViewStates.Gone;
            }
            else
            {
                ToolbarPage.SetTitle(Resource.String.addseekios_updateSeekios);
                DeleteButton.Visibility = ViewStates.Visible;
                BuySeekiosButton.Visibility = ViewStates.Gone;
                SetSeekiosDataToView();
                NotificationTrackingSwitch.Checked = App.Locator.AddSeekios.UpdatingSeekios.SendNotificationOnNewTrackingLocation;
                NotificationZoneSwitch.Checked = App.Locator.AddSeekios.UpdatingSeekios.SendNotificationOnNewOutOfZoneLocation;
                NotificationDontModeSwitch.Checked = App.Locator.AddSeekios.UpdatingSeekios.SendNotificationOnNewDontMoveLocation;
            }
        }

        #endregion

        #region ===== Private Methods =============================================================

        /// <summary>
        /// Initialise la vue pour mettre à jour un seekios
        /// </summary>
        private void SetSeekiosDataToView()
        {
            if (App.Locator.AddSeekios.SeekiosImage != null)
            {
                // Seekios image
                var bitmap = BitmapFactory.DecodeByteArray(
                    App.Locator.AddSeekios.SeekiosImage, 0,
                    App.Locator.AddSeekios.SeekiosImage.Length);
                if (bitmap != null)
                {
                    SeekiosImageView.SetImageBitmap(bitmap);
                    bitmap.Dispose();
                }
            }
            // Seekios name
            SeekiosNameEditText.Text = App.Locator.AddSeekios.SeekiosName;

            // Seekios imei and pin
            IMEIEditText.Text = App.Locator.AddSeekios.SeekiosIMEI;
            PINEditText.SetHint(Resource.String.addSeekios_versionEmbedded);
            PINEditText.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(150) });
            PINEditText.Enabled = false;
            PINEditText.InputType = InputTypes.Null;
            var pinCodeLayout = FindViewById<TextInputLayout>(Resource.Id.seekios_pinLayout);
            pinCodeLayout.CounterEnabled = false;
            pinCodeLayout.Hint = Resources.GetString(Resource.String.addSeekios_versionEmbedded);
            IMEIEditText.Enabled = false;
            SaveButton.Enabled = false;

            // Seekios version embedded
            if (App.CurrentUserEnvironment.LastVersionEmbedded != null 
                && App.Locator.DetailSeekios.SeekiosSelected.VersionEmbedded_idversionEmbedded != App.CurrentUserEnvironment.LastVersionEmbedded.IdVersionEmbedded
                && !App.CurrentUserEnvironment.LastVersionEmbedded.IsBetaVersion)
            {
                PINEditText.Text = string.Format("{0} {1}"
                    , App.Locator.DetailSeekios.SeekiosEmbeddedVersion
                    , string.Format(Resources.GetString(Resource.String.addSeekios_versionEmbeddedUpdate)
                    , App.CurrentUserEnvironment.LastVersionEmbedded.VersionName));
                FirmwareUpdateImageView.Visibility = ViewStates.Visible;
            }
            else PINEditText.Text = App.Locator.DetailSeekios.SeekiosEmbeddedVersion;
        }

        /// <summary>
        /// If the IMEI number is 15 chars long, pin 4 chars long, the seekios name is not empty and the device is connected to the internet
        /// </summary>
        private void EnableSaveButton()
        {
            if (SeekiosNameEditText.Text.Count() > 0
                && App.DeviceIsConnectedToInternet
                && PINEditText.Text.Count() == 4
                && IMEIEditText.Text.Count() == 15)
            {
                SaveButton.Enabled = true;
            }
            else SaveButton.Enabled = false;
        }

        #endregion

        #region ===== Event =======================================================================

        /// <summary>
        /// Take a picture from the camera or the library
        /// </summary>
        private void PickImageFromCameraOrGallery_Click(object sender, EventArgs e)
        {
            var options = new[] {
                Resources.GetString(Resource.String.addSeekios_takePicture),
                Resources.GetString(Resource.String.addSeekios_gallery),
                Resources.GetString(Resource.String.addSeekios_clearImage)
            };

            AlertDialog.Builder builder = new AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog);
            builder.SetTitle(Resource.String.addSeekios_modifyPicture);
            builder.SetItems(options, (innersender, innerargs) =>
            {
                if (innerargs.Which == 0)
                {
                    _isFromGallery = false;
                    _cameraService.TakePictureFromCamera();
                }
                else if (innerargs.Which == 1)
                {
                    _isFromGallery = true;
                    _cameraService.TakePictureFromGallery(Resources.GetString(Resource.String.addSeekios_addPicture));
                }
                else if (innerargs.Which == 2)
                {
                    var image = Resource.Drawable.DefaultSeekios;

                    var drawable = Resources.GetDrawable(image);
                    Bitmap bitmap = ((BitmapDrawable)drawable).Bitmap;
                    using (var stream = new System.IO.MemoryStream())
                    {
                        bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                        byte[] bitMapData = stream.ToArray();
                        App.Locator.AddSeekios.SeekiosImage = bitMapData;
                    }
                    SeekiosImageView.SetImageResource(Resource.Drawable.DefaultSeekios);
                    SaveButton.Enabled = true;
                }
            });
            builder.Show();
        }

        /// <summary>
        /// Trigger to enable the save button if the IMEI is okay
        /// </summary>
        private void SeekiosNameEditText_TextChanged(object sender, EventArgs e)
        {
            if (!App.Locator.AddSeekios.IsAdding && App.DeviceIsConnectedToInternet)
            {
                if (!SaveButton.Enabled) SaveButton.Enabled = true;
            }
            else EnableSaveButton();
        }

        /// <summary>
        /// Trigger to update the IMEI and validate it from the database
        /// </summary>
        private void OnImeiNumberTextView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            App.Locator.AddSeekios.SeekiosIMEI = IMEIEditText.Text;
            EnableSaveButton();
        }

        /// <summary>
        /// Trigger to update the IMEI and validate it from the database
        /// </summary>
        private void PINEditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            App.Locator.AddSeekios.SeekiosPIN = PINEditText.Text;
            EnableSaveButton();
        }

        /// <summary>
        /// Shows the last update available in a popup
        /// </summary>
        private void FirmwareUpdateImageView_Click(object sender, EventArgs e)
        {
            CreateFirmwareUpdatePopup(this);
        }

        /// <summary>
        /// Save the Seekios in the database
        /// </summary>
        private async void SaveSeekios_Click(object sender, EventArgs e)
        {
            if (await App.Locator.AddSeekios.InsertOrUpdateSeekios(SeekiosNameEditText.Text
                , IMEIEditText.Text
                , PINEditText.Text
                , App.Locator.AddSeekios.SeekiosImage))
            {
                SaveButton.Enabled = false;
                Finish();
            }
        }

        /// <summary>
        /// When the connection change
        /// </summary>
        private void UpdateUIOnConnectionStateChanged(bool isConnected)
        {
            if (App.Locator.AddSeekios.IsAdding) SaveButton.Enabled = isConnected;
        }

        /// <summary>
        /// Show a popup confirmation and delete the seekios
        /// </summary>
        private void DeleteSeekiosButton_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder confirmationPopup = new AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog_Alert);
            Dialog dialog = null;
            var title = Resources.GetString(Resource.String.detailSeekios_deletePopupTitle);
            confirmationPopup.SetTitle(string.Format(title, App.Locator.DetailSeekios.SeekiosSelected.SeekiosName));
            confirmationPopup.SetMessage(Resource.String.detailSeekios_deletePopupContent);
            confirmationPopup.SetPositiveButton(Resource.String.detailSeekios_deletePopupButtonDelete, async (senderAlert, args) =>
            {
                LoadingLayout.Visibility = ViewStates.Visible;
                int result = await App.Locator.DetailSeekios.DeleteSeekios();
                LoadingLayout.Visibility = ViewStates.Gone;
                if (result == 1)
                {
                    App.Locator.DetailSeekios.IsSeekiosDeleted = true;
                    Finish();
                }
            });
            confirmationPopup.SetNegativeButton(Resource.String.detailSeekios_deletePopupButtonCancel, (senderAlert, args) =>
            {
                dialog.Dismiss();
            });

            dialog = confirmationPopup.Create();
            dialog.Show();
        }

        /// <summary>
        /// Open the webbrowser on the seekios prestashop to buy a seekios
        /// </summary>
        private void BuySeekiosButton_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse(App.BuySeekiosLink)));
        }

        #endregion

        #region ===== CallBack ====================================================================

        /// <summary>
        /// CallBack du Picker de la sélection d'une image Seekios
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
                // service qui récupère et resize l'image
                bitmap = _cameraService.GetPictureFromGallery(data.Data);
            }
            else if (resultCode == Result.Ok)
            {
                // service qui récupère et resize l'image
                bitmap = _cameraService.GetPictureFromCamera();
            }

            // met à jour l'image du seekios
            if (bitmap != null)
            {
                SeekiosImageView.SetImageBitmap(bitmap);
                bitmap.Dispose();

                // transfère l'image dans le ViewModel pour l'update en bdd
                App.Locator.AddSeekios.SeekiosImage = _cameraService.PictureBinary;
                GC.Collect();
            }

            if (!App.Locator.AddSeekios.IsAdding && !SaveButton.Enabled) SaveButton.Enabled = true;
        }

        #endregion
    }
}