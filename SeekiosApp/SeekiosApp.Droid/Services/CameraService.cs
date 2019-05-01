using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Android.Content.PM;
using Android.Provider;
using SeekiosApp.Droid.Helper;
using Android.Graphics;
using Android.Media;

namespace SeekiosApp.Droid.Services
{
    public class CameraService : IDisposable
    {
        #region ===== Attributs ===================================================================

        private Context _context;
        private const int SIZE_OF_THE_BITMAP = 640; // 640 car la taille maximale d'affichage en xxxdpi est (mdpi 160) * (xxxhdpi 4) soit 640
        private const string DIRECTORY_SEEKIOS_PICTURE = "SeekiosPictures";
        private const string FILENAME_SEEKIOS_PICTURE = "seekios_{0}.jpg";
        private const string INTENT_TYPE = "image/*";
        #endregion

        #region ===== Propriétés ==================================================================

        /// <summary>Image sélectionnée en binaire</summary>
        public byte[] PictureBinary { get; private set; }
        /// <summary>Chemin des image Seekios</summary>
        public File Directory { get; private set; }
        /// <summary>Est ce que le smartphone est équipé d'un appareil photo</summary>
        public bool IsAppToTakePictures { get; private set; }

        #endregion

        #region ===== Constructeur ================================================================

        /// <summary>
        /// Constructeur
        ///  - Création du répertoire des photos
        /// </summary>
        public CameraService(Context context)
        {
            _context = context;
            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();
            }
        }

        #endregion

        #region ===== Méthode Public ==============================================================

        /// <summary>
        /// Lance l'intent de l'appareil photo
        /// </summary>
        public void TakePictureFromCamera()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            Directory = new File(Directory, String.Format(FILENAME_SEEKIOS_PICTURE, Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(Directory));
            ((Activity)_context).StartActivityForResult(intent, 0);
        }

        /// <summary>
        /// Lance l'intent de la gallerie
        /// </summary>
        public void TakePictureFromGallery(string message)
        {
            var imageIntent = new Intent();
            imageIntent.SetType(INTENT_TYPE);
            imageIntent.SetAction(Intent.ActionGetContent);
            ((Activity)_context).StartActivityForResult(Intent.CreateChooser(imageIntent, message), 0);
        }

        /// <summary>
        /// Récupère et redimensionne l'image provenant de l'appareil photo
        /// </summary>
        /// <returns>image redimensionnée</returns>
        public Bitmap GetPictureFromCamera()
        {
            // la directory doit être initialisé pour récupérer le chemin vers l'image
            if (Directory == null) throw new Exception("GetPictureFromCamera: the directory can not be null");
            if (string.IsNullOrEmpty(Directory.Path )) throw new Exception("GetPictureFromCamera: the directory.Path can not be null");
            if (_context == null) throw new Exception("GetPictureFromCamera: the _context can not be null");

            // récupération du chemin vers l'image
            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Android.Net.Uri contentUri = Android.Net.Uri.FromFile(Directory);
            mediaScanIntent.SetData(contentUri);
            _context.SendBroadcast(mediaScanIntent);

            // chemin vers l'image
            var pathFile = Directory.Path;

            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(pathFile, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > SIZE_OF_THE_BITMAP || outWidth > SIZE_OF_THE_BITMAP)
                inSampleSize = outHeight > outWidth ? outHeight / SIZE_OF_THE_BITMAP : outWidth / SIZE_OF_THE_BITMAP;

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(pathFile, options);

            if (outHeight > outWidth)    // portrait
            {
                var centerMiddleHeight = (resizedBitmap.Height - resizedBitmap.Width) / 2;
                resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, centerMiddleHeight, resizedBitmap.Width, resizedBitmap.Width, null, false);
            }
            else                        // paysage
            {
                var centerMiddleWidth = (resizedBitmap.Width - resizedBitmap.Height) / 2;
                resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, centerMiddleWidth, 0, resizedBitmap.Height, resizedBitmap.Height, null, false);
            }

            using (var stream = new System.IO.MemoryStream())
            {
                resizedBitmap.Compress(Bitmap.CompressFormat.Jpeg, 40, stream);
                var bytes = stream.ToArray();
                PictureBinary = stream.ToArray();
            }

            CreateDirectoryForPictures();   // initialisation de Directory 

            return resizedBitmap;
        }

        /// <summary>
        /// Récupère et redimensionne l'image provenant de la librarie
        /// </summary>
        /// <param name="data">URI de l'image sélectionnée</param>
        /// <returns>image redimensionnée</returns>
        public Bitmap GetPictureFromGallery(Android.Net.Uri data)
        {
            System.IO.Stream tmp = null;
            try
            {
                tmp = _context.ContentResolver.OpenInputStream(data);
            }
            catch(FileNotFoundException ex) { }

            if (null == tmp) return null;

            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            var bitmap = Android.Graphics.BitmapFactory.DecodeStream(tmp, null, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > SIZE_OF_THE_BITMAP || outWidth > SIZE_OF_THE_BITMAP)
                inSampleSize = outHeight > outWidth ? outHeight / SIZE_OF_THE_BITMAP : outWidth / SIZE_OF_THE_BITMAP;

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeStream(_context.ContentResolver.OpenInputStream(data), null, options);

            if (outHeight > outWidth)    // portrait
            {
                var centerMiddleHeight = (resizedBitmap.Height - resizedBitmap.Width) / 2;
                resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, centerMiddleHeight, resizedBitmap.Width, resizedBitmap.Width, null, false);
            }
            else                        // paysage
            {
                var centerMiddleWidth = (resizedBitmap.Width - resizedBitmap.Height) / 2;
                resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, centerMiddleWidth, 0, resizedBitmap.Height, resizedBitmap.Height, null, false);
            }

            using (var stream = new System.IO.MemoryStream())
            {
                resizedBitmap.Compress(Bitmap.CompressFormat.Jpeg, 40, stream);
                var bytes = stream.ToArray();
                PictureBinary = stream.ToArray();
            }

            return resizedBitmap;
        }

        /// <summary>
        /// Libération des objets
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
            Directory.Dispose();
        }

        #endregion

        #region ===== Méthode Private =============================================================

        /// <summary>
        /// Création d'un répertoir pour les photos prises par l'utilisateur 
        /// </summary>
        private void CreateDirectoryForPictures()
        {
            Directory = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures)
                , DIRECTORY_SEEKIOS_PICTURE);
            if (!Directory.Exists())
            {
                Directory.Mkdirs();
            }
        }

        /// <summary>
        /// Vérification de la présence d'un capteur photo
        /// </summary>
        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = _context.PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            IsAppToTakePictures = (availableActivities != null && availableActivities.Count > 0);
            return IsAppToTakePictures;
        }

        #endregion
    }
}