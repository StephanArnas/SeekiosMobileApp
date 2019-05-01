using System;
using UIKit;
using SeekiosApp.ViewModel;
using SeekiosApp.iOS.Views.BaseView;
using System.Linq;
using SeekiosApp.iOS.ControlManager;
using SeekiosApp.iOS.CustomComponents.FMCalendar;
using System.Text;

namespace SeekiosApp.iOS
{
    public partial class MapHistoricView : BaseViewControllerMap
    {
        #region ===== Attributs ===================================================================

        private UITapGestureRecognizer _nextButtonGesture = null;
        private UITapGestureRecognizer _previousButtonGesture = null;

        #endregion

        #region ===== Constructor =================================================================

        public MapHistoricView(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _nextButtonGesture = new UITapGestureRecognizer(UpperDateLabel_Click);
            _previousButtonGesture = new UITapGestureRecognizer(LowerDateLabel_Click);
            UpperDateLabel.AddGestureRecognizer(_nextButtonGesture);
            LowerDateLabel.AddGestureRecognizer(_previousButtonGesture);
            App.Locator.MapAllSeekios.HasRemovedButtonAnnotationView = true;

            App.Locator.Historic.PropertyChanged += Historic_PropertyChanged;
            Slider.ValueChanged += OnSliderValueChanged;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            App.Locator.Historic.IsHistoryActivated = true;
        }

        public override void ViewWillDisappear(bool animated)
        {
            App.Locator.MapAllSeekios.HasRemovedButtonAnnotationView = false;
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
            if (MapViewControl != null)
            {
                MapViewControl.MapType = MapKit.MKMapType.Hybrid;
                MapViewControl.Delegate = null;
                MapViewControl.RemoveFromSuperview();
                MapViewControl.Dispose();
                MapViewControl = null;
            }
        }

        #endregion

        #region ===== Initialisze View ============================================================

        /// <summary>
        /// Set up  data and style to the view
        /// </summary>
        public override void SetDataAndStyleToView()
        {
            Title = Application.LocalizedString("MapHistoricTitle");

            //Initialise all strings
            InitialiseAllStrings();

            // display the loading layout
            if (!App.Locator.Historic.LsSeekiosLocations.Any(a => a.IdSeekios == MapViewModelBase.Seekios.Idseekios))
            {
                LoadingIndicator.Hidden = false;
                LoadingIndicator.StartAnimating();
            }

            // set up the text of the current position
            PositionLabel.Text = string.Format(Application.LocalizedString("HistoricPosition"), 1, 1);

            // set up the dates
            UpperDateLabel.Text = App.Locator.Historic.CurrentUpperDate.ToString("M");
            LowerDateLabel.Text = App.Locator.Historic.CurrentLowerDate.ToString("M");
            UpperDateLabel.AdjustsFontSizeToFitWidth = true;
            LowerDateLabel.AdjustsFontSizeToFitWidth = true;

            // get the data for one month ago
            System.Threading.Tasks.Task result = App.Locator.Historic.GetLocationsBySeekios(
                MapViewModelBase.Seekios.Idseekios
                , App.Locator.Historic.CurrentLowerDate
                , App.Locator.Historic.CurrentUpperDate);

            // set up the call back when data are load
            App.Locator.Historic.OnGetLocationsComplete += Historic_OnGetLocationsComplete; ;

            // set the seekbar with default values
            Slider.MaxValue = 0;
            Slider.Value = 0;

            if (App.Locator.Historic.LsSeekiosLocations != null &&
                App.Locator.Historic.LsSeekiosLocations.Any(a => a.IdSeekios == MapViewModelBase.Seekios.Idseekios))
            {
                UpdateView();
                LoadingIndicator.Hidden = true;
            }
            else
            {
                PositionLabel.Text = Application.LocalizedString("NoHistoricPosition");
                UpperDateLabel.Hidden = true;
                LowerDateLabel.Hidden = true;
            }
        }

        /// <summary>
        /// Set up the business logic for the map
        /// </summary>
        public override void SetMapControlManager()
        {
            _mapControlManager = new MapControlManager(MapViewControl
                , this
                , FocusOnSeekiosButton
                , ChangeMapTypeButton
                , MapZoomInButton
                , MapZoomOutButton
                , MapViewModelBase.Seekios.Idseekios.ToString());

            App.Locator.Historic.MapControlManager = _mapControlManager;
            App.Locator.Historic.InitMap();

            _mapControlManager.RegisterMethodes();
        }

        #endregion

        #region ===== Private Methodes ============================================================

        private void InitialiseAllStrings()
        {
            OldDateLabel.Text = Application.LocalizedString("OldDate");
            RecentDateLabel.Text = Application.LocalizedString("RecentDate");
            LowerDateLabel.Text = Application.LocalizedString("LowerDateExample");
            UpperDateLabel.Text = Application.LocalizedString("UpperDateExample");
            NextPositionButton.SetTitle(Application.LocalizedString("NextPosition"), UIControlState.Normal);
            PreviousPositionButton.SetTitle(Application.LocalizedString("PreviousPosition"), UIControlState.Normal);
        }

        /// <summary>
        /// Update the view
        /// </summary>
        private void UpdateView()
        {
            var item = App.Locator.Historic.SelectedSeekiosLocations;
            var o = App.Locator.Historic.LsSeekiosLocations[0];
            if (item != null)
            {
                if (App.Locator.Historic.SelectedSeekiosLocations.LsLocations.Count > 0)
                {
                    App.Locator.Historic.SelectedLocationHistory = App.Locator.Historic.SelectedSeekiosLocations.LsLocations[App.Locator.Historic.SelectedSeekiosLocations.LsLocations.Count - 1];
                    Slider.MaxValue = item.LsLocations.Count - 1;
                    Slider.Value = item.LsLocations.Count - 1;

                    PositionLabel.Text = string.Format(Application.LocalizedString("HistoricPosition"), 1, item.LsLocations.Count);

                    UpperDateLabel.Hidden = false;
                    LowerDateLabel.Hidden = false;
                    NextPositionButton.Hidden = false;
                    PreviousPositionButton.Hidden = false;
                }
                else
                {
                    PositionLabel.Text = Application.LocalizedString("NoHistoricPosition");
                    NextPositionButton.Hidden = true;
                    PreviousPositionButton.Hidden = true;
                    LoadingIndicator.Hidden = true;
                }
            }
        }

        /// <summary>
        /// Update the current position in the text view
        /// </summary>
        private void UpdateCurrentPosition(int index)
        {
            App.Locator.Historic.SelectedLocationHistory = App.Locator.Historic.SelectedSeekiosLocations.LsLocations[index];
            PositionLabel.Text = string.Format(Encoding.UTF8.GetString(Encoding.Default.GetBytes(Application.LocalizedString("HistoricPosition")))
                , index + 1
                , App.Locator.Historic.SelectedSeekiosLocations.LsLocations.Count);
        }

        #endregion

        #region ===== Event =======================================================================

        /// <summary>
        /// Trigger on the ViewModel 
        /// </summary>
        private void Historic_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LsLocations")
            {
                var item = App.Locator.Historic.LsSeekiosLocations.FirstOrDefault(f => f.IdSeekios == MapViewModelBase.Seekios.Idseekios);
                if (item != null && item.LimitLowerDate > App.Locator.Historic.CurrentLowerDate)
                    LowerDateLabel.Text = item.LimitLowerDate.Value.ToString("M");
                UpdateView();
                Slider.Hidden = true;
            }
        }

        /// <summary>
        /// Trigger when the view model has load data
        /// </summary>
        private async System.Threading.Tasks.Task Historic_OnGetLocationsComplete(object arg1, EventArgs arg2)
        {
            LoadingIndicator.Hidden = true;
            if (App.Locator.Historic.LsSeekiosLocations.Any(a => a.IdSeekios == MapViewModelBase.Seekios.Idseekios))
            {
                UpdateView();
                NextPositionButton.Hidden = false;
                PreviousPositionButton.Hidden = false;
            }
            else
            {
                PositionLabel.Text = Application.LocalizedString("NoHistoricPosition");
                NextPositionButton.Hidden = true;
                PreviousPositionButton.Hidden = true;
            }
            if ((bool)arg1)
            {
                LowerDateLabel.Text = App.Locator.Historic.CurrentLowerDate.ToString("M");
            }
        }

        /// <summary>
        /// Slider position changed
        /// </summary>
        private void OnSliderValueChanged(object sender, EventArgs e)
        {
            if (App.Locator.Historic.SelectedSeekiosLocations != null
                && App.Locator.Historic.SelectedSeekiosLocations.LsLocations != null
                && App.Locator.Historic.SelectedSeekiosLocations.LsLocations.Count > 0)
            {
                App.Locator.Historic.SelectedLocationHistory = App.Locator.Historic.SelectedSeekiosLocations.LsLocations[(int)Math.Round(Slider.Value)];
                PositionLabel.Text = string.Format(Application.LocalizedString("HistoricPosition")
                    , Math.Round(Slider.Value) + 1
                    , App.Locator.Historic.SelectedSeekiosLocations.LsLocations.Count);
            }
        }

        /// <summary>
        /// On click upper date
        /// </summary>
        private void UpperDateLabel_Click()
        {
            if (App.Locator.Historic != null && App.Locator.Historic.SelectedSeekiosLocations != null)
            {
                var actionSheetAlert = UIAlertController.Create(null, Application.LocalizedString("RecentDate_n"), UIAlertControllerStyle.Alert);
                var fmCalendar = new FMCalendar(new CoreGraphics.CGRect(0, 60, 267f, 248f));

                fmCalendar.SelectedDate = App.Locator.Historic.CurrentUpperDate;
                fmCalendar.IsDateAvailable = (date) =>
                {
                    return (date <= DateTime.Now
                     && date > App.Locator.Historic.CurrentLowerDate);
                };

                fmCalendar.DateSelected += (date) =>
                {
                    if (date <= DateTime.Now
                    && date > App.Locator.Historic.CurrentLowerDate)
                    {
                        if (date != App.Locator.Historic.CurrentUpperDate)
                        {
                            LoadingIndicator.Hidden = false;
                            UpperDateLabel.Text = date.Date.ToString("M");

                            App.Locator.Historic.GetLocationsBySeekios(
                            MapViewModelBase.Seekios.Idseekios
                            , App.Locator.Historic.CurrentLowerDate
                            , date
                            , true);

                            App.Locator.Historic.CurrentUpperDate = date;
                        }
                        LoadingIndicator.Hidden = true;
                    }
                };

                actionSheetAlert.Add(fmCalendar);
                actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Close"), UIAlertActionStyle.Cancel, null));
                PresentViewController(actionSheetAlert, true, null);
            }
        }

        /// <summary>
        /// On click lower date
        /// </summary>
        private void LowerDateLabel_Click()
        {
            if (App.Locator.Historic != null && App.Locator.Historic.SelectedSeekiosLocations != null)
            {
                var actionSheetAlert = UIAlertController.Create(null, Application.LocalizedString("OldDate_n"), UIAlertControllerStyle.Alert);
                var fmCalendar = new FMCalendar(new CoreGraphics.CGRect(0, 60, 267f, 248f));

                fmCalendar.SelectedDate = App.Locator.Historic.CurrentLowerDate;
                fmCalendar.CurrentMonthYear = App.Locator.Historic.CurrentLowerDate;
                fmCalendar.GoToDate(App.Locator.Historic.CurrentLowerDate);
                fmCalendar.IsDateAvailable = (date) =>
                {
                    return (date >= App.Locator.Historic.SelectedSeekiosLocations.LimitLowerDate
                     && date <= App.Locator.Historic.CurrentUpperDate);
                };

                fmCalendar.DateSelected += (date) =>
                {
                    if (date >= App.Locator.Historic.SelectedSeekiosLocations.LimitLowerDate
                    && date <= App.Locator.Historic.CurrentUpperDate)
                    {
                        if (date != App.Locator.Historic.CurrentLowerDate)
                        {
                            LoadingIndicator.Hidden = false;
                            LowerDateLabel.Text = date.Date.ToString("M");

                            App.Locator.Historic.GetLocationsBySeekios(
                            MapViewModelBase.Seekios.Idseekios
                            , date
                            , App.Locator.Historic.CurrentUpperDate
                            , true);

                            App.Locator.Historic.CurrentLowerDate = date;
                        }
                        LoadingIndicator.Hidden = true;
                    }
                };

                actionSheetAlert.Add(fmCalendar);
                actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Close"), UIAlertActionStyle.Cancel, null));

                PresentViewController(actionSheetAlert, true, null);
            }
        }

        /// <summary>
        /// Display the next position
        /// </summary>
        partial void NextPositionButton_Click(UIButton sender)
        {
            if (App.Locator.Historic.SelectedSeekiosLocations != null
                    && Slider.Value < App.Locator.Historic.SelectedSeekiosLocations.LsLocations.Count - 1)
            {
                var index = Slider.Value + 1;
                Slider.Value = index;
                UpdateCurrentPosition((int)index);
            }
        }

        /// <summary>
        /// Display the previous position
        /// </summary>
        partial void PreviousPositionButton_Click(UIButton sender)
        {
            if (Slider.Value > 0)
            {
                var index = Slider.Value - 1;
                Slider.Value = index;
                UpdateCurrentPosition((int)index);
            }
        }

        #endregion
    }
}