using Newtonsoft.Json;
using SeekiosApp.Model.DTO;

namespace SeekiosApp.Model.APP
{
    public class LocalCredentials
    {
        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "Password")]
        public string Password { get; set; }
        [JsonProperty(PropertyName = "AuthToken")]
        public TokenDTO Token { get; set; }
    }
}
