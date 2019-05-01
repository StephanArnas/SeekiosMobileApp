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
using Android.Views.InputMethods;

namespace SeekiosApp.Droid.CustomComponents
{
    public class ImageEditTextLayout : RelativeLayout
    {
        #region ===== Attributs ===================================================================

        private Context _context = null;

        #endregion

        #region ===== Propriétés =================================================================

        public EditText EditText = null;

        #endregion

        #region ===== Constructeur(s) =============================================================

        public ImageEditTextLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { InitLayout(null, null); }
        public ImageEditTextLayout(Context context) : this(context, null) { }
        public ImageEditTextLayout(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }
        public ImageEditTextLayout(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { InitLayout(attrs, context); }

        #endregion

        #region ===== Méthodes Privées ============================================================

        private void InitLayout(IAttributeSet attrs, Context context)
        {
            if (attrs == null) throw new Exception("You need to set attribut with this component <SectionBarLayout>");
            if (context == null) throw new Exception("You need to set context for <SectionBarLayout>");
            _context = context;

            // placeholder de l'EditText
            var titleAttributArray = _context.ObtainStyledAttributes(attrs, Resource.Styleable.ImageEditTextLayout);
            var titleValue = titleAttributArray.GetString(Resource.Styleable.ImageEditTextLayout_textPlaceholder);

            // image de l'EditText
            var imageAttributArray = _context.ObtainStyledAttributes(attrs, Resource.Styleable.ImageEditTextLayout);
            var imageValue = imageAttributArray.GetResourceId(Resource.Styleable.ImageEditTextLayout_imageEditText, 0);

            // size du text de l'edit text
            var sizeTextAttributeArray = _context.ObtainStyledAttributes(attrs, Resource.Styleable.ImageEditTextLayout);
            var sizeTextValue = sizeTextAttributeArray.GetResourceId(Resource.Styleable.ImageEditTextLayout_imageEditTextSize, 0);

            // layout de l'image
            var imageLayout = new RelativeLayout(_context);
            imageLayout.LayoutParameters = new RelativeLayout.LayoutParams(AccessResources.Instance.SizeOf60Dip(), AccessResources.Instance.SizeOf60Dip());
            imageLayout.SetBackgroundColor(AccessResources.Instance.ColorLayoutBackgroundSecondary());
            imageLayout.Clickable = true;

            // initialisation de l'EditText
            EditText = new EditText(Context);
            EditText.SetTextColor(AccessResources.Instance.ColorTextColorContent());
            EditText.SetHintTextColor(AccessResources.Instance.ColorTextColorHint());
            EditText.SetTextSize(ComplexUnitType.Dip, 16);
            EditText.Hint = titleValue;
            EditText.SetPadding(0, 2, 0, 0);
            EditText.InputType = Android.Text.InputTypes.TextVariationUri;
            EditText.SetBackgroundResource(Resource.Drawable.EditTextParameterTemplate);
            EditText.Focusable = true;
            var titleTextParam = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            titleTextParam.AddRule(LayoutRules.CenterVertical);
            titleTextParam.SetMargins(AccessResources.Instance.SizeOf70Dip(), 0, 0, 0);
            EditText.LayoutParameters = titleTextParam;

            // image SVG
            var imageButtonSvgImage = new XamSvg.SvgImageView(_context, attrs);
            imageButtonSvgImage.SetSvg(_context, imageValue, string.Empty, string.Empty);
            var imageButtonParam = new LayoutParams(AccessResources.Instance.SizeOf30Dip(), AccessResources.Instance.SizeOf30Dip());
            imageButtonParam.AddRule(LayoutRules.CenterInParent);
            imageButtonSvgImage.LayoutParameters = imageButtonParam;
            imageLayout.Click += ((o, e) =>
            {
                EditText.SetSelection(EditText.Text.Length);
                EditText.RequestFocus();
                InputMethodManager imm = (InputMethodManager)_context.GetSystemService(Context.InputMethodService);
                imm.ShowSoftInput(EditText, InputMethodManager.ShowImplicit);
            });

            // ajout du titre dans la vue
            AddView(EditText);
            imageLayout.AddView(imageButtonSvgImage);
            AddView(imageLayout);
        }

        #endregion
    }
}