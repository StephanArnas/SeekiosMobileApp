using Newtonsoft.Json;

namespace SeekiosApp.Model.DTO
{
    public class FriendUserDTO
    {
        [JsonProperty(PropertyName = "Iduser")]
        public int IdUser { get; set; }
        [JsonProperty(PropertyName = "FirstName")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "LastName")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "User_IdUser")]
        public int User_IdUser { get; set; }
        [JsonProperty(PropertyName = "Friend_IdUser")]
        public int Friend_IdUser { get; set; }
        [JsonProperty(PropertyName = "UserPicture")]
        public string UserPicture { get; set; }
    }
}
