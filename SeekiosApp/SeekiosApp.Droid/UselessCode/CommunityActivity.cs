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
//using SeekiosApp.Droid.Helper;
//using SeekiosApp.Droid.View.FragmentView;
//using SeekiosApp.Droid.CustomComponents.Adapter;
//using SeekiosApp.Model.DTO;
//using Android.Graphics.Drawables;

//namespace SeekiosApp.Droid.View
//{
//    //[Activity(Theme = "@style/Theme.Community")]
//    //public class CommunityActivity : LeftMenuActivity
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
//    //        base.OnCreate(savedInstanceState, Resource.Layout.CommunityLayout, false, true);

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
//    //    protected override void OnStop()
//    //    {
//    //        base.OnStop();
//    //    }

//    //    #endregion

//    //    #region ===== ActionBar ===================================================================

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

//    //    #region ===== Initialisation Vue ==========================================================

//    //    /// <summary>
//    //    /// Récupère les objets de la vue
//    //    /// </summary>
//    //    private void GetObjectsFromView()
//    //    {
//    //        ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
//    //        TabLayout = FindViewById<Android.Support.Design.Widget.TabLayout>(Resource.Id.community_sliding_tabs);
//    //        ViewPager = FindViewById<Android.Support.V4.View.ViewPager>(Resource.Id.viewpager);
//    //    }

//    //    /// <summary>
//    //    /// Fais les liaisons entre les données du vue modèle et les objets de la vue
//    //    /// </summary>
//    //    private void SetDataToView()
//    //    {
//    //        ToolbarPage.SetTitle(Resource.String.listFriend_actionBarTitle);
//    //        ToolbarPage.SetBackgroundColor(Resources.GetColor(Resource.Color.communityDarkBlue));

//    //        // initialisation des onglets 
//    //        var fragments = new Android.Support.V4.App.Fragment[]
//    //        {
//    //            new TabListFriendsFragment(),
//    //            new TabRequestsFragment(),
//    //        };

//    //        var titles = CharSequence.ArrayFromStringArray(new[]
//    //        {
//    //            Resources.GetString(Resource.String.community_TabListFriendsTitle),
//    //            Resources.GetString(Resource.String.community_TabRequestsTitle),
//    //        });

//    //        var adapter = new PagerAdapter(SupportFragmentManager, fragments, titles);
//    //        ViewPager.Adapter = adapter;
//    //        TabLayout.SetupWithViewPager(ViewPager);
//    //    }

//    //    #endregion
//    //}
//}