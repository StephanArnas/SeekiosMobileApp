using Newtonsoft.Json;
using System;

namespace SeekiosApp.Model.DTO
{
    public class PurchaseDTO
    {
        [JsonProperty(PropertyName = "IdUser")]
        public int IdUser { get; set; }
        [JsonProperty(PropertyName = "InnerData")]
        public string InnerData { get; set; }
        [JsonProperty(PropertyName = "StoreId")]
        public int StoreId { get; set; }
        [JsonProperty(PropertyName = "Signature")]
        public String Signature { get; set; }
        [JsonProperty(PropertyName = "VersionApp")]
        public String VersionApp { get; set; }
        [JsonProperty(PropertyName = "KeyProduct")]
        public string KeyProduct { get; set; }
    }
}
