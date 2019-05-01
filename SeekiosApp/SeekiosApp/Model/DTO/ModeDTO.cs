using Newtonsoft.Json;
using System;

namespace SeekiosApp.Model.DTO
{
    public class ModeDTO
    {
        public ModeDTO() { }

        [JsonProperty(PropertyName = "Idmode")]
        public int Idmode { get; set; }
        [JsonProperty(PropertyName = "DateModeCreation")]
        public DateTime DateModeCreation { get; set; }
        [JsonProperty(PropertyName = "DateModeActivation")]
        public DateTime? DateModeActivation { get; set; }
        [JsonProperty(PropertyName = "Trame")]
        public string Trame { get; set; }
        [JsonProperty(PropertyName = "NotificationPush")]
        public int NotificationPush { get; set; }
        [JsonProperty(PropertyName = "CountOfTriggeredAlert")]
        public int CountOfTriggeredAlert { get; set; }
        [JsonProperty(PropertyName = "LastTriggeredAlertDate")]
        public DateTime? LastTriggeredAlertDate { get; set; }
        [JsonProperty(PropertyName = "Seekios_idseekios")]
        public int Seekios_idseekios { get; set; }
        [JsonProperty(PropertyName = "ModeDefinition_idmodeDefinition")]
        public int ModeDefinition_idmodeDefinition { get; set; }
        [JsonProperty(PropertyName = "StatusDefinition_idstatusDefinition")]
        public int StatusDefinition_idstatusDefinition { get; set; }
        [JsonProperty(PropertyName = "Device_iddevice")]
        public int Device_iddevice { get; set; }
        [JsonProperty(PropertyName = "IsPowerSavingEnabled")]
        public bool IsPowerSavingEnabled { get; set; }
    }
}
