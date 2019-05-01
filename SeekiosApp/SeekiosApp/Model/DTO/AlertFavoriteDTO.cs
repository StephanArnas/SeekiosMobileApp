using Newtonsoft.Json;

namespace SeekiosApp.Model.DTO
{
    public class AlertFavoriteDTO
    {
        [JsonProperty(PropertyName = "IdalertFavorite")]
        public int IdAlertFavorite { get; set; }
        [JsonProperty(PropertyName = "AlerteDefintion_IdAlerteDefinition")]
        public int IdAlertType { get; set; }
        [JsonProperty(PropertyName = "User_iduser")]
        public int? IdUser { get; set; }
        [JsonProperty(PropertyName = "Title")]
        public string EmailObject { get; set; }
        [JsonProperty(PropertyName = "Content")]
        public string Content { get; set; }
        [JsonProperty(PropertyName = "Record")]
        public byte[] Record { get; set; }
    }
}
