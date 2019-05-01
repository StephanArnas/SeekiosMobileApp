//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using SeekiosApp.Droid.View.FragmentView;
//using SeekiosApp.ViewModel;
//using Android.Gms.Maps;
//using SeekiosApp.Droid.ControlManager;
//using Android.Locations;
//using SeekiosApp.Droid.Helper;
//using SeekiosApp.Model.APP;

//namespace SeekiosApp.Droid.View
//{
//    [Activity(Label = "ModeInTimeActivity")]
//    public class ModeInTimeActivity : MapBaseActivity
//    {
//        #region ===== Attributs ===================================================================

//        private IMenuItem _menuSinceToday;
//        private IMenuItem _menuSinceYesterday;
//        private IMenuItem _menuSinceLastWeek;
//        private IMenuItem _menuSinceLastMonth;

//        /// <summary>Timer for the research : we start a research only if the user didn't enter more text after 1 second</summary>
//        private System.Timers.Timer _timer;

//        /// <summary></summary>
//        private int _countSeconds;

//        #endregion

//        #region ===== Properties ==================================================================

//        /// <summary>AutoCompTextView for retrieving and displaying addresses</summary>
//        AutoCompleteTextView SearchAddressesAutoCompleteTextView { get; set; }

//        /// <summary>Search Layout </summary>
//        LinearLayout SearchLayout { get; set; }

//        /// <summary>Address List</summary>
//        List<string> AddressList { get; set; }

//        #endregion

//        #region ===== Cycle De Vie ================================================================

//        /// <summary>Création de la page</summary>
//        protected override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState
//                , Resource.Layout.ModeInTimeLayout
//                , string.Format(Resources.GetString(Resource.String.modeInTime_pageTitle))
//                , null);

//            GetObjectsFromView();
//            SetDataToView();
//        }

//        /// <summary>
//        /// Reprise de la page
//        ///  - Enregistre les boutons de la vue
//        /// </summary>
//        protected override void OnResume()
//        {
//            base.OnResume();
//            //App.Locator.ModeInTime.OnInTimePositionAdded += OnInTimePositionAdded;
//            //App.Locator.ModeInTime.PropertyChanged += ModeInTime_PropertyChanged;
//            SearchAddressesAutoCompleteTextView.AfterTextChanged += SearchAddressesAutoCompleteTextView_AfterTextChanged;
//            SearchAddressesAutoCompleteTextView.ItemClick += SearchAddressesAutoCompleteTextView_ItemClick;
//            SearchAddressesAutoCompleteTextView.FocusChange += SearchAddressesAutoCompleteTextView_FocusChange;
//        }

//        /// <summary>
//        /// Suspension de la page
//        ///  - Désenregistre les boutons de la vue
//        /// </summary>
//        protected override void OnPause()
//        {
//            base.OnPause();
//            //App.Locator.ModeInTime.OnInTimePositionAdded -= OnInTimePositionAdded;
//            //App.Locator.ModeInTime.PropertyChanged -= ModeInTime_PropertyChanged;
//            SearchAddressesAutoCompleteTextView.AfterTextChanged -= SearchAddressesAutoCompleteTextView_AfterTextChanged;
//            SearchAddressesAutoCompleteTextView.ItemClick -= SearchAddressesAutoCompleteTextView_ItemClick;
//            SearchAddressesAutoCompleteTextView.FocusChange -= SearchAddressesAutoCompleteTextView_FocusChange;
//            if (_mapControlManager != null) _mapControlManager.UnregisterMethodes();
//            //App.Locator.ModeInTime.IsWaitingForValidation = false;
//        }

//        #endregion

//        #region ===== ActionBar ===================================================================

//        /// <summary>
//        /// Création de l'action bar
//        /// </summary>
//        public override bool OnCreateOptionsMenu(IMenu menu)
//        {
//            MenuInflater.Inflate(Resource.Menu.ModeTrackingActionBar, menu);

//            _menuSinceToday = menu.FindItem(Resource.Id.menu_sinceToday);
//            _menuSinceYesterday = menu.FindItem(Resource.Id.menu_sinceYesterday);
//            _menuSinceLastWeek = menu.FindItem(Resource.Id.menu_sinceLastWeek);
//            _menuSinceLastMonth = menu.FindItem(Resource.Id.menu_sinceLastMonth);

//            //if (App.Locator.ModeInTime.IsWaitingForValidation)
//            //{
//            //    //we display save button
//            //    menu.FindItem(Resource.Id.menu_save_modeTracking).SetVisible(true);
//            //    //and we hide the other part
//            //    _menuSinceToday.SetVisible(false);
//            //    _menuSinceYesterday.SetVisible(false);
//            //    _menuSinceLastWeek.SetVisible(false);
//            //    _menuSinceLastMonth.SetVisible(false);
//            //}
//            //else
//            //{
//            //    menu.FindItem(Resource.Id.menu_save_modeTracking).SetVisible(false);
//            //    checkUselessMenuItem();
//            //}

