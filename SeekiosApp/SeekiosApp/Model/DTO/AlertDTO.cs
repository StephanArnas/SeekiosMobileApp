using Newtonsoft.Json;
using System;

namespace SeekiosApp.Model.DTO
{
   public class AlertDTO : BaseDTO
    {
        [JsonProperty(PropertyName = "Idalert")]
        public int IdAlert { get; set; }
        [JsonProperty(PropertyName = "AlertDefinition_idalertType")]
        public int IdAlertType { get; set; }
        [JsonProperty(PropertyName = "Mode_idmode")]
        public int? IdMode { get; set; }
        [JsonProperty(PropertyName = "Title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "Content")]
        public string Content { get; set; }
        [JsonProperty(PropertyName = "CreationDate")]
        public DateTime CreationDate { get; set; }
    }
}
