//using System;
//using Android.App;
//using Android.OS;
//using Android.Views;
//using Android.Widget;
//using SeekiosApp.ViewModel;
//using SeekiosApp.Droid.View.FragmentView;
//using Android.Gms.Maps;
//using SeekiosApp.Droid.ControlManager;
//using System.Threading;

//namespace SeekiosApp.Droid.View
//{
//    [Activity(Theme = "@style/Theme.Normal")]
//    public class ModeFollowMeActivity : MapBaseActivity
//    {
//        #region ===== Attributs ===================================================================

//        /// <summary></summary>
//        private bool _firstCenterOnUserLocation = false;

//        /// <summary>Bouton sauvegarder</summary>
//        private IMenuItem _saveButton = null;

//        #endregion

//        #region ===== Properties ==================================================================

//        /// <summary>Layout de chargement propre au follow me</summary>
//        public RelativeLayout BLEConnexionloadingRelativeLayout { get; set; }

//        public ProgressBar BLEConnexionLoadingStartBLELoading { get; set; }
//        public XamSvg.SvgImageView BLEConnexionLoadingStartBLEOk { get; set; }
//        public XamSvg.SvgImageView BLEConnexionLoadingStartBLEPb { get; set; }

//        public ProgressBar BLEConnexionLoadingSendMsgLoading { get; set; }
//        public XamSvg.SvgImageView BLEConnexionLoadingSendMsgOk { get; set; }
//        public XamSvg.SvgImageView BLEConnexionLoadingSendMsgPb { get; set; }

//        public ProgressBar BLEConnexionLoadingLookForSeekiosLoading { get; set; }
//        public XamSvg.SvgImageView BLEConnexionLoadingLookForSeekiosOk { get; set; }
//        public XamSvg.SvgImageView BLEConnexionLoadingLookForSeekiosPb { get; set; }

//        public ProgressBar BLEConnexionLoadingConnectWithSeekiosLoading { get; set; }
//        public XamSvg.SvgImageView BLEConnexionLoadingConnectWithSeekiosOk { get; set; }
//        public XamSvg.SvgImageView BLEConnexionLoadingConnectWithSeekiosPb { get; set; }

//        public ProgressBar BLEConnexionLoadingSeekiosConnectedLayoutLoading { get; set; }
//        public XamSvg.SvgImageView BLEConnexionLoadingSeekiosConnectedLayoutOk { get; set; }
//        public XamSvg.SvgImageView BLEConnexionLoadingSeekiosConnectedLayoutPb { get; set; }

//        #endregion

//        #region ===== Cycle De Vie ================================================================

//        /// <summary>
//        /// Création de la page
//        /// </summary>
//        protected override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState
//                , Resource.Layout.ModeFollowMeLayout
//                , string.Format(Resources.GetString(Resource.String.modeFollowMe_titlePage))
//                , null);

//            GetObjectsFromView();
//            SetDataToView();
//        }

//        /// <summary>
//        /// Reprise de la page
//        /// </summary>
//        protected override void OnResume()
//        {
//            base.OnResume();
//            App.Locator.ModeFollowMe.PropertyChanged += OnModeFollowMePropertyChanged;
//            App.Locator.ModeFollowMe.NewFollowMeTrackingLocationAddedNotified += OnNewFollowMeTrackingLocationAddedNotified;
//        }

//        /// <summary>
//        /// Suspension de la page
//        /// </summary>
//        protected override void OnPause()
//        {
//            base.OnPause();
//            App.Locator.ModeFollowMe.PropertyChanged -= OnModeFollowMePropertyChanged;
//            App.Locator.ModeFollowMe.NewFollowMeTrackingLocationAddedNotified -= OnNewFollowMeTrackingLocationAddedNotified;
//        }

//        #endregion

//        #region ===== ActionBar ===================================================================

//        /// <summary>
//        /// Création de l'action bar
//        /// </summary>
//        public override bool OnCreateOptionsMenu(IMenu menu)
//        {
//            MenuInflater.Inflate(Resource.Menu.ModeFollowMeActionBar, menu);

//            _saveButton = menu.FindItem(Resource.Id.menu_save_modeFollowMe);
//            _saveButton.SetVisible(App.Locator.ModeFollowMe.IsWaitingForValidation);

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
//                // bouton retour
//                case Resource.Id.menu_save_modeFollowMe:
//                    OnSaveMode();
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
//            BLEConnexionloadingRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.BLEConnexionloadingPanel);

//            BLEConnexionLoadingStartBLELoading = FindViewById<ProgressBar>(Resource.Id.BLEConnexionLoading_startBLELoading);
//            BLEConnexionLoadingStartBLEOk = FindViewById<XamSvg.SvgImageView>(Resource.Id.BLEConnexionLoading_startBLEOk);
//            BLEConnexionLoadingStartBLEPb = FindViewById<XamSvg.SvgImageView>(Resource.Id.BLEConnexionLoading_startBLEPb);

//            BLEConnexionLoadingSendMsgLoading = FindViewById<ProgressBar>(Resource.Id.BLEConnexionLoading_sendMsgLoading);
//            BLEConnexionLoadingSendMsgOk = FindViewById<XamSvg.SvgImageView>(Resource.Id.BLEConnexionLoading_sendMsgOk);
//            BLEConnexionLoadingSendMsgPb = FindViewById<XamSvg.SvgImageView>(Resource.Id.BLEConnexionLoading_sendMsgPb);

//            BLEConnexionLoadingLookForSeekiosLoading = FindViewById<ProgressBar>(Resource.Id.BLEConnexionLoading_lookForSeekiosLoading);
//            BLEConnexionLoadingLookForSeekiosOk = FindViewById<XamSvg.SvgImageView>(Resource.Id.BLEConnexionLoading_lookForSeekiosOk);
//            BLEConnexionLoadingLookForSeekiosPb = FindViewById<XamSvg.SvgImageView>(Resource.Id.BLEConnexionLoading_lookForSeekiosPb);

//            BLEConnexionLoadingConnectWithSeekiosLoading = FindViewById<ProgressBar>(Resource.Id.BLEConnexionLoading_connectWithSeekiosLoading);
//            BLEConnexionLoadingConnectWithSeekiosOk = FindViewById<XamSvg.SvgImageView>(Resource.Id.BLEConnexionLoading_connectWithSeekiosOk);
//            BLEConnexionLoadingConnectWithSeekiosPb = FindViewById<XamSvg.SvgImageView>(Resource.Id.BLEConnexionLoading_connectWithSeekiosPb);

//            BLEConnexionLoadingSeekiosConnectedLayoutLoading = FindViewById<ProgressBar>(Resource.Id.BLEConnexionLoading_seekiosConnectedLoading);
//            BLEConnexionLoadingSeekiosConnectedLayoutOk = FindViewById<XamSvg.SvgImageView>(Resource.Id.BLEConnexionLoading_seekiosConnectedOk);
//            BLEConnexionLoadingSeekiosConnectedLayoutPb = FindViewById<XamSvg.SvgImageView>(Resource.Id.BLEConnexionLoading_seekiosConnectedPb);
//        }

//        /// <summary>
//        /// Initialise les objets de la vue avec les données
//        /// </summary>
//        private void SetDataToView()
//        {
//            App.Locator.ModeFollowMe.InitModeFollowMe();
//            //Si un tracking est lancé
//            if (MapViewModelBase.Mode != null
//                && MapViewModelBase.Mode.StatusDefinition_idstatusDefinition > 1
//                && App.Locator.ModeFollowMe.IsTrackingAfterBLEConnexionLoss
//                && !App.Locator.ModeFollowMe.IsWaitingForValidation)
//            {
//                StartOrRestartChrono();
//            }

//            //if (App.Locator.ModeFollowMe.IsWaitingForValidation)
//            //{
//            //    var openBottomPanelThread = new Thread(new ThreadStart(() =>
//            //    {
//            //        Thread.Sleep(2000);
//            //        RunOnUiThread(() => SlidingUpLayout.ExpandPane(SlidingUpLayout.AnchorPoint));
//            //    }));
//            //    openBottomPanelThread.Start();
//            //}
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        private void InitBLEConnectionLoadingPanel()
//        {
//            BLEConnexionLoadingStartBLELoading.Visibility = ViewStates.Visible;
//            BLEConnexionLoadingStartBLEOk.Visibility = ViewStates.Gone;
//            BLEConnexionLoadingStartBLEPb.Visibility = ViewStates.Gone;

//            BLEConnexionLoadingSendMsgLoading.Visibility = ViewStates.Invisible;
//            BLEConnexionLoadingSendMsgOk.Visibility = ViewStates.Gone;
//            BLEConnexionLoadingSendMsgPb.Visibility = ViewStates.Gone;

//            BLEConnexionLoadingLookForSeekiosLoading.Visibility = ViewStates.Invisible;
//            BLEConnexionLoadingLookForSeekiosOk.Visibility = ViewStates.Gone;
//            BLEConnexionLoadingLookForSeekiosPb.Visibility = ViewStates.Gone;

//            BLEConnexionLoadingConnectWithSeekiosLoading.Visibility = ViewStates.Invisible;
//            BLEConnexionLoadingConnectWithSeekiosOk.Visibility = ViewStates.Gone;
//            BLEConnexionLoadingConnectWithSeekiosPb.Visibility = ViewStates.Gone;

//            BLEConnexionLoadingSeekiosConnectedLayoutLoading.Visibility = ViewStates.Invisible;
//            BLEConnexionLoadingSeekiosConnectedLayoutOk.Visibility = ViewStates.Gone;
//            BLEConnexionLoadingSeekiosConnectedLayoutPb.Visibility = ViewStates.Gone;
//        }

//        #endregion

//        #region ===== Handlers ====================================================================

//        /// <summary>
//        /// Sauvegarde du mode 
//        /// </summary>
//        private async void OnSaveMode()
//        {
//            //Check if there is a follow me mode already in use with this device
//            if (App.Locator.ModeFollowMe.CheckIfFollowMeModeAlreadyInUse()) return;

//            RunOnUiThread(() =>
//            {
//                InitBLEConnectionLoadingPanel();

//                BLEConnexionloadingRelativeLayout.Visibility = ViewStates.Visible;
//            });

//            var isBleActivate = await App.Locator.ModeFollowMe.CheckIfBLEIsEnable();
//            if (isBleActivate)
//            {
//                RunOnUiThread(() =>
//                {
//                    BLEConnexionLoadingStartBLELoading.Visibility = ViewStates.Gone;
//                    BLEConnexionLoadingStartBLEOk.Visibility = ViewStates.Visible;
//                    BLEConnexionLoadingStartBLEPb.Visibility = ViewStates.Gone;

//                    BLEConnexionLoadingSendMsgLoading.Visibility = ViewStates.Visible;
//                    BLEConnexionLoadingSendMsgOk.Visibility = ViewStates.Gone;
//                    BLEConnexionLoadingSendMsgPb.Visibility = ViewStates.Gone;
//                });

//                var connected = await App.Locator.ModeFollowMe.SendMessageToSeekios();
//            }
//            else
//            {
//                RunOnUiThread(() =>
//                {
//                    BLEConnexionLoadingStartBLELoading.Visibility = ViewStates.Gone;
//                    BLEConnexionLoadingStartBLEOk.Visibility = ViewStates.Gone;
//                    BLEConnexionLoadingStartBLEPb.Visibility = ViewStates.Visible;
//                });

//                Thread.Sleep(1000);

//                RunOnUiThread(() => BLEConnexionloadingRelativeLayout.Visibility = ViewStates.Gone);
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnModeRestarted(object sender, EventArgs e)
//        {
//            StopChrono();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnUserLocationChanged(object sender, EventArgs e)
//        {
//            if (!_firstCenterOnUserLocation) return;
//            if (!_mapControlManager.CenterOnMyLocation(true)) return;
//            _firstCenterOnUserLocation = false;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnModeFollowMePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
//        {
//            if (e.PropertyName.Equals("BLEConnexionState"))
//            {
//                if (App.Locator.ModeFollowMe.IsTrackingAfterBLEConnexionLoss
//                    && !App.Locator.ModeFollowMe.IsWaitingForValidation
//                    && App.Locator.ModeFollowMe.BLEConnexionState == Interfaces.ConnexionState.Disconnected)
//                    RunOnUiThread(() => StartOrRestartChrono());
//                switch (App.Locator.ModeFollowMe.BLEConnexionState)
//                {
//                    case Interfaces.ConnexionState.None:
//                        break;
//                    case Interfaces.ConnexionState.SeekiosUnreachable:
//                        RunOnUiThread(() =>
//                        {
//                            BLEConnexionLoadingLookForSeekiosLoading.Visibility = ViewStates.Gone;
//                            BLEConnexionLoadingLookForSeekiosOk.Visibility = ViewStates.Gone;
//                            BLEConnexionLoadingLookForSeekiosPb.Visibility = ViewStates.Visible;
//                        });
//                        new Thread(new ThreadStart(() =>
//                        {
//                            Thread.Sleep(2000);
//                            RunOnUiThread(() =>
//                            {
//                                BLEConnexionloadingRelativeLayout.Visibility = ViewStates.Gone;
//                            });
//                        })).Start();
//                        break;
//                    case Interfaces.ConnexionState.MessageReceivedBySeekios:
//                        RunOnUiThread(() =>
//                        {
//                            BLEConnexionLoadingSendMsgLoading.Visibility = ViewStates.Gone;
//                            BLEConnexionLoadingSendMsgOk.Visibility = ViewStates.Visible;
//                            BLEConnexionLoadingSendMsgPb.Visibility = ViewStates.Gone;
//                        });
//                        break;
//                    case Interfaces.ConnexionState.MessageNotReceivedBySeekios:
//                        RunOnUiThread(() =>
//                        {
//                            BLEConnexionLoadingSendMsgLoading.Visibility = ViewStates.Gone;
//                            BLEConnexionLoadingSendMsgOk.Visibility = ViewStates.Gone;
//                            BLEConnexionLoadingSendMsgPb.Visibility = ViewStates.Visible;
//                        });
//                        new Thread(new ThreadStart(() =>
//                        {
//                            Thread.Sleep(2000);
//                            RunOnUiThread(() =>
//                            {
//                                BLEConnexionloadingRelativeLayout.Visibility = ViewStates.Gone;
//                            });
//                        })).Start();
//                        break;
//                    case Interfaces.ConnexionState.LookingForSeekios:
//                        RunOnUiThread(() =>
//                        {
//                            BLEConnexionLoadingLookForSeekiosLoading.Visibility = ViewStates.Visible;
//                            BLEConnexionLoadingLookForSeekiosOk.Visibility = ViewStates.Gone;
//                            BLEConnexionLoadingLookForSeekiosPb.Visibility = ViewStates.Gone;
//                        });
//                        break;
//                    case Interfaces.ConnexionState.Disconnecting:
//                        break;
//                    case Interfaces.ConnexionState.Disconnected:
//                        break;
//                    case Interfaces.ConnexionState.Connecting:
//                        RunOnUiThread(() =>
//                        {
//                            BLEConnexionLoadingLookForSeekiosLoading.Visibility = ViewStates.Gone;
//                            BLEConnexionLoadingLookForSeekiosOk.Visibility = ViewStates.Visible;
//                            BLEConnexionLoadingLookForSeekiosPb.Visibility = ViewStates.Gone;

//                            BLEConnexionLoadingConnectWithSeekiosLoading.Visibility = ViewStates.Visible;
//                            BLEConnexionLoadingConnectWithSeekiosOk.Visibility = ViewStates.Gone;
//                            BLEConnexionLoadingConnectWithSeekiosPb.Visibility = ViewStates.Gone;
//                        });
//                        break;
//                    case Interfaces.ConnexionState.ConnectionFailed:
//                        new Thread(new ThreadStart(() =>
//                        {
//                            RunOnUiThread(() =>
//                            {
//                                BLEConnexionLoadingLookForSeekiosLoading.Visibility = ViewStates.Gone;
//                                BLEConnexionLoadingLookForSeekiosOk.Visibility = ViewStates.Visible;
//                                BLEConnexionLoadingLookForSeekiosPb.Visibility = ViewStates.Gone;

//                                BLEConnexionLoadingConnectWithSeekiosLoading.Visibility = ViewStates.Gone;
//                                BLEConnexionLoadingConnectWithSeekiosOk.Visibility = ViewStates.Gone;
//                                BLEConnexionLoadingConnectWithSeekiosPb.Visibility = ViewStates.Visible;
//                            });
//                            Thread.Sleep(1000);
//                            RunOnUiThread(() =>
//                            {
//                                BLEConnexionloadingRelativeLayout.Visibility = ViewStates.Gone;
//                            });
//                        })).Start();
//                        break;
//                    case Interfaces.ConnexionState.Connected:
//                        new Thread(new ThreadStart(() =>
//                        {
//                            RunOnUiThread(() =>
//                            {
//                                BLEConnexionLoadingConnectWithSeekiosLoading.Visibility = ViewStates.Gone;
//                                BLEConnexionLoadingConnectWithSeekiosOk.Visibility = ViewStates.Visible;
//                                BLEConnexionLoadingConnectWithSeekiosPb.Visibility = ViewStates.Gone;

//                                BLEConnexionLoadingSeekiosConnectedLayoutLoading.Visibility = ViewStates.Gone;
//                                BLEConnexionLoadingSeekiosConnectedLayoutOk.Visibility = ViewStates.Visible;
//                                BLEConnexionLoadingSeekiosConnectedLayoutPb.Visibility = ViewStates.Gone;
//                            });
//                            Thread.Sleep(1000);
//                            RunOnUiThread(() =>
//                            {
//                                BLEConnexionloadingRelativeLayout.Visibility = ViewStates.Gone;
//                            });
//                        })).Start();
//                        break;
//                    default:
//                        break;
//                }
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="lat"></param>
//        /// <param name="lon"></param>
//        /// <param name="accuracy"></param>
//        private void OnNewFollowMeTrackingLocationAddedNotified(double lat, double lon, double alt, double accuracy, DateTime dateCommunication)
//        {
//            if (_mapControlManager != null && MapViewModelBase.Seekios != null)
//            {
//                _mapControlManager.ChangeMarkerLocation(MapViewModelBase.Seekios.Idseekios.ToString(), lat, lon, accuracy);
//                _mapControlManager.CenterInMarker(MapViewModelBase.Seekios.Idseekios.ToString());
//            }
//            if (!App.Locator.ModeFollowMe.IsWaitingForValidation)
//                StartOrRestartChrono();
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
//                App.Locator.ModeFollowMe.MapControlManager = _mapControlManager;
//                App.Locator.ModeFollowMe.InitMap();
//                _mapControlManager.UserLocationChanged += OnUserLocationChanged;

//                _mapControlManager.RegisterMethodes();
//                _firstCenterOnUserLocation = true;
//            }
//            base.OnMapReady(googleMap);
//        }

//        #endregion
//    }
//}