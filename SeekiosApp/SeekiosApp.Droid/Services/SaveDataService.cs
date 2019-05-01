using System;
using Android.Content;
using SeekiosApp.Interfaces;

namespace SeekiosApp.Droid.Services
{
    class SaveDataService : ISaveDataService
    {
        /// <summary>
        /// Contexte de l'application. Nécessaire pour appeler GetSharedPreferences
        /// </summary>
        private Context _context;

        private const string _globalParametersName = "Global";

        /// <summary>
        /// Initialise le service si nécessaire 
        /// (Pour Android il est nécessaire d'ajouter le contexte de l'application pour accéder aux données sauvegardées)
        /// </summary>
        public void Init(object require)
        {
            if (require is Context)
            {
                _context = require as Context;
            }
            else
            {
                throw new Exception("Le context nécessaire pour la sauvegarde de paramètre n'est pas correcte");
            }
        }

        /// <summary>
        /// Obtenir une donnée avec sa clé. Retourne Chaine vide si la clé n'existe pas
        /// </summary>
        public string GetData(string key)
        {
            var prefs = _context.GetSharedPreferences(_globalParametersName, FileCreationMode.Private);
            return prefs.GetString(key, string.Empty);
        }

        /// <summary>
        /// Supprime une donnée. Si la clé n'existe pas il ne se passe rien
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
        /// Enregistre une donnée avec sa clé et sa valeur
        /// </summary>
        public void SaveData(string key, object data)
        {
            var prefs = _context.GetSharedPreferences(_globalParametersName, FileCreationMode.Private);
            var edit = prefs.Edit();
            edit.PutString(key, data.ToString());
            edit.Commit();
        }

        /// <summary>
        /// Permet de vérifier qu'une donnée existe
        /// </summary>
        public bool Contains(string key)
        {
            var prefs = _context.GetSharedPreferences(_globalParametersName, FileCreationMode.Private);
            return prefs.Contains(key);
        }
    }
}