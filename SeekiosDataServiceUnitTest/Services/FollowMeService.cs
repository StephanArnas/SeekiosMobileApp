using SeekiosApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeekiosApp.Model.DTO;

namespace SeekiosDataServiceUnitTest.Services
{
    public class FollowMeService : IFollowMeService
    {
        public event EventHandler<ConnexionStateChangedEventArgs> ConnexionStateChanged;

        public bool TryToStartFollowMe(SeekiosDTO seekios)
        {
            return true;
        }
    }
}
