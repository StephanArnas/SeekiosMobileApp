using Newtonsoft.Json;

namespace SeekiosApp.Model.DTO
{
    public class UserWithLanguageDTO : UserDTO
    {
        [JsonProperty(PropertyName = "Language")]
        public string Language { get; set; }
    }
}
