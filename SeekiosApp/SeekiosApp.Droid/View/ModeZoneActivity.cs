using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using SeekiosApp.Droid.Helper;
using SeekiosApp.Droid.CustomComponents;
using System.Collections.ObjectModel;
using System.Globalization;
using SeekiosApp.Model.DTO;
using SeekiosApp.Droid.ControlManager;
using SeekiosApp.Extension;
using SeekiosApp.ViewModel;
using SeekiosApp.Droid.View.FragmentView;
using Android.Hardware;
using SeekiosApp.Model.APP;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class ModeZoneActivity : MapBaseActivity
    {
        #region ===== Attributs ===================================================================

        private int _countOfUpdatesOnFavoriteAreaSelection = 0;
        private ObservableCollection<Action> _undoActions = new ObservableCollection<Action>();
        //private FavoriteAreaAdapter _modeZoneFavoritesAdapter = null;
        private AlertDialog.Builder _addModeZoneFavorisAlertDialog = null;
        private Dialog _addModeZoneFavorisDialog = null;

        #endregion

        #region ===== Propriétés ==================================================================

        public XamSvg.SvgImageView AddPointSvgImageView { get; set; }

        public XamSvg.SvgImageView UndoSvgImageView { get; set; }

        public RelativeLayout ModeZoneMapLayout { get; set; }

        public XamSvg.SvgImageView ShowZoneSvgImageView { get; set; }

        //public TextView CountOfPointsTextView { get; set; }

        //public TextView SurfaceTextView { get; set; }

        public TextView Instructions { get; set; }

        public TextView TutoTextView { get; set; }

        public TextView NextButton { get; set; }

        public RelativeLayout BottomLayout { get; set; }

        //public GridLayout BottomInformationsLayout { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        /// <summary>
        /// Création de la page
        /// </summary>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState
                , Resource.Layout.ModeZoneLayout
                , string.Format(Resources.GetString(Resource.String.modeZone_titlePage), "1/3")
                , null);

            GetObjectsFromView();
            SetDataToView();
        }

        protected override void OnDestroy()
        {
            _mapControlManager.Dispose();
            base.OnDestroy();
        }

        /// <summary>
        /// Reprise de la page
        ///  - Enregistre les boutons de la vue
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            if (App.Locator.ModeZone.IsGoingBack)
            {
                App.Locator.ModeZone.IsGoingBack = false;
                Finish();
            }
            if (_mapControlManager != null) _mapControlManager.ZoneInformationUpdated += OnInformationZoneRefresh;
            App.Locator.ModeZone.PropertyChanged += OnModeZonePropertyChanged;
            App.Locator.ModeZone.OnSeekiosBackInZoneNotified += OnSeekiosBackInZone;
            App.Locator.ModeZone.OnNewZoneTrackingLocationAddedNotified += OnNewZoneTrackingLocationAdded;
            ShowZoneSvgImageView.Click += OnZoneSeekiosSvgImageViewClick;
            NextButton.Click += OnClickNextPage;
            AddPointSvgImageView.Click += OnAddPointSvgImageViewClick;
        }

        /// <summary>
        /// Suspension de la page
        ///  - Désenregistre les boutons de la vue
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            if (_mapControlManager != null) _mapControlManager.ZoneInformationUpdated -= OnInformationZoneRefresh;
            App.Locator.ModeZone.PropertyChanged -= OnModeZonePropertyChanged;
            App.Locator.ModeZone.OnSeekiosBackInZoneNotified -= OnSeekiosBackInZone;
            App.Locator.ModeZone.OnNewZoneTrackingLocationAddedNotified -= OnNewZoneTrackingLocationAdded;
            ShowZoneSvgImageView.Click -= OnZoneSeekiosSvgImageViewClick;
            NextButton.Click -= OnClickNextPage;
            AddPointSvgImageView.Click -= OnAddPointSvgImageViewClick;
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            App.Locator.ModeZone.IsOnEditMode = false;
            App.Locator.ModeZone.IsActivityFocused = false;
        }

        #endregion

        #region ===== ActionBar ===================================================================

        /// <summary>
        /// Création de l'action bar
        /// </summary>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }

        /// <summary>
        /// Sélection d'un bouton de l'action bar
        /// </summary>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            App.Locator.ModeZone.IsActivityFocused = false;
            App.Locator.ModeZone.IsOnEditMode = false;
            Finish();
            return true;
        }

        #endregion

        #region ===== Initialize View =============================================================

        /// <summary>
        /// Get objects from view
        /// </summary>
        private void GetObjectsFromView()
        {
            AddPointSvgImageView = FindViewById<XamSvg.SvgImageView>(Resource.Id.modeZone_addPoint);
            UndoSvgImageView = FindViewById<XamSvg.SvgImageView>(Resource.Id.modeZone_undo);
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
            ModeZoneMapLayout = FindViewById<RelativeLayout>(Resource.Id.modeZone_map);
            ShowZoneSvgImageView = FindViewById<XamSvg.SvgImageView>(Resource.Id.modeZone_showZone);
            //CountOfPointsTextView = FindViewById<TextView>(Resource.Id.modeZone_countOfPoints);
            //SurfaceTextView = FindViewById<TextView>(Resource.Id.modeZone_surface);
            //BottomInformationsLayout = FindViewById<GridLayout>(Resource.Id.modeZone_bottomGridView);
            Instructions = FindViewById<TextView>(Resource.Id.modeZone_instructions);
            TutoTextView = FindViewById<TextView>(Resource.Id.modeZone_tuto);
            NextButton = FindViewById<TextView>(Resource.Id.modeZone_suivant);
            BottomLayout = FindViewById<RelativeLayout>(Resource.Id.mapZoneBottom_layout);
        }

        /// <summary>
        /// Initialize the view with the data
        /// </summary>
        private void SetDataToView()
        {
            //disable next button by default
            NextButton.Enabled = false;
            // read map
            if (!App.Locator.ModeZone.IsOnEditMode)
            {
                AddPointSvgImageView.Visibility = ViewStates.Gone;
                UndoSvgImageView.Visibility = ViewStates.Gone;
                MapViewModelBase.RefreshTime = 0;
            }
            // edit map
            else
            {
                UndoSvgImageView.SetSvg(this, Resource.Drawable.ModeZoneGoBack, "#36da3e=#999999");    // TODO : centraliser dans un fichier ressource
                //BottomLayout.Visibility = ViewStates.Gone;
                if (CreditLayout != null) CreditLayout.Visibility = ViewStates.Gone;
                AddPointSvgImageView.Visibility = ViewStates.Invisible;
                ShowZoneSvgImageView.Visibility = ViewStates.Gone;
            }
            CenterSeekiosSvgImageView.Visibility = ViewStates.Visible;
        }

        #endregion

        #region ===== Méthodes Métier =============================================================

        #region Gestion des marker

        /// <summary>
        /// 
        /// </summary>
        private void RefreshTutoText(bool isOnPointAddingGoingToChange = false)
        {
            TutoTextView.Text = Resources.GetString(Resource.String.modeZone_tuto);
            if (!_mapControlManager.IsOnPointAdding && !isOnPointAddingGoingToChange
                || _mapControlManager.IsOnPointAdding && isOnPointAddingGoingToChange)
                TutoTextView.Text = Resources.GetString(Resource.String.modeZone_tutoReturnToEditMode);
            else if (_mapControlManager.ZonePolygon != null)
            {
                if (_mapControlManager.ZonePolygon.Points.Count >= 1
                    && _mapControlManager.ZonePolygon.Points.Count <= 3)
                    TutoTextView.Text = Resources.GetString(Resource.String.modeZone_tutoAtLeast3Points);
                else if (_mapControlManager.ZonePolygon.Points.Count > 3)
                    TutoTextView.Text = Resources.GetString(Resource.String.modeZone_tutoAdviceMovePoints);
                if (_mapControlManager.ZonePolygon.Points.Count >= 11)
                    TutoTextView.Text = Resources.GetString(Resource.String.modeZone_tutoMax10Points);
            }
            ShowZoneSvgImageView.Visibility = _mapControlManager.ZonePolygon != null && _mapControlManager.ZonePolygon.Points.Count >= 1
                ? ViewStates.Visible : ViewStates.Gone;
        }

        /// <summary>
        /// Rafraichit les différentes variables affichés sur la map
        /// </summary>
        private void RefreshMarkers()
        {
            /*var metaDataFragment = _modeMetaDataFragment as ZoneMetaDataFragment;
            if (metaDataFragment == null) return;
            metaDataFragment.ZonePolygon = _mapControlManager.ZonePolygon;
            metaDataFragment.RefreshMarkers();*/
            NextButton.Enabled = _mapControlManager.ZonePolygon != null && _mapControlManager.ZonePolygon.Points.Count > 3;

            if (_mapControlManager.ZonePolygon == null || _mapControlManager.ZonePolygon.Points.Count == 0)
            {
                //CountOfPointsTextView.Text = "0 / " + App.Locator.ModeZone.MAX_NUMBER_OF_POINTS;
                //SurfaceTextView.Text = string.Format("0 {0}", Resources.GetString(Resource.String.unit_square));
                Instructions.Visibility = ViewStates.Visible;
                //BottomInformationsLayout.Visibility = ViewStates.Gone;
                //ExplanationTextView.Visibility = ViewStates.Visible;
                return;
            }
            //BottomInformationsLayout.Visibility = ViewStates.Visible;
            Instructions.Visibility = ViewStates.Gone;
            //ExplanationTextView.Visibility = ViewStates.Gone;

            //if (_mapControlManager.ZonePolygon.Points.Count == 1) CountOfPointsTextView.Text = "1 / " + App.Locator.ModeZone.MAX_NUMBER_OF_POINTS;
            //else CountOfPointsTextView.Text = (_mapControlManager.ZonePolygon.Points.Count - 1).ToString() + " / " + App.Locator.ModeZone.MAX_NUMBER_OF_POINTS;

            //var surface = AreaHelper.CalculateAreaOfGPSPolygonOnEarthInSquareMeters(_mapControlManager.ZonePolygon.Points.ToList());
            //SurfaceTextView.Text = AreaHelper.SerializeArea(surface);
        }

        /// <summary>
        /// Déclanché lorsque le seekios sort de la zone
        /// </summary>
        private void OnSeekiosBackInZone(int idSeekios, double lat, double lon, double alt, double accuracy, DateTime dateCommunication)
        {
            if (MapViewModelBase.Seekios == null || MapViewModelBase.Seekios.Idseekios == idSeekios) return;

            if (_mapControlManager != null && MapViewModelBase.Seekios != null)
            {
                _mapControlManager.ChangeMarkerLocation(MapViewModelBase.Seekios.Idseekios.ToString(), lat, lon, accuracy);
                _mapControlManager.CenterInMarker(MapViewModelBase.Seekios.Idseekios.ToString());
            }
            //StopChrono();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        private void OnNewZoneTrackingLocationAdded(int idSeekios, double lat, double lon, double alt, double accuracy, DateTime dateCommunication)
        {
            if (MapViewModelBase.Seekios == null || MapViewModelBase.Seekios.Idseekios == idSeekios) return;

            if (_mapControlManager != null && MapViewModelBase.Seekios != null)
            {
                _mapControlManager.ChangeMarkerLocation(MapViewModelBase.Seekios.Idseekios.ToString(), lat, lon, accuracy);
                _mapControlManager.CenterInMarker(MapViewModelBase.Seekios.Idseekios.ToString());
            }
            //if (App.Locator.ModeZone.LsSeekiosInTrackingAfterOOZ.Contains(App.Locator.MySeekiosDetail.SeekiosSelected.Idseekios))
            //    StartOrRestartChrono();
        }

        #endregion

        #region Favoris Area

        /// <summary>
        /// Initialise la popup de la liste des zones favorites
        /// </summary>
        private void BuildAreaListFavorisPopup()
        {
            // création d'une alerte dialog
            Dialog listModeZoneFavorisDialog = null;
            var listModeZoneFavorisAlertDialog = new AlertDialog.Builder(this);

            // création de la liste adapter des zones favorites
            Android.Views.LayoutInflater inflater = LayoutInflater.From(this);
            Android.Views.View view = inflater.Inflate(Resource.Layout.ZoneFavoriteLayout, null);
            var zoneFavoriteListView = view.FindViewById<ListView>(Resource.Id.zoneFavorite_listView);
            FavoriteAreaAdapter favoriteAreaAdapter = null;
            favoriteAreaAdapter = new FavoriteAreaAdapter(this
                , () => { listModeZoneFavorisDialog.Dismiss(); }
                , () => { favoriteAreaAdapter.NotifyDataSetChanged(); });
            zoneFavoriteListView.Adapter = favoriteAreaAdapter;

            if (zoneFavoriteListView.Parent != null)
                ((ViewGroup)zoneFavoriteListView.Parent).RemoveView(zoneFavoriteListView); // <- fix

            zoneFavoriteListView.ChoiceMode = ChoiceMode.Single;
            zoneFavoriteListView.ItemsCanFocus = true;
            listModeZoneFavorisAlertDialog.SetView(zoneFavoriteListView);

            // instatiation de notre popup dans un objet dialog
            listModeZoneFavorisDialog = listModeZoneFavorisAlertDialog.Create();
            listModeZoneFavorisDialog.Show();
        }

        /// <summary>
        /// Initialise la popup de l'ajout d'une zone en favoris
        /// </summary>
        private void BuildAreaAddFavorisPopup()
        {
            // création d'une alerte dialog
            _addModeZoneFavorisAlertDialog = new AlertDialog.Builder(this);

            var editText = new EditText(this);
            var editTextLayout = new LinearLayout(this);

            var editTextParam = new Android.Widget.LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            editTextParam.SetMargins(25, 5, 25, 5);
            editText.InputType = Android.Text.InputTypes.TextVariationUri;
            editText.SetHint(Resource.String.addFavoriteArea_areaName);
            editText.SetTextColor(Android.Graphics.Color.ParseColor("#cccccc")); // TODO : dans fichier ressource
            editText.SetHintTextColor(Android.Graphics.Color.ParseColor("#888888")); // TODO : dans fichier ressource
            editText.LayoutParameters = editTextParam;

            var editTextLayoutParam = new Android.Widget.LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            editTextLayoutParam.SetMargins(20, 5, 20, 5);
            editTextLayout.LayoutParameters = editTextLayoutParam;
            editTextLayout.AddView(editText);

            // titre de l'alerte 
            _addModeZoneFavorisAlertDialog.SetTitle(Resource.String.modeZone_addAreaFavorisTitle);

            // bouton ajouter
            _addModeZoneFavorisAlertDialog.SetPositiveButton(Resource.String.popup_add, async (o, e) =>
            {
                if (string.IsNullOrEmpty(editText.Text)) return;

                var pointCount = 0;

                if (_mapControlManager.ZonePolygon != null)
                {
                    if (_mapControlManager.ZonePolygon.Points.Count == 1) pointCount = 1;
                    else pointCount = _mapControlManager.ZonePolygon.Points.Count - 1;
                    var trame = GetTrame(_mapControlManager.ZonePolygon.Points.ToList());

                    var surface = AreaHelper.CalculateAreaOfGPSPolygonOnEarthInSquareMeters(_mapControlManager.ZonePolygon.Points.ToList());

                    LoadingLayout.Visibility = ViewStates.Visible;
                    var result = await App.Locator.ModeZone.AddFavoriteArea(new FavoriteAreaDTO
                    {
                        Trame = trame,
                        PointsCount = pointCount,
                        AreaGeodesic = surface,
                        AreaName = editText.Text.ToUpperCaseFirst(),
                    });
                    if (result)
                    {
                        var textAddedAreaToast = string.Format(Resources.GetString(Resource.String.modeZone_toast_AddedArea), editText.Text.ToUpperCaseFirst());
                        Toast.MakeText(this, textAddedAreaToast, ToastLength.Long).Show();
                    }
                    LoadingLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    string error = Resources.GetString(Resource.String.addFavoriteArea_errorZone);
                    AlertDialog alertMessage = new AlertDialog.Builder(this).Create();
                    alertMessage.SetTitle("Error: ");
                    alertMessage.SetMessage(error);
                    alertMessage.Show();
                }
                _addModeZoneFavorisDialog.Dismiss();
            });

            // bouton annuler
            _addModeZoneFavorisAlertDialog.SetNegativeButton(Resource.String.popup_cancelled, (o, e) =>
            {
                _addModeZoneFavorisDialog.Dismiss();
            });

            // ajout de la vue dans l'alerte
            _addModeZoneFavorisAlertDialog.SetView(editTextLayout);

            // instatiation de notre popup dans un objet dialog
            _addModeZoneFavorisDialog = _addModeZoneFavorisAlertDialog.Create();
            _addModeZoneFavorisAlertDialog.Show();
        }

        /// <summary>
        /// Construit une trame avec des ; et des : à partir d'une liste de points
        /// (format de trame utilisé en BDD)
        /// </summary>
        /// <param name="points">liste de points</param>
        private string GetTrame(List<LatLng> points)
        {
            var provider = new CultureInfo("en-US");

            var trame = string.Empty;
            var first = true;
            foreach (var point in points)
            {
                if (!first) trame += ";";
                trame += point.Latitude.ToString(provider) + ":" + point.Longitude.ToString(provider);
                first = false;
            }
            return trame;
        }

        #endregion

        #endregion

        #region ===== Event =======================================================================

        /// <summary>
        /// Raised from the view model
        /// </summary>
        private void OnModeZonePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "StartRefreshPosition")
            {
                RefreshInProgressButton.Visibility = ViewStates.Visible;
            }

            if (e.PropertyName == "CompleteRefreshPosition")
            {
                RefreshInProgressButton.Visibility = ViewStates.Gone;
                if (_mapControlManager.SelectedMarker != null)
                {
                    _mapControlManager.SelectedMarker.ShowInfoWindow();
                }
            }
            if (e.PropertyName == "SelectedFavoriteArea")
            {
                _countOfUpdatesOnFavoriteAreaSelection = _mapControlManager.UndoActions.Count;

                if (App.Locator.ModeZone.SelectedFavoriteArea == null) return;
                if (App.Locator.ModeZone.SelectedFavoriteArea.Trame == null) return;
                if (!App.Locator.ModeZone.SelectedFavoriteArea.Trame.Contains(":")) return;

                var pointsToRestore = new List<LatLng>();
                if (_mapControlManager.ZonePolygon != null) pointsToRestore = _mapControlManager.ZonePolygon.Points.ToList();

                var culture = new CultureInfo("en-US");
                var lsCoords = new List<LatLng>();

                foreach (var coords in App.Locator.ModeZone.SelectedFavoriteArea.Trame.Split(';'))
                {
                    var lat = Double.Parse(coords.Split(':').First(), culture);
                    var lon = Double.Parse(coords.Split(':').Last(), culture);
                    lsCoords.Add(new LatLng(lat, lon));
                }

                if (!App.Locator.ModeZone.IsOnEditMode) App.Locator.ModeZone.IsOnEditMode = true;
                if (_mapControlManager.ZonePolygon != null) _mapControlManager.ZonePolygon.Remove();
                _mapControlManager.ZonePolygon = null;
                var isInAlert = MapViewModelBase.Mode != null && MapViewModelBase.Mode.StatusDefinition_idstatusDefinition != 1;
                _mapControlManager.CreateZone(_mapControlManager.ConvertLatitudeLongitudeToLatLng(lsCoords), isInAlert, true, -1);
                _mapControlManager.UndoActions.Add(new Action(() =>
                {
                    _mapControlManager.CreateZone(_mapControlManager.ConvertLatitudeLongitudeToLatLng(pointsToRestore), true);
                    RefreshMarkers();
                }));
                RefreshMarkers();
            }
        }

        /// <summary>
        /// Update the zone
        /// </summary>
        private void OnInformationZoneRefresh(object sender, EventArgs e)
        {
            RefreshMarkers();
            RefreshTutoText();
        }

        /// <summary>
        /// Center the map on the seekios
        /// </summary>
        private void OnZoneSeekiosSvgImageViewClick(object sender, EventArgs e)
        {
            App.Locator.ModeZone.CenterOnZone();
        }

        /// <summary>
        /// Next button
        /// </summary>
        private void OnClickNextPage(object sender, EventArgs e)
        {
            List<LatitudeLongitude> listOfPoints = new List<LatitudeLongitude>();

            //If last position known > 1 hour ago
            if (App.Locator.DetailSeekios.SeekiosSelected != null && App.Locator.DetailSeekios.SeekiosSelected.DateLastCommunication != null
                && (DateTime.UtcNow.ToLocalTime() - App.Locator.DetailSeekios.SeekiosSelected.DateLastCommunication).Value.TotalHours > 1)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog_Alert);
                AlertDialog alertDialog = builder.Create();
                alertDialog.SetTitle(Resource.String.modeZone_oldSeekiosPositionTitle);
                alertDialog.SetMessage(Resources.GetString(Resource.String.modeZone_oldSeekiosPositionMessage));
                alertDialog.SetButton(Resources.GetString(Resource.String.modeZone_continue), (senderAlert, args) =>
                {
                    if (_mapControlManager.ZonePolygon != null)
                    {
                        listOfPoints = _mapControlManager.ZonePolygon.Points.Select(el => new LatitudeLongitude(el.Latitude, el.Longitude)).ToList();
                        if (listOfPoints.Count > 2) listOfPoints.RemoveAt(listOfPoints.Count - 1);

                        VerifyIfSeekiosIsInZone(new LatitudeLongitude(App.Locator.DetailSeekios.SeekiosSelected.LastKnownLocation_latitude,
                            App.Locator.DetailSeekios.SeekiosSelected.LastKnownLocation_longitude)
                            , listOfPoints);
                    }
                    else App.Locator.ModeZone.GoToSecondPage(null);
                });
                alertDialog.SetButton2(Resources.GetString(Resource.String.modeZone_cancel), (senderAlert, args) =>
                {
                    alertDialog.Dismiss();
                    return;
                });
                alertDialog.Show();
            }
            //else we verify if the last position known is in the zone or not
            else
            {
                //verify zone validity
                if (_mapControlManager.ZonePolygon != null)
                {
                    listOfPoints = _mapControlManager.ZonePolygon.Points.Select(el => new LatitudeLongitude(el.Latitude, el.Longitude)).ToList();
                    if (listOfPoints.Count > 2) listOfPoints.RemoveAt(listOfPoints.Count - 1);
                }
                else App.Locator.ModeZone.GoToSecondPage(null);

                VerifyIfSeekiosIsInZone(new LatitudeLongitude(App.Locator.DetailSeekios.SeekiosSelected.LastKnownLocation_latitude,
                    App.Locator.DetailSeekios.SeekiosSelected.LastKnownLocation_longitude)
                    , listOfPoints);               
            }
        }

        /// <summary>
        /// Verify that the seekios is in the zone at start and show a popup to the user if its outside
        /// </summary>
        /// <param name="seekiosPos"></param>
        /// <param name="listOfPoints"></param>
        private void VerifyIfSeekiosIsInZone(LatitudeLongitude seekiosPos, List<LatitudeLongitude> zone)
        {
            if (App.Locator.ModeZone.VerifyIfSeekiosIsInZone(new LatitudeLongitude(App.Locator.DetailSeekios.SeekiosSelected.LastKnownLocation_latitude, App.Locator.DetailSeekios.SeekiosSelected.LastKnownLocation_longitude), zone))
                App.Locator.ModeZone.GoToSecondPage(zone);
            else
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog_Alert);
                AlertDialog alertDialog = builder.Create();
                alertDialog.SetTitle(Resource.String.modeZone_seekiosNotInZoneTitle);
                alertDialog.SetMessage(Resources.GetString(Resource.String.modeZone_seekiosNotInZoneMessage));
                alertDialog.SetButton(Resources.GetString(Resource.String.modeZone_continue), (senderAlert, args) =>
                {
                    App.Locator.ModeZone.GoToSecondPage(zone);
                });
                alertDialog.SetButton2(Resources.GetString(Resource.String.modeZone_cancel), (senderAlert, args) =>
                {
                    alertDialog.Dismiss();
                    return;
                });
                alertDialog.Show();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAddPointSvgImageViewClick(object sender, EventArgs e)
        {
            RefreshTutoText(true);
        }

        #endregion

        #region ===== Callback ====================================================================

        /// <summary>
        /// Callback d'initialisation de la map
        /// </summary>
        public override void OnMapReady(GoogleMap googleMap)
        {
            if (googleMap != null)
            {
                this.MapWrapperLayout.Init(googleMap);

                if (_mapControlManager == null)
                {
                    // initialisation de la map
                    _mapControlManager = new MapControlManager(googleMap
                        , BaseContext
                        , AddPointSvgImageView
                        , UndoSvgImageView
                        , CenterSeekiosSvgImageView
                        , MapViewModelBase.Seekios.Idseekios.ToString());
                    App.Locator.ModeZone.MapControlManager = _mapControlManager;
                    App.Locator.ModeZone.InitMap();
                    //Ajout d'un polygone pour la zone du mode zone
                    App.Locator.ModeZone.InitMode();
                    App.Locator.ModeZone.InitZone();
                    if (_mapControlManager.ZonePolygon != null)
                        ShowZoneSvgImageView.Visibility = ViewStates.Visible;

                    //(_modeMetaDataFragment as ZoneMetaDataFragment).ZonePolygon = _mapControlManager.ZonePolygon;

                    _mapControlManager.ZoneInformationUpdated += OnInformationZoneRefresh;
                    _mapControlManager.RegisterMethodes();
                }
            }
            base.OnMapReady(googleMap);
        }

        #endregion
    }
}