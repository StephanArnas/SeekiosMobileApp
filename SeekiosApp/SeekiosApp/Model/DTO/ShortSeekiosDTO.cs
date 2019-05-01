using Newtonsoft.Json;

namespace SeekiosApp.Model.DTO
{
    public class ShortSeekiosDTO
    {
        [JsonProperty(PropertyName = "Idseekios")]
        public int IdSeekios { get; set; }
        [JsonProperty(PropertyName = "SeekiosName")]
        public string SeekiosName { get; set; }
        [JsonProperty(PropertyName = "SeekiosPicture")]
        public string SeekiosPicture { get; set; }
        [JsonProperty(PropertyName = "User_iduser")]
        public string User_iduser { get; set; }
    }
}
