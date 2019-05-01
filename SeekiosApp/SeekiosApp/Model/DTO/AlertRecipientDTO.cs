using Newtonsoft.Json;

namespace SeekiosApp.Model.DTO
{
    public class AlertRecipientDTO
    {
        [JsonProperty(PropertyName = "IdalertRecipient")]
        public int IdRecipient { get; set; }
        [JsonProperty(PropertyName = "Alert_idalert")]
        public int IdAlert { get; set; }
        [JsonProperty(PropertyName = "NameRecipient")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "PhoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty(PropertyName = "PhoneNumberType")]
        public string PhoneNumberType { get; set; }
        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "EmailType")]
        public string EmailType { get; set; }
    }
}
