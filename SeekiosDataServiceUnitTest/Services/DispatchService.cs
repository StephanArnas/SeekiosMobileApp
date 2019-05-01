using SeekiosApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosDataServiceUnitTest.Services
{
    public class DispatchService : IDispatchOnUIThread
    {
        public void Invoke(Action action)
        {
            action?.Invoke(); 
        }
    }
}
