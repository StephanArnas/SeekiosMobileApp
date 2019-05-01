using Newtonsoft.Json;

namespace SeekiosApp.Model.DTO
{
    public class SubscriptionDTO
    {
        [JsonProperty(PropertyName = "Idsubscription")]
        public int Idsubscription { get; set; }
        [JsonProperty(PropertyName = "SubscriptionName")]
        public string SubscriptionName { get; set; }
        [JsonProperty(PropertyName = "SubscriptionPrice")]
        public double SubscriptionPrice { get; set; }
    }
}
