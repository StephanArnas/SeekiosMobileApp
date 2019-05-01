using System;
using Android.Content;
using SeekiosApp.Interfaces;

namespace SeekiosApp.Droid.Services
{
    class SaveDataService : ISaveDataService
    {
        /// <summary>
        /// Contexte de l'application. N�cessaire pour appeler GetSharedPreferences
        /// </summary>
        private Context _context;

        private const string _globalParametersName = "Global";

        /// <summary>
        /// Initialise le service si n�cessaire 
        /// (Pour Android il est n�cessaire d'ajouter le contexte de l'application pour acc�der aux donn�es sauvegard�es)
        /// </summary>
        public void Init(object require)
        {
            if (require is Context)
            {
                _context = require as Context;
            }
            else
            {
                throw new Exception("Le context n�cessaire pour la sauvegarde de param�tre n'est pas correcte");
            }
        }

        /// <summary>
        /// Obtenir une donn�e avec sa cl�. Retourne Chaine vide si la cl� n'existe pas
        /// </summary>
        public string GetData(string key)
        {
            var prefs = _context.GetSharedPreferences(_globalParametersName, FileCreationMode.Private);
            return prefs.GetString(key, string.Empty);
        }

        /// <summary>
        /// Supprime une donn�e. Si la cl� n'existe pas il ne se passe rien
        /// </summary>
        public void RemoveData(string key)
        {
            if (!Contains(key))
                return;

            var prefs = _context.GetSharedPreferences(_globalParametersName, FileCreationMode.Private);
            var edit = prefs.Edit();
            edit.Remove(key);
            edit.Commit();
        }

        /// <summary>
        /// Enregistre une donn�e avec sa cl� et sa valeur
        /// </summary>
        public void SaveData(string key, object data)
        {
            var prefs = _context.GetSharedPreferences(_globalParametersName, FileCreationMode.Private);
            var edit = prefs.Edit();
            edit.PutString(key, data.ToString());
            edit.Commit();
        }

        /// <summary>
        /// Permet de v�rifier qu'une donn�e existe
        /// </summary>
        public bool Contains(string key)
        {
            var prefs = _context.GetSharedPreferences(_globalParametersName, FileCreationMode.Private);
            return prefs.Contains(key);
        }
    }
}