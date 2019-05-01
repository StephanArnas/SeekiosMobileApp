using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosApp.Model.DTO
{
    public class VersionEmbeddedDTO
    {
        [JsonProperty(PropertyName = "IdVersionEmbedded")]
        public int IdVersionEmbedded { get; set; }
        [JsonProperty(PropertyName = "VersionName")]
        public string VersionName { get; set; }
        [JsonProperty(PropertyName = "DateVersionCreation")]
        public DateTime DateVersionCreation { get; set; }
        [JsonProperty(PropertyName = "ReleaseNotes")]
        public string ReleaseNotes { get; set; }
        [JsonProperty(PropertyName = "IsBetaVersion")]
        public bool IsBetaVersion { get; set; }
        [JsonProperty(PropertyName = "SHA1Hash")]
        public string SHA1Hash { get; set; }
    }
}
