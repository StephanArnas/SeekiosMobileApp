using System;
using SeekiosApp.Interfaces;
using Foundation;
using UIKit;

namespace SeekiosApp.iOS.Services
{
    public class SaveDataService : ISaveDataService
    {
        /// <summary>
        /// Contexte de l'application. Nécessaire pour appeler GetSharedPreferences
        /// </summary>

        private const string _globalParametersName = "Global";


        public SaveDataService()
        {

        }

        /// <summary>
        /// Initialise le service si nécessaire 
        /// (Pour Android il est nécessaire d'ajouter le contexte de l'application pour accéder aux données sauvegardées)
        /// </summary>
        public void Init(object require)
        {
            // It is not neccessory to initialise object.
        }

        /// <summary>
        /// Obtenir une donnée avec sa clé. Retourne Chaine vide si la clé n'existe pas
        /// </summary>
        public string GetData(string key)
        {
            string value = NSUserDefaults.StandardUserDefaults.StringForKey(key);
            if (value == null)
                return "";
            else
                return value;
        }

        /// <summary>
        /// Supprime une donnée. Si la clé n'existe pas il ne se passe rien
        /// </summary>
        public void RemoveData(string key)
        {
            if (!Contains(key))
                return;

            NSUserDefaults.StandardUserDefaults.RemoveObject(key);
            NSUserDefaults.StandardUserDefaults.Synchronize();

        }

        /// <summary>
        /// Enregistre une donnée avec sa clé et sa valeur
        /// </summary>
        public void SaveData(string key, object data)
        {
            NSUserDefaults.StandardUserDefaults.SetString(data.ToString(), key);
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }

        /// <summary>
        /// Permet de vérifier qu'une donnée existe
        /// </summary>
        public bool Contains(string key)
        {
            if (string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey(key))) return false;
            return true;
        }
    }
}