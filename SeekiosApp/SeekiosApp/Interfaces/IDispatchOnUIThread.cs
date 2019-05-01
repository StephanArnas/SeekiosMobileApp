using System;

namespace SeekiosApp.Interfaces
{
    public interface IDispatchOnUIThread
    {
        void Invoke(Action action);
    }
}
