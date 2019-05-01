using Newtonsoft.Json;
using System;

namespace SeekiosApp.Model.DTO
{
    public class CreditTransactionDTO
    {
        [JsonProperty(PropertyName = "IdCreditTransaction")]
        public int IdCreditTransaction { get; set; }
        [JsonProperty(PropertyName = "IdUser")]
        public int IdUser { get; set; }
        [JsonProperty(PropertyName = "IdSeekios")]
        public int IdSeekios { get; set; }
        [JsonProperty(PropertyName = "IdMode")]
        public int IdMode { get; set; }
        [JsonProperty(PropertyName = "IdModeDefinition")]
        public int IdModeDefinition { get; set; }
        [JsonProperty(PropertyName = "InstructionType")]
        public int InstructionType { get; set; }
        [JsonProperty(PropertyName = "CreditAmount")]
        public int CreditAmount { get; set; }
        [JsonProperty(PropertyName = "Date")]
        public DateTime Date { get; set; }
        [JsonProperty(PropertyName = "DebitType")]
        public int DebitType { get; set; }
        [JsonProperty(PropertyName = "IdDevice")]
        public int IdDevice { get; set; }
    }
}
