
namespace SeekiosApp.Interfaces
{
    public interface ISaveDataService
    {
        /// <summary>
        /// Initialize the service if necessary
        /// For Android it is necessary to add the application context to access the saved data with "ApplicationContext"
        /// </summary>
        void Init(object require);

        /// <summary>
        /// Save data with a key and his value associated
        /// </summary>
        void SaveData(string key, object data);

        /// <summary>
        /// Get data by giving its key. Warning : the data must exist (use Contains to verify)
        /// </summary>
        string GetData(string key);

        /// <summary>
        /// Delete data by giving its key. If the key can't be found, nothing happens
        /// </summary>
        void RemoveData(string key);

        /// <summary>
        /// Method used to verify if a data exists
        /// </summary>
        bool Contains(string key);
    }
}
