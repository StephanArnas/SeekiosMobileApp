using Newtonsoft.Json;

namespace SeekiosApp.Model.DTO
{
    public class PackCreditDTO
    {
        [JsonProperty(PropertyName = "IdPackCredit")]
        public int IdPackCredit { get; set; }
        [JsonProperty(PropertyName = "IdProduct")]
        public string IdProduct { get; set; }
        [JsonProperty(PropertyName = "Price")]
        public string  Price { get; set; }
        [JsonProperty(PropertyName = "Rewarding")]
        public string RewardingCredit { get; set; }
        [JsonProperty(PropertyName = "Title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "IsPromotion")]
        public int IsPromotion { get; set; }
        [JsonProperty(PropertyName = "Promotion")]
        public string Promotion { get; set; }
        [JsonProperty(PropertyName = "ColorBackground")]
        public string ColorBackground { get; set; }
        [JsonProperty(PropertyName = "ColorHeaderBackground")]
        public string ColorHeaderBackground { get; set; }
    }
}