//            return base.OnCreateOptionsMenu(menu);
//        }

//        /// <summary>
//        /// Sélection d'un bouton de l'action bar
//        /// </summary>
//        public override bool OnOptionsItemSelected(IMenuItem item)
//        {
//            switch (item.ItemId)
//            {
//                // bouton retour
//                case Resource.Id.home:
//                    Finish();
//                    break;
//                // bouton save
//                case Resource.Id.menu_save_modeTracking:
//                    OnSaveMode();
//                    break;
//                // bouton today
//                //case Resource.Id.menu_sinceToday:
//                //    App.Locator.ModeInTime.LocationSince = Enum.AmountOfTime.Today;
//                //    break;
//                //// bouton yesterday
//                //case Resource.Id.menu_sinceYesterday:
//                //    App.Locator.ModeInTime.LocationSince = Enum.AmountOfTime.FromYesterday;
//                //    break;
//                //// bouton last week
//                //case Resource.Id.menu_sinceLastWeek:
//                //    App.Locator.ModeInTime.LocationSince = Enum.AmountOfTime.FromAWeek;
//                //    break;
//                //// bouton last month
//                //case Resource.Id.menu_sinceLastMonth:
//                //    App.Locator.ModeInTime.LocationSince = Enum.AmountOfTime.FromAMonth;
//                //    break;
//                //default:
//                //    Finish();
//                //    break;
//            }
//            return true;
//        }

//        #endregion

//        #region ===== Initialisation Vue ==========================================================

//        /// <summary>
//        /// Récupère les objets de la vue
//        /// </summary>
//        private void GetObjectsFromView()
//        {
//            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
//            SearchAddressesAutoCompleteTextView = FindViewById<AutoCompleteTextView>(Resource.Id.inTime_autoCompleteTextView);
//            SearchLayout = FindViewById<LinearLayout>(Resource.Id.autoComp_layout);
//            //if (!App.Locator.ModeInTime.IsWaitingForValidation)
//            //    SearchLayout.Visibility = ViewStates.Gone;
//            //else SearchLayout.Visibility = ViewStates.Visible;
//        }

//        /// <summary>
//        /// Initialise les objets de la vue avec les données
//        /// </summary>
//        private void SetDataToView()
//        {
//            SearchAddressesAutoCompleteTextView.SetTextColor(Resources.GetColor(Resource.Color.textColorContent));
//        }

//        #endregion

//        #region ===== Méthodes privées ============================================================

//        /// <summary>
//        /// Désactive les éléments du menu innutiles
//        /// </summary>
//        private void checkUselessMenuItem()
//        {
//            //if (_menuSinceToday == null || _menuSinceYesterday == null || _menuSinceLastWeek == null || _menuSinceLastMonth == null) return;

//            //var lsUselessAmoutOfTime = App.Locator.ModeInTime.GetUselessAmountOfTime();
//            //if (lsUselessAmoutOfTime.Contains(Enum.AmountOfTime.Today)) _menuSinceToday.SetEnabled(false);
//            //if (lsUselessAmoutOfTime.Contains(Enum.AmountOfTime.FromYesterday)) _menuSinceYesterday.SetEnabled(false);
//            //if (lsUselessAmoutOfTime.Contains(Enum.AmountOfTime.FromAWeek)) _menuSinceLastWeek.SetEnabled(false);
//            //if (lsUselessAmoutOfTime.Contains(Enum.AmountOfTime.FromAMonth)) _menuSinceLastMonth.SetEnabled(false);
//        }

//        #endregion

//        #region ===== Handlers ====================================================================

//        /// <summary>
//        /// Sauvegarde du mode zone 
//        /// </summary>
//        private async void OnSaveMode()
//        {
//            LoadingLayout.Visibility = ViewStates.Visible;
//            //var result = await App.Locator.ModeInTime.SaveMode();
//            LoadingLayout.Visibility = ViewStates.Gone;
//            //if (result)
//            //    Finish();
//        }

//        /// <summary>
//        /// Déclanché à la fin de l'ajout d'une nouvelle position de tracking
//        /// </summary>
//        private void OnInTimePositionAdded(double lat, double lon, double accuracy)
//        {
//            _mapControlManager.ChangeMarkerLocation(MapViewModelBase.Seekios.Idseekios.ToString(), lat, lon, accuracy);
//            checkUselessMenuItem();
//        }


//        private void SearchAddressesAutoCompleteTextView_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
//        {
//            _timer = new System.Timers.Timer();
//            //Trigger event every 1 seconds
//            _timer.Interval = 1500;
//            _timer.Elapsed += OnTimedEvent;
//            //count down 2 seconds : un seul tick d'une seconde
//            _countSeconds = 2;

//            _timer.Enabled = true;
//            _timer.AutoReset = false;

//        }

//        private void SearchAddressesAutoCompleteTextView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
//        {
//            var address = AddressList[e.Position];
//            LatitudeLongitude addressLocation = null;
//            //if (address != null)
//            //    addressLocation = App.Locator.ModeInTime.GetLatLongFromAddress(address);
//            //if(addressLocation != null)
//            //{
//            //    App.Locator.ModeInTime.GoToLocation(addressLocation, address);
//            //    App.Locator.ModeInTime.PositionChosenOnMap = addressLocation;
//            //}

//            //Clear all elements
//            SearchAddressesAutoCompleteTextView.Text = string.Empty;
//            AddressList.Clear();
//            SearchAddressesAutoCompleteTextView.DismissDropDown();
//            //Hide Keyboard
//            Android.Views.InputMethods.InputMethodManager imm = (Android.Views.InputMethods.InputMethodManager)GetSystemService(InputMethodService);
//            imm.HideSoftInputFromWindow(SearchAddressesAutoCompleteTextView.WindowToken, 0);
//        }

//        /// <summary>
//        /// Method triggered on focus change
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void SearchAddressesAutoCompleteTextView_FocusChange(object sender, Android.Views.View.FocusChangeEventArgs e)
//        {
//            if (!SearchAddressesAutoCompleteTextView.HasFocus)
//            {
//                //Hide Keyboard
//                Android.Views.InputMethods.InputMethodManager imm = (Android.Views.InputMethods.InputMethodManager)GetSystemService(InputMethodService);
//                imm.HideSoftInputFromWindow(SearchAddressesAutoCompleteTextView.WindowToken, 0);
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void ModeInTime_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
//        {
//            //if (e.PropertyName == "AddressGet")
//            //    App.Locator.ModeInTime.GoToLocation(App.Locator.ModeInTime.PositionChosenOnMap, App.Locator.ModeInTime.PositionChosenOnMapTitle);
//        }

//        #endregion

//        #region ===== Callback ====================================================================

//        /// <summary>
//        /// Callback d'initialisation de la map
//        /// </summary>
//        public override void OnMapReady(GoogleMap googleMap)
//        {
//            if (googleMap != null)
//            {
//                this.MapWrapperLayout.Init(googleMap);

//                // initialisation de la map
//                _mapControlManager = new MapControlManager(googleMap
//                    , BaseContext
//                    , CenterSeekiosSvgImageView
//                    , MapViewModelBase.Seekios.Idseekios.ToString());
//                //Ajout d'un tracé pour le parcours du seekios
//                //App.Locator.ModeInTime.MapControlManager = _mapControlManager;
//                //App.Locator.ModeInTime.InitMap();

//                //if (!App.Locator.ModeInTime.IsWaitingForValidation)
//                //{
//                //    App.Locator.ModeInTime.InitTrackingRoute();
//                //    App.Locator.ModeInTime.SelectedLocation = App.Locator.ModeInTime.LsLocations.LastOrDefault();
//                //}
                               
//                _mapControlManager.RegisterMethodes();
//            }
//            base.OnMapReady(googleMap);
//            //if(App.Locator.ModeInTime.IsWaitingForValidation)
//            //    googleMap.SetPadding(0, AccessResources.Instance.SizeOf110Dip, 0, AccessResources.Instance.SizeOf80Dip);
//        }

//        /// <summary>
//        /// Event fired every 1.5 seconds when entering text in the AutoCompleteTextView
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
//        {
//            //while (_countSeconds > 0)
//            //{
//            //    _countSeconds--;
//            //    if (_countSeconds < 1)
//            //    {
//            //        //Update visual representation here
//            //        //Remember to do it on UI thread
//            //        RunOnUiThread(async () =>
//            //        {
//            //            var text = SearchAddressesAutoCompleteTextView.Text;
//            //            if (text.Length > 6)
//            //            {
//            //                AddressList = await App.Locator.ModeInTime.GetAddressList(text);
//            //                if (AddressList != null)
//            //                {
//            //                    SearchAddressesAutoCompleteTextView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, AddressList);
//            //                    SearchAddressesAutoCompleteTextView.ShowDropDown();
//            //                }
//            //                else return;
//            //            }
//            //        });
//            //    }
//            //}
//            //_timer.Stop();
//        }


//        #endregion
//    }
//}