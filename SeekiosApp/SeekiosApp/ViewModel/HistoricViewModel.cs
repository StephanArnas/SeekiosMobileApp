using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using SeekiosApp.Enum;
using SeekiosApp.Helper;
using SeekiosApp.Interfaces;
using SeekiosApp.Model.APP;
using SeekiosApp.Model.DTO;
using SeekiosApp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SeekiosApp.ViewModel
{
    public class HistoricViewModel : ViewModelBase
    {
        #region ===== Attributs ===================================================================

        private Interfaces.IDialogService _dialogService;
        private IDataService _dataService;
        private LocationDTO _selectedLocationHistory;
        public bool IsHistoryActivated = false;
        public int LimitOfLocations = 300;

        public event Func<object, EventArgs, Task> OnGetLocationsComplete;

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Map control manager</summary>
        public IMapControlManager MapControlManager { get; set; }

        /// <summary>Data access service</summary>
        public List<SeekiosLocations> LsSeekiosLocations { get; set; }

        /// <summary>Start of the date interval of time we want to get location information</summary>
        public DateTime CurrentLowerDate { get; set; }

        /// <summary>End of the date interval of time we want to get location information</summary>
        public DateTime CurrentUpperDate { get; set; }

        /// <summary>Location history selected</summary>
        public LocationDTO SelectedLocationHistory
        {
            get { return _selectedLocationHistory; }
            set
            {
                _selectedLocationHistory = value;
                if (value != null && MapControlManager != null) MapControlManager.ChangeSelectedLocationHistory(value);
            }
        }

        /// <summary>The current seekios locations</summary>
        public SeekiosLocations SelectedSeekiosLocations { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public HistoricViewModel(IDataService dataService, Interfaces.IDialogService dialogService)
        {
            _dataService = dataService;
            _dialogService = dialogService;
            LsSeekiosLocations = new List<SeekiosLocations>();
            CurrentLowerDate = DateTime.Now.AddMonths(-1);
            CurrentUpperDate = DateTime.Now;
        }

        #endregion

        #region ===== Public Methods ==============================================================

        /// <summary>
        /// Get the locations on the last month
        /// </summary>
        public async Task GetLocationsBySeekios(int idSeekios, DateTime lowerDate, DateTime upperDate, bool requestNewData = false)
        {
            // if there is any seekios already present in the list
            // we don't need to take the data from the bdd
            if (requestNewData || (LsSeekiosLocations != null && !LsSeekiosLocations.Any(a => a.IdSeekios == idSeekios)))
            {
                // if the date the user wants if lower than the actual date
                // we need to take the data from the bdd
                if (lowerDate <= CurrentLowerDate)
                {
                    if (LsSeekiosLocations.Any(a => a.IdSeekios == idSeekios))
                    {
                        try
                        {
                            var index = LsSeekiosLocations.FindIndex(0, LsSeekiosLocations.Count, (e) => e.IdSeekios == idSeekios);
                            SelectedSeekiosLocations.LsLocations = await _dataService.Locations(idSeekios, lowerDate, upperDate);
                            if (null != SelectedSeekiosLocations.LsLocations && index > -1 && null != LsSeekiosLocations[index]) //we have to check
                            {
                                LsSeekiosLocations[index].LsLocations = SelectedSeekiosLocations.LsLocations.OrderBy(l => l.DateLocationCreation).ToList();
                            }
                        }
                        catch (TimeoutException)
                        {
                            await _dialogService.ShowError(
                                Resources.TimeoutError
                                , Resources.TimeoutErrorTitle
                                , Resources.Close, null);
                        }
                        catch (WebException)
                        {
                            await _dialogService.ShowError(
                                Resources.TimeoutError
                                , Resources.TimeoutErrorTitle
                                , Resources.Close, null);
                        }
                        catch (Exception)
                        {
                            await _dialogService.ShowError(
                                Resources.UnexpectedError
                                , Resources.UnexpectedErrorTitle
                                , Resources.Close, null);
                        }
                    }
                    else
                    {
                        LocationUpperLowerDates limitDates = null;
                        try
                        {
                            limitDates = await _dataService.LowerDateAndUpperDate(idSeekios);
                        }
                        catch (TimeoutException)
                        {
                            await _dialogService.ShowError(
                                Resources.TimeoutError
                                , Resources.TimeoutErrorTitle
                                , Resources.Close, null);
                        }
                        catch (WebException)
                        {
                            await _dialogService.ShowError(
                                Resources.TimeoutError
                                , Resources.TimeoutErrorTitle
                                , Resources.Close, null);
                        }
                        catch (Exception)
                        {
                            await _dialogService.ShowError(
                                Resources.UnexpectedError
                                , Resources.UnexpectedErrorTitle
                                , Resources.Close, null);
                        }

                        if (limitDates != null)
                        {
                            List<LocationDTO> locationsBySeekios = null;
                            try
                            {
                                locationsBySeekios = await _dataService.Locations(idSeekios, lowerDate, upperDate);
                            }
                            catch (TimeoutException)
                            {
                                await _dialogService.ShowError(
                                    Resources.TimeoutError
                                    , Resources.TimeoutErrorTitle
                                    , Resources.Close, null);
                            }
                            catch (WebException)
                            {
                                await _dialogService.ShowError(
                                    Resources.TimeoutError
                                    , Resources.TimeoutErrorTitle
                                    , Resources.Close, null);
                            }
                            catch (Exception)
                            {
                                await _dialogService.ShowError(
                                    Resources.UnexpectedError
                                    , Resources.UnexpectedErrorTitle
                                    , Resources.Close, null);
                            }

                            if (locationsBySeekios != null)
                            {
                                SelectedSeekiosLocations = new SeekiosLocations
                                {
                                    IdSeekios = idSeekios,
                                    LimitUpperDate = limitDates.UppderDate,
                                    LimitLowerDate = limitDates.LowerDate,
                                    LsLocations = locationsBySeekios.OrderBy(l => l.DateLocationCreation).ToList()
                                };
                            }
                            //si LimitLowerDate est trop ancien ? en 1970 ? alors on prend une date par defaut ?
                            //TODO: peut etre prendre la date du point le plus ancien ?
                            //ex: json est => "{\"LowerDate\":\"\\/Date(0+0000)\\/\",\"UppderDate\":\"\\/Date(1475851229000+0000)\\/\"}"
                            if (SelectedSeekiosLocations.LimitLowerDate <= new DateTime(1970, 1, 1, 1, 0, 0))
                            {
                                SelectedSeekiosLocations.LimitLowerDate = DateHelper.GetDateTimeFromOneMonthAgo();
                            }
                            LsSeekiosLocations.Add(SelectedSeekiosLocations);
                        }
                    }
                }
            }
            else
            {
                if (LsSeekiosLocations == null) LsSeekiosLocations = new List<SeekiosLocations>();
                SeekiosLocations locations = LsSeekiosLocations.FirstOrDefault(a => a.IdSeekios == idSeekios);//si on l'a deja on le recupere
                //il faut recuperer la liste, l'ordonner et la croiser avec LsSeekiosLocations...
                //todo: check if locations is null ?
                ModeDTO[] mm = App.CurrentUserEnvironment.LsMode.Where(m => m.Seekios_idseekios == App.Locator.DetailSeekios.SeekiosSelected.Idseekios).ToArray();
                foreach (ModeDTO mode in mm)
                {
                    List<LocationDTO> pp = App.CurrentUserEnvironment.LsLocations.Where(el => el.Mode_idmode == mode.Idmode).ToList();
                    //.OrderBy(el => el.DateLocationCreation).ToList();
                    foreach (LocationDTO l in pp)
                    {
                        if (!locations.LsLocations.Any(item => item.Idlocation == l.Idlocation))
                        {
                            locations.LsLocations.Add(l);
                        }
                    }
                }
                locations.LsLocations = locations.LsLocations.OrderBy(el => el.DateLocationCreation).ToList();
                App.Locator.Historic.SelectedSeekiosLocations = locations;
            }
            if (SelectedSeekiosLocations != null && SelectedSeekiosLocations.LimitLowerDate != null
                && SelectedSeekiosLocations.LimitLowerDate.HasValue
                && SelectedSeekiosLocations.LimitLowerDate > CurrentLowerDate)
            {
                CurrentLowerDate = SelectedSeekiosLocations.LimitLowerDate.Value;
                await OnGetLocationsComplete.Invoke(true, null);
            }
            else
            {
                await OnGetLocationsComplete.Invoke(false, null);
            }
        }

        /// <summary>
        /// Initialize the map with markers
        /// </summary>
        public virtual void InitMap()
        {
            MapControlManager.InitMap(22f, true);
            var seekios = MapViewModelBase.Seekios;
            var isDontMove = MapViewModelBase.Mode != null && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove;
            if (seekios != null && seekios.LastKnownLocation_dateLocationCreation.HasValue)
            {
                MapControlManager.CreateSeekiosMarkerAsync(seekios.Idseekios.ToString()
                    , seekios.SeekiosName
                    , seekios.SeekiosPicture
                    , seekios.LastKnownLocation_dateLocationCreation.Value
                    , seekios.LastKnownLocation_latitude
                    , seekios.LastKnownLocation_longitude
                    , seekios.LastKnownLocation_accuracy
                    , isDontMove);
                MapControlManager.CenterInLocalisation(seekios.LastKnownLocation_latitude, seekios.LastKnownLocation_longitude, (float)ZoomLevelEnum.MediumZoom);
            }
        }
    }

    #endregion
}
