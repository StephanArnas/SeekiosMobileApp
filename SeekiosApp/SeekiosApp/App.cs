using Microsoft.AspNet.SignalR.Client;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using SeekiosApp.Model.DTO;
using SeekiosApp.ViewModel;
using System;

namespace SeekiosApp
{
    public class App
    {
        #region ===== Constants ===================================================================

        #region NAVIGATION

        /// <summary>
        /// Key linking the page login with the navigator service
        /// </summary>
        public const string LOGIN_PAGE = "LoginPage";

        /// <summary>
        /// Key linking the page seekios list with the navigator service
        /// </summary>
        public const string LIST_SEEKIOS_PAGE = "ListSeekiosPage";

        /// <summary>
        /// Key linking the MySeekiosDetailActivity with the navigator service
        /// </summary>
        public const string DETAIL_SEEKIOS_PAGE = "DetailSeekiosPage";

        /// <summary>
        /// Key linking the page add seekios with the navigator service
        /// </summary>
        public const string ADD_SEEKIOS_PAGE = "AddSeekiosPage";

        /// <summary>
        /// Key linking the ModeZoneActivity with the navigator service
        /// </summary>
        public const string MODE_ZONE_PAGE = "ModeZonePage";

        /// <summary>
        /// key for the second page of the Mode Zone
        /// </summary>
        public const string MODE_ZONE_2_PAGE = "ModeZone2Page";

        /// <summary>
        /// key for the third page of the Mode Zone
        /// </summary>
        public const string MODE_ZONE_3_PAGE = "ModeZone3Page";

        /// <summary>
        /// Key linking the ModeDontMoveActivity with the navigator service
        /// </summary>
        public const string MODE_DONT_MOVE_PAGE = "ModeDontMovePage";

        /// <summary>
        /// key for the second page of the Mode Dont Move
        /// </summary>
        public const string MODE_DONT_MOVE_2_PAGE = "ModeDontMove2Page";

        /// <summary>
        /// Key linking the ModeTrackingActivity with the navigator service
        /// </summary>
        public const string MODE_TRACKING_PAGE = "ModeTrackingPage";

        /// <summary>
        /// Key linking the page map with the navigator service
        /// </summary>
        public const string MAP_PAGE = "MapPage";

        /// <summary>
        /// Key linking the MapAllSeekiosActivity with the navigator service
        /// </summary>
        public const string MAP_ALL_SEEKIOS_PAGE = "MapAllSeekiosPage";

        /// <summary>
        /// Key linking the HistoricActivity with the navigator service
        /// </summary>
        public const string MAP_HISTORIC_PAGE = "MapHistoricPage";

        /// <summary>
        /// Key linking the page credit with the navigator service
        /// </summary>
        public const string ADD_CREDITS_PAGE = "AddCreditsPage";

        /// <summary>
        /// Key linking the AlertActivity with the navigator service
        /// </summary>
        public const string ALERT_PAGE = "AlertPage";

        /// <summary>
        /// Key linking the AlertSOSActivity with the navigator service
        /// </summary>
        public const string ALERT_SOS_PAGE = "AlertSOSPage";

        /// <summary>
        /// Key linking the page about with the navigator service
        /// </summary>
        public const string ABOUT_PAGE = "AboutPage";

        /// <summary>
        /// Key linking the page community with the navigator service
        /// </summary>
        public const string COMMUNITY_PAGE = "CommunityPage";

        /// <summary>
        /// Key linking the page list of sharing with the navigator service
        /// </summary>
        public const string LIST_SHARINGS_PAGE = "ListSharingsPage";

        /// <summary>
        /// Key linking the page share a seekios with the navigator service
        /// </summary>
        public const string SHARE_SEEKIOS_PAGE = "ShareSeekiosPage";

        /// <summary>
        /// Key linking the page add a friend with the navigator service
        /// </summary>
        public const string ADD_FRIEND_PAGE = "AddFriendPage";

        /// <summary>
        /// Key linking the page reload credit with the navigator service
        /// </summary>
        public const string RELOAD_CREDIT_PAGE = "ReloadCreditPage";

        /// <summary>
        /// Key linking the page historic of transaction with the navigator service
        /// </summary>
        public const string TRANSACTION_HISTORIC_PAGE = "TransactionHistoricPage";

