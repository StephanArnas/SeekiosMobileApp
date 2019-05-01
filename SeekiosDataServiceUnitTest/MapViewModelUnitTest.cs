using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx.UnitTests;
using SeekiosApp;
using SeekiosApp.Interfaces;
using SeekiosApp.Services;
using System;
using System.Threading.Tasks;
using SeekiosDataServiceUnitTest.Utilities;
using SeekiosApp.Model.DTO;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Views;
using SeekiosDataServiceUnitTest.Services;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.ViewModel;
using SeekiosApp.Enum;
using System.Timers;
using SeekiosApp.Model.APP;

namespace SeekiosDataServiceUnitTest
{
    [TestClass]
    public class MapViewModelUnitTest
    {
        #region ===== Attributs ===================================================================
        private static DataService _dataService = new DataService();
        private UserEnvironmentDTO _UserEnvironmentDTO = null;
        private static int _IdUser = 1;
        private SeekiosDTO _seekios = null;
        private static int _idseekios = 171; // 171 staging
        private static int _iduser = 81;  // 81 staging
        private ModeDTO _mode = null;
        private static string _Email = "stef868@gmail.com";
        private static string _Password = "123456";
        private static string _PasswordHash = "2a08e825a0e47f0635548a58cceeef88";
        private static string _uniqueDeviceId = "339";
        private static string _fbUserId = "1";
        private UserDTO _user = null;
        #endregion

        #region ===== Constructor =================================================================
        public MapViewModelUnitTest()
        {
            App.IsTestMode = true;
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IDataService, DataService>();
            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<ISaveDataService, SaveDataService>();
            SimpleIoc.Default.Register<IDispatchOnUIThread, DispatchService>();
            //SimpleIoc.Default.Register<IFollowMeService, FollowMeService>();
            //SimpleIoc.Default.Register<IBluetoothService, BluetoothService>();
            //SimpleIoc.Default.Register<IDontMoveService, DontMoveService>();
            SimpleIoc.Default.Register<IMapControlManager, MapControlManager>();
            SetData();
        }
        #endregion

        #region ===== Instanciate Data ============================================================
        private void SetData()
        {
            _user = new UserDTO()
            {
                IdUser = _IdUser
            };
            _seekios = new SeekiosDTO()
            {
                Idseekios = _idseekios,
                SeekiosName = "TestSeekios",
                SeekiosPicture = null,
                SeekiosDateCreation = DateTime.Now,
                BatteryLife = 20,
                SignalQuality = 99,
                DateLastCommunication = DateTime.Now,
                LastKnownLocation_longitude = 0.0,
                LastKnownLocation_latitude = 0.0,
                LastKnownLocation_altitude = 0.0,
                LastKnownLocation_accuracy = 0.0,
                LastKnownLocation_dateLocationCreation = DateTime.Now,
                Subscription_idsubscription = 2,
                Category_idcategory = 1,
                User_iduser = _iduser,
                HasGetLastInstruction = false,
                IsAlertLowBattery = true,
                IsInPowerSaving = false,
                PowerSaving_hourStart = 0,
                PowerSaving_hourEnd = 0,
                AlertSOS_idalert = null,
                IsRefreshingBattery = false,
                FreeCredit = 100
            };
            _UserEnvironmentDTO = new UserEnvironmentDTO()
            {
                LsSeekios = new List<SeekiosDTO>()
            ,
                LsMode = new List<ModeDTO>()
            ,
                LsAlert = new List<AlertDTO>()
            ,
                LsAlertFavorites = new List<AlertFavoriteDTO>()
            ,
                LsAlertRecipient = new List<AlertRecipientDTO>()
            ,
                LsFavoriteArea = new List<FavoriteAreaDTO>()
            ,
                LsFriend = new List<FriendUserDTO>()
            ,
                LsSharing = new List<SharingDTO>()
            ,
                LsLocations = new List<LocationDTO>()
            ,
                LsModeDefinition = new List<ModeDefinitionDTO>()
            };
            App.CurrentUserEnvironment = _UserEnvironmentDTO;
            App.CurrentUserEnvironment.User = _user;
        }
        #endregion

        #region ===== Map Methodes ==============================================================

        [TestMethod]
        public void InitMap()
        {
            App.Locator.Map.MapControlManager = ServiceLocator.Current.GetInstance<IMapControlManager>();
            MapViewModelBase.Seekios = _seekios;
            App.Locator.Map.InitMap();
        }

        [TestMethod]
        public void OnHasToRefreshPosition()
        {
            Tuple<int, int> batteryAndSignal = new Tuple<int, int>(1, 1);
            Tuple<double, double, double, double> location = new Tuple<double, double, double, double>(1.0, 1.0, 1.0, 1.0);

            //App.Locator.ListSeekios.LsSeekios.Add(_seekios);
            _seekios.UIdSeekios = _idseekios.ToString();
            _seekios.Idseekios = _idseekios;
            App.CurrentUserEnvironment.LsSeekios.Add(_seekios);
            App.Locator.Map.OnDemandPositionReceived(_idseekios.ToString()
                , batteryAndSignal
                , location
                , DateTime.Now
                );
        }

        [TestMethod]
        public void RefreshSeekiosPosition()
        {
            _seekios.Idseekios = _idseekios;
            App.CurrentUserEnvironment.User.RemainingRequest = 2;
            App.CurrentUserEnvironment.User.IdUser = _IdUser;
            App.Locator.MySeekiosDetail.SeekiosSelected = _seekios;
            App.Locator.Map.RefreshActionDictionnary.Add(_idseekios,
                new Tuple<Action, RefreshResultEnum, DateTime>(
                    new Action(() => { })
                    , RefreshResultEnum.RequestSucceeded
                    , DateTime.Now));

            App.Locator.Map.RefreshSeekiosPosition();
        }

        #endregion
    }
}
