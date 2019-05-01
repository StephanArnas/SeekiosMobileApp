using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SeekiosApp.Model.DTO
{
    public class OperationDTO
    {
        [JsonProperty(PropertyName = "IdO")]
        public int IdOperation { get; set; }
        [JsonProperty(PropertyName = "IdU")]
        public int? IdUser { get; set; }
        [JsonProperty(PropertyName = "IdS")]
        public int? IdSeekios { get; set; }
        [JsonProperty(PropertyName = "IdM")]
        public int? IdMode { get; set; }
        [JsonProperty(PropertyName = "Op")]
        public int OperationType { get; set; }
        [JsonProperty(PropertyName = "CA")]
        public int CreditAmount { get; set; }
        [JsonProperty(PropertyName = "DE")]
        public DateTime? DateEnd { get; set; }
        [JsonProperty(PropertyName = "DB")]
        public DateTime DateBegin { get; set; }
        [JsonProperty(PropertyName = "IdD")]
        public int? IdDevice { get; set; }
    }
}
