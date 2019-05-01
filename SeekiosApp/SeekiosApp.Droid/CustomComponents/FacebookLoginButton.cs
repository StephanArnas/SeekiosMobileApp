//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Xamarin.Facebook.Login.Widget;
//using Android.Util;
//using Xamarin.Facebook;
//using SeekiosApp.Droid.Helper;

//namespace SeekiosApp.Droid.CustomComponents
//{
//    public class FacebookLoginButton : LoginButton
//    {
//        #region ===== Attributs ===================================================================

//        // It allows access to application-specific resources and classes
//        private Context _context = null;

//        #endregion

//        #region ===== Constructeur(s) =============================================================

//        public FacebookLoginButton(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { InitLayout(null, null); }
//        public FacebookLoginButton(Context context) : this(context, null) { }
//        public FacebookLoginButton(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }
//        public FacebookLoginButton(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle ) { InitLayout(attrs, context); }

//        #endregion

//        #region ===== Méthodes Privées ============================================================

//        private void InitLayout(IAttributeSet attrs, Context context)
//        {
//            if (attrs == null) throw new Exception("You need to set attribut with this component <SectionBarLayout>");
//            if (context == null) throw new Exception("You need to set context for <SectionBarLayout>");
//            _context = context;

//            // Attrs : allow to get custom view

//            // Texte du button
//            // Personnalisation du button avec un texte depuis le fichier Attrs.xml
//            var textAttributArray = _context.ObtainStyledAttributes(attrs, Resource.Styleable.FacebookLoginButton);
//            var textValue = textAttributArray.GetString(Resource.Styleable.FacebookLoginButton_buttonText);

//            // Taille du text du button
//            // Personnalisation du button avec la taille du texte depuis le fichier Attrs.xml
//            var textSizeAttributArray = _context.ObtainStyledAttributes(attrs, Resource.Styleable.FacebookLoginButton);
//            var textSizeValue = textSizeAttributArray.GetString(Resource.Styleable.FacebookLoginButton_textSize);

//            // Image du bouton
//            var imageAttributArray = _context.ObtainStyledAttributes(attrs, Resource.Styleable.FacebookLoginButton);
//            var imageValue = imageAttributArray.GetResourceId(Resource.Styleable.FacebookLoginButton_imageFacebookButton, 0);

//            // initialisation du layout
//            Background = Resources.GetDrawable(Resource.Drawable.ButtonTemplate);
//            Clickable = true;

//            // initialisation du titre 
//            var textFacebookTextView = new TextView(Context);
//            textFacebookTextView.Text = textValue;
//            textFacebookTextView.SetTextColor(Resources.GetColorStateList(Resource.Drawable.TextViewButtonColorSelector));
//            textFacebookTextView.SetTextSize(ComplexUnitType.Dip, 16);
//            var titleTextParam = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
//            titleTextParam.AddRule(LayoutRules.CenterInParent);
//            textFacebookTextView.LayoutParameters = titleTextParam;
//            //AddView(textFacebookTextView);

//            // image SVG
//            if (imageValue != 0)
//            {
//                var facebookButtonSvgImage = new XamSvg.SvgImageView(_context, attrs);
//                facebookButtonSvgImage.SetSvg(_context, imageValue, string.Empty, "36da3e=008506");
//                var imageButtonParam = new RelativeLayout.LayoutParams(AccessResources.Instance.SizeOf30Dip(), AccessResources.Instance.SizeOf30Dip());
//                imageButtonParam.AddRule(LayoutRules.CenterVertical);
//                imageButtonParam.LeftMargin = AccessResources.Instance.SizeOf10Dip();
//                facebookButtonSvgImage.LayoutParameters = imageButtonParam;
//                //AddView(facebookButtonSvgImage);
//            }
//        }

//        #endregion
//    }
//}