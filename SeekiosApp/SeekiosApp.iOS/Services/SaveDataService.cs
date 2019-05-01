using System;
using SeekiosApp.Interfaces;
using Foundation;
using UIKit;

namespace SeekiosApp.iOS.Services
{
    public class SaveDataService : ISaveDataService
    {
        /// <summary>
        /// Contexte de l'application. N�cessaire pour appeler GetSharedPreferences
        /// </summary>

        private const string _globalParametersName = "Global";


        public SaveDataService()
        {

        }

        /// <summary>
        /// Initialise le service si n�cessaire 
        /// (Pour Android il est n�cessaire d'ajouter le contexte de l'application pour acc�der aux donn�es sauvegard�es)
        /// </summary>
        public void Init(object require)
        {
            // It is not neccessory to initialise object.
        }

        /// <summary>
        /// Obtenir une donn�e avec sa cl�. Retourne Chaine vide si la cl� n'existe pas
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
        /// Supprime une donn�e. Si la cl� n'existe pas il ne se passe rien
        /// </summary>
        public void RemoveData(string key)
        {
            if (!Contains(key))
                return;

            NSUserDefaults.StandardUserDefaults.RemoveObject(key);
            NSUserDefaults.StandardUserDefaults.Synchronize();

        }

        /// <summary>
        /// Enregistre une donn�e avec sa cl� et sa valeur
        /// </summary>
        public void SaveData(string key, object data)
        {
            NSUserDefaults.StandardUserDefaults.SetString(data.ToString(), key);
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }

        /// <summary>
        /// Permet de v�rifier qu'une donn�e existe
        /// </summary>
        public bool Contains(string key)
        {
            if (string.IsNullOrEmpty(NSUserDefaults.StandardUserDefaults.StringForKey(key))) return false;
            return true;
        }
    }
}