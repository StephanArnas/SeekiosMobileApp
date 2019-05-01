using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeekiosApp;
using SeekiosApp.Services;
using System;
using SeekiosDataServiceUnitTest.Utilities;
using SeekiosApp.Model.DTO;
using System.Collections.Generic;
using System.Linq;
using SeekiosApp.Enum;

namespace SeekiosDataServiceUnitTest
{
    [TestClass]
    public class DataServiceUnitTest
    {
        #region ===== Attributs ===================================================================


        private static DataService _dataService = new DataService();
        private UserDTO _user = null;
        private SeekiosDTO _seekios = null;
        private ModeDTO _mode = null;
        private AlertDTO _alert = null;
        private AlertWithRecipientDTO _alertwithrecipient = null;
        private static int _idDevice = 339; // 339 staging
        private static int _idseekios = 175; // 175 staging
        private static int _iduser = 81;  // 81 staging
        private static int _idmode = 5135;//5135 staging
        private static int _idalert = 370;//370 staging
        private List<AlertWithRecipientDTO> alertes = new List<AlertWithRecipientDTO>();
        private List<AlertWithRecipientDTO> _listAlertWithRecipient = new List<AlertWithRecipientDTO>();

        private const string LAST_APP_VERSION = "1.01.0702.1304"; //needs to be updated

        #endregion

        #region ===== Constructor =================================================================

        public DataServiceUnitTest()
        {
            App.IsTestMode = true;
            SetData();
        }

        #endregion

        #region ===== Instantiate Data ============================================================

        private void SetData()
        {
            _user = new UserDTO()
            {
                DateLastConnection = DateTime.Now,
                DateLocation = DateTime.Now,
                DefaultTheme = 0,
                Email = "seekiosunittest@gmail.com",
                FirstName = "TestFirstName",
                GCMRegistrationToken = "noToken",
                IsValidate = true,
                LastName = "TestLastName",
                LocationLatitude = 0,
                LocationLongitude = 0,
                NumberView = 1,
                Password = "6697c0bc8622a254cbb2e62121e9cf02",
                PhoneNumber = "33|64564866",
                RemainingRequest = 1000,
                SocialNetworkType = 0,
                SocialNetworkUserId = null,
                UserPicture = null
            };

            DataService.Email = _user.Email;
            DataService.Pass = _user.Password;

            _seekios = new SeekiosDTO()
            {
                Idseekios = _idseekios,
                SeekiosName = "UnitTestSeekios",
                SeekiosPicture = null,
                SeekiosDateCreation = DateTime.Now,
                BatteryLife = 99,
                SignalQuality = 50,
                DateLastCommunication = DateTime.Now,
                LastKnownLocation_longitude = 0.0,
                LastKnownLocation_latitude = 0.0,
                LastKnownLocation_altitude = 0.0,
                LastKnownLocation_accuracy = 0.0,
                LastKnownLocation_dateLocationCreation = DateTime.Now,
                Subscription_idsubscription = 2,
                Category_idcategory = 1,
                User_iduser = _iduser,
                HasGetLastInstruction = true,
                IsAlertLowBattery = true,
                IsInPowerSaving = false,
                PowerSaving_hourStart = 0,
                PowerSaving_hourEnd = 0,
                AlertSOS_idalert = null,
                IsRefreshingBattery = false,
                FreeCredit = 100000000
            };

            _mode = new ModeDTO()
            {
                DateModeCreation = DateTime.Now,
                Trame = "1",
                NotificationPush = 0,
                CountOfTriggeredAlert = 0,
                LastTriggeredAlertDate = DateTime.Now,
                Seekios_idseekios = _idseekios,
                ModeDefinition_idmodeDefinition = (int)SeekiosApp.Enum.ModeDefinitionEnum.ModeTracking,
                StatusDefinition_idstatusDefinition = 1
            };

            alertes.Add(new AlertWithRecipientDTO()
            {
                IdAlertType = 1,
                Title = "Test",
                Content = "Test"
            });

            _alert = new AlertDTO()
            {
                IdAlertType = 1,
                IdMode = _idmode,
                Title = "Test",
                Content = "Test"
            };

            var recipients = new List<AlertRecipientDTO>();
            recipients.Add(new AlertRecipientDTO()
            {
                Email = "testemailrecipient@gmail.com",
                DisplayName = "TestDisplayname",
            });

            _alertwithrecipient = new AlertWithRecipientDTO()
            {
                IdAlert = _idalert,
                IdAlertType = 3,
                IdMode = _idmode,
                Title = "Test",
                Content = "Test",
                LsRecipients = recipients
            };

            _listAlertWithRecipient.Add(_alertwithrecipient);
        }

        #endregion

        #region ===== Logic Operator ==============================================================

        /// <summary>
        /// Asserts that N1 is STRICTLY bigger than N2
        /// </summary>
        /// <param name="N1"></param>
        /// <param name="N2"></param>
        private void AssertN1IsBiggerThanN2(int N1, int N2)
        {
            if (N1 < N2)
            {
                Assert.Fail();
            }
            else
            {
                Assert.IsTrue(true);
            }
        }

        #endregion

        #region ===== Testing Methodes ============================================================

        /// <summary>
        /// This method returns:
        /// -1 if an error occurs
        /// 0 if the version is the last-obligatory version
        /// 1 if the version needs to be updated
        /// To be covered the test must include both cases 0 & 1
        /// we need to keep updated the last version variable in the data settings & in the database
        /// </summary>
        [TestMethod]
        public void A_IsSeekiosVersionApplicationNeedForceUpdate()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                //Asserting we need to update : case 1
                var result = await _dataService.IsSeekiosVersionApplicationNeedForceUpdate("1.01.3010.807", "1");
                Assert.AreEqual(result, 1);

                //Asserting we don't need to update : case 0
                var result2 = await _dataService.IsSeekiosVersionApplicationNeedForceUpdate(LAST_APP_VERSION, "1");
                Assert.AreEqual(result2, 0);
            });
        }

        /// <summary>
        /// This method returns:
        /// -1 if an error occurs
        /// 0 if a user already exists
        /// the IdUser if the user is correctly added
        /// </summary>
        [TestMethod]
        public void B_InsertUser()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                //Asserting the user is correctly added : case return IdUser
                var result = await _dataService.InsertUser(_user);
                _iduser = result;
                AssertN1IsBiggerThanN2(result, 1);

                //Asserting the user already exists : case 0
                //we might need to wait a little bit as the database is being updated (to be tested)
                var result2 = await _dataService.InsertUser(_user);
                Assert.AreEqual(result2, 0);
            });
        }

        /// <summary>
        /// This method returns:
        /// -1 if it fails
        /// an integer (x > 0) if a field is updated
        /// </summary>
        [TestMethod]
        public void C_UpdateUser()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                //Asserting we managed to update the user
                _user.FirstName = "TestFirstname2";
                _user.LastName = "TestLastname2";
                _user.IdUser = _iduser;
                var result = await _dataService.UpdateUser(_user);
                AssertN1IsBiggerThanN2(result, 1);
            });
        }

     
        /// <summary>
        /// This method returns:
        /// null if the anything goes wrong
        /// the seekios object if evrything is ok
        /// </summary>
        [TestMethod]
        public void F_InsertSeekios()
        {
            //Asserting evrything went ok
            GeneralThreadAffineContext.Run(async () =>
            {
                var result = await _dataService.InsertSeekios(_seekios);
                _idseekios = result.Idseekios;
                Assert.IsNotNull(result);
            });
        }

        /// <summary>
        /// This method returns:
        /// -1 if somethings goes wrong
        /// the seekios ID if something is updated
        /// </summary>
        [TestMethod]
        public void G_UpdateSeekios()
        {
            //Asserting update went ok
            GeneralThreadAffineContext.Run(async () =>
            {
                _seekios.SeekiosName = "UnitTestSeekios2";
                _seekios.Idseekios = _idseekios;
                var result = await _dataService.UpdateSeekios(_seekios);
                AssertN1IsBiggerThanN2(result, 1);
            });
        }

        /// <summary>
        /// This method returns:
        /// null if it fails
        /// the seekios object if it works
        /// </summary>
        [TestMethod]
        public void H_RefreshSeekios()
        {
            //Asserting everything is ok
            GeneralThreadAffineContext.Run(async () =>
            {
                var result = await _dataService.RefreshSeekios(_idseekios,
                    _iduser);
                Assert.IsNotNull(result);
            });
        }

        /// <summary>
        /// This method returns:
        /// -1 if failed
        /// 1 if worked
        /// </summary>
        [TestMethod]
        public void I_RequestBatteryLevel()
        {
            //Asserting it worked
            GeneralThreadAffineContext.Run(async () =>
            {
                int expectedAnswer = 1;
                var result = await _dataService.RequestBatteryLevel(_idseekios);
                Assert.AreEqual(result, expectedAnswer);
            });
        }

        /// <summary>
        /// This method returns:
        /// -1 if failed
        /// the mode ID if worked
        /// </summary>
        [TestMethod]
        public void J_InsertModeZone()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                //Asserting it worked
                _mode.ModeDefinition_idmodeDefinition = (int)ModeDefinitionEnum.ModeZone;
                _mode.Device_iddevice = _idDevice;
                var result = await _dataService.InsertModeZone(_mode, _listAlertWithRecipient);
                _idmode = result;
                AssertN1IsBiggerThanN2(result, 1);
            });
        }

        /// <summary>
        /// This method returns:
        /// -1 if failed
        /// x>0 if something has been updated
        /// </summary>
        [TestMethod]
        public void K_UpdateMode()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                _mode.Device_iddevice = _idDevice;
                _mode.Idmode = _idmode;
                _mode.Trame = "2";
                var result = await _dataService.UpdateMode(_mode);
                AssertN1IsBiggerThanN2(result, 1);
            });
        }

        /// <summary>
        /// This method returns:
        /// null if it failed
        /// a List<AlertDTO> if it worked
        /// </summary>
        [TestMethod]
        public void L_GetAlertsByMode()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                //Asserting everything is ok
                int expectedAnswer = 1;
                _mode.ModeDefinition_idmodeDefinition = (int)ModeDefinitionEnum.ModeZone;
                _mode.Device_iddevice = _idDevice;
                _mode.Idmode = _idmode;
                var result = await _dataService.GetAlertsByMode(_mode);
                Assert.IsNotNull(result);
                var result2 = result.Count;
                _idalert = result.First().IdAlert;
                Assert.AreEqual(result2, expectedAnswer);
            });
        }

        /// <summary>
        /// This method returns:
        /// -1 if failed
        /// x>0 if worked
        /// </summary>
        [TestMethod]
        public void M_UpdateAlert()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                //Asserting everything went ok
                _alert.IdAlert = _idalert;
                _alert.Title = "Test 2";
                _alert.IdMode = _idmode;
                var result = await _dataService.UpdateAlert(_alert);

                AssertN1IsBiggerThanN2(result, 1);
            });
        }

        /// <summary>
        /// This method returns:
        /// -1 if failed
        /// x>0 if worked
        /// </summary>
        [TestMethod]
        public void N_UpdateAlertWithRecipient()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                //Asserting everything went ok
                _alertwithrecipient.IdAlert = _idalert;
                _alertwithrecipient.Title = "Test 3";
                _alertwithrecipient.Content = "Test 3";
                _alertwithrecipient.LsRecipients.First().IdAlert = _idalert;
                var result = await _dataService.UpdateAlertWithRecipient(_alertwithrecipient);
                AssertN1IsBiggerThanN2(result, 1);
            });
        }

        /// <summary>
        /// This method returns:
        /// -1 if failed
        /// 1 if worked
        /// </summary>
        [TestMethod]
        public void O_DeleteAlert()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                int id = _idalert;
                int expectedAnswer = 1;
                var result = await _dataService.DeleteAlert(id);
                Assert.AreEqual(result, expectedAnswer);
            });
        }

        [TestMethod]
        public void P_DeleteMode()
        {
            DeleteMode();
        }

        /// <summary>
        /// This method returns:
        /// -1 if failed
        /// the mode ID if worked
        /// </summary>
        [TestMethod]
        public void Q_InsertMode()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                //Asserting evrything is ok : we insert the mode zone previously inserted
                //it will only have a different id
                _mode.Device_iddevice = _idDevice;
                var result = await _dataService.InsertMode(_mode);
                _idmode = result;
                AssertN1IsBiggerThanN2(result, 1);
            });
        }

        [TestMethod]
        public void R_DeleteMode()
        {
            DeleteMode();
        }

        /// <summary>
        /// This method returns:
        /// -1 if failed
        /// the mode ID if worked
        /// </summary>
        [TestMethod]
        public void S_InsertModeDontMove()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                _mode.ModeDefinition_idmodeDefinition = (int)ModeDefinitionEnum.ModeDontMove;
                _mode.Device_iddevice = _idDevice;
                var result = await _dataService.InsertModeDontMove(_mode, alertes);
                _idmode = result;
                AssertN1IsBiggerThanN2(result, 1);
            });
        }

        [TestMethod]
        public void T_DeleteMode()
        {
            DeleteMode();
        }

        /// <summary>
        /// This method returns:
        /// -1 if it fails
        /// 1 if it works
        /// </summary>
        [TestMethod]
        public void U_DeleteSeekios()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                int id = _idseekios;
                int expectedAnswer = 1;
                var result = await _dataService.DeleteSeekios(id);
                Assert.AreEqual(result, expectedAnswer);
            });
        }

        /// <summary>
        /// This method returns:
        /// -1 if it fails
        /// 0 if the userToDelete is null
        /// 1 if it works
        /// This method deletes every seekios linked to the user
        /// + every device connected to the user
        /// </summary>
        [TestMethod]
        public void V_DeleteUser()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                int expectedAnswer = 1;
                var result = await _dataService.DeleteUser(_iduser.ToString());
                Assert.AreEqual(result, expectedAnswer);
            });
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// This method returns:
        /// -1 if failed
        /// x>0 if worked
        /// </summary>
        private void DeleteMode()
        {
            GeneralThreadAffineContext.Run(async () =>
            {
                string id = _idmode.ToString();
                var result = await _dataService.DeleteMode(id);
                AssertN1IsBiggerThanN2(result, 1);
            });
        }

        #endregion
    }
}
