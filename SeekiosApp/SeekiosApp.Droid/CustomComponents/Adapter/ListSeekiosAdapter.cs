using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;
using SeekiosApp.Model.DTO;
using Android.Graphics;
using System.Linq;
using System;
using SeekiosApp.Extension;
using SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter;
using XamSvg;
using SeekiosApp.Enum;
using SeekiosApp.Droid.Helper;
using SeekiosApp.Helper;

namespace SeekiosApp.Droid.CustomComponents
{
    //TODO: bouton mode en vert quand on le clique
    public class ListSeekiosAdapter : BaseAdapter<SeekiosDTO>
    {
        #region ===== Attributs ===================================================================

        private Activity _context;
        private Action<SeekiosDTO> _onModeSelection;
        private Action<SeekiosDTO> _onMenuOpened;
        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();
        private bool _isfirstSharedSeekiosHandled = false;

        #endregion

        #region ===== Constructeur(s) =============================================================

        public ListSeekiosAdapter(Activity context)
           : base()
        {
            _context = context;
        }

        public ListSeekiosAdapter(Activity context, Action<SeekiosDTO> onModeSelection, Action<SeekiosDTO> onMenuOpened)
            : base()
        {
            _context = context;
            _onModeSelection = onModeSelection;
            _onMenuOpened = onMenuOpened;
        }

        #endregion

        #region ===== ListAdapter =================================================================

        public override long GetItemId(int position)
        {
            return position;
        }

        public override SeekiosDTO this[int position]
        {
            get { return App.Locator.ListSeekios.LsSeekios[position]; }
        }

        public override int Count
        {
            get { return App.Locator.ListSeekios.LsSeekios.Count; }
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            try
            {
                // We get the current seekios
                var item = this[position];
                // We get the recycle view
                ListSeekiosViewHolder holder = null;
                if (!_alreadyCreatedViews.ContainsKey(item.Idseekios)) _alreadyCreatedViews.Add(item.Idseekios, null);
                Android.Views.View view = _alreadyCreatedViews[item.Idseekios];
                if (view != null) holder = view.Tag as ListSeekiosViewHolder;
                if (holder == null)
                {
                    holder = new ListSeekiosViewHolder();
                    view = _context.LayoutInflater.Inflate(Resource.Layout.ListSeekiosRow, null);
                    view.Tag = holder;
                    holder.ImageSeekiosRoundedImageView = view.FindViewById<RoundedImageView>(Resource.Id.seekiosRow_seekiosImage);
                    holder.PowerSavingImageView = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.seekiosList_powerSavingImage);
                    holder.FirmwareUpdateImageView = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.seekiosList_firmwareUpdate);
                    holder.LastPositionTextView = view.FindViewById<TextView>(Resource.Id.seekiosRow_lastPosition);
                    holder.RootLayout = view.FindViewById<RelativeLayout>(Resource.Id.root_layout);
                    holder.SeekiosNameTextView = view.FindViewById<TextView>(Resource.Id.seekiosRow_seekiosName);
                    holder.ButtonRightLayout = view.FindViewById<RelativeLayout>(Resource.Id.myseekios_ButtonsRight);
                    holder.Seekios = item; // what is it needed for?
                    holder.HeaderLayout = view.FindViewById<LinearLayout>(Resource.Id.listSeekios_headerLayout);
                    holder.HeaderTitle = view.FindViewById<TextView>(Resource.Id.listSeekios_header_title);
                    holder.ModeLayout = view.FindViewById<LinearLayout>(Resource.Id.seekiosList_modeLayout);
                    holder.ModeSvgImageView = view.FindViewById<SvgImageView>(Resource.Id.seekiosList_modeImage);
                    holder.ModeTextView = view.FindViewById<TextView>(Resource.Id.seekiosList_modeImageText);
                    holder.AlertLayout = view.FindViewById<LinearLayout>(Resource.Id.seekiosList_alertLayout);
                    holder.AlertSvgImageView = view.FindViewById<SvgImageView>(Resource.Id.seekiosList_alertImage);
                    holder.AlertTextView = view.FindViewById<TextView>(Resource.Id.seekiosList_alertImageText);
                    _alreadyCreatedViews[item.Idseekios] = view;

                    // Seekios picture
                    if (!string.IsNullOrEmpty(item.SeekiosPicture))
                    {
                        var bytes = Convert.FromBase64String(item.SeekiosPicture);
                        var imageBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
                        holder.ImageSeekiosRoundedImageView.SetImageBitmap(imageBitmap);
                        imageBitmap.Dispose();
                    }
                    else holder.ImageSeekiosRoundedImageView.SetImageResource(Resource.Drawable.DefaultSeekios);
                }

