using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosApp.Model.DTO
{
    public class TokenDTO
    {
        public TokenDTO()
        {
            CreationDate = DateTime.SpecifyKind(CreationDate, DateTimeKind.Utc);
            ExpirationDate = new DateTime(0, DateTimeKind.Utc);
        }
        [JsonProperty(PropertyName = "AuthToken")]
        public string AuthToken { get; set; }
        [JsonProperty(PropertyName = "IdUser")]
        public int IdUser { get; set; }
        [JsonProperty(PropertyName = "DateCreation")]
        public DateTime CreationDate { get; set; }
        [JsonProperty(PropertyName = "DateExpires")]
        public DateTime ExpirationDate { get; set; }
    }
}
