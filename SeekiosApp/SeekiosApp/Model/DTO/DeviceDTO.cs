using Newtonsoft.Json;
using System;

namespace SeekiosApp.Model.DTO
{
    public class DeviceDTO
    {
        [JsonProperty(PropertyName = "Iddevice")]
        public int Iddevice { get; set; }
        [JsonProperty(PropertyName = "UidDevice")]
        public string UidDevice { get; set; }
        [JsonProperty(PropertyName = "DeviceName")]
        public string DeviceName { get; set; }
        [JsonProperty(PropertyName = "Os")]
        public string Os { get; set; }
        [JsonProperty(PropertyName = "Plateform")]
        public string Plateform { get; set; }
        [JsonProperty(PropertyName = "Password")]
        public string Password { get; set; }
        [JsonProperty(PropertyName = "NotificationPlayerId")]
        public string NotificationPlayerId { get; set; }
        [JsonProperty(PropertyName = "LastUseDate")]
        public DateTime LastUseDate { get; set; }
    }
}
