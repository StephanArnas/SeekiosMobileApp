using Newtonsoft.Json;
using System;

namespace SeekiosApp.Model.DTO
{
    public class OperationFromStore
    {
        [JsonProperty(PropertyName = "IdOperationFromStore")]
        public int IdOperationFromStore { get; set; }
        [JsonProperty(PropertyName = "IdPack")]
        public int IdPack { get; set; }
        [JsonProperty(PropertyName = "IdUser")]
        public int IdUser { get; set; }
        [JsonProperty(PropertyName = "PricePaid")]
        public double PricePaid { get; set; }
        [JsonProperty(PropertyName = "Status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "DateTransaction")]
        public DateTime DateTransaction { get; set; }
        [JsonProperty(PropertyName = "RefStore")]
        public string RefStore { get; set; }
        [JsonProperty(PropertyName = "VersionApp")]
        public string VersionApp { get; set; }
        [JsonProperty(PropertyName = "CreditsPurchased")]
        public int CreditsPurchased { get; set; }
        [JsonProperty(PropertyName = "IsPackPremium")]
        public bool IsPackPremium { get; set; }
    }
}
