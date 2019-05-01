//using Android.App;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using SeekiosApp.Droid.CustomComponents.Adapter;
//using SeekiosApp.Droid.Services;
//using SeekiosApp.Droid.View.FragmentView;

//namespace SeekiosApp.Droid.View
//{
//    //[Activity(Theme = "@style/Theme.Community")]
//    //public class ListSharingsActivity : AppCompatActivityBase
//    //{
//    //    #region ===== Propriétés ==================================================================

//    //    public Android.Support.Design.Widget.TabLayout TabLayout { get; set; }

//    //    public Android.Support.V4.View.ViewPager ViewPager { get; set; }

//    //    #endregion

//    //    #region ===== Cycle De Vie ================================================================

//    //    /// <summary>
//    //    /// Création de la page
//    //    /// </summary>
//    //    protected override void OnCreate(Bundle savedInstanceState)
//    //    {
//    //        SetContentView(Resource.Layout.ListSharingsLayout);
//    //        base.OnCreate(savedInstanceState);

//    //        GetObjectsFromView();
//    //        SetDataToView();

//    //        if (ToolbarPage != null)
//    //        {
//    //            SetSupportActionBar(ToolbarPage);
//    //            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
//    //        }
//    //    }

//    //    /// <summary>
//    //    /// Reprise de la page
//    //    /// </summary>
//    //    protected override void OnResume()
//    //    {
//    //        base.OnResume();
//    //    }

//    //    /// <summary>
//    //    /// Suspension de la page
//    //    /// </summary>
//    //    protected override void OnPause()
//    //    {
//    //        base.OnPause();
//    //    }

//    //    /// <summary>
//    //    /// Destruction de la page
//    //    /// </summary>
//    //    protected override void OnDestroy()
//    //    {
//    //        App.ActualTheme = App.THEME_LIGHT;
//    //        base.OnDestroy();
//    //    }

//    //    #endregion

//    //    #region ===== ActionBar ===================================================================


//    //    /// <summary>
//    //    /// Création de l'action bar
//    //    /// </summary>
//    //    public override bool OnCreateOptionsMenu(IMenu menu)
//    //    {
//    //        Title = App.Locator.ListSharings.FormattedFriendName;

//    //        return base.OnCreateOptionsMenu(menu);
//    //    }

//    //    /// <summary>
//    //    /// Sélection d'un bouton de l'action bar
//    //    /// </summary>
//    //    public override bool OnOptionsItemSelected(IMenuItem item)
//    //    {
//    //        switch (item.ItemId)
//    //        {
//    //            case Android.Resource.Id.Home:
//    //                Finish();
//    //                return true;
//    //            default:
//    //                return base.OnOptionsItemSelected(item);
//    //        }
//    //    }

//    //    #endregion

//    //    #region ===== ListSharing ContextMenu =====================================================

//    //    ///// <summary>
//    //    ///// Création du contexte menu
//    //    ///// </summary>
//    //    //public override void OnCreateContextMenu(IContextMenu menu, Android.Views.View v, IContextMenuContextMenuInfo menuInfo)
//    //    //{
//    //    //    base.OnCreateContextMenu(menu, v, menuInfo);
//    //    //    MenuInflater inflater = base.MenuInflater;
//    //    //    inflater.Inflate(Resource.Menu.ListFriendMenu, menu);

//    //    //    if (v.Id == Resource.Id.user_friendList)
//    //    //    {
//    //    //        AdapterView.AdapterContextMenuInfo info = (AdapterView.AdapterContextMenuInfo)menuInfo;
//    //    //        menu.SetHeaderTitle(Resource.String.listSharing_confirmDelete);
//    //    //    }
//    //    //}

//    //    ///// <summary>
//    //    ///// Sélection d'un Seekios dans le contexte menu
//    //    ///// </summary>
//    //    //public override bool OnContextItemSelected(IMenuItem item)
//    //    //{
//    //    //    switch (item.ItemId)
//    //    //    {
//    //    //        case Resource.Id.listFriendMenu_deleteFriendship:
//    //    //            var sharingSelected = App.Locator.ShareSeekios.SharingSelected;
//    //    //            App.Locator.ListSharings.DeleteSharing(sharingSelected);
//    //    //            return true;
//    //    //        default:
//    //    //            return base.OnContextItemSelected(item);
//    //    //    }
//    //    //}

//    //    #endregion

//    //    #region ===== Initialisation Vue ==========================================================

//    //    /// <summary>
//    //    /// Récupère les objets de la vue
//    //    /// </summary>
//    //    private void GetObjectsFromView()
//    //    {
//    //        ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
//    //        TabLayout = FindViewById<Android.Support.Design.Widget.TabLayout>(Resource.Id.sliding_tabs);
//    //        ViewPager = FindViewById<Android.Support.V4.View.ViewPager>(Resource.Id.viewpager);
//    //    }

//    //    /// <summary>
//    //    /// Fais les liaisons entre les données du vue modèle et les objets de la vue
//    //    /// </summary>
//    //    private void SetDataToView()
//    //    {
//    //        ToolbarPage.Title = App.Locator.ListSharings.FormattedFriendName;
//    //        ToolbarPage.SetBackgroundColor(Resources.GetColor(Resource.Color.communityDarkBlue));

//    //        // initialisation des onglets 
//    //        var fragments = new Android.Support.V4.App.Fragment[]
//    //        {
//    //            new TabListSharingsFragment(App.Locator.ListSharings.LsOwnSharing, true),
//    //            new TabListSharingsFragment(App.Locator.ListSharings.LsOtherSharing, false),
//    //        };

//    //        var titles = CharSequence.ArrayFromStringArray(new[]
//    //        {
//    //            Resources.GetString(Resource.String.tabListOwnSharings_title),
//    //            Resources.GetString(Resource.String.tabListOtherSharings_title),
//    //        });

//    //        var adapter = new PagerAdapter(SupportFragmentManager, fragments, titles);
//    //        ViewPager.Adapter = adapter;
//    //        TabLayout.SetupWithViewPager(ViewPager);
//    //    }

//    //    #endregion
//    //}
//}