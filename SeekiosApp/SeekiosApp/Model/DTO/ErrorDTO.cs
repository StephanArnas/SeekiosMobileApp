using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosApp.Model.DTO
{
    public class ErrorDTO
    {
        [JsonProperty(PropertyName = "ErrorCode")]
        public string ErrorCode { get; private set; }
        [JsonProperty(PropertyName = "ErrorInfo")]
        public string ErrorInfo { get; private set; }
        [JsonProperty(PropertyName = "ErrorDetails")]
        public string ErrorDetails { get; private set; }
    }
}
