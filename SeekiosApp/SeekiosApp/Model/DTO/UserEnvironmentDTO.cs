using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using SeekiosApp.Enum;
using SeekiosApp.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeekiosApp.Model.DTO
{
    public class UserEnvironmentDTO : ViewModelBase
    {
        public UserEnvironmentDTO()
        {
            LsSeekios = new List<SeekiosDTO>();
            LsMode = new List<ModeDTO>();
            LsAlert = new List<AlertDTO>();
            LsAlertFavorites = new List<AlertFavoriteDTO>();
            LsAlertRecipient = new List<AlertRecipientDTO>();
            LsFavoriteArea = new List<FavoriteAreaDTO>();
            LsFriend = new List<FriendUserDTO>();
            LsSharing = new List<SharingDTO>();
            LsLocations = new List<LocationDTO>();
        }

        [JsonProperty(PropertyName = "User")]
        public UserDTO User { get; set; }
        [JsonProperty(PropertyName = "Device")]
        public DeviceDTO Device { get; set; }
        [JsonProperty(PropertyName = "LsSeekios")]
        public List<SeekiosDTO> LsSeekios { get; set; }
        [JsonProperty(PropertyName = "LsMode")]
        public List<ModeDTO> LsMode { get; set; }
        [JsonProperty(PropertyName = "LsAlert")]
        public List<AlertDTO> LsAlert { get; set; }
        [JsonProperty(PropertyName = "LsAlertFavorite")]
        public List<AlertFavoriteDTO> LsAlertFavorites { get; set; }
        [JsonProperty(PropertyName = "LsAlertRecipient")]
        public List<AlertRecipientDTO> LsAlertRecipient { get; set; }
        [JsonProperty(PropertyName = "LsFriend")]
        public List<FriendUserDTO> LsFriend
        {
            get { return lsFriend; }
            set
            {
                lsFriend = value;
                RaisePropertyChanged("LsFriend");
            }
        }
        private List<FriendUserDTO> lsFriend;

        [JsonProperty(PropertyName = "LsSharing")]
        public List<SharingDTO> LsSharing
        {
            get
            {
                return lsSharing;
            }
            set
            {
                lsSharing = value;
                RaisePropertyChanged("LsSharing");
            }
        }
        private List<SharingDTO> lsSharing;
        
        [JsonProperty(PropertyName = "LsLocations")]
        public List<LocationDTO> LsLocations { get; set; }
        [JsonProperty(PropertyName = "LsFavoriteArea")]
        public List<FavoriteAreaDTO> LsFavoriteArea { get; set; }
        [JsonProperty(PropertyName = "ServerSynchronisationDate")]
        public DateTime ServerSynchronisationDate { get; set; }

        public DateTime ClientSynchronisationDate { get; set; }

        [JsonProperty(PropertyName = "LastVersionEmbedded")]
        public VersionEmbeddedDTO LastVersionEmbedded { get; set; }

        public DateTime ServerCurrentDateTime
        {
            get
            {
                var jetLag = ServerSynchronisationDate - ClientSynchronisationDate;
                return DateHelper.GetSystemTime() + jetLag;
            }
        }

        /// <summary>
        /// Gathers the alerts configured for a seekios in a list
        /// </summary>
        /// <param name="seekios">Seekios</param>
        /// <returns>alert list</returns>
        public List<AlertDTO> GetAlertsFromSeekios(SeekiosDTO seekios)
        {
            var source = new List<AlertDTO>(from a in LsAlert
                                            join m in LsMode on a.IdMode equals m.Idmode
                                            join s in LsSeekios on m.Seekios_idseekios equals s.Idseekios
                                            where s.Idseekios == seekios.Idseekios
                                            select a);
            return source;
        }

        /// <summary>
        /// Get the whole alerts list
        /// </summary>
        /// <returns>alert list</returns>
        public List<AlertDTO> GetAlertsFromSeekios()
        {
            var source = new List<AlertDTO>(from a in LsAlert
                                            join m in LsMode on a.IdMode equals m.Idmode
                                            join s in LsSeekios on m.Seekios_idseekios equals s.Idseekios
                                            select a);
            return source;
        }

        /// <summary>
        /// Count how many Seekios are configured with at least an alert and a mode
        ///</summary>
        public int GetNumberOfSeekiosWithAlert()
        {
            return (from a in LsAlert
                    join m in LsMode on a.IdMode equals m.Idmode
                    join s in LsSeekios on m.Seekios_idseekios equals s.Idseekios
                    group s by s.Idseekios into seekios
                    select seekios).Count();
        }

        /// <summary>
        /// Get the seekios related to an alert
        /// </summary>
        /// <param name="alert">alert</param>
        /// <returns>seekios</returns>
        public SeekiosDTO GetSeekiosFromAlert(AlertDTO alert)
        {
            if (alert == null) return null;
            return (from s in LsSeekios
                    join m in App.CurrentUserEnvironment.LsMode on s.Idseekios equals m.Seekios_idseekios
                    join a in App.CurrentUserEnvironment.LsAlert on m.Idmode equals a.IdMode
                    where a.IdAlert == alert.IdAlert
                    select s).FirstOrDefault();
        }

        /// <summary>
        /// Get the mode configured on a seekios
        /// </summary>
        /// <param name="seekios"></param>
        /// <returns></returns>
        public ModeDTO GetModeFromSeekios(SeekiosDTO seekios)
        {
            if (seekios == null) return null;
            return (from m in LsMode
                    where m.Seekios_idseekios == seekios.Idseekios
                    select m).FirstOrDefault();
        }

        /// <summary>
        /// Get the recipients configured in an alert
        /// </summary>
        /// <param name="idAlert"></param>
        /// <returns></returns>
        public List<AlertRecipientDTO> GetRecipientsFromAlert(int idAlert)
        {
            if (idAlert == 0) return null;
            return (from a in LsAlertRecipient
                    where a.IdAlert == idAlert
                    select a).ToList();
        }

        /// <summary>
        /// Get the alert favorite according to the alert type (SMS, email, voicemail)
        /// </summary>
        public List<AlertFavoriteDTO> GetAlertsFavoritesFromAlertType(AlertDefinitionEnum alertType)
        {
            return (from af in LsAlertFavorites
                    where af.IdAlertType == (int)alertType
                    select af).ToList();
        }

        /// <summary>
        /// Get the list of the seekios which belong to the user
        /// </summary>
        public List<SeekiosDTO> GetSeekiosOfUsers()
        {
            return (from s in LsSeekios
                    where s.User_iduser == User.IdUser
                    orderby s.SeekiosName
                    select s).ToList();
        }
    }
}
