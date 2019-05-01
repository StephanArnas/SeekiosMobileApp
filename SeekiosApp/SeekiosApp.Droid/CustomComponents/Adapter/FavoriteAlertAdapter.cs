using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.Res;
using SeekiosApp.Model.DTO;
using System.Collections.ObjectModel;
using SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter;

namespace SeekiosApp.Droid.CustomComponents
{
    public class FavoriteAlertAdapter : BaseAdapter<AlertFavoriteDTO>
    {
        #region ===== Attributs ===================================================================

        private Activity context;
        private Action refreshView;
        private Action dissmissPopup;
        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

        #endregion

        #region ===== Constructeur(s) =============================================================

        /// <summary>
        /// Initialise le adapter pour les alertes favorites dans l'activity AlertActivity
        /// </summary>
        public FavoriteAlertAdapter(Activity context, Action refreshView, Action dissmissPopup) : base()
        {
            this.context = context;
            this.refreshView = refreshView;
            this.dissmissPopup = dissmissPopup;
        }

        #endregion

        #region ===== ListAdapter =================================================================

        public override AlertFavoriteDTO this[int position]
        {
            get { return App.CurrentUserEnvironment.GetAlertsFavoritesFromAlertType(App.Locator.Alert.IdAlertTypeEnum.Value)[position]; }
        }

        public override int Count
        {
            get { return App.CurrentUserEnvironment.GetAlertsFavoritesFromAlertType(App.Locator.Alert.IdAlertTypeEnum.Value).Count; }
        }

        public override long GetItemId(int position)
        {
            return 1;//App.Locator.Alert.LsAlertFavorite[position].IdAlertFavorite;
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            // item courant
            var item = App.CurrentUserEnvironment.GetAlertsFavoritesFromAlertType(App.Locator.Alert.IdAlertTypeEnum.Value)[position];
            // initialisation de la vue (si première fois) ou récupération de la vue
            FavoriteAlertViewHolder holder = null;
            if (!_alreadyCreatedViews.ContainsKey(item.IdAlertFavorite))
                _alreadyCreatedViews.Add(item.IdAlertFavorite, null);
            Android.Views.View view = _alreadyCreatedViews[item.IdAlertFavorite];
            // Si on a récupéré la vue on récupère le holder dans son tag
            if (view != null) holder = view.Tag as FavoriteAlertViewHolder;
            // Si le holder n'est pas défini, on le fait et on crée la vue
            if (holder == null)
            {
                holder = new FavoriteAlertViewHolder();
                view = context.LayoutInflater.Inflate(Resource.Layout.AlertFavoriteRow, null);
                view.Tag = holder;

                // récupération des objets de la vue
                holder.AlertContains = view.FindViewById<TextView>(Resource.Id.alerts_alertContains);
                holder.EmailObject = view.FindViewById<TextView>(Resource.Id.alertsFavorite_emailObject);
                holder.DeleteAlertButton = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.alerts_alertDelete);
                holder.AlertLayoutFavorite = view.FindViewById<RelativeLayout>(Resource.Id.listAlertsRow_alertFavorite);

                // bouton vers detail de l'alerte depuis la liste des favoris
                SetAlertLayoutFavorite(item, holder.AlertLayoutFavorite);

                // Supprime l'alerte actuelle dans la liste des favoris
                holder.DeleteAlertButton.Click += new EventHandler(async (o, e) =>
                {
                    await App.Locator.Alert.DeleteAlertFavorite(item);
                    refreshView.Invoke();
                    if (App.CurrentUserEnvironment.GetAlertsFavoritesFromAlertType(App.Locator.Alert.IdAlertTypeEnum.Value).Count == 0)
                    {
                        dissmissPopup.Invoke();
                    }
                });

                _alreadyCreatedViews[item.IdAlertFavorite] = view;
            }

            // contenu de l'alerte favorite
            holder.AlertContains.Text = SeekiosApp.Helper.StringHelper.SetMaxSizeText(item.Content, 31);
            holder.EmailObject.Text = SeekiosApp.Helper.StringHelper.SetMaxSizeText(item.EmailObject, 31);
            
            // retourne la view 
            return view;
        }

        /// <summary>
        /// Configure le click de l'alerte pour naviguer au détail de l'alerte depuis la liste des favoris
        /// </summary>
        /// <param name="AlertLayoutFavorite">root layout</param>
        private void SetAlertLayoutFavorite(AlertFavoriteDTO item, RelativeLayout alertLayoutFavorite)
        {

            alertLayoutFavorite.Click += ((o, e) =>
            {
                App.Locator.Alert.ContentAlert = item.Content;
                App.Locator.Alert.TitleAlert = item.EmailObject;
                dissmissPopup.Invoke();
            });
        }

        #endregion
    }
}