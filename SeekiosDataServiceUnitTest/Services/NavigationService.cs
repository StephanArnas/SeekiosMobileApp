using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosDataServiceUnitTest.Services
{
    public class NavigationService : INavigationService
    {
        public string CurrentPageKey
        {
            get
            {
                return "HelloWorld";
            }
        }

        public void GoBack()
        {
            
        }

        public void NavigateTo(string pageKey)
        {
            
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            
        }
    }
}
