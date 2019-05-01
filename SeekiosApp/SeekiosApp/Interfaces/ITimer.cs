using System;

namespace SeekiosApp.Interfaces
{
    public interface ITimer
    {
        void SetEndEvent(EventHandler e);

        void UnsetEndEvent(EventHandler e);

        void SetEnabled(bool enabled, int interval);
    }
}