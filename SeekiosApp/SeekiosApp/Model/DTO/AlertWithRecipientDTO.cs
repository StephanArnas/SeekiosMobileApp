using Newtonsoft.Json;
using System.Collections.Generic;

namespace SeekiosApp.Model.DTO
{
    public class AlertWithRecipientDTO : AlertDTO
    {
        public AlertWithRecipientDTO()
        {
            LsRecipients = new List<AlertRecipientDTO>();
        }

        public AlertWithRecipientDTO(AlertDTO alert)
        {
            IdAlert = alert.IdAlert;
            IdAlertType = alert.IdAlertType;
            IdMode = alert.IdMode;
            Title = alert.Title;
            Content = alert.Content;
            LsRecipients = new List<AlertRecipientDTO>();
        }

        [JsonProperty(PropertyName = "LsRecipients")]
        public List<AlertRecipientDTO> LsRecipients { get; set; }
    }
}
