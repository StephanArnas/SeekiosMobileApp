using System;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SeekiosApp.Droid.Helper;
using SeekiosApp.Droid.CustomComponents;
using Android.Text;
using Android.Text.Style;
using Android.Graphics;
using com.refractored.fab;
using SeekiosApp.Model.DTO;
using GalaSoft.MvvmLight.Helpers;
using Android.Content;

namespace SeekiosApp.Droid.View
{
    [Activity(Label = "Liste des alertes", Theme = "@style/Theme.Normal")]
    public class ListAlertsSeekiosActivity : LeftMenuActivity
    {
        /// <summary>
        /// Création de la page
        /// </summary>
        protected override void OnCreate(Bundle bundle)
        {
            ThemeHelper.OnActivityCreateSetTheme(this);
            base.OnCreate(bundle, Resource.Layout.ListAlertsLayout, true);
            getObjectsFromView();
            setBindings();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            Finish();
        }

        /// <summary>
        /// permet l'enregistrement du menu auprès de la vue
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            RegisterForContextMenu(listViewAlerts);
            listViewAlertsAdapter.NotifyDataSetChanged();
        }

        /// <summary>
        /// permet d'unregister du menu auprès de la vue
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            //App.Locator.ListAlert.Seekios = null;
            UnregisterForContextMenu(listViewAlerts);
        }

        /// <summary>
        /// Récupère les objets de la vue
        /// </summary>
        private void getObjectsFromView()
        {
            listViewAlerts = FindViewById<ListView>(Resource.Id.listAlerts_alertsSeekiosList);
            textViewNumberOfSeekiosConnected = FindViewById<TextView>(Resource.Id.listAlerts_seekiosNumber);
            switchNotificationPush = FindViewById<Switch>(Resource.Id.listAlerts_notificationPush);
            textViewSubTitlePage = FindViewById<TextView>(Resource.Id.listAlerts_title);
            alertFloatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.listAlert_floatingActionButton);
            var toolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
        }

        /// <summary>
        /// Permet d'afficher la liste des alertes et le nombre de seekios connectés
        /// </summary>
        private void setBindings()
        {
            listViewAlertsAdapter = new ListAlertsAdapter(this);
            listViewAlerts.Adapter = listViewAlertsAdapter;
            listViewAlerts.ChoiceMode = ChoiceMode.Single;
            listViewAlerts.ItemsCanFocus = true;
           
            if (App.Locator.ListAlert.IsFromLeftMenu)
            {
                // affichage du text indiquant le nombre de seekios possédant une alerte
                textViewSubTitlePage.SetText(Resources.GetString(Resource.String.listAlert_subTitle1), TextView.BufferType.Normal);
                var textSeekiosConnected = string.Format(Resources.GetString(Resource.String.listAlert_seekiosConnected), App.CurrentUserEnvironment.GetNumberOfSeekiosWithAlert());
                var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfNumberInString(textSeekiosConnected);
                var formattedTextNumberOfSeekiosConnected = new SpannableString(textSeekiosConnected);
                formattedTextNumberOfSeekiosConnected.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.MainColor)), resultTuple.Item1, resultTuple.Item2, 0);
                textViewNumberOfSeekiosConnected.SetText(formattedTextNumberOfSeekiosConnected, TextView.BufferType.Spannable);
                switchNotificationPush.Visibility = ViewStates.Gone;
                alertFloatingActionButton.Visibility = ViewStates.Gone;
            }
            else
            {
                // affichage du contrôle Switch pour activer / désactiver une alerte
                textViewSubTitlePage.SetText(Resources.GetString(Resource.String.listAlert_subTitle2), TextView.BufferType.Normal);
                textViewNumberOfSeekiosConnected.Visibility = ViewStates.Gone;
                alertFloatingActionButton.AttachToListView(listViewAlerts);
                alertFloatingActionButton.Show();
            }
        }

        /// <summary>
        /// Affiche une popup pour ajouter une nouvelle alerte
        /// </summary>
        /// <param name="v"></param>
        [Java.Interop.Export("FloatingActionButton_Click")]
        public void Onclick(Android.Views.View v)
        {
            {
                LayoutInflater inflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
                Android.Views.View popup = inflater.Inflate(Resource.Drawable.PopupAlertTypeChoice, null);

#pragma warning disable CS0618 // Le type ou le membre est obsolète
                PopupWindow window = new PopupWindow(popup, ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent, true);
#pragma warning restore CS0618 // Le type ou le membre est obsolète

                popup.FindViewById<Button>(Resource.Id.alert_smsChoice).Click += (sender, e) =>
                {
                    App.Locator.ListAlert.GoToAlert(Enum.AlertDefinitionEnum.SMS);
                };

                popup.FindViewById<Button>(Resource.Id.alert_emailChoice).Click += (sender, e) =>
                {
                    window.Dismiss();
                    App.Locator.ListAlert.GoToAlert(Enum.AlertDefinitionEnum.Email);
                };

                popup.FindViewById<Button>(Resource.Id.alert_vocalCallChoice).Click += (sender, e) =>
                {
                    window.Dismiss();
                    App.Locator.ListAlert.GoToAlert(Enum.AlertDefinitionEnum.VocalCall);
                };

                popup.FindViewById<RelativeLayout>(Resource.Id.mainLayout).Click += (sender, e) =>
                {
                    window.Dismiss();
                };

                window.ShowAtLocation(popup, GravityFlags.Center, 0, 100);
            }
        }

        /// <summary>
        /// Adapter de la ListView des alertes
        /// </summary>
        private ListAlertsAdapter listViewAlertsAdapter = null;
        /// <summary>
        /// Affiche la liste des alertes
        /// </summary>
        private ListView listViewAlerts { get; set; }
        /// <summary>
        /// Affiche un sous titre de la page
        /// </summary>
        private TextView textViewSubTitlePage { get; set; }
        /// <summary>
        /// Affiche le nombre de Seekios qui possèdent des alertes
        /// </summary>
        private TextView textViewNumberOfSeekiosConnected { get; set; }
        /// <summary>
        /// Switch pour activer ou désactiver les notifications Push 
        /// </summary>
        private Switch switchNotificationPush { get; set; }
        /// <summary>
        /// Bouton ajouter alerte
        /// </summary>
        private FloatingActionButton alertFloatingActionButton { get; set; }
        /// <summary>
        /// La text box pour le type d'alerte
        /// </summary>
        public GridLayout gridLayoutAlerts;
    }
}
