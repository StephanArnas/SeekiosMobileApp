using Newtonsoft.Json;
using System;

namespace SeekiosApp.Model.APP
{
    public class LocationUpperLowerDates
    {
        [JsonProperty(PropertyName = "UppderDate")]
        public DateTime UppderDate { get; set; }
        [JsonProperty(PropertyName = "LowerDate")]
        public DateTime LowerDate { get; set; }
    }
}
