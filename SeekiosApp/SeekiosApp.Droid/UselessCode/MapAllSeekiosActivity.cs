namespace SeekiosApp.Droid.View
{
    //[Activity(Theme = "@style/Theme.Normal")]
    //public class MapAllSeekiosActivity : MapBaseActivity, IOnMapReadyCallback, IConnectionCallbacks, IOnConnectionFailedListener
    //{
    //    #region ===== Attributs ===================================================================

    //    #endregion

    //    #region ===== Propriétés ==================================================================

    //    public ListView SeekiosListView { get; set; }
    //    public DrawerLayout ListSeekiosDrawerLayout { get; set; }
    //    public GoogleApiClient mGoogleApiClient { get; set; }
    //    public MapWrapperLayout Map { get; set; }

    //    #endregion

    //    #region ===== Cycle De Vie ================================================================

    //    /// <summary>
    //    /// Création de la page
    //    /// </summary>
    //    protected override void OnCreate(Bundle savedInstanceState)
    //    {
    //        base.OnCreate(savedInstanceState, Resource.Layout.MapAllSeekios, Resources.GetString(Resource.String.mapAllSeekios_titlePage), null);

    //        GoogleApiClientInstance();
    //        GetObjectsFromView();
    //        SetDataToView();
    //    }

    //    /// <summary>
    //    /// Reprise de la page
    //    /// </summary>
    //    protected override void OnResume()
    //    {
    //        mGoogleApiClient.Connect();
    //        base.OnResume();
    //    }

    //    /// <summary>
    //    /// Suspension de la page
    //    /// </summary>
    //    protected override void OnPause()
    //    {
    //        mGoogleApiClient.Disconnect();
    //        base.OnPause();
    //    }

    //    #endregion

    //    #region ===== Initialisation Vue ==========================================================

    //    /// <summary>
    //    /// Récupère les objets de la vue
    //    /// </summary>
    //    private void GetObjectsFromView()
    //    {
    //        SeekiosListView = FindViewById<ListView>(Resource.Id.rightMenuAllSeekios_seekiosList);
    //        ListSeekiosDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
    //    }

    //    /// <summary>
    //    /// Initialisation de la liste du menu de droite
    //    /// </summary>
    //    private void SetDataToView()
    //    {
    //        SeekiosListView.Adapter = new ListSeekiosMapAllSeekiosAdapter(this, OnSeekiosListViewItemClick);
    //    }

    //    #endregion

    //    #region ===== Google API Client ===========================================================

    //    private void GoogleApiClientInstance()
    //    {
    //        // Create an instance of GoogleAPIClient.
    //        if (mGoogleApiClient == null)
    //        {
    //            mGoogleApiClient = new Builder(this)
    //                .AddConnectionCallbacks(this)
    //                .AddOnConnectionFailedListener(this)
    //                .AddApi(LocationServices.API)
    //                .Build();
    //        }
    //    }

    //    public void OnConnected(Bundle connectionHint)
    //    {
    //        var mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);
    //        if(mLastLocation != null)
    //        {
    //            //Map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new Android.Gms.Maps.Model.LatLng(mLastLocation.Latitude, mLastLocation.Longitude), 14.0f));
    //        }
    //    }

    //    public void OnConnectionSuspended(int cause)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void OnConnectionFailed(ConnectionResult result)
    //    {
    //        throw new NotImplementedException();
    //    }


    //    #endregion

    //    #region ===== Évènements ==================================================================

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    private void OnSeekiosListViewItemClick(SeekiosDTO seekios)
    //    {
    //        //On donne le seekios cliqué au viewModel
    //        MapViewModelBase.Seekios = seekios;
    //        //On zoom dessus
    //        App.Locator.MapAllSeekios.ZoomToSeekios();
    //        //On ferme le volet
    //        ListSeekiosDrawerLayout.CloseDrawers();
    //    }

    //    #endregion

    //    #region ===== Callback ====================================================================

    //    /// <summary>
    //    /// Callback d'initialisation de la map
    //    /// </summary>
    //    public override void OnMapReady(GoogleMap googleMap)
    //    {
    //        if (googleMap != null)
    //        {
    //            MapWrapperLayout.Init(googleMap);

    //            // initialisation de la map
    //            _mapControlManager = new MapControlManager(googleMap, BaseContext);
    //            //Ajout d'un tracé pour le parcours du seekios
    //            App.Locator.MapAllSeekios.MapControlManager = _mapControlManager;
    //            App.Locator.MapAllSeekios.InitMap();

    //            var centerPoint = GetCentralGeoCoordinate(GetSeekiosListGeoCoordinate());

    //            googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(centerPoint.Latitude, centerPoint.Longitude), 11.0f));
    //        }
    //        base.OnMapReady(googleMap);
    //    }

    //    #endregion


    //    public static GeoCoordinate GetCentralGeoCoordinate(List<GeoCoordinate> geoCoordinates)
    //    {
    //        if (geoCoordinates.Count == 1)
    //            return geoCoordinates.Single();

    //        double x = 0;
    //        double y = 0;
    //        double z = 0;

    //        foreach(var geoCoordinate in geoCoordinates)
    //        {
    //            var latitude = geoCoordinate.Latitude * Math.PI / 180;
    //            var longitude = geoCoordinate.Longitude * Math.PI / 180;

    //            x += Math.Cos(latitude) * Math.Cos(longitude);
    //            y += Math.Cos(latitude) * Math.Sin(longitude);
    //            z += Math.Sin(latitude);
    //        }

    //        var total = geoCoordinates.Count;

    //        x = x / total;
    //        y = y / total;
    //        z = z / total;

    //        var centralLongitude = Math.Atan2(y, x);
    //        var centralSquareRoot = Math.Sqrt(x * x + y * y);
    //        var centralLatitude = Math.Atan2(z, centralSquareRoot);

    //        return new GeoCoordinate(centralLatitude * 180 / Math.PI, centralLongitude * 180 / Math.PI);
    //    }

    //    public List<GeoCoordinate> GetSeekiosListGeoCoordinate()
    //    {
    //        List<GeoCoordinate> listGeoCoordinate = new List<GeoCoordinate>();
    //        GeoCoordinate geoCoordinate;
    //        foreach (var seekios in App.CurrentUserEnvironment.LsSeekios)
    //        {
    //            if (seekios != null && seekios.LastKnownLocation_dateLocationCreation.HasValue)
    //            {
    //                geoCoordinate = new GeoCoordinate(seekios.LastKnownLocation_latitude, seekios.LastKnownLocation_longitude);
    //                listGeoCoordinate.Add(geoCoordinate);
    //            }
    //        }

    //        return listGeoCoordinate;
    //    }
    //}
}