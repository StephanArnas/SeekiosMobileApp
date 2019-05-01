using Newtonsoft.Json;
using System;

namespace SeekiosApp.Model.DTO
{
    public class FriendshipDTO
    {
        [JsonProperty(PropertyName = "User_IdUser")]
        public int User_IdUser { get; set; }
        [JsonProperty(PropertyName = "Friend_IdUser")]
        public int Friend_IdUser { get; set; }
        [JsonProperty(PropertyName = "IsPending")]
        public bool IsPending { get; set; }
        [JsonProperty(PropertyName = "DateFriendshipCreation")]
        public DateTime DateFriendshipCreation { get; set; }
        [JsonProperty(PropertyName = "DateFriendshipAcceptance")]
        public DateTime? DateFriendshipAcceptance { get; set; }
    }
}