                // Initialize primary color on all elements
                var primaryColor = _context.Resources.GetColor(Resource.Color.primary);
                var contentColor = _context.Resources.GetColor(Resource.Color.textColorTitle);
                holder.ModeTextView.SetTextColor(contentColor);
                holder.SeekiosNameTextView.SetTextColor(primaryColor);
                holder.LastPositionTextView.SetTextColor(contentColor);

                // Seekios need update ? 
                if (App.CurrentUserEnvironment.LastVersionEmbedded != null 
                    && item.VersionEmbedded_idversionEmbedded != App.CurrentUserEnvironment.LastVersionEmbedded.IdVersionEmbedded
                    && !App.CurrentUserEnvironment.LastVersionEmbedded.IsBetaVersion)
                {
                    if (item.IsInPowerSaving)
                    {
                        holder.FirmwareUpdateImageView.TranslationY = -AccessResources.Instance.SizeOf25Dip;
                    }
                    holder.FirmwareUpdateImageView.Visibility = ViewStates.Visible;
                }

                // Display or not power saving picture
                if (item.IsInPowerSaving) holder.PowerSavingImageView.Visibility = ViewStates.Visible;
                else holder.PowerSavingImageView.Visibility = ViewStates.Gone;

                // Seekios name
                if (item.SeekiosName.Length > 10)
                {
                    holder.SeekiosNameTextView.Text = item.SeekiosName.Substring(0, 9) + "...";
                }
                else holder.SeekiosNameTextView.Text = item.SeekiosName;

                // Last location
                if (item.LastKnownLocation_latitude == App.DefaultLatitude && item.LastKnownLocation_longitude == App.DefaultLongitude)
                {
                    // Display statement to say there is no position
                    holder.LastPositionTextView.Text = _context.Resources.GetString(Resource.String.listSeekios_lastPositionNone);
                }
                else
                {
                    if (item.LastKnownLocation_dateLocationCreation.HasValue && item.LastKnownLocation_dateLocationCreation.Value.Year != 1)
                    {
                        if (item.IsOnDemand)
                        {
                            // Display on refresh state (OnDemand)
                            var textToDisplay = _context.Resources.GetString(Resource.String.listSeekios_refreshPosition);
                            var _seekiosOnDemand = App.Locator.Map.LsSeekiosOnDemand.FirstOrDefault(x => x.Seekios.Idseekios == item.Idseekios);
                            if (_seekiosOnDemand != null)
                            {
                                int minutes = (int)_seekiosOnDemand.Timer.CountDown / 60;
                                int seconds = (int)_seekiosOnDemand.Timer.CountDown - (minutes * 60);
                                holder.LastPositionTextView.Text = textToDisplay + string.Format(" {00:00}:{01:00}", minutes, seconds);
                                _seekiosOnDemand.Timer.UpdateUI = () =>
                                {
                                    minutes = (int)_seekiosOnDemand.Timer.CountDown / 60;
                                    seconds = (int)_seekiosOnDemand.Timer.CountDown - (minutes * 60);
                                    holder.LastPositionTextView.Text = textToDisplay + string.Format(" {00:00}:{01:00}", minutes, seconds);
                                };
                                // Hidden the count down, specific UI when it's the first location
                                _seekiosOnDemand.Timer.Stopped = () =>
                                {
                                    if (item.LastKnownLocation_dateLocationCreation.HasValue)
                                    {
                                        holder.LastPositionTextView.Text = item.LastKnownLocation_dateLocationCreation.Value.FormatDateFromNow();
                                    }
                                    else holder.LastPositionTextView.Text = _context.Resources.GetString(Resource.String.listSeekios_lastPositionNone);
                                };
                            }
                            else holder.LastPositionTextView.Text = _context.Resources.GetString(Resource.String.listSeekios_refreshPosition);
                        }
                        else holder.LastPositionTextView.Text = item.LastKnownLocation_dateLocationCreation.Value.FormatDateFromNow();
                    }
                    else holder.LastPositionTextView.Text = _context.Resources.GetString(Resource.String.listSeekios_lastPositionNone);
                }

                // Update alert layout && low battery layout
                holder.AlertLayout.Visibility = ViewStates.Invisible;
                if (item.BatteryLife <= 20 && item.LastKnownLocation_dateLocationCreation.HasValue)
                {
                    holder.AlertSvgImageView.SetSvg(_context, Resource.Drawable.CriticalBattery);
                    holder.AlertTextView.Text = item.BatteryLife + "%";
                    holder.AlertLayout.Visibility = ViewStates.Visible;
                }
                else if (!item.IsLastSOSRead)
                {
                    holder.AlertSvgImageView.SetSvg(_context, Resource.Drawable.SOS, "2FAD62=da2e2e");
                    holder.AlertTextView.Text = _context.GetString(Resource.String.detailSeekios_sos);
                    holder.AlertLayout.Visibility = ViewStates.Visible;
                }

                // Logo of the mode
                var mode = App.CurrentUserEnvironment.LsMode.Where(el => el.Seekios_idseekios == item.Idseekios).FirstOrDefault();
                if (mode != null)
                {
                    holder.ModeLayout.Visibility = ViewStates.Visible;
                    holder.ButtonRightLayout.Visibility = ViewStates.Visible;
                    if (!item.HasGetLastInstruction
                        && !App.Locator.Map.LsSeekiosOnDemand.Any(x => x.Seekios.Idseekios == item.Idseekios)
                        && mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.RAS
                        && !item.IsRefreshingBattery
                        || !mode.DateModeActivation.HasValue)
                    {
                        if (item.IsInPowerSaving)
                        {
                            if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                            {
                                holder.ModeSvgImageView.SetSvg(_context, Resource.Drawable.ModeTracking, "62da73=c8c8c8");
                            }
                            if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                            {
                                holder.ModeSvgImageView.SetSvg(_context, Resource.Drawable.ModeDontMove, "62da73=c8c8c8");
                            }
                            else if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
                            {
                                holder.ModeSvgImageView.SetSvg(_context, Resource.Drawable.ModeZone, "62da73=c8c8c8");
                            }
                            holder.ModeTextView.Text = string.Format(_context.GetString(Resource.String.listSeekios_nextNoon), DateHelper.TimeLeftUntilNextNoon());
                        }
                        else
                        {
                            holder.ModeSvgImageView.SetSvg(_context, Resource.Drawable.CloudSync);
                            holder.ModeTextView.SetText(Resource.String.listSeekios_synchr);
                        }
                    }
                    // The seekios has moved or is out of the area
                    else if (mode.StatusDefinition_idstatusDefinition != 1)
                    {
                        holder.ModeLayout.Visibility = ViewStates.Visible;
                        switch (mode.ModeDefinition_idmodeDefinition)
                        {
                            case (int)ModeDefinitionEnum.ModeDontMove:
                                if (mode.StatusDefinition_idstatusDefinition == 3)
                                {
                                    holder.LastPositionTextView.SetText(Resource.String.modeDontMove_seekiosMoved);
                                    holder.ModeTextView.SetText(Resource.String.detailSeekios_dontMove);
                                    holder.ModeSvgImageView.SetSvg(_context, Resource.Drawable.ModeDontMove, "62da73=da2e2e");
                                }
                                break;
                            case (int)ModeDefinitionEnum.ModeZone:
                                if (mode.StatusDefinition_idstatusDefinition == 2)
                                {
                                    holder.LastPositionTextView.SetText(Resource.String.modeZone_seekiosOutOfZone);
                                    holder.ModeTextView.SetText(Resource.String.detailSeekios_zone);
                                    holder.ModeSvgImageView.SetSvg(_context, Resource.Drawable.ModeZone, "62da73=da2e2e");
                                }
                                break;
                            default:
                                holder.ModeLayout.Visibility = ViewStates.Gone;
                                break;
                        }
                        var colorRed = _context.Resources.GetColor(Resource.Color.color_red);
                        holder.ModeTextView.SetTextColor(colorRed);
                        holder.SeekiosNameTextView.SetTextColor(colorRed);
                        holder.LastPositionTextView.SetTextColor(colorRed);
                    }
                    // Seekios is in a mode
                    else
                    {
                        // Configuration of the mode layout
                        switch (mode.ModeDefinition_idmodeDefinition)
                        {
                            case 3:
                                holder.ModeSvgImageView.SetSvg(_context, Resource.Drawable.ModeTracking);
                                holder.ModeTextView.SetText(Resource.String.listSeekios_modeTracking);
                                break;
                            case 4:
                                holder.ModeSvgImageView.SetSvg(_context, Resource.Drawable.ModeDontMove);
                                holder.ModeTextView.SetText(Resource.String.listSeekios_modeDontMove);
                                break;
                            case 5:
                                holder.ModeSvgImageView.SetSvg(_context, Resource.Drawable.ModeZone);
                                holder.ModeTextView.SetText(Resource.String.listSeekios_modeZone);
                                break;
                        }
                    }
                }
                else
                {
                    var layoutParamTextView = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    layoutParamTextView.RightMargin = 0;
                    holder.SeekiosNameTextView.LayoutParameters = layoutParamTextView;
                    holder.ModeLayout.Visibility = ViewStates.Invisible;
                }
                if (!_isfirstSharedSeekiosHandled && item.User_iduser != App.CurrentUserEnvironment.User.IdUser)
                {
                    holder.HeaderLayout.Visibility = ViewStates.Visible;
                    holder.HeaderTitle.Text = _context.Resources.GetString(Resource.String.listSeekios_headertitle);
                    _isfirstSharedSeekiosHandled = true;
                }
                return view;
            }
            catch (NotSupportedException) { }
            return null;
        }

        #endregion
    }
}