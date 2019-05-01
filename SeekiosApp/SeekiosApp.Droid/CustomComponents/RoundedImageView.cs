using Android.Content;
using Android.Widget;
using Android.Util;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace SeekiosApp.Droid.CustomComponents
{
    public class RoundedImageView : ImageView
    {
        public RoundedImageView(Context context)
            : base(context)
        {
            // TODO Auto-generated constructor stub
        }

        public RoundedImageView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public RoundedImageView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
        }

        protected override void OnDraw(Canvas canvas)
        {
            Drawable drawable = base.Drawable;

            if (drawable == null)
            {
                return;
            }

            if (base.Width == 0 || base.Height == 0)
            {
                return;
            }
            Bitmap b = ((BitmapDrawable)drawable).Bitmap;
            Bitmap bitmap = b.Copy(Bitmap.Config.Argb8888, true);
            b.Dispose();

            int w = base.Width, h = base.Height;

            Bitmap roundBitmap = getCroppedBitmap(bitmap, w);
            bitmap.Dispose();
            canvas.DrawBitmap(roundBitmap, 0, 0, null);
            roundBitmap.Dispose();
        }

        public static Bitmap getCroppedBitmap(Bitmap bmp, int radius)
        {
            Bitmap sbmp;
            if (bmp.Width != radius || bmp.Height != radius)
                sbmp = Bitmap.CreateScaledBitmap(bmp, radius, radius, false);
            else
                sbmp = bmp;
            Bitmap output = Bitmap.CreateBitmap(sbmp.Width,
                    sbmp.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(output);

            int color = Color.Black;//0xffa19774
            Paint paint = new Paint();
            Rect rect = new Rect(0, 0, sbmp.Width, sbmp.Height);

            paint.AntiAlias = true;
            paint.FilterBitmap = true;
            paint.Dither = true;
            canvas.DrawARGB(0, 0, 0, 0);
            paint.Color = Color.ParseColor("#BAB399");
            canvas.DrawCircle(sbmp.Width / 2 + 0.7f, sbmp.Height / 2 + 0.7f,
                    sbmp.Width / 2 + 0.1f, paint);
            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
            // radius=10, y-offset=2, color=black 
            paint.SetShadowLayer(10.0f, 4.0f, 4.0f, Color.Black);
            canvas.DrawBitmap(sbmp, rect, rect, paint);

            sbmp.Dispose();
            canvas.Dispose();

            return output;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}