using Microsoft.Toolkit.Uwp;
using SeekiosApp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SeekiosApp.UWP.Services
{
    public class SaveDataService : ISaveDataService
    {
        private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public bool Contains(string key)
        {
            object result = null;
            _localSettings.Values.TryGetValue(key, out result);
            if (result != null) return true;
            else return false;
        }

        public string GetData(string key)
        {
            return _localSettings.Values[key].ToString();
        }

        public void Init(object require) { }

        public void RemoveData(string key)
        {
            _localSettings.Values[key] = string.Empty;
        }

        public void SaveData(string key, object data)
        {
            _localSettings.Values[key] = data.ToString();
        }
    }
}
