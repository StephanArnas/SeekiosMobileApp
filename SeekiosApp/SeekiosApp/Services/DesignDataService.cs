using SeekiosApp.Enum;
using SeekiosApp.Interfaces;
using SeekiosApp.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SeekiosApp.Model.APP;
using Xamarin.Contacts;
using SeekiosApp.Helper;

namespace SeekiosApp.Services
{
    public class DesignDataService : IDataService
    {

        public Task<int> UpdateSeekios(SeekiosDTO seekios)
        {
            return Task.FromResult<int>(1);
        }

        public Task<int> UpdateUser(UserDTO user)
        {
            return Task.FromResult<int>(1);
        }

        public Task<List<LocationDTO>> LocationsByMode(int id)
        {
            return null;
        }

        public Task<int> InsertUser(UserDTO user)
        {
            return Task.FromResult<int>(1);
        }

        public Task<int> InsertModeZone(ModeDTO modeToAdd, List<AlertWithRecipientDTO> alertes)
        {
            return Task.FromResult<int>(1);
        }

        public Task<int> RefreshSeekiosLocation(int idSeekios)
        {
            var rand = new Random();

            SeekiosDTO seekios = lsSeekios.Where(el => el.Idseekios == idSeekios && el.User_iduser == idUser).FirstOrDefault();
            if (seekios == null) return null;

            seekios.BatteryLife = rand.Next(0, 101);
            seekios.SignalQuality = rand.Next(0, 101);

            seekios.LastKnownLocation_dateLocationCreation = DateTime.Now;
            seekios.LastKnownLocation_latitude = seekios.LastKnownLocation_latitude + rand.Next(-100, 101) / 100.0;
            seekios.LastKnownLocation_longitude = seekios.LastKnownLocation_longitude + rand.Next(-100, 101) / 100.0;

            Task.Delay(4000).Wait();

            return Task.FromResult(seekios.Idseekios);
        }

        public Task<List<FavoriteAreaDTO>> GetFavoritesAreaByUser(int id)
        {
            List<FavoriteAreaDTO> seekios = new List<FavoriteAreaDTO>();
            return Task.FromResult<List<FavoriteAreaDTO>>(seekios);
        }

        public Task<int> InsertFavoriteArea(FavoriteAreaDTO area)
        {
            return Task.FromResult<int>(1);
        }

        public Task<int> UpdateFavoriteArea(FavoriteAreaDTO area)
        {
            return Task.FromResult<int>(1);
        }

        public Task<int> DeleteFavoriteArea(int id)
        {
            return Task.FromResult<int>(1);
        }

        public void ConfirmChangeUserLocationRequestTreated(int id) { }

        public Task<int> InsertAlertFavorite(AlertFavoriteDTO alertFavorite)
        {
            return Task.FromResult<int>(1);
        }

        public Task<int> DeleteAlertFavorite(int id)
        {
            return Task.FromResult<int>(1);
        }

        public Task<int> InsertAlertSOSWithRecipient(int idSeekios, AlertWithRecipientDTO alert)
        {
            return Task.FromResult<int>(1);
        }

        public Task<int> UpdateAlertSOSWithRecipient(int idSeekios, AlertWithRecipientDTO alert)
        {
            return Task.FromResult<int>(1);
        }

        public Task<SeekiosDTO> InsertSeekios(SeekiosDTO seekios)
        {
            return Task.FromResult<SeekiosDTO>(new SeekiosDTO());
        }

        public Task<int> DeleteSeekios(int id)
        {
            return Task.FromResult<int>(1);
        }

        #region Private Methode

        private UserEnvironmentDTO InitialiseUserNicolatViot()
        {
            var source = new UserEnvironmentDTO();

            int idMode1Seekios1 = 1000;
            int idMode2Seekios1 = 1001;
            int idMode1Seekios2 = 1002;
            int idAlert1ModeSeekios1 = 1;
            int idAlert2ModeSeekios1 = 2;
            int idAlert3ModeSeekios1 = 3;
            int idAlertRecipient1Alert1Mode1Seekios1 = 1;
            int idAlertRecipien2tAlert1Mode1Seekios1 = 2;
            int idAlertRecipien1tAlert2Mode1Seekios1 = 3;

            source.ClientSynchronisationDate = DateHelper.GetSystemTime();

            // area favorite 1

            source.LsFavoriteArea.Add(new FavoriteAreaDTO
            {
                AreaName = "Home",
                User_iduser = idUser,
                IdfavoriteArea = 1,
                Trame = "",
                DateAddedFavorite = DateTime.Now,
                PointsCount = 3,
                AreaGeodesic = 1
            });

            // alert recipient from alert 1

            source.LsAlertRecipient.Add(new AlertRecipientDTO
            {
                IdAlert = idAlert1ModeSeekios1,
                IdRecipient = idAlertRecipient1Alert1Mode1Seekios1,
                DisplayName = "Bastien Monleau",
                Email = "b.m@gmail.com",
                EmailType = "Home",
                PhoneNumber = "0661350559",
                PhoneNumberType = "Home"
            });

            source.LsAlertRecipient.Add(new AlertRecipientDTO
            {
                IdAlert = idAlert1ModeSeekios1,
                IdRecipient = idAlertRecipien2tAlert1Mode1Seekios1,
                DisplayName = "Mathilde Etcheleku",
                Email = "mathilde.e@gmail.com",
                EmailType = "Home",
                PhoneNumber = null,
                PhoneNumberType = null
            });

            // alert recipient from alert 2

            source.LsAlertRecipient.Add(new AlertRecipientDTO
            {
                IdAlert = idAlert2ModeSeekios1,
                IdRecipient = idAlertRecipien1tAlert2Mode1Seekios1,
                DisplayName = "Renaud Jourdy",
                Email = null,
                EmailType = null,
                PhoneNumber = "0784659595",
                PhoneNumberType = "Work"
            });

            // alert 1

            source.LsAlert.Add(new AlertDTO
            {
                Content = "Votre Seekios s'éloigne de la zone",
                IdAlert = idAlert1ModeSeekios1,
                IdAlertType = (int)AlertDefinitionEnum.SMS,
                IdMode = idMode1Seekios1,
                Title = "Seekios Chien en fuite !"
            });

            // alert 2 

            source.LsAlert.Add(new AlertDTO
            {
                Content = "Le chien est encore parti de la maison ! il faut appeler les voisins pour les prevenir d'aller le récupérer. Bisou à ce soir.",
                IdAlert = idAlert2ModeSeekios1,
                IdAlertType = (int)AlertDefinitionEnum.Email,
                IdMode = idMode1Seekios1,
                Title = string.Empty
            });

            // alert 3

            source.LsAlert.Add(new AlertDTO
            {
                Content = string.Empty,
                IdAlert = idAlert3ModeSeekios1,
                IdAlertType = (int)AlertDefinitionEnum.MessageCall,
                IdMode = idMode1Seekios1,
                Title = string.Empty
            });

            // alert 4

            source.LsAlert.Add(new AlertDTO
            {
                Content = string.Empty,
                IdAlert = 4,
                IdAlertType = (int)AlertDefinitionEnum.MessageCall,
                IdMode = idMode1Seekios1,
                Title = string.Empty
            });

            // mode seekios 1

            source.LsMode.Add(new ModeDTO
            {
                Idmode = idMode1Seekios1,
                DateModeCreation = DateTime.Now,
                ModeDefinition_idmodeDefinition = (int)ModeDefinitionEnum.ModeDontMove,
                Seekios_idseekios = idSeekios1,
                StatusDefinition_idstatusDefinition = 1, //TODO : statut mode à définir (on garde ou on jetter ?)
                Trame = string.Empty
            });

            // mode seekios 2

            source.LsMode.Add(new ModeDTO
            {
                Idmode = idMode2Seekios1,
                DateModeCreation = DateTime.Now,
                ModeDefinition_idmodeDefinition = (int)ModeDefinitionEnum.ModeZone,
                Seekios_idseekios = idSeekios1,
                StatusDefinition_idstatusDefinition = 1, //TODO : statut mode à définir (on garde ou on jetter ?)
                Trame = string.Empty
            });

            //Mode 1 seekios 2

            source.LsMode.Add(new ModeDTO()
            {
                Idmode = idMode1Seekios2,
                DateModeCreation = DateTime.Now,
                ModeDefinition_idmodeDefinition = (int)ModeDefinitionEnum.ModeTracking,
                Seekios_idseekios = idSeekios2,
                StatusDefinition_idstatusDefinition = 1, //TODO : statut mode à définir (on garde ou on jetter ?)
                Trame = string.Empty
            });

            // initialise seekios 1

            source.LsSeekios.Add(lsSeekios[0]);

            // initialise seekios 2

            source.LsSeekios.Add(lsSeekios[1]);

            // alert favorite 1

            source.LsAlertFavorites.Add(new AlertFavoriteDTO
            {
                IdAlertFavorite = 1,
                IdAlertType = (int)Enum.AlertDefinitionEnum.SMS,
                IdUser = idUser,
                EmailObject = "Mon chien sort",
                Content = "Mon chien s'est encore barré de la maison, récupère le en rentrant. Bisous."
            });

            // alert favorite 2

            source.LsAlertFavorites.Add(new AlertFavoriteDTO
            {
                IdAlertFavorite = 2,
                IdAlertType = (int)Enum.AlertDefinitionEnum.SMS,
                IdUser = idUser,
                EmailObject = "Mon chat sort",
                Content = "Mon chat s'est encore barré de la maison, récupère le en rentrant. Bisous."
            });

            // alert favorite 3

            source.LsAlertFavorites.Add(new AlertFavoriteDTO
            {
                IdAlertFavorite = 3,
                IdAlertType = (int)Enum.AlertDefinitionEnum.Email,
                IdUser = idUser,
                EmailObject = "Ma voiture Clio 2",
                Content = "Ma voiture est en train de bouger, il faut que je vérifie sa position dans la rue."
            });

            // initialise user

            source.User = new UserDTO()
            {
                IdUser = 1,
                Password = "456",
                Email = "nicola.viot@gmail.com",
                FirstName = "Nico",
                LastName = "Viot",
                //DateLocation = DateTime.Now,
                IsValidate = true,
                //DefaultTheme = 0,
                RemainingRequest = 80,
                //LocationLatitude = 50.856614,
                //LocationLongitude = 2.352222,
                UserPicture = "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AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==",
            };

            source.LsLocations.Add(new LocationDTO()
            {
                Idlocation = 1,
                Altitude = 0,
                DateLocationCreation = DateTime.Now.AddDays(-1),
                Latitude = 0,
                Longitude = 0,
                Mode_idmode = idMode1Seekios2,
            });

            source.LsLocations.Add(new LocationDTO()
            {
                Idlocation = 2,
                Altitude = 0,
                DateLocationCreation = DateTime.Now.AddDays(-4),
                Latitude = 45,
                Longitude = -1,
                Mode_idmode = idMode1Seekios2,
            });

            return source;
        }

        public Task<int> DeleteMode(string id)
        {
            throw new NotImplementedException();
        }

        public Task<double> GenerateUniqueCodeAndSendMessage(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<List<PackCreditDTO>> CreditPacksByLanguage(string language)
        {
            var source = new List<PackCreditDTO>();
            source.Add(new PackCreditDTO
            {
                IdPackCredit = 1,
                Title = "Small Pack",
                Description = "Recharge de 20 crédits",
                Price = "1 €",
                IsPromotion = 0,
                Promotion = string.Empty,
                RewardingCredit = "20",
                ColorBackground = "#4dbea0",
                ColorHeaderBackground = "#3da086"
            });
            source.Add(new PackCreditDTO
            {
                IdPackCredit = 2,
                Title = "Medium Pack",
                Description = "Recharge de 40 crédits",
                Price = "2 €",
                IsPromotion = 0,
                Promotion = string.Empty,
                RewardingCredit = "40",
                ColorBackground = "#154b54",
                ColorHeaderBackground = "#0c373e"
            });
            source.Add(new PackCreditDTO
            {
                IdPackCredit = 3,
                Title = "Big Pack",
                Description = "Recharge de 100 crédits",
                Price = "5 €",
                IsPromotion = 1,
                Promotion = "Noel",
                RewardingCredit = "100",
                ColorBackground = "#2db35e",
                ColorHeaderBackground = "#24944d"
            });
            source.Add(new PackCreditDTO
            {
                IdPackCredit = 3,
                Title = "Big Pack",
                Description = "Recharge de 100 crédits",
                Price = "5 €",
                IsPromotion = 1,
                Promotion = "Noel",
                RewardingCredit = "100",
                ColorBackground = "#2db35e",
                ColorHeaderBackground = "#24944d"
            });
            source.Add(new PackCreditDTO
            {
                IdPackCredit = 3,
                Title = "Big Pack",
                Description = "Recharge de 100 crédits",
                Price = "5 €",
                IsPromotion = 1,
                Promotion = "Noel",
                RewardingCredit = "100",
                ColorBackground = "#2db35e",
                ColorHeaderBackground = "#24944d"
            });
            return Task.FromResult(source);
        }

        public Task<int> DeleteFriendship(int idUser, int idFriend)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertFriendship(FriendshipDTO friendship)
        {
            throw new NotImplementedException();
        }


        public Task<List<FriendshipDTO>> GetPendingFriendshipByUser(int idUser)
        {
            throw new NotImplementedException();
        }

        public Task<int> AcceptFriendship(FriendshipDTO friendship)
        {
            throw new NotImplementedException();
        }

        public Task<List<ShortUserDTO>> GetShortUsersByPendingFriendship(string id)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertSharing(SharingDTO sharing)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateSharing(SharingDTO sharing)
        {
            throw new NotImplementedException();
        }

        public Task<List<SharingDTO>> GetSharingByUser(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteSharing(int idu, int idf, int idseekios)
        {
            throw new NotImplementedException();
        }

        public Task<int> NotifyBLEConnexionLost(int idSeekios)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertMultipleSharing(List<SharingDTO> seekiosSharing)
        {
            throw new NotImplementedException();
        }

        public Task<List<LocationDTO>> Locations(int id, DateTime lowerDate, DateTime upperDate)
        {
            throw new NotImplementedException();
        }

        public Task<LocationUpperLowerDates> LowerDateAndUpperDate(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<AlertDTO>> AlertsByMode(ModeDTO mode)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertModeDontMove(ModeDTO modeToAdd, List<AlertWithRecipientDTO> alertes)
        {
            throw new NotImplementedException();
        }

        public Task<int> IsSeekiosVersionApplicationNeedForceUpdate(string versionNumber, string plateforme)
        {
            throw new NotImplementedException();
        }

        public Task<int> RefreshSeekiosBatteryLevel(int idSeekios)
        {
            throw new NotImplementedException();
        }

        public Task<List<OperationDTO>> OperationHistoric()
        {
            throw new NotImplementedException();
        }

        //public Task<List<OperationFromStore>> OperationFromStoreHistoric(string idUserStr)
        //{
        //    throw new NotImplementedException();
        //}

        public Task<int> InsertInAppPurchase(PurchaseDTO purchase)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetDatForInAppBilling(string versionApp)
        {
            throw new NotImplementedException();
        }

        public Task<int> AskForNewPassword(string email)
        {
            throw new NotImplementedException();
        }

        public Task<int> AlertSOSHasBeenRead(string idAlert)
        {
            throw new NotImplementedException();
        }

        public Task<int> SendLogData(string data)
        {
            throw new NotImplementedException();
        }

        public Task<List<SeekiosDTO>> GetSeekios()
        {
            throw new NotImplementedException();
        }

        public Task<UserEnvironmentDTO> GetUserEnvironment(string idapp, string platform, string deviceModel, string version, string uidDevice, string countryCode)
        {
            throw new NotImplementedException();
        }

        public void LogOut()
        {
            throw new NotImplementedException();
        }

        public Task<int> RegisterDeviceForNotification(string deviceModel, string platform, string version, string uidDevice, string countryCode)
        {
            throw new NotImplementedException();
        }

        public Task<int> UnregisterDeviceForNotification(string uidDevice)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertModeTracking(ModeDTO modeToAdd)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateNotificationSetting(SeekiosDTO seekios)
        {
            return Task.FromResult<int>(1);
        }

        private static int idSeekios1 = 1;
        private static int idSeekios2 = 2;
        private static int idUser = 1;

        private List<SeekiosDTO> lsSeekios = new List<SeekiosDTO> { new SeekiosDTO()
            {
                Idseekios = idSeekios1,
                User_iduser = idUser,
                BatteryLife = 100,
                SignalQuality = 90,
                SeekiosName = "Seekios Chien",
                LastKnownLocation_dateLocationCreation = DateTime.Now,
                LastKnownLocation_altitude = 32,
                LastKnownLocation_latitude = 48.856614,
                LastKnownLocation_longitude = 2.352222,
                SeekiosPicture = "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AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==",
            }, new SeekiosDTO()
            {
                Idseekios = idSeekios2,
                User_iduser = idUser,
                BatteryLife = 50,
                SignalQuality = 20,
                SeekiosName = "Seekios Voiture",
                LastKnownLocation_dateLocationCreation = DateTime.Now,
                LastKnownLocation_altitude = 32,
                LastKnownLocation_latitude = 50.856614,
                LastKnownLocation_longitude = 4.352222,
                SeekiosPicture = "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AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==",
            } };

        #endregion
    }
}
