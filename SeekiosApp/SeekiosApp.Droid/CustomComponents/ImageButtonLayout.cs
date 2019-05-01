using System;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using SeekiosApp.Droid.Helper;

namespace SeekiosApp.Droid.CustomComponents
{
    public class ImageButtonLayout : RelativeLayout
    {
        #region ===== Attributs ===================================================================

        private Context _context = null;

        #endregion

        #region ===== Constructeur(s) =============================================================

        public ImageButtonLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { InitLayout(null, null); }
        public ImageButtonLayout(Context context) : this(context, null) { }
        public ImageButtonLayout(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }
        public ImageButtonLayout(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { InitLayout(attrs, context); }

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

            // image du bouton
            var imageAttributArray = _context.ObtainStyledAttributes(attrs, Resource.Styleable.ImageButtonLayout);
            var imageValue = imageAttributArray.GetResourceId(Resource.Styleable.ImageButtonLayout_imageButton, 0);

            // initialisation du layout
            Background = Resources.GetDrawable(Resource.Drawable.ButtonTemplate);
            Clickable = true;

            // initialisation du titre 
            var titleTextView = new TextView(Context);
            titleTextView.Text = titleValue;
            titleTextView.SetTextColor(Resources.GetColorStateList(Resource.Drawable.TextViewButtonColorSelector));
            titleTextView.SetTextSize(ComplexUnitType.Dip, 16);
            var titleTextParam = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            titleTextParam.AddRule(LayoutRules.CenterInParent);
            titleTextView.LayoutParameters = titleTextParam;
            AddView(titleTextView);

            // image SVG
            if (imageValue != 0)
            {
                var imageButtonSvgImage = new XamSvg.SvgImageView(_context, attrs);
                imageButtonSvgImage.SetSvg(_context, imageValue, string.Empty, "36da3e=008506");
                var imageButtonParam = new RelativeLayout.LayoutParams(AccessResources.Instance.SizeOf30Dip(), AccessResources.Instance.SizeOf30Dip());
                imageButtonParam.AddRule(LayoutRules.CenterVertical);
                imageButtonParam.LeftMargin = AccessResources.Instance.SizeOf10Dip();
                imageButtonSvgImage.LayoutParameters = imageButtonParam;
                AddView(imageButtonSvgImage);
            }
        }

        #endregion
    }
}