using Newtonsoft.Json;
using SeekiosApp.Enum;

namespace SeekiosApp.Model.APP
{
    public class TrackingSetting
    {
        [JsonProperty(PropertyName = "IsEnable")]
        public bool IsEnable { get; set; }
        [JsonProperty(PropertyName = "RefreshTime")]
        public int RefreshTime { get; set; }
        [JsonProperty(PropertyName = "IdSeekios")]
        public int IdSeekios { get; set; }
        [JsonProperty(PropertyName = "ModeDefinition")]
        public ModeDefinitionEnum ModeDefinition { get; set; }
        [JsonProperty(PropertyName = "IsPowerSavingEnabled")]
        public bool IsPowerSavingEnabled { get; set; }
    }
}
