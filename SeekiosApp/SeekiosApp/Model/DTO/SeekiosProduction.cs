using Newtonsoft.Json;
using System;

namespace SeekiosApp.Model.DTO
{
    public class SeekiosProductionDTO
    {
        [JsonProperty(PropertyName = "IdseekiosProduction")]
        public int IdseekiosProduction { get; set; }
        [JsonProperty(PropertyName = "UIdSeekios")]
        public string UIdSeekios { get; set; }
        [JsonProperty(PropertyName = "Imei")]
        public string Imei { get; set; }
        [JsonProperty(PropertyName = "MacAddress")]
        public string MacAddress { get; set; }
        [JsonProperty(PropertyName = "Imsi")]
        public string Imsi { get; set; }
        [JsonProperty(PropertyName = "LastUpdateConfirmed")]
        public bool LastUpdateConfirmed { get; set; }
        [JsonProperty(PropertyName = "VersionEmbedded_idversionEmbedded")]
        public int VersionEmbedded_idversionEmbedded { get; set; }
        [JsonProperty(PropertyName = "FreeCredit")]
        public int FreeCredit { get; set; }
        [JsonProperty(PropertyName = "DateFirstRegistration")]
        public DateTime? DateFirstRegistration { get; set; }
    }
}
