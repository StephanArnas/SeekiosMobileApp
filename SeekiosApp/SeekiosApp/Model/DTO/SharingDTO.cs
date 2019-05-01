using Newtonsoft.Json;
using System;

namespace SeekiosApp.Model.DTO
{
    public class SharingDTO
    {
        public SharingDTO() { }

        [JsonProperty(PropertyName = "User_IdUser")]
        public int User_IdUser { get; set; }
        [JsonProperty(PropertyName = "Friend_IdUser")]
        public int Friend_IdUser { get; set; }
        [JsonProperty(PropertyName = "Seekios_IdSeekios")]
        public int Seekios_IdSeekios { get; set; }
        [JsonProperty(PropertyName = "DateBeginSharing")]
        public DateTime DateBeginSharing { get; set; }
        [JsonProperty(PropertyName = "DateEndSharing")]
        public DateTime? DateEndSharing { get; set; }
        [JsonProperty(PropertyName = "IsUserOwner")]
        public bool IsUserOwner { get; set; }
    }
}
