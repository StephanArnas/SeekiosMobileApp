using Newtonsoft.Json;

namespace SeekiosApp.Model.DTO
{
    public class ShortUserDTO
    {
        [JsonProperty(PropertyName = "IdUser")]
        public int IdUser { get; set; }
        [JsonProperty(PropertyName = "FirstName")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "LastName")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "UserPicture")]
        public string UserPicture { get; set; }
    }
}
