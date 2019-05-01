using Newtonsoft.Json;
using System;

namespace SeekiosApp.Model.DTO
{
    public class UserDTO
    {
        [JsonProperty(PropertyName = "Iduser")]
        public int IdUser { get; set; }
        [JsonProperty(PropertyName = "IdCountryResource")]
        public int IdCountryResource { get; set; }
        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "Password")]
        public string Password { get; set; }
        [JsonProperty(PropertyName = "FirstName")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "LastName")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "RemainingRequest")]
        public int RemainingRequest { get; set; }
        [JsonProperty(PropertyName = "UserPicture")]
        public string UserPicture { get; set; }
        [JsonProperty(PropertyName = "IsValidate")]
        public bool IsValidate { get; set; }
        [JsonProperty(PropertyName = "DateLastConnection")]
        public DateTime? DateLastConnection { get; set; }
    }
}
