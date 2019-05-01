using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Views;
using Android.Widget;
using SeekiosApp.Model.DTO;
using SeekiosApp.Enum;
using Android.Support.V4.Content;
using System.Collections.ObjectModel;
using System;
using SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter;

namespace SeekiosApp.Droid.CustomComponents
{
    public class ListAlertsAdapter : BaseAdapter<AlertDTO>
    {
        #region ===== Attibuts =================================================================== 

        private Activity _context = null;
        private bool _isAllAlert = false;
        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

        #endregion

        #region ===== Ctr(s) =============================================================

        public ListAlertsAdapter(Activity context, bool isAllAlert)
            : base()
        {
            this._context = context;
            this._isAllAlert = isAllAlert;
        }

        #endregion

        #region ===== Surcharge de l'adapter =====================================================

        public override AlertDTO this[int position]
        {
            get { return _isAllAlert ? App.Locator.ListAlert.LsAllAlerts[position] : App.Locator.ListAlert.LsSeekiosAlerts[position]; }
        }

        public override int Count
        {
            get { return _isAllAlert ? App.Locator.ListAlert.LsAllAlerts.Count : App.Locator.ListAlert.LsSeekiosAlerts.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            // item en cours de traitement
            var item = _isAllAlert ? App.Locator.ListAlert.LsAllAlerts[position] : App.Locator.ListAlert.LsSeekiosAlerts[position];

            // pas de vue à utiliser ? création d'une vue
            ListAlertsViewHolder holder = null;
            if (!_alreadyCreatedViews.ContainsKey(item.IdAlert))
                _alreadyCreatedViews.Add(item.IdAlert, null);
            Android.Views.View view = _alreadyCreatedViews[item.IdAlert];
            // Si on a récupéré la vue on récupère le holder dans son tag
            if (view != null) holder = view.Tag as ListAlertsViewHolder;
            // Si le holder n'est pas défini, on le fait et on crée la vue
            if (holder == null)
            {
                holder = new ListAlertsViewHolder();
                view = _context.LayoutInflater.Inflate(Resource.Layout.ListAlertsRow, null);
                view.Tag = holder;
                // récupération des objets de la vue
                holder.TypeAlertTextView = view.FindViewById<TextView>(Resource.Id.listAlertsRow_alertType);
                holder.TypeAlertLayout = view.FindViewById<RelativeLayout>(Resource.Id.listAlertsRow_alertTypeEllipsis);
                holder.SeekiosNameAlertTextView = view.FindViewById<TextView>(Resource.Id.listAlertsRow_seekiosName);
                holder.ContentAlertTextView = view.FindViewById<TextView>(Resource.Id.alerts_alertContains);
                holder.NumberOfRecipient = view.FindViewById<TextView>(Resource.Id.listAlertsRow_recipientNumber);
                holder.RowLayout = view.FindViewById<RelativeLayout>(Resource.Id.listAlertsRow_relativeLayout);

                // Configure le click de l'alerte pour naviguer au détail de l'alerte
                holder.RowLayout.Click += ((o, e) =>
                {
                    App.Locator.ListAlert.GoToAlert(item);
                });

                _alreadyCreatedViews[item.IdAlert] = view;
            }
            // ellipsis et text (sms/email/appel) du type de l'alerte 
            SetTypeAlert(item, holder.TypeAlertTextView, holder.TypeAlertLayout, _context);
            // nom du seekios
            SetTitleAlertTextView(item, holder.SeekiosNameAlertTextView, _isAllAlert);
            // description de l'alerte
            SetContentAlertTextView(item, holder.ContentAlertTextView);
            // nombre de recipient
            var countOfRecipients = 0;
            if (App.CurrentUserEnvironment.LsAlertRecipient != null
                && App.CurrentUserEnvironment.LsAlertRecipient.Count > 0)
                countOfRecipients = App.CurrentUserEnvironment.LsAlertRecipient.Where(w => w.IdAlert == item.IdAlert).Count();
            SetNumberOfRecipient(countOfRecipients, holder.NumberOfRecipient);

            return view;
        }

        #endregion

        #region ===== Configure l'interface (setView) ============================================

        /// <summary>
        /// Configure le type d'alerte
        /// </summary>
        /// <param name="typeAlertTextView">représente le type d'alerte (sms/email/vocal)</param>
        /// <param name="typeAlertLayout">Ellipsis de l'alerte</param>
        public static void SetTypeAlert(AlertDTO item, TextView typeAlertTextView, RelativeLayout typeAlertLayout, Activity context)
        {
            switch (item.IdAlertType)
            {
                case (int)AlertDefinitionEnum.SMS:
                    typeAlertTextView.Text = context.Resources.GetString(Resource.String.alert_sms);
                    typeAlertLayout.Background = ContextCompat.GetDrawable(context, Resource.Drawable.EllipsisListAlert1);
                    break;
                case (int)AlertDefinitionEnum.Email:
                    typeAlertTextView.Text = context.Resources.GetString(Resource.String.alert_email);
                    typeAlertLayout.Background = ContextCompat.GetDrawable(context, Resource.Drawable.EllipsisListAlert2);
                    break;
                case (int)AlertDefinitionEnum.MessageCall:
                    typeAlertTextView.Text = context.Resources.GetString(Resource.String.alert_vocalCall);
                    typeAlertLayout.Background = ContextCompat.GetDrawable(context, Resource.Drawable.EllipsisListAlert3);
                    break;
            }
        }

        /// <summary>
        /// Configure le premier TextView de la liste des alertes qui est le titre
        /// </summary>
        /// <param name="seekiosNameAlertTextView">objet titre de l'alerte</param>
        public static void SetTitleAlertTextView(AlertDTO item, TextView seekiosNameAlertTextView, bool isAllAlert)
        {
            if (isAllAlert)
            {
                var seekios = App.CurrentUserEnvironment.GetSeekiosFromAlert(item);
                if (seekios != null)
                {
                    seekiosNameAlertTextView.Text = seekios.SeekiosName;
                }
            }
            else
            {
                seekiosNameAlertTextView.Text = item.Title;
            }
        }

        /// <summary>
        /// Configure le second TextView de la liste des alertes qui est la description
        /// </summary>
        /// <param name="contentAlertTextView">objet description de l'alerte</param>
        public static void SetContentAlertTextView(AlertDTO item, TextView contentAlertTextView)
        {
            contentAlertTextView.Text = SeekiosApp.Helper.StringHelper.SetMaxSizeText(item.Content, 61);
        }

        /// <summary>
        /// Configure le second TextView de la liste des alertes qui est la description
        /// </summary>
        /// <param name="numberOfRecipient"></param>
        public static void SetNumberOfRecipient(int countOfRecipients, TextView numberOfRecipient)
        {
            numberOfRecipient.Text = countOfRecipients.ToString();
        }

        #endregion
    }
}