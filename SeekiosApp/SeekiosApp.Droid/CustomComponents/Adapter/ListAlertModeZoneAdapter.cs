using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;
using SeekiosApp.Model.DTO;
using Android.Views.Animations;
using GalaSoft.MvvmLight.Views;
using Android.Graphics;
using System.Linq;
using System;
using SeekiosApp.Extension;
using SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter;
using XamSvg;
using System.Threading;
using SeekiosApp.Droid.Services;
using Android.Content.Res;
using SeekiosApp.Droid.Helper;
using SeekiosApp.Enum;

namespace SeekiosApp.Droid.CustomComponents
{
    //TODO: bouton mode en vert quand on le clique
    public class ListAlertModeZoneAdapter : BaseAdapter<AlertDTO>
    {
        #region ===== Attributs ===================================================================

        private Activity _context;
        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

        #endregion

        #region ===== Constructeur(s) =============================================================

        public ListAlertModeZoneAdapter(Activity context) : base()
        {
            _context = context;
        }

        #endregion

        #region ===== ListAdapter =================================================================

        public override long GetItemId(int position)
        {
            return position;

        }
        public override AlertDTO this[int position]
        {
            get
            {
                if (App.Locator.Alert.ModeDefinition == ModeDefinitionEnum.ModeZone)
                {
                    return App.Locator.ModeZone.LsAlertsModeZone[position];
                }
                else if (App.Locator.Alert.ModeDefinition == ModeDefinitionEnum.ModeDontMove)
                {
                    return App.Locator.ModeDontMove.LsAlertsModeDontMove[position];
                }
                else return null;
            }
        }

        public override int Count
        {
            get
            {
                if (App.Locator.Alert.ModeDefinition == ModeDefinitionEnum.ModeZone)
                {
                    return App.Locator.ModeZone.LsAlertsModeZone.Count;
                }
                else if (App.Locator.Alert.ModeDefinition == ModeDefinitionEnum.ModeDontMove)
                {
                    return App.Locator.ModeDontMove.LsAlertsModeDontMove.Count;
                }
                else return 0;
            }
        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            if (App.Locator.ModeZone.InvalidViews) InvalidateAllViews();
            // on récupère l'alerte en question
            var item = this[position];
            // on essaie de récupérer la vue potentiellement déjà créé
            ListAlertModeZoneViewHolder holder = null;
            if (!_alreadyCreatedViews.ContainsKey(position))
            {
                _alreadyCreatedViews.Add(position, null);
            }
            Android.Views.View view = _alreadyCreatedViews[position];
            // si on a récupéré la vue on récupère le holder dans son tag
            if (view != null) holder = view.Tag as ListAlertModeZoneViewHolder;
            // si le holder n'est pas défini, on le fait et on crée la vue
            if (holder == null)
            {
                holder = new ListAlertModeZoneViewHolder();
                view = _context.LayoutInflater.Inflate(Resource.Layout.ListAlertModeZoneRow, null);//pour une ligne ?
                view.Tag = holder;
                holder.titre = view.FindViewById<TextView>(Resource.Id.listAlertsModeZoneRow_titre);
                holder.description = view.FindViewById<TextView>(Resource.Id.listAlertsModeZoneRow_contenu);
                holder.nbrRecipients = view.FindViewById<TextView>(Resource.Id.listAlertsModeZoneRow_nbrRecipients);
                {
                    var alertWithRecipient = ((AlertWithRecipientDTO)item);
                    // updates the contents of the holder
                    UpdateHolderContents(holder, alertWithRecipient);
                    // attaches the click event (edit alert) on the view
                    view.Click += ((object o, EventArgs e) =>
                    {
                        App.Locator.ModeZone.GoToAlertDetail(alertWithRecipient, position);
                    });
                }
                _alreadyCreatedViews[position] = view;
            }
            // if the holder exists, we may need to update its properties
            else
            {
                UpdateHolderContents(holder, (AlertWithRecipientDTO)item);
            }
            return view;
        }

        /// <summary>
        /// Updates the content of one holder
        /// </summary>
        private void UpdateHolderContents(ListAlertModeZoneViewHolder holder, AlertWithRecipientDTO contents)
        {
            holder.titre.Text = contents.Title;
            holder.description.Text = contents.Content;
            holder.nbrRecipients.Text = contents.LsRecipients != null ? contents.LsRecipients.Count.ToString() : "0";
        }

        /// <summary>
        /// Flags all views as obsolete. detroys all views
        /// </summary>
        public void InvalidateAllViews()
        {
            _alreadyCreatedViews.Clear();
            App.Locator.ModeZone.InvalidViews = false;
        }


        #endregion

        #region ===== Destructeur =================================================================

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}