using Android.Graphics;
using System.IO;
using Android.Graphics.Drawables;
using System;

namespace SeekiosApp.Droid.Helper
{
    public class ImageHelper
    {
        /// <summary>
        /// Converti une couleur en son code hexadécimal
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ConvertColorToHex(Color color)
        {
            String hexColor = String.Empty;
            try
            {
                hexColor = color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
            }
            catch (Exception ex)
            {
                //doing nothing
            }

            return hexColor;
        }

        /// <summary>
        /// Créer un tableau d'octet à partir d'une image
        /// </summary>
        /// <param name="imageSource">image à transformer</param>
        /// <returns></returns>
        public static byte[] GetBytesFromImage(Drawable imageSource)
        {
            Bitmap bitmap = ((BitmapDrawable)imageSource).Bitmap;
            MemoryStream stream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, (System.IO.Stream)stream);
            bitmap.Dispose();

            return stream.GetBuffer();
        }

        /// <summary>
        /// Découpe l'image en rond
        /// </summary>
        public static Bitmap GetCroppedBitmap(Bitmap bitmap)
        {
            Bitmap output = Bitmap.CreateBitmap(bitmap.Width,
                    bitmap.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(output);

            Paint paint = new Paint();
            Rect rect = new Rect(0, 0, bitmap.Width, bitmap.Width);

            paint.AntiAlias = true;

            canvas.DrawCircle(bitmap.Width / 2, bitmap.Width / 2,
                    bitmap.Width / 2, paint);
            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
            canvas.DrawBitmap(bitmap, rect, rect, paint);
            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Bitmap Base64ToBitmap(string base64)
        {
            //Conversion de l'image du Seekios de Base64 vers bytes
            var bytes = Convert.FromBase64String(base64);
            //Créaiton du bitmap
            return BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
        }
    }
}