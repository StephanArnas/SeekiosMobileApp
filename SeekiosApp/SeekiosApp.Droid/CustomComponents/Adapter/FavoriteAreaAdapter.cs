using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Views;
using Android.Widget;
using SeekiosApp.Model.DTO;
using SeekiosApp.Droid.Helper;
using GalaSoft.MvvmLight.Messaging;
using SeekiosApp.Droid.Messages;
using SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter;

namespace SeekiosApp.Droid.CustomComponents
{
    public class FavoriteAreaAdapter : BaseAdapter<FavoriteAreaDTO>
    {
        #region Attributs

        private Activity _context = null;
        private Action _dismiss = null;
        private Action _refresh = null;
        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

        #endregion

        #region Ctr(s)

        public FavoriteAreaAdapter(Activity context, Action dismiss, Action refresh) : base()
        {
            _context = context;
            _dismiss = dismiss;
            _refresh = refresh;
        }

        #endregion

        #region Surcharge

        public override long GetItemId(int position)
        {
            return position;
        }

        public override FavoriteAreaDTO this[int position]
        {
            get { return App.CurrentUserEnvironment.LsFavoriteArea[position]; }
        }

        public override int Count
        {
            get { return App.CurrentUserEnvironment.LsFavoriteArea.Count; }
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            // item en cours de traitement
            var item = App.CurrentUserEnvironment.LsFavoriteArea[position];

            // pas de vue à utiliser ? création d'une vue
            FavoriteAreaViewHolder holder = null;
            if (!_alreadyCreatedViews.ContainsKey(item.IdfavoriteArea))
                _alreadyCreatedViews.Add(item.IdfavoriteArea, null);
            Android.Views.View view = _alreadyCreatedViews[item.IdfavoriteArea];
            // Si on a récupéré la vue on récupère le holder dans son tag
            if (view != null) holder = view.Tag as FavoriteAreaViewHolder;
            // Si le holder n'est pas défini, on le fait et on crée la vue
            if (holder == null)
            {
                holder = new FavoriteAreaViewHolder();
                view = _context.LayoutInflater.Inflate(Resource.Layout.ZoneFavoriteRow, null);
                view.Tag = holder;
                // récupération des objets de la vue
                holder.AreaName = view.FindViewById<TextView>(Resource.Id.favoriteArea_areaName);
                holder.PointsCount = view.FindViewById<TextView>(Resource.Id.favoriteArea_pointsCount);
                holder.AreaGeodesic = view.FindViewById<TextView>(Resource.Id.favoriteArea_area);
                holder.DeleteAlerte = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.favoriteArea_delete);
                holder.RootGrid = view.FindViewById<GridLayout>(Resource.Id.favoriteArea_rootGrid);

                // boutton supprimer
                holder.DeleteAlerte.Click += new EventHandler(async (o, e) =>
                {
                    await App.Locator.ModeZone.DeleteFavoriteArea(item);
                    if (_refresh != null)
                    {
                        _refresh.Invoke();
                    }
                });

                // clique sur l'item (root)
                holder.RootGrid.Click += new EventHandler((o, e) =>
                {
                    App.Locator.ModeZone.SelectedFavoriteArea = item;
                    if (_dismiss != null)
                    {
                        // quitte la popup (dismiss)
                        _dismiss.Invoke();
                    }
                });
                _alreadyCreatedViews[item.IdfavoriteArea] = view;
            }

            // initialisation des variables de la vue
            holder.AreaName.Text = item.AreaName;
            holder.PointsCount.Text = string.Format("{0} points", item.PointsCount);
            holder.AreaGeodesic.Text = AreaHelper.SerializeArea(item.AreaGeodesic);

            return view;
        }

        #endregion
    }
}