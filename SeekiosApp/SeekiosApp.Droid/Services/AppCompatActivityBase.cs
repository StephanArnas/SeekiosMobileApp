using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using SeekiosApp.Droid.Helper;
using System.Diagnostics;
using SeekiosApp.Model.DTO;
using System.Linq;
using SeekiosApp.Enum;
using System.Threading.Tasks;
using Android.Text;

namespace SeekiosApp.Droid.Services
{
    public class AppCompatActivityBase : AppCompatActivity, IComponentCallbacks2
    {
        #region ===== Properties ==================================================================

        internal string ActivityKey { get; private set; }

        internal static string NextPageKey { get; set; }

        public static AppCompatActivityBase CurrentActivity { get; set; }

        public TextView AppInformationLayout { get; set; }

        public Android.Support.V7.Widget.Toolbar ToolbarPage { get; set; }

        public RelativeLayout LoadingLayout { get; set; }

        public static Stopwatch Timer = new Stopwatch();

        private static string stateOfLifeCycle = "";

        //private static BroadcastReceiver phoneUnlockedBR = null;

        //private static BroadcastReceiver phoneLockedBR = null;

        #endregion

        #region ===== Cycle De Vie ================================================================

        protected override void OnCreate(Bundle savedInstanceState)
        {
            UpdateTheme();
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            base.OnCreate(savedInstanceState);
            stateOfLifeCycle = "Create";
            //if (phoneLockedBR == null) phoneLockedBR = new PhoneLockedReceiver();
            //if (phoneUnlockedBR == null) phoneUnlockedBR = new PhoneUnlockedReceiver();
            //RegisterReceiver(phoneLockedBR, new IntentFilter(Intent.ActionScreenOff));
            //RegisterReceiver(phoneUnlockedBR, new IntentFilter(Intent.ActionUserPresent));
        }

        public override void SetContentView(int layoutResID)
        {
            base.SetContentView(layoutResID);
            AppInformationLayout = FindViewById<TextView>(Resource.Id.appInformationLayout);
        }

        protected override void OnStart()
        {
            base.OnStart();
            App.ConnectivityChanged += OnConnectivityChanged;
            if (AppInformationLayout != null) AppInformationLayout.Visibility = App.DeviceIsConnectedToInternet ? ViewStates.Gone : ViewStates.Visible;
            stateOfLifeCycle = "Start";
        }

        protected override void OnResume()
        {
            CurrentActivity = this;

            if (string.IsNullOrEmpty(ActivityKey))
            {
                ActivityKey = NextPageKey;
                NextPageKey = null;
            }

            // Gestion du retour en arrière jusqu'à une interface donnée
            if (_activityKeyToGoBackTo != string.Empty && _activityKeyToGoBackTo != ActivityKey)
            {
                Finish();
            }
            else if (_activityKeyToGoBackTo != string.Empty)
            {
                _activityKeyToGoBackTo = string.Empty;
            }

            // If we are not on the page when the internet connection is lost, we need to update the UI
            if (!App.DeviceIsConnectedToInternet && OnConnectionStateChanged != null) OnConnectionStateChanged.Invoke(false);

            if (LoadingLayout != null) LoadingLayout.BringToFront();

            base.OnResume();
            stateOfLifeCycle = "Resume";
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            //if (phoneUnlockedBR != null) UnregisterReceiver(phoneUnlockedBR);
            //if (phoneLockedBR != null) UnregisterReceiver(phoneLockedBR);
        }

        protected override void OnStop()
        {
            base.OnStop();
            App.ConnectivityChanged -= OnConnectivityChanged;
            stateOfLifeCycle = "Stop";
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            Finish();
        }

        #endregion

        #region ===== Public methods ==============================================================


        public static void GoBack()
        {
            CurrentActivity?.OnBackPressed();
        }

        /// <summary>
        /// Gestion du retour en arrière jusqu'à une interface donnée
        /// </summary>
        public void GoBackTo(string activityKey)
        {
            _activityKeyToGoBackTo = activityKey;
            Finish();
        }

        /// <summary>
        /// Update theme every time a page is created
        /// </summary>
        public void UpdateTheme()
        {
            if (ActivityKey == App.COMMUNITY_PAGE
                || ActivityKey == App.ADD_FRIEND_PAGE
                || ActivityKey == App.LIST_SHARINGS_PAGE
                || ActivityKey == App.SHARE_SEEKIOS_PAGE)
            {
                App.ActualTheme = App.THEME_COMMUNITY;
            }
            else App.ActualTheme = App.THEME_LIGHT;
            ThemeHelper.OnActivityCreateSetTheme(this);
        }

        public void CreateFirmwareUpdatePopup(Context context)
        {
            AlertDialog.Builder firmwareUpdatePopupBuilder = new AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog_Alert);
            AlertDialog firmwareUpdateDialog = null;

            var view = LayoutInflater.Inflate(Resource.Layout.FirmwareUpdatePopup, null);

            var firmwareUpdateContent = view.FindViewById<TextView>(Resource.Id.firmwareUpdatePopupContent);
            var updateContent = App.CurrentUserEnvironment.LastVersionEmbedded.ReleaseNotes.Replace("<li>", "").Replace("</li>", "<br/>");
            firmwareUpdateContent.SetText(Html.FromHtml(updateContent), TextView.BufferType.Spannable);
            firmwareUpdatePopupBuilder.SetView(view);
            firmwareUpdatePopupBuilder.SetPositiveButton(Resources.GetString(Resource.String.firmwareUpdatePopupPositiveButtonText), (senderAlert, args) =>
            {
                firmwareUpdateDialog.Dismiss();
            });

            firmwareUpdateDialog = firmwareUpdatePopupBuilder.Create();
            firmwareUpdateDialog.Show();
        }

        #endregion

        #region ===== Private methods =============================================================

        /// <summary>interface jusqu'à laquelle revenir</summary>
        private static string _activityKeyToGoBackTo = string.Empty;

        private void OnConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            if (AppInformationLayout != null)
            {
                AppInformationLayout.Visibility = e.IsConnected ? ViewStates.Gone : ViewStates.Visible;
                OnConnectionStateChanged?.Invoke(e.IsConnected);
            }
            if (e.IsConnected)
            {
                App.Locator.ListSeekios.SubscribeToSignalR();
                App.Locator.ListSeekios.LoadUserEnvironment(DeviceInfoHelper.DeviceModel
                    , DeviceInfoHelper.Platform
                    , DeviceInfoHelper.Version
                    , DeviceInfoHelper.GetDeviceUniqueId(this)
                    , DeviceInfoHelper.CountryCode);
            }
        }

        public event UpdateUIWithRespectToInternetConnectionState OnConnectionStateChanged;

        public delegate void UpdateUIWithRespectToInternetConnectionState(bool isConnected);

        #endregion
    }
}