using Android.App;
using Android.OS;
using Android.Widget;
using SeekiosApp.Droid.CustomComponents;
using Android.Views;
using SeekiosApp.Model.DTO;
using System;
using Android.Support.V7.App;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Interfaces;
using SeekiosApp.Droid.Services;
using com.refractored.fab;
using static SeekiosApp.Droid.Helper.OneSignalHelper;
using Com.OneSignal.Android;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class ListSeekiosActivity : LeftMenuActivity
    {
        #region ===== Attributs ===================================================================

        private ListSeekiosAdapter _seekiosAdapter = null;
        private DispatchService _dispatcher = null;

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Liste des Seekios</summary>
        public ListView SeekiosListView { get; set; }

        /// <summary>Floating action button</summary>
        public FloatingActionButton AddSeekiosFloatingActionButton { get; set; }

        /// <summary>Layout affiché si la liste des seekios est vide</summary>
        public LinearLayout EmptySeekiosListLayout { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState, Resource.Layout.ListSeekiosLayout, true);

            var drawerToggle = new ActionBarDrawerToggle(this, DrawerNavigation, ToolbarPage, Resource.String.open_drawer, Resource.String.close_drawer);
            DrawerNavigation.SetDrawerListener(drawerToggle);
            drawerToggle.SyncState();

            _dispatcher = (ServiceLocator.Current.GetInstance<IDispatchOnUIThread>() as DispatchService);

            GetObjectsFromView();
            SetDataToView();
            CurrentActivity = this;

            // Connect to OneSignal
            OneSignal.Init(this, "1077123816365"
                , "ee8851b0-f171-4de0-b86b-74ef18eefa02"
                , new NotificationOpenedHandler()
                , new NotificationReceivedHandler(this));
            OneSignal.SetSubscription(true);
            OneSignal.IdsAvailable(new IdsAvailableHandler());

            // Display popup if the new reload credit is available
            App.Locator.ListSeekios.PopupRelaodCreditMonthly();

            // Display a popup if the notification push are not registered
            App.Locator.ListSeekios.PopupNotificationNotAvailable(() =>
            {
                using (var intent = new Android.Content.Intent(Android.Content.Intent.ActionView
                    , Android.Net.Uri.Parse(App.TutorialNotificationLink)))
                {
                    StartActivity(intent);
                }
            });

            // Register to SignalR
            App.Locator.ListSeekios.SubscribeToSignalR();

            // Get the uidDevice (required by webservice for identify the sender of the broadcast)
            App.UidDevice = Helper.DeviceInfoHelper.GetDeviceUniqueId(this);
        }

        protected override void OnResume()
        {
            Timer.Stop();
            Console.WriteLine("Time from splash to list : {0}", Timer.ElapsedMilliseconds);
            base.OnResume();
            _seekiosAdapter.NotifyDataSetChanged();
            if (App.Locator.ListSeekios.ActivityNeedsUIToBeUpdated)
            {
                App.Locator.ListSeekios.ActivityNeedsUIToBeUpdated = false;
                _seekiosAdapter = new ListSeekiosAdapter(this);
                SeekiosListView.Adapter = _seekiosAdapter;
            }
            App.SeekiosChanged += App_SeekiosChanged;
            EmptySeekiosListLayout.Click += OnEmptySeekiosListClick;
            AddSeekiosFloatingActionButton.Click += OnEmptySeekiosListClick;
            SeekiosListView.ItemClick += OnListSeekiosItemClick;
            LoadingLayout.Visibility = ViewStates.Gone;
            RegisterForContextMenu(SeekiosListView);
        }

        protected override void OnPause()
        {
            base.OnPause();
            App.SeekiosChanged -= App_SeekiosChanged;
            EmptySeekiosListLayout.Click -= OnEmptySeekiosListClick;
            AddSeekiosFloatingActionButton.Click -= OnEmptySeekiosListClick;
            SeekiosListView.ItemClick -= OnListSeekiosItemClick;
            UnregisterForContextMenu(SeekiosListView);
        }

        public override void OnBackPressed()
        {
            if (App.Locator.ListSeekios.IsNotFromLogin)
            {
                base.OnBackPressed();
                Finish();
            }
        }

        #endregion

        #region ===== ActionBar ===================================================================

        /// <summary>
        /// Création de l'action bar
        /// </summary>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //MenuInflater.Inflate(Resource.Menu.ListSeekiosActionBar, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        /// <summary>
        /// Sélection d'un bouton de l'action bar
        /// </summary>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.TitleFormatted.ToString() == "Searching")
            {
                //FindViewById<LinearLayout>(Resource.Id.seekiossearchingLayout).Visibility = ViewStates.Visible;
                App.Locator.LeftMenu.GoToAddSeekios();

            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region ===== Initialize View =============================================================

        private void GetObjectsFromView()
        {
            SeekiosListView = FindViewById<ListView>(Resource.Id.seekios_seekiosList);
            AddSeekiosFloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.listSeekios_floatingActionButton);
            EmptySeekiosListLayout = FindViewById<LinearLayout>(Resource.Id.seekios_emptyLayout);
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
        }

        private void SetDataToView()
        {
            _seekiosAdapter = new ListSeekiosAdapter(this);
            SeekiosListView.EmptyView = EmptySeekiosListLayout;
            SeekiosListView.Adapter = _seekiosAdapter;
            SeekiosListView.ChoiceMode = ChoiceMode.Single;
            SeekiosListView.ItemsCanFocus = true;

            if (App.Locator.Map.LsSeekiosOnDemand?.Count > 0)
            {
                foreach (var seekiosOnDemand in App.Locator.Map.LsSeekiosOnDemand)
                {
                    seekiosOnDemand.OnSuccess = null;
                    seekiosOnDemand.OnSuccess += () =>
                    {
                        App_SeekiosChanged(null, 0);
                    };
                }
            }

            AddSeekiosFloatingActionButton.AttachToListView(SeekiosListView);
            App.Locator.BaseMap.InitialiseLsAlertState();
        }

        public override void SetSupportActionBar(Android.Support.V7.Widget.Toolbar toolbar)
        {
            toolbar.Title = Resources.GetString(Resource.String.listSeekios_title)
                + (SeekiosApp.Services.DataService.UseStaging ? " (Stag" + (SeekiosApp.Services.DataService.UseHttps ? " + https" : "") + ")" : "");
        }

        private void ScrollToEndToShowLastitemMenu(SeekiosDTO item)
        {
            if (item != App.Locator.ListSeekios.LsSeekios.LastOrDefault()) return;
            SeekiosListView.SetSelection(SeekiosListView.Count - 1);
        }

        #endregion

        #region ===== Events ======================================================================

        private void App_SeekiosChanged(object sender, int e)
        {
            SetDataToView();
        }

        private void OnEmptySeekiosListClick(object sender, EventArgs e)
        {
            App.Locator.ListSeekios.GoToAddSeekios();
            App.Locator.AddSeekios.IsAdding = true;
        }

        private void OnListSeekiosItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (App.Locator.ListSeekios.LsSeekios[e.Position].User_iduser == App.CurrentUserEnvironment.User.IdUser)
            {
                App.Locator.DetailSeekios.SeekiosSelected = App.Locator.ListSeekios.LsSeekios[e.Position];
                App.Locator.ListSeekios.GoToSeekiosDetail();
            }
        }

        #endregion
    }
}