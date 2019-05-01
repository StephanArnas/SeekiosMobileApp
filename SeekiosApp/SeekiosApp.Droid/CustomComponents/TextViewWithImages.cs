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
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Java.Util.Regex;
using Android.Graphics;
using Java.Lang;
using XamSvg;
using System.Text.RegularExpressions;

namespace SeekiosApp.Droid.CustomComponents
{
    class TextViewWithImages : TextView
    {
        public TextViewWithImages(IntPtr a, JniHandleOwnership b) : base (a, b)
        {

        }

        public TextViewWithImages(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {

        }
        public TextViewWithImages(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }
        public TextViewWithImages(Context context) : base(context)
        {

        }

        public override void SetText(ICharSequence text, BufferType type)
        {
            SpannableString s = GetTextWithImages(Context, new Java.Lang.String(text.ToArray(), 0, text.Count()));
            base.SetText(s, BufferType.Spannable);
        }

        private static SpannableFactory spannableFactory = SpannableFactory.Instance;

        private static bool AddImages(Context context, SpannableString spannable)
        {
            string pattern = "\\Q[img src=\\E([a-zA-Z0-9_]+?)\\Q/]\\E";
            //MatchCollection m = Regex.Matches(spannable, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //foreach (Match match in m)
            //    Console.WriteLine("{0} (duplicates '{1}') at position {2}", match.Value, match.Groups[1].Value, match.Index);


            Java.Util.Regex.Pattern refImg = Java.Util.Regex.Pattern.Compile(pattern);
            bool hasChanges = false;

            Matcher matcher = refImg.Matcher(spannable);
            while (matcher.Find())
            {
                bool set = true;
                foreach (ImageSpan span in spannable.GetSpans(matcher.Start(), matcher.End(), Java.Lang.Class.FromType(typeof(ImageSpan)))) {
                if (spannable.GetSpanStart(span) >= matcher.Start() && spannable.GetSpanEnd(span) <= matcher.End()) {
                    spannable.RemoveSpan(span);
                } 
                else {
                    set = false;
                    break;
                }
            }

            string resname = spannable.SubSequence(matcher.Start(1), matcher.End(1)).ToString().Trim();
            //int id = context.Resources.GetIdentifier(resname, "drawable", context.PackageName);
            if (set) {
                hasChanges = true;

                int identifier = context.Resources.GetIdentifier("modezoneedit", "drawable", context.PackageName);
                bool isSvg = true; Bitmap bitmap2 = null;
                if (isSvg)
                {
                    SvgBitmapDrawable oo = SvgFactory.GetDrawable(context.Resources, identifier);
                    //oo.Mutate().SetColorFilter(0xffff0000, Android.Graphics.PorterDuff.Mode.Multiply);
                    Bitmap bitmap = Bitmap.CreateBitmap(oo.Picture.Width, oo.Picture.Height, Bitmap.Config.Argb8888);
                    Canvas canvas = new Canvas(bitmap);
                    canvas.DrawPicture(oo.Picture);
                    bitmap2 = Bitmap.CreateScaledBitmap(bitmap, (int)(bitmap.Width * 0.3), (int)(bitmap.Height * 0.3), false);
                }
                else
                {
                    bitmap2 = BitmapFactory.DecodeResource(context.Resources, identifier);
                    bitmap2 = Bitmap.CreateScaledBitmap(bitmap2, (int)(bitmap2.Width * 0.3), (int)(bitmap2.Height * 0.3), false);
                }

                ImageSpan span = new ImageSpan(context, bitmap2);
                spannable.SetSpan(span, matcher.Start(), matcher.End(), SpanTypes.ExclusiveExclusive);

                    /*spannable.SetSpan(new ImageSpan(context, id),
                    matcher.Start(),
                    matcher.End(),
                    SpanTypes.ExclusiveExclusive
                );*/
                }
                //if (isLastPoint) canvas.DrawColor(Color.Green, PorterDuff.Mode.SrcAtop);

                //Bitmap b = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.ModeZoneEdit);


            }
        return hasChanges;
    }
    private static SpannableString GetTextWithImages(Context context, Java.Lang.String text)
    {
        SpannableString spannable = new SpannableString(text);
        //Spann spannable = spannableFactory.NewSpannable(text);
        AddImages(context, spannable);
        return spannable;
    }
    }
}