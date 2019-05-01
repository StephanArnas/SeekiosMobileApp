using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using SeekiosApp.Enum;
using SeekiosApp.Model.DTO;
using SeekiosApp.UWP.ControlManager;
using SeekiosApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace SeekiosApp.UWP
{
    public sealed partial class ListSeekiosPage : Page
    {
        #region ===== Attributs ===================================================================

        private MapControlManager _mapControlManager = null;
        private SeekiosDTO _selectedSeekios = null;
        private bool _firstInitialise = false;

        private Image _batteryImage = null;
        private TextBlock _batteryDate = null;
        private Button _batteryButton = null;
        private ProgressRing _batteryLoading = null;

        #endregion

        #region ===== Properties ==================================================================

        private List<SeekiosDTO> LsSeekios
        {
            get
            {
                return SeekiosApp.App.CurrentUserEnvironment.LsSeekios;
            }
        }

        #endregion

        #region ===== Constructor =================================================================

        public ListSeekiosPage()
        {
            InitializeComponent();
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 240, 240, 240);
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(100, 240, 240, 240);
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Title = string.Empty;
            SetDataAndStyleToView();
        }

        #endregion

        #region ====== Life Cycle =================================================================

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested -= App_BackRequested;
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public void SetDataAndStyleToView()
        {
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                HamburgerMenu.DisplayMode = SplitViewDisplayMode.Overlay;
                ListSeekiosView.Margin = new Thickness(0, 50, 0, 0);
            }
            UserEmail_HamburgerMenu.Label = SeekiosApp.App.CurrentUserEnvironment.User.Email;
        }

        #endregion

        #region ====== Event ======================================================================

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
            {
                ListSeekiosView.SelectedItem = SeekiosApp.App.CurrentUserEnvironment.LsSeekios.FirstOrDefault();
            }
        }

        private void HamburgerMenu_ItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as HamburgerMenuGlyphItem;
            if (menuItem.Tag.ToString() == "AddSeekios")
            {
                SeekiosApp.App.Locator.LeftMenu.GoToAddSeekios();
            }
            else if (menuItem.Tag.ToString() == "ParameterUser")
            {
                SeekiosApp.App.Locator.LeftMenu.GoToParameter();
            }
        }

        private void ListSeekiosView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListSeekiosView.SelectedItem == null) return;

            // Get the current seekios 
            MapViewModelBase.Seekios = ListSeekiosView.SelectedItem as SeekiosDTO;                              // usefull fot the viewmodel
            SeekiosApp.App.Locator.DetailSeekios.SeekiosSelected = ListSeekiosView.SelectedItem as SeekiosDTO;  // usefull fot the viewmodel
            _selectedSeekios = ListSeekiosView.SelectedItem as SeekiosDTO;                                      // usefull for the view logic

            // Is the seekios a new one ? (without position)
            InitializeFirstLocation();

            // Setup the battery
            InitializeBattery();

            // Setup the map
            InitializeMap();
        }

        private async void BatteryButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            LoadingControl.IsLoading = true;
            await SeekiosApp.App.Locator.DetailSeekios.RequestBatteryLevel();
            LoadingControl.IsLoading = false;
            _selectedSeekios.IsRefreshingBattery = true;
            _batteryLoading.IsActive = true;
            _batteryButton.IsEnabled = false;
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e) { }

        #endregion

        #region ====== Private Methods ============================================================

        private void InitializeFirstLocation()
        {
            if (_selectedSeekios.LastKnownLocation_latitude == SeekiosApp.App.DefaultLatitude
               && _selectedSeekios.LastKnownLocation_longitude == SeekiosApp.App.DefaultLongitude)
            {
                _firstInitialise = true;
            }
            else _firstInitialise = false;
        }

        private void InitializeBattery()
        {
            // Get elements from the view
            _batteryImage = (Image)GetChildren(ListSeekiosView).First(x => x.Name == "BatteryImage");
            _batteryDate = (TextBlock)GetChildren(ListSeekiosView).First(x => x.Name == "BatteryDate");
            _batteryButton = (Button)GetChildren(ListSeekiosView).First(x => x.Name == "BatteryButton");
            _batteryLoading = (ProgressRing)GetChildren(ListSeekiosView).First(x => x.Name == "BatteryLoading");
            _batteryButton.Click += BatteryButton_ClickAsync;
            // Setup the battery informations
            if (_firstInitialise)
            {
                _batteryImage.Visibility = Visibility.Collapsed;
                _batteryDate.Text = App.ResourceLoader.GetString("UpdatePosition");
                _batteryButton.IsEnabled = false;
            }
            else
            {
                // Make the indicator loading
                if (_selectedSeekios.IsRefreshingBattery)
                {
                    _batteryLoading.IsActive = true;
                    _batteryButton.IsEnabled = false;
                }
                else
                {
                    _batteryLoading.IsActive = false;
                    _batteryButton.IsEnabled = true;
                }
                // Display the value
                if (_selectedSeekios.DateLastCommunication.HasValue)
                {
                    _batteryDate.Text = string.Format(App.ResourceLoader.GetString("BatteryLevel")
                        , _selectedSeekios.BatteryLife
                        , _selectedSeekios.DateLastCommunication.Value.ToString("dd/MM/yyyy")
                        , _selectedSeekios.DateLastCommunication.Value.ToString("HH:mm"));
                    _batteryImage.Visibility = Visibility.Visible;
                }
                // No information about the battery
                else
                {
                    _batteryImage.Visibility = Visibility.Collapsed;
                    _batteryDate.Text = App.ResourceLoader.GetString("NoInformation");
                }
            }

            // Setup the image of the battery
            if (_selectedSeekios.BatteryLife > 95) _batteryImage.Source = new BitmapImage(new Uri("ms-appx://SeekiosApp.UWP/Assets/Icons/Battery/Battery100.png"));
            else if (_selectedSeekios.BatteryLife > 85) _batteryImage.Source = new BitmapImage(new Uri("ms-appx://SeekiosApp.UWP/Assets/Icons/Battery/Battery90.png"));
            else if (_selectedSeekios.BatteryLife > 75) _batteryImage.Source = new BitmapImage(new Uri("ms-appx://SeekiosApp.UWP/Assets/Icons/Battery/Battery80.png"));
            else if (_selectedSeekios.BatteryLife > 65) _batteryImage.Source = new BitmapImage(new Uri("ms-appx://SeekiosApp.UWP/Assets/Icons/Battery/Battery70.png"));
            else if (_selectedSeekios.BatteryLife > 55) _batteryImage.Source = new BitmapImage(new Uri("ms-appx://SeekiosApp.UWP/Assets/Icons/Battery/Battery60.png"));
            else if (_selectedSeekios.BatteryLife > 45) _batteryImage.Source = new BitmapImage(new Uri("ms-appx://SeekiosApp.UWP/Assets/Icons/Battery/Battery50.png"));
            else if (_selectedSeekios.BatteryLife > 35) _batteryImage.Source = new BitmapImage(new Uri("ms-appx://SeekiosApp.UWP/Assets/Icons/Battery/Battery40.png"));
            else if (_selectedSeekios.BatteryLife > 25) _batteryImage.Source = new BitmapImage(new Uri("ms-appx://SeekiosApp.UWP/Assets/Icons/Battery/Battery30.png"));
            else if (_selectedSeekios.BatteryLife > 15) _batteryImage.Source = new BitmapImage(new Uri("ms-appx://SeekiosApp.UWP/Assets/Icons/Battery/Battery20.png"));
            else if (_selectedSeekios.BatteryLife > 5) _batteryImage.Source = new BitmapImage(new Uri("ms-appx://SeekiosApp.UWP/Assets/Icons/Battery/Battery10.png"));
            else _batteryImage.Source = new BitmapImage(new Uri("ms-appx://SeekiosApp.UWP/Assets/Icons/Battery/Battery0.png"));
        }

        private void InitializeMap()
        {
            var map = (MapControl)GetChildren(ListSeekiosView).First(x => x.Name == "Map");
            if (_mapControlManager == null)
            {
                _mapControlManager = new MapControlManager(map, null, null);
                _mapControlManager.RegisterMethodes();
            }
            if (MapViewModelBase.Mode != null
                    && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
            {
                if (MapViewModelBase.Mode.StatusDefinition_idstatusDefinition != 1) MapControlManager.IsOutOf = true;
                SeekiosApp.App.Locator.ModeDontMove.MapControlManager = _mapControlManager;
                //SeekiosApp.App.Locator.ModeDontMove.MapControlManager.OnInitTrackingRouteComplete -= OnInitTrackingRouteComplete;
                //SeekiosApp.App.Locator.ModeDontMove.MapControlManager.OnInitTrackingRouteComplete += OnInitTrackingRouteComplete;
                SeekiosApp.App.Locator.ModeDontMove.InitMap();
                SeekiosApp.App.Locator.ModeDontMove.InitModeDontMove();
                if (SeekiosApp.App.Locator.ModeDontMove.LsSeekiosInTrackingAfterMove.Contains(SeekiosApp.App.Locator.DetailSeekios.SeekiosSelected.Idseekios))
                {
                    Task.Factory.StartNew(() => { SeekiosApp.App.Locator.ModeDontMove.InitDontMoveTrackingRouteAsync(); });
                }
            }
            else if (MapViewModelBase.Mode != null
                && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
            {
                if (MapViewModelBase.Mode.StatusDefinition_idstatusDefinition != 1) MapControlManager.IsOutOf = true;
                SeekiosApp.App.Locator.ModeZone.MapControlManager = _mapControlManager;
                //SeekiosApp.App.Locator.ModeZone.MapControlManager.OnInitTrackingRouteComplete -= OnInitTrackingRouteComplete;
                //SeekiosApp.App.Locator.ModeZone.MapControlManager.OnInitTrackingRouteComplete += OnInitTrackingRouteComplete;
                SeekiosApp.App.Locator.ModeZone.InitMap();
                var isInAlert = MapViewModelBase.Mode != null && MapViewModelBase.Mode.StatusDefinition_idstatusDefinition != 1;

                _mapControlManager.CreateZone(SeekiosApp.App.Locator.ModeZone.DecodeTrame(MapViewModelBase.Mode.Trame), isInAlert);
                if (SeekiosApp.App.Locator.ModeZone.LsSeekiosInTrackingAfterOOZ.Contains(SeekiosApp.App.Locator.DetailSeekios.SeekiosSelected.Idseekios))
                {
                    Task.Factory.StartNew(() => { SeekiosApp.App.Locator.ModeZone.InitZoneTrackingRouteAsync(); });
                }
                //FocusOnZoneButton.Hidden = false;
            }
            else if (MapViewModelBase.Mode != null
                && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
            {
                MapControlManager.IsOutOf = false;
                SeekiosApp.App.Locator.ModeTracking.MapControlManager = _mapControlManager;
                //SeekiosApp.App.Locator.ModeTracking.MapControlManager.OnInitTrackingRouteComplete -= OnInitTrackingRouteComplete;
                //SeekiosApp.App.Locator.ModeTracking.MapControlManager.OnInitTrackingRouteComplete += OnInitTrackingRouteComplete;
                SeekiosApp.App.Locator.ModeTracking.InitMap();
                Task.Factory.StartNew(() => { SeekiosApp.App.Locator.ModeTracking.InitTrackingRoute(); });
            }
            else
            {
                MapControlManager.IsOutOf = false;
                SeekiosApp.App.Locator.Map.MapControlManager = _mapControlManager;
                SeekiosApp.App.Locator.Map.InitMap();
            }
            //SetZoom();
        }

        private List<FrameworkElement> GetChildren(DependencyObject parent)
        {
            var controls = new List<FrameworkElement>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement)
                {
                    controls.Add(child as FrameworkElement);
                }
                controls.AddRange(GetChildren(child));
            }
            return controls;
        }

        #endregion
    }
}
