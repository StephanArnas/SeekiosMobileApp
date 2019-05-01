using SeekiosApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosDataServiceUnitTest.Utilities
{
    public class InternetConnectionService : IInternetConnectionService
    {
        public void Initialize(object context, string connectivity) { }

        public bool IsDeviceBeingConnectedToInternet()
        {
            return true;
        }

        public bool IsDeviceConnectedToInternet()
        {
            return true;
        }
    }
}
