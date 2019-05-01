//using System;
//using System.Linq;
//using Android.App;
//using Android.OS;
//using Android.Views;
//using Android.Widget;
//using SeekiosApp.Droid.View.FragmentView;
//using SeekiosApp.ViewModel;
//using SeekiosApp.Droid.ControlManager;
//using Android.Gms.Maps;

//namespace SeekiosApp.Droid.View
//{
//    [Activity(Label = "ModeDailyTrackActivity")]
//    public class ModeDailyTrackActivity : MapBaseActivity
//    {
//        #region ===== Attributs ===================================================================

//        private IMenuItem _menuSinceToday;
//        private IMenuItem _menuSinceYesterday;
//        private IMenuItem _menuSinceLastWeek;
//        private IMenuItem _menuSinceLastMonth;

//        #endregion

//        #region ===== Cycle De Vie ================================================================

//        /// <summary>
//        /// Création de la page
//        /// </summary>
//        protected override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState
//                , Resource.Layout.ModeDailyTrackLayout
//                , string.Format(Resources.GetString(Resource.String.modeDailyTrack_pageTitle))
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
//            App.Locator.ModeDailyTrack.OnDailyTrackPositionAdded += OnDailyTrackPositionAdded;
//        }

//        /// <summary>
//        /// Suspension de la page
//        ///  - Désenregistre les boutons de la vue
//        /// </summary>
//        protected override void OnPause()
//        {
//            base.OnPause();
//            App.Locator.ModeDailyTrack.OnDailyTrackPositionAdded -= OnDailyTrackPositionAdded;
//            if (_mapControlManager != null) _mapControlManager.UnregisterMethodes();
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

//            if (App.Locator.ModeDailyTrack.IsWaitingForValidation)
//            {
//                //we display save button
//                menu.FindItem(Resource.Id.menu_save_modeTracking).SetVisible(true);
//                //and we hide the other part
//                _menuSinceToday.SetVisible(false);
//                _menuSinceYesterday.SetVisible(false);
//                _menuSinceLastWeek.SetVisible(false);
//                _menuSinceLastMonth.SetVisible(false);
//            }
//            else
//            {
//                menu.FindItem(Resource.Id.menu_save_modeTracking).SetVisible(false);
//                checkUselessMenuItem();
//            }

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
//                case Resource.Id.menu_sinceToday:
//                    App.Locator.ModeDailyTrack.LocationSince = Enum.AmountOfTime.Today;
//                    break;
//                // bouton yesterday
//                case Resource.Id.menu_sinceYesterday:
//                    App.Locator.ModeDailyTrack.LocationSince = Enum.AmountOfTime.FromYesterday;
//                    break;
//                // bouton last week
//                case Resource.Id.menu_sinceLastWeek:
//                    App.Locator.ModeDailyTrack.LocationSince = Enum.AmountOfTime.FromAWeek;
//                    break;
//                // bouton last month
//                case Resource.Id.menu_sinceLastMonth:
//                    App.Locator.ModeDailyTrack.LocationSince = Enum.AmountOfTime.FromAMonth;
//                    break;
//                default:
//                    Finish();
//                    break;
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
//        }

//        /// <summary>
//        /// Initialise les objets de la vue avec les données
//        /// </summary>
//        private void SetDataToView()
//        {
//            //Si le seekios est en dehors de la zone et qu'un tracking est configuré
//            //Init mode uniquement si on reconfigure le mode daily track après un daily track?
//            App.Locator.ModeDailyTrack.InitMode();
//        }

//        #endregion

//        #region ===== Méthodes privées ============================================================

//        /// <summary>
//        /// Désactive les éléments du menu innutiles
//        /// </summary>
//        private void checkUselessMenuItem()
//        {
//            if (_menuSinceToday == null || _menuSinceYesterday == null || _menuSinceLastWeek == null || _menuSinceLastMonth == null) return;

//            var lsUselessAmoutOfTime = App.Locator.ModeDailyTrack.GetUselessAmountOfTime();
//            if (lsUselessAmoutOfTime.Contains(Enum.AmountOfTime.Today)) _menuSinceToday.SetEnabled(false);
//            if (lsUselessAmoutOfTime.Contains(Enum.AmountOfTime.FromYesterday)) _menuSinceYesterday.SetEnabled(false);
//            if (lsUselessAmoutOfTime.Contains(Enum.AmountOfTime.FromAWeek)) _menuSinceLastWeek.SetEnabled(false);
//            if (lsUselessAmoutOfTime.Contains(Enum.AmountOfTime.FromAMonth)) _menuSinceLastMonth.SetEnabled(false);
//        }

//        #endregion

//        #region ===== Handlers ====================================================================

//        /// <summary>
//        /// Sauvegarde du mode zone 
//        /// </summary>
//        private async void OnSaveMode()
//        {
//            LoadingLayout.Visibility = ViewStates.Visible;
//            var result = await App.Locator.ModeDailyTrack.SaveMode();
//            LoadingLayout.Visibility = ViewStates.Gone;
//            if (result)
//                Finish();
//        }

//        /// <summary>
//        /// Déclanché à la fin de l'ajout d'une nouvelle position de tracking
//        /// </summary>
//        private void OnDailyTrackPositionAdded(double lat, double lon, double alt, double accuracy, DateTime dateCommunication)
//        {
//            _mapControlManager.ChangeMarkerLocation(MapViewModelBase.Seekios.Idseekios.ToString(), lat, lon, accuracy);
//            checkUselessMenuItem();
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
//                App.Locator.ModeDailyTrack.MapControlManager = _mapControlManager;
//                App.Locator.ModeDailyTrack.InitMap();

//                if (!App.Locator.ModeDailyTrack.IsWaitingForValidation)
//                {
//                    App.Locator.ModeDailyTrack.InitTrackingRoute();
//                    App.Locator.ModeDailyTrack.SelectedLocation = App.Locator.ModeDailyTrack.LsLocations.LastOrDefault();
//                }

//                _mapControlManager.RegisterMethodes();
//            }
//            base.OnMapReady(googleMap);
//        }

//        #endregion
//    }
//}