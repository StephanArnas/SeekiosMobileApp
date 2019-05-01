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
    public class LoginViewModelUnitTest
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
        public LoginViewModelUnitTest()
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

        #region ===== Testing Methodes ============================================================

        #region ===== Logic Operator ==============================================================

        private void IsBiggerThanSucces(int number1, int number2)
        {
            if (number1 < number2)
            {
                Assert.Fail();
            }
            else
            {
                Assert.IsTrue(true);
            }
        }

        #endregion

        #region ===== Login Methodes ==============================================================

        [TestMethod]
        public void Login_Connection()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                App.Locator.Login.Email = _Email;
                App.Locator.Login.Password = _Password;
                var result = await App.Locator.Login.Connect("TestDeviceModel"
                    , "TestPlatform"
                    , "TestVersion"
                    , _uniqueDeviceId
                    , "fr"
                    , false);
                bool expectedAnswer = true;
                Assert.AreEqual(result, expectedAnswer);
            });
        }

        [TestMethod]
        public void Login_Disconnect()
        {
            GeneralThreadAffineContext.Run(async () =>
            {

                App.CurrentUserEnvironment.User = _user;
                var result = await App.Locator.Login.Disconnect();
                int expectedAnswer = 1;
                if (result == 1)
                {
                    Assert.AreEqual(result, expectedAnswer);
                }
                else if (result == -1)
                {
                    expectedAnswer = -1;
                    Assert.AreEqual(result, expectedAnswer);
                }
                else
                {
                    Assert.AreEqual(result, expectedAnswer);
                }
            });
        }

        [TestMethod]
        public void CreateAccountAndDeleteAccount()
        {
            string CurEmailID = Guid.NewGuid().ToString().Substring(0, 5) + "@gmail.com";
            GeneralThreadAffineContext.Run(async () =>
            {
                App.Locator.Login.UserEmail = CurEmailID;
                App.Locator.Login.UserPassword = "123456";
                var result = await App.Locator.Login.CreateAccount("TestFirstName"
                    , "TestLastName"
                    , "TestModel"
                    , "TestPlateform"
                    , "TestVersion"
                    , _uniqueDeviceId
                    , "fr");
                var data = ServiceLocator.Current.GetInstance<IDataService>();
                string Userid = App.CurrentUserEnvironment.User.IdUser.ToString();
                var deleteuserresult = await data.DeleteUser(Userid);

                bool expectedAnswer = false;
                if (result == true && deleteuserresult == 1)
                {
                    expectedAnswer = true;
                }
                Assert.AreEqual(result, expectedAnswer);
            });


        }

        //[TestMethod]
        //public void ConnectWithFacebook()
        //{
        //    GeneralThreadAffineContext.Run(async () =>
        //    {

        //        var result = await App.Locator.Login.ConnectWithFacebook("amitdesai789@yahoo.com"
        //             , "TestFirstName"
        //             , "TestLastName"
        //             , _fbUserId
        //             , ""
        //             , "TestModel"
        //             , "TestPlateform"
        //             , "TestVersion"
        //             , _uniqueDeviceId
        //             , "fr");
        //        bool expectedAnswer = true;
        //        Assert.AreEqual(result, expectedAnswer);
        //    });
        //}

        [TestMethod]
        public void AutoConnect()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                App.Locator.Login.Email = _Email;
                App.Locator.Login.Password = _PasswordHash;
                var result = await App.Locator.Login.AutoConnect("TestModel"
                     , "TestPlateform"
                     , "TestVersion"
                     , _uniqueDeviceId
                     , "fr");
                bool expectedAnswer = true;
                Assert.AreEqual(result, expectedAnswer);
            });
        }

        [TestMethod]
        public void ForgetPassword()
        {
            App.CurrentUserEnvironment.User.Email = _Email;
            App.Locator.Login.ForgetPassword(_Email);

        }

        [TestMethod]
        public void GetRamdomImageName()
        {
            var result = App.Locator.Login.GetRamdomImageName();
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void IsEmailValid()
        {
            string Email = "sasasasa";
            Email = _Email;
            var result = App.Locator.Login.IsEmailValid(Email);
            bool expectedAnswer = true;
            Assert.AreEqual(result, expectedAnswer);
        }
        #endregion

        #endregion
    }
}
