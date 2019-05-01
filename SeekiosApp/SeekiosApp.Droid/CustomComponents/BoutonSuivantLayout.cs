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
using Android.Util;
using SeekiosApp.Droid.Helper;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace SeekiosApp.Droid.CustomComponents
{
    class BoutonSuivantLayout : RelativeLayout
    {
        #region ===== Attributs ===================================================================

        private Context _context = null;

        #endregion

        #region ===== Constructeur(s) =============================================================

        public BoutonSuivantLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { InitLayout(null, null); }
        public BoutonSuivantLayout(Context context) : this(context, null) { }
        public BoutonSuivantLayout(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }
        public BoutonSuivantLayout(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { InitLayout(attrs, context); }

        #endregion

        #region ===== Méthodes Privées ============================================================

        private void InitLayout(IAttributeSet attrs, Context context)
        {
            if (attrs == null) throw new Exception("You need to set attribut with this component <SectionBarLayout>");
            if (context == null) throw new Exception("You need to set context for <SectionBarLayout>");
            _context = context;

            // titre du bouton
            var titleAttributArray = _context.ObtainStyledAttributes(attrs, Resource.Styleable.ImageButtonLayout);
            var titleValue = titleAttributArray.GetString(Resource.Styleable.ImageButtonLayout_titleButton);

            var colorAttributArray = _context.ObtainStyledAttributes(attrs, Resource.Styleable.BoutonSuivant);
            var colorValue = colorAttributArray.GetString(Resource.Styleable.BoutonSuivant_boutonSuivantColor);

            // image du bouton
            //var imageAttributArray = _context.ObtainStyledAttributes(attrs, Resource.Styleable.ImageButtonLayout);
            //var imageValue = imageAttributArray.GetResourceId(Resource.Styleable.ImageButtonLayout_imageButton, 0);

            // initialisation du layout

            GradientDrawable shape = new GradientDrawable();
            //Canvas canvas = new Canvas();
            shape.SetColor(Color.ParseColor(colorValue));
            shape.SetCornerRadius(8);

            //Drawable d = Resources.GetDrawable(Resource.Drawable.ButtonTemplate);
            //Canvas canvas = new Canvas();
            //d.Draw(canvas);
            //canvas.DrawColor(Color.Green, PorterDuff.Mode.SrcAtop);

            

            Background = shape;
            Clickable = true;
            //SetBackgroundColor(Color.ParseColor("#36da3e"));

            // initialisation du titre 
            var titleTextView = new TextView(Context);
            titleTextView.Text = titleValue;
            //titleTextView.SetTextColor(Resources.GetColorStateList(Resource.Drawable.TextViewButtonColorSelector));
            titleTextView.SetTextColor(Color.White);
            titleTextView.SetTextSize(ComplexUnitType.Dip, 14);
            var titleTextParam = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            titleTextParam.AddRule(LayoutRules.CenterInParent);
            titleTextView.LayoutParameters = titleTextParam;
            AddView(titleTextView);

            // image SVG
           /* if (imageValue != 0)
            {
                var imageButtonSvgImage = new XamSvg.SvgImageView(_context, attrs);
                imageButtonSvgImage.SetSvg(_context, imageValue, string.Empty, "36da3e=008506");
                var imageButtonParam = new RelativeLayout.LayoutParams(AccessResources.Instance.SizeOf30Dip(), AccessResources.Instance.SizeOf30Dip());
                imageButtonParam.AddRule(LayoutRules.CenterVertical);
                imageButtonParam.LeftMargin = AccessResources.Instance.SizeOf10Dip();
                imageButtonSvgImage.LayoutParameters = imageButtonParam;
                AddView(imageButtonSvgImage);
            }*/

        }

        #endregion
    }
}