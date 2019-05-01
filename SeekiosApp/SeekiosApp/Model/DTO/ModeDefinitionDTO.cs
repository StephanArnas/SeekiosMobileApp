using Newtonsoft.Json;

namespace SeekiosApp.Model.DTO
{
    public class ModeDefinitionDTO
    {
        [JsonProperty(PropertyName = "IdmodeDefinition")]
        public int IdmodeDefinition { get; set; }
        [JsonProperty(PropertyName = "ModeName")]
        public string ModeName { get; set; }
        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }
    }
}
