using SeekiosApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosDataServiceUnitTest.Services
{
    public class SaveDataService : ISaveDataService
    {
        public bool Contains(string key)
        {
            return true;
        }

        public string GetData(string key)
        {
            return key;
        }

        public void Init(object require)
        {
            
        }

        public void RemoveData(string key)
        {
            
        }

        public void SaveData(string key, object data)
        {
            
        }
    }
}
