using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Widget;
using SeekiosApp.Model.DTO;
using Android.App;
using SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter;
using SeekiosApp.Enum.FromDataBase;
using SeekiosApp.Enum;

namespace SeekiosApp.Droid.CustomComponents.Adapter
{
    public class TransactionHistoricAdapter : BaseAdapter<OperationDTO>
    {
        #region ===== Attributs ===================================================================

        private Activity _context;
        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

        #endregion

        #region ===== Constructeur(s) =============================================================

        public TransactionHistoricAdapter(Activity context)
           : base()
        {
            _context = context;
        }

        #endregion

        public override OperationDTO this[int position]
        {
            get
            {
                return App.Locator.TransactionHistoric.LsOperation[position];
            }
        }

        public override int Count
        {
            get
            {
                return App.Locator.TransactionHistoric.LsOperation.Count();
            }
        }

        public override long GetItemId(int position)
        {
            return App.Locator.TransactionHistoric.LsOperation[position].IdOperation;
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            OperationDTO operation = this[position];

            // On essaie de récupérer la vue potentiellement déjà créé
            TransactionHistoricViewHolder holder = null;
            if (!_alreadyCreatedViews.ContainsKey(operation.IdOperation))
                _alreadyCreatedViews.Add(operation.IdOperation, null);
            Android.Views.View view = _alreadyCreatedViews[operation.IdOperation];
            // Si on a récupéré la vue on récupère le holder dans son tag
            if (view != null) holder = view.Tag as TransactionHistoricViewHolder;
            // Si le holder n'est pas défini, on le fait et on crée la vue
            if (holder == null)
            {
                holder = new TransactionHistoricViewHolder();
                view = _context.LayoutInflater.Inflate(Resource.Layout.TransactionHistoricRow, null);
                view.Tag = holder;
                // récupération des objets de la vue
                holder.OperationSvgImageView = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.transactionHistoric_historicImgActionClick);
                holder.OperationTitle = view.FindViewById<TextView>(Resource.Id.transactionHistoric_title);
                holder.OperationTitle.SetTypeface(null, Android.Graphics.TypefaceStyle.BoldItalic);
                holder.OperationSubtitle = view.FindViewById<TextView>(Resource.Id.transactionHistoric_subtitle);
                holder.OperationDate = view.FindViewById<TextView>(Resource.Id.transactionHistoric_date);
                holder.OperationCreditAmount = view.FindViewById<TextView>(Resource.Id.transactionHistoric_creditAmount);

                _alreadyCreatedViews[operation.IdOperation] = view;

                //Init date
                var date = operation.DateEnd.Value.ToLocalTime();
                var time = string.Format("{0}h{1:00}", date.Hour, date.Minute);
                holder.OperationDate.Text = string.Format(_context.Resources.GetString(Resource.String.transactionHistoric_date), date.ToShortDateString(), time);

                //Seekios Name
                if (operation.IdSeekios != null)
                {
                    if (App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.Idseekios == operation.IdSeekios) != null)
                        holder.OperationSubtitle.Text = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.Idseekios == operation.IdSeekios).SeekiosName;
                    else holder.OperationSubtitle.Text = _context.Resources.GetString(Resource.String.transactionHistoric_deletedSeekios);
                }

                //Credit amount + or -
                holder.OperationCreditAmount.Text = operation.CreditAmount.ToString();
                if (operation.CreditAmount >= 0)
                {
                    holder.OperationCreditAmount.SetTextColor(_context.Resources.GetColor(Resource.Color.primary));
                    holder.OperationCreditAmount.Text = string.Format("+{0}", operation.CreditAmount.ToString());
                }

                //Initialize components with respect to the operationType
                switch (operation.OperationType)
                {
                    default:
                        break;
                    case (int)OperationTypeEnum.ConfigureMode:
                        var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Idmode == operation.IdMode);
                        if (mode == null)
                        {
                            holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transactionHistoric_activeNewMode);
                            holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.CloudSync);
                            break;
                        }
                        var modeIdDefinition = mode.ModeDefinition_idmodeDefinition;
                        if (modeIdDefinition == (int)ModeDefinitionEnum.ModeZone)
                        {
                            holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transactionHistoric_zone);
                            holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.ModeZone);
                        }
                        if (modeIdDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                        {
                            holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transactionHistoric_dontmove);
                            holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.ModeDontMove);
                        }
                        if (modeIdDefinition == (int)ModeDefinitionEnum.ModeTracking)
                        {
                            holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transactionHistoric_tracking);
                            holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.ModeTracking);
                        }
                        break;
                    case (int)OperationTypeEnum.RefreshBattery:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transactionHistoric_refreshBattery);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.Battery);
                        break;
                    case (int)OperationTypeEnum.RefreshPosition:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transactionHistoric_refreshPosition);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.RefreshSeekios);
                        break;
                    case (int)OperationTypeEnum.SeekiosMonthlyGift:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transactionHistoric_monthlyCredits);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.Credit);
                        break;
                    case (int)OperationTypeEnum.SendSOS:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transactionHistoric_sos);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.SOS);
                        break;
                    case (int)OperationTypeEnum.AddDontMoveTrackingPosition:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transactionHistoric_dontMoveTracking);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.ModeDontMove);
                        break;
                    case (int)OperationTypeEnum.AddTrackingPosition:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transactionHistoric_tracking);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.ModeTracking);
                        break;
                    case (int)OperationTypeEnum.AddZoneTrackingPosition:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transactionHistoric_zoneTracking);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.ModeZone);
                        break;
                    case (int)OperationTypeEnum.BoughtCredits1:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transaction_observationPack);
                        holder.OperationSubtitle.Text = _context.Resources.GetString(Resource.String.transaction_instantPack);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.Credit);
                        break;
                    case (int)OperationTypeEnum.BoughtCredits2:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transaction_discoveryPack);
                        holder.OperationSubtitle.Text = _context.Resources.GetString(Resource.String.transaction_instantPack);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.Credit);
                        break;
                    case (int)OperationTypeEnum.BoughtCredits3:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transaction_explorationPack);
                        holder.OperationSubtitle.Text = _context.Resources.GetString(Resource.String.transaction_instantPack);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.Credit);
                        break;
                    case (int)OperationTypeEnum.BoughtCredits4:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transaction_adventurePack);
                        holder.OperationSubtitle.Text = _context.Resources.GetString(Resource.String.transaction_instantPack);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.Credit);
                        break;
                    case (int)OperationTypeEnum.BoughtCredits5:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transaction_epicPack);
                        holder.OperationSubtitle.Text = _context.Resources.GetString(Resource.String.transaction_instantPack);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.Credit);
                        break;
                    case (int)OperationTypeEnum.BoughtCredits1Subscription:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transaction_instantPack);
                        holder.OperationSubtitle.Text = _context.Resources.GetString(Resource.String.transaction_subscriptionPack);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.Credit);
                        break;
                    case (int)OperationTypeEnum.BoughtCredits2Subscription:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transaction_discoveryPack);
                        holder.OperationSubtitle.Text = _context.Resources.GetString(Resource.String.transaction_subscriptionPack);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.Credit);
                        break;
                    case (int)OperationTypeEnum.BoughtCredits3Subscription:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transaction_explorationPack);
                        holder.OperationSubtitle.Text = _context.Resources.GetString(Resource.String.transaction_subscriptionPack);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.Credit);
                        break;
                    case (int)OperationTypeEnum.BoughtCredits4Subscription:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transaction_adventurePack);
                        holder.OperationSubtitle.Text = _context.Resources.GetString(Resource.String.transaction_subscriptionPack);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.Credit);
                        break;
                    case (int)OperationTypeEnum.BoughtCredits5Subscription:
                        holder.OperationTitle.Text = _context.Resources.GetString(Resource.String.transaction_epicPack);
                        holder.OperationSubtitle.Text = _context.Resources.GetString(Resource.String.transaction_subscriptionPack);
                        holder.OperationSvgImageView.SetSvg(_context, Resource.Drawable.Credit);
                        break;
                }
            }

            return view;
        }
    }
}