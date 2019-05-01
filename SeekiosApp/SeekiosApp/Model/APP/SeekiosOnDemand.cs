using SeekiosApp.Model.DTO;
using System;

namespace SeekiosApp.Model.APP
{
    public class SeekiosOnDemand
    {
        public SeekiosDTO Seekios { get; set; }
        public Timers.Timer Timer { get; set; }
        public DateTime DateEndRefreshTimer { get; set; }
        public Action OnSuccess { get; set; }
        public Action OnFailed { get; set; }
    }
}
