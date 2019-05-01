using Newtonsoft.Json;
using System;

namespace SeekiosApp.Model.DTO
{
    public class LocationDTO
    {
        [JsonProperty(PropertyName = "IdL")]
        public int Idlocation { get; set; }
        [JsonProperty(PropertyName = "Lo")]
        public double Longitude { get; set; }
        [JsonProperty(PropertyName = "La")]
        public double Latitude { get; set; }
        [JsonProperty(PropertyName = "Al")]
        public double Altitude { get; set; }
        [JsonProperty(PropertyName = "Ac")]
        public double Accuracy { get; set; }
        [JsonProperty(PropertyName = "Dc")]
        public DateTime DateLocationCreation { get; set; }
        [JsonProperty(PropertyName = "IdM")]
        public int? Mode_idmode { get; set; }
        [JsonProperty(PropertyName = "IdS")]
        public int Seekios_idseekios { get; set; }
        [JsonProperty(PropertyName = "IdLD")]
        public int IdLocationDefinition { get; set; }
    }
}
