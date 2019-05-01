using System;

namespace SeekiosApp.Interfaces
{
    public interface IInternetConnectionService
    {
        bool IsDeviceConnectedToInternet();

        bool IsDeviceBeingConnectedToInternet();

        void Initialize(object context, string connectivity);
    }
}