using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosApp.Model.APP
{
    public class RelaodCreditMonthly
    {
        [JsonProperty(PropertyName = "HasBeenRead")]
        public bool HasBeenRead { get; set; }
        [JsonProperty(PropertyName = "DateHasBeenReand")]
        public DateTime DateHasBeenReand { get; set; }
    }
}