        /// <summary>
        /// Key linking the page user parameter with the navigator service
        /// </summary>
        public const string PARAMETER_PAGE = "ParameterPage";

        /// <summary>
        /// Key linking the page need update with the navigator service
        /// </summary>
        public const string NEED_UPDATE_PAGE = "NeedUpdatePage";

        /// <summary>
        /// Key linking the page credits with the navigator service
        /// </summary>
        public const string CREDITS_PAGE = "CreditsPage";

        /// <summary>
        /// Key linking the page cgu with the navigator service
        /// </summary>
        public const string CGU_PAGE = "CGUPage";

        /// <summary>
        /// Key linking the ListTutorialActivity with the navigator service
        /// </summary>
        public const string LIST_TUTORIAL_PAGE = "ListTutorialPage";

        /// <summary>
        /// Key linking the page first launch with the navigator service
        /// </summary>
        public const string TUTORIAL_BACKGROUND_FIRST_LAUNCH_PAGE = "FirstLaunchPage";

        /// <summary>
        /// Key linking the page tutorial with the navigator service
        /// </summary>
        public const string TUTORIAL_PAGE = "TutorialPage";

        /// <summary>
        /// Key linking the page tutorial with the navigator service
        /// </summary>
        public const string TUTORIAL_POWERSAVING_PAGE = "TutorialPowerSaving";

        /// <summary>
        /// Key linking the page tutorial with the navigator service
        /// </summary>
        public const string TUTORIAL_CREDITCOST_PAGE = "TutorialCreditCost";

        /// <summary>
        /// Key linking the page seekios Led with the navigator service
        /// </summary>
        public const string TUTORIAL_SEEKIOS_LED_PAGE = "SeekiosLedPage";

        #endregion

        #region THEMES

        /// <summary>
        /// Default theme
        /// </summary>
        public const int THEME_LIGHT = 0;

        /// <summary>
        /// Black theme
        /// </summary>
        public const int THEME_BLACK = 1;

        /// <summary>
        /// Community theme
        /// </summary>
        public const int THEME_COMMUNITY = 2;

        #endregion

        #region SAVE_DATA

        /// <summary>
        /// Parameter used to save the password for the auto-connection
        /// </summary>
        public const string LocalCredentials = "LocalCredentials";

        /// <summary>
        /// Parameter used to save the notifications if the app is not in focus
        /// </summary>
        public const string Notification = "notification";

        /// <summary>
        /// Parameter used to save if the app first time login
        /// </summary>
        public const string IsLoginFirstTime = "Yes/No";

        /// <summary>
        /// Parameter used to save if the user has seen the tuto
        /// </summary>
        public const string IsFirstLaunchTutorial = "firstLaunchTuto";

        /// <summary>
        /// Parameter used to save the state of the map (satellite or plan)
        /// </summary>
        public const string MapChange = "mapChange";

        /// <summary>
        /// Parameter used to save the tracking setting when the seekios is out of zone (enable or not, refresh value)
        /// </summary>
        public const string TrackingSetting = "trackingSetting";

        /// <summary>
        /// Parameter used to save the value true/false if the user have seen the popup reload credit 
        /// </summary>
        public const string NeedToDisplayReloadCreditMonthly = "needToDisplayReloadCreditMonthly4";

        #endregion

        #region REFRESH SEEKIOS

        public const int TIME_FOR_REFRESH_SEEKIOS_IN_SECOND = 240;

        #endregion

        #region DEFAULT TRACKING SETTING 

        public const int DEFAULT_TRACKING_SETTING = 60;

        #endregion 

        #endregion

        #region ===== Properties ==================================================================

        #region COLORS

        /// <summary>Main color (green)</summary>
        public static string MainColor
        {
            get
            {
                return "#62da73";
            }
        }

        /// <summary>Community color (blue)</summary>
        public static string CommunityColor
        {
            get
            {
                return "#2cc5c2";
            }
        }

        /// <summary>Main Orange</summary>
        public static string Orange
        {
            get { return "#f69d35"; }
        }

        /// <summary>Main Red</summary>
        public static string Red
        {
            get { return "#ff4c39"; }
        }

        #endregion

        #region DEFAULT COORDONATES

        public static double DefaultLongitude { get { return -1.534283; } }

        public static double DefaultLatitude { get { return 43.489498; } }

