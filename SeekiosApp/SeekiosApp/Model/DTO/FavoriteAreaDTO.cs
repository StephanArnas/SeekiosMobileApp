using Newtonsoft.Json;
using System;

namespace SeekiosApp.Model.DTO
{
    public class FavoriteAreaDTO
    {
        [JsonProperty(PropertyName = "IdfavoriteArea")]
        public int IdfavoriteArea { get; set; }
        [JsonProperty(PropertyName = "AreaName")]
        public string AreaName { get; set; }
        [JsonProperty(PropertyName = "Trame")]
        public string Trame { get; set; }
        [JsonProperty(PropertyName = "DateAddedFavorite")]
        public DateTime DateAddedFavorite { get; set; }
        [JsonProperty(PropertyName = "User_iduser")]
        public int User_iduser { get; set; }
        [JsonProperty(PropertyName = "PointsCount")]
        public int PointsCount { get; set; }
        [JsonProperty(PropertyName = "AreaGeodesic")]
        public double AreaGeodesic { get; set; }
    }
}
