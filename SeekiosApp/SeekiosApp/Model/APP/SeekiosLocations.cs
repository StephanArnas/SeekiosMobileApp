using GalaSoft.MvvmLight;
using SeekiosApp.Model.DTO;
using System;
using System.Collections.Generic;

namespace SeekiosApp.Model.APP
{
    public class SeekiosLocations
    {
        public int IdSeekios { get; set; }
        public List<LocationDTO> LsLocations { get; set; }
        public DateTime? LimitUpperDate { get; set; }
        public DateTime? LimitLowerDate { get; set; }
    }
}