        #endregion

        #region BACKGROUND IMAGES

        /// <summary>
        /// Return background image 
        /// (before this propertie could return a random image)
        /// Background2 : https://www.pexels.com/photo/adult-book-business-cactus-297755/
        /// Background About : https://www.pexels.com/photo/adventure-climb-climbers-daylight-282547/
        /// </summary>
        public static readonly string[] LoginBackgrounds = { "loginbackground" }; //LoginBackground1 & LoginBackground3 removed for the moment

        #endregion

        #region LINK 

        public static string FacebookLink { get { return "https://www.facebook.com/seekios/?fref=ts"; } }
        public static string TwitterLink { get { return "https://twitter.com/Seekios"; } }
        public static string InstagramLink { get { return "https://www.instagram.com/seekios"; } }
        public static string LinkedinLink { get { return "https://fr.linkedin.com/company/things-of-tomorrow"; } }
        public static string SeekiosLink { get { return "https://seekios.com"; } }
        public static string PolicyLink { get { return "https://seekios.com/Home/CGU"; } }
        public static string BuySeekiosLink { get { return "http://shop.seekios.com/index.php?id_product=1&controller=product"; } }
        public static string TutorialHelpLink { get { return "https://seekios.com/Home/UserManual"; } }
        public static string TutorialCreditCostLink { get { return "https://seekios.com/Home/Usage"; } }
        public static string TutorialNotificationLink { get { return "http://fr.wikihow.com/activer-les-notifications-push"; } }

        #endregion

        /// <summary>Context user environment (all user data)</summary>
        public static UserEnvironmentDTO CurrentUserEnvironment { get; set; }

        /// <summary>Give access to view models</summary>
        public static ViewModelLocator Locator
        {
            get
            {
                return _locator ?? (_locator = new ViewModelLocator());
            }
        }

        /// <summary>Actual active theme</summary>
        public static int ActualTheme
        {
            get { return _actualTheme; }
            set
            {
                _actualTheme = value;
                OnThemeChanged?.Invoke(null, value);
            }
        }

        /// <summary>Bool use for the unit test</summary>
        public static bool IsTestMode { get; set; }

        /// <summary>Is the device as internet (always true when it's in unit test)</summary>
        public static bool DeviceIsConnectedToInternet
        {
            get
            {
                return IsTestMode ? true : CrossConnectivity.Current.IsConnected;
            }
        }

        /// <summary>Bool user to know if an update is available (only for iOS)</summary>
        public static bool IsAppNeedUpdate = false;

        /// <summary>SignalR object used for real time</summary>
        public static HubConnection SeekiosSignalR { get; set; }

        /// <summary>Unique identifier of the seekios</summary>
        public static string UidDevice { get; set; }

        #endregion

        #region ===== Attributs ===================================================================

        /// <summary>Give access to view models</summary>
        private static ViewModelLocator _locator = null;

        /// <summary>Actual active theme</summary>
        private static int _actualTheme = THEME_LIGHT;

        #endregion

        #region ===== Constructor =================================================================

        public App() { }

        #endregion

        #region ===== Public Methods ==============================================================

        /// <summary>
        /// Update user credits in the entire app
        /// </summary>
        public static void RaiseRemainingRequestChangedEverywhere()
        {
            RemainingRequestChanged?.Invoke(null, null);
        }

        /// <summary>
        /// Update user information in the entire app
        /// </summary>
        public static void RaiseSeekiosInformationChangedEverywhere(int idSeekios)
        {
            SeekiosChanged?.Invoke(null, idSeekios);
        }

        #endregion

        #region ===== Event =======================================================================

        /// <summary>Fired when a theme is changed</summary>
        public static event EventHandler<int> OnThemeChanged;

        /// <summary>Fired when credits amount has changed</summary>
        public static event EventHandler RemainingRequestChanged;

        /// <summary>Fired when seekios information changed</summary>
        public static event EventHandler<int> SeekiosChanged;

        /// <summary>Fired when the connectivity of the app change</summary>
        public static event ConnectivityChangedEventHandler ConnectivityChanged
        {
            add
            {
                CrossConnectivity.Current.ConnectivityChanged += value;
            }
            remove
            {
                CrossConnectivity.Current.ConnectivityChanged -= value;
            }
        }

        #endregion
    }
}