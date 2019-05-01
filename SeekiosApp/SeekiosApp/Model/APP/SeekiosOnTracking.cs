using SeekiosApp.Enum;
using SeekiosApp.Model.DTO;
using System;
using System.Linq;

namespace SeekiosApp.Model.APP
{
    public class SeekiosOnTracking
    {
        public SeekiosDTO Seekios { get; set; }
        public ModeDTO Mode { get; set; }
        public Timers.Timer Timer { get; set; }
        public int RefreshTime { get; set; }
        public int MaxRefreshTime
        {
            get
            {
                return RefreshTime * 60;
            }
        }

        private DateTime _runJustOneTime = DateTime.Now.AddSeconds(-1);

        public void SetAndStartTimer()
        {
            //we need to wait at least one position from the seekios before to start the timer
            if (Seekios.LastKnownLocation_dateLocationCreation.HasValue && Seekios.LastKnownLocation_dateLocationCreation.Value.ToLocalTime() < Mode.DateModeCreation.ToLocalTime()) return;
            // if the timer is already running, need to stop it
            if (Timer != null && Timer.IsRunning) Timer.Stop();
            // get the last location
            var lastLocation = App.CurrentUserEnvironment.LsLocations
                .Where(el => el.Mode_idmode == Mode.Idmode)
                .OrderBy(el => el.DateLocationCreation)
                .LastOrDefault();

            // date last location is mode creation date by default
            var dateLastLocation = Seekios.DateLastCommunication ?? Mode.DateModeCreation;
            if (lastLocation != null)
            {
                // if at least one location exists we take the date of the last one
                dateLastLocation = lastLocation.DateLocationCreation.ToLocalTime();
            }
            else if (Mode.ModeDefinition_idmodeDefinition != (int)ModeDefinitionEnum.ModeTracking)
            {
                // if not, if we are not in tracking mode, we take the date of alert event
                dateLastLocation = Mode.LastTriggeredAlertDate ?? DateTime.Now;
            }
            var timeSinceLastLocation = DateTime.Now - dateLastLocation;
            if (timeSinceLastLocation.TotalSeconds < 0)
            {
                timeSinceLastLocation = new TimeSpan(0);
            }

            // setup the timer
            if (timeSinceLastLocation.TotalMinutes > RefreshTime)
            {
                Timer.CountDown = MaxRefreshTime - MaxRefreshTime;
            }
            else Timer.CountDown = MaxRefreshTime - (int)(DateTime.Now - dateLastLocation).TotalSeconds;
            Timer.Tick = () =>
            {
                if (_runJustOneTime.AddMilliseconds(750) > DateTime.Now) return;
                _runJustOneTime = DateTime.Now;
                if (Timer.CountDown <= 0)
                {
                    Timer.CountDown = 0;
                    Timer.Stop();
                }
                Timer.UpdateUI?.Invoke();
                Timer.CountDown--;
            };
            if (!Timer.IsRunning) Timer.Start();
        }
    }
}
