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
//using SeekiosApp.Droid.Services;

//namespace SeekiosApp.Droid.View
//{
//    //[Activity(Theme = "@style/Theme.Normal")]
//    //public class PowerSaving : AppCompatActivityBase
//    //{
//    //    #region ===== Attributs ===================================================================



//    //    #endregion

//    //    #region ===== Properties =================================================================



//    //    #endregion

//    //    #region ===== Life cycle ==================================================================

//    //    /// <summary>
//    //    /// View creation
//    //    /// </summary>
//    //    protected override void OnCreate(Bundle bundle)
//    //    {
//    //        SetContentView(Resource.Layout.PowerSaving);
//    //        base.OnCreate(bundle);

//    //        GetObjectsFromView();
//    //        SetDataToView();

//    //        ToolbarPage.SetTitle(Resource.String.powerSaving_title);
//    //        SetSupportActionBar(ToolbarPage);
//    //        SupportActionBar.SetDisplayHomeAsUpEnabled(true);
//    //    }

//    //    /// <summary>
//    //    /// View resumed 
//    //    /// </summary>
//    //    protected override void OnResume()
//    //    {
//    //        base.OnResume();
//    //    }

//    //    /// <summary>
//    //    /// View suspended
//    //    /// </summary>
//    //    protected override void OnPause()
//    //    {
//    //        base.OnPause();
//    //    }

//    //    /// <summary>
//    //    /// View destryed
//    //    /// </summary>
//    //    protected override void OnDestroy()
//    //    {
//    //        base.OnDestroy();
//    //    }

//    //    #endregion

//    //    #region ===== ActionBar ===================================================================

//    //    /// <summary>
//    //    /// Navigation Bar
//    //    /// </summary>
//    //    public override bool OnOptionsItemSelected(IMenuItem item)
//    //    {
//    //        switch (item.ItemId)
//    //        {
//    //            case Resource.Id.home:
//    //                Finish();
//    //                break;
//    //            default:
//    //                Finish();
//    //                break;
//    //        }
//    //        return true;
//    //    }

//    //    #endregion

//    //    #region ===== Initialisze View ============================================================

//    //    /// <summary>
//    //    /// Get the objects from the view
//    //    /// </summary>
//    //    private void GetObjectsFromView()
//    //    {
//    //        ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
//    //        LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
//    //    }


//    //    /// <summary>
//    //    /// Set up view data
//    //    /// </summary>
//    //    private void SetDataToView()
//    //    {

//    //    }

//    //    #endregion

//    //    #region ===== Private Methodes ============================================================



//    //    #endregion

//    //    #region ===== Event =======================================================================



//    //    #endregion

//    //    #region ===== CallBack ====================================================================



//    //    #endregion
//    //}
//}