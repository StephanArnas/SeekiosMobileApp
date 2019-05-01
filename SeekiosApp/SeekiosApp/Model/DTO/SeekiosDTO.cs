using Newtonsoft.Json;
using SeekiosApp.Enum.FromDataBase;
using System;

namespace SeekiosApp.Model.DTO
{
    public class SeekiosDTO : SeekiosProductionDTO
    {
        [JsonProperty(PropertyName = "Idseekios")]
        public int Idseekios { get; set; }
        [JsonProperty(PropertyName = "PinCode")]
        public string PinCode { get; set; }
        [JsonProperty(PropertyName = "SeekiosName")]
        public string SeekiosName { get; set; }
        [JsonProperty(PropertyName = "SeekiosPicture")]
        public string SeekiosPicture { get; set; }
        [JsonProperty(PropertyName = "SeekiosDateCreation")]
        public DateTime SeekiosDateCreation { get; set; }
        [JsonProperty(PropertyName = "BatteryLife")]
        public int BatteryLife { get; set; }
        [JsonProperty(PropertyName = "SignalQuality")]
        public int SignalQuality { get; set; }
        [JsonProperty(PropertyName = "DateLastCommunication")]
        public DateTime? DateLastCommunication { get; set; }
        [JsonProperty(PropertyName = "LastKnownLocation_longitude")]
        public double LastKnownLocation_longitude { get; set; }
        [JsonProperty(PropertyName = "LastKnownLocation_latitude")]
        public double LastKnownLocation_latitude { get; set; }
        [JsonProperty(PropertyName = "LastKnownLocation_altitude")]
        public double LastKnownLocation_altitude { get; set; }
        [JsonProperty(PropertyName = "LastKnownLocation_accuracy")]
        public double LastKnownLocation_accuracy { get; set; }
        [JsonProperty(PropertyName = "LastKnownLocation_dateLocationCreation")]
        public DateTime? LastKnownLocation_dateLocationCreation { get; set; }
        [JsonProperty(PropertyName = "LastKnownLocation_idLocationDefinition")]
        public int LastKnownLocation_idLocationDefinition { get; set; }
        [JsonProperty(PropertyName = "User_iduser")]
        public int User_iduser { get; set; }
        [JsonProperty(PropertyName = "HasGetLastInstruction")]
        public bool HasGetLastInstruction { get; set; }
        [JsonProperty(PropertyName = "IsAlertLowBattery")]
        public bool IsAlertLowBattery { get; set; }
        [JsonProperty(PropertyName = "IsInPowerSaving")]
        public bool IsInPowerSaving { get; set; }
        [JsonProperty(PropertyName = "PowerSaving_hourStart")]
        public int PowerSaving_hourStart { get; set; }
        [JsonProperty(PropertyName = "PowerSaving_hourEnd")]
        public int PowerSaving_hourEnd { get; set; }
        [JsonProperty(PropertyName = "AlertSOS_idalert")]
        public int? AlertSOS_idalert { get; set; }
        [JsonProperty(PropertyName = "IsRefreshingBattery")]
        public bool IsRefreshingBattery { get; set; }
        [JsonProperty(PropertyName = "DateLastOnDemandRequest")]
        public DateTime? DateLastOnDemandRequest { get; set; }
        [JsonProperty(PropertyName = "IsLastSOSRead")]
        public bool IsLastSOSRead { get; set; }
        [JsonProperty(PropertyName = "DateLastSOSSent")]
        public DateTime? DateLastSOSSent { get; set; }
        [JsonProperty(PropertyName = "SendNotificationOnNewTrackingLocation")]
        public bool SendNotificationOnNewTrackingLocation { get; set; }
        [JsonProperty(PropertyName = "SendNotificationOnNewOutOfZoneLocation")]
        public bool SendNotificationOnNewOutOfZoneLocation { get; set; }
        [JsonProperty(PropertyName = "SendNotificationOnNewDontMoveLocation")]
        public bool SendNotificationOnNewDontMoveLocation { get; set; }

        public bool IsOnDemand {
            get
            {
                if (DateLastOnDemandRequest.HasValue)
                {
                    return (DateLastOnDemandRequest.Value.AddSeconds(App.TIME_FOR_REFRESH_SEEKIOS_IN_SECOND) - DateTime.Now).TotalSeconds > 0;
                }
                else return false;
            }
        }
    }
}