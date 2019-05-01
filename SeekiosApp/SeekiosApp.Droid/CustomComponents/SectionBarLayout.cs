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

namespace SeekiosApp.Droid.CustomComponents
{
    public class SectionBarLayout : RelativeLayout
    {
        #region ===== Attributs ===================================================================

        private Context _context = null;

        #endregion

        #region ===== Constructeur(s) =============================================================

        public SectionBarLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { InitLayout(null, null); }
        public SectionBarLayout(Context context) : this(context, null) { }
        public SectionBarLayout(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }
        public SectionBarLayout(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { InitLayout(attrs, context); }

        #endregion

        #region ===== Méthodes Privées ============================================================

        private void InitLayout(IAttributeSet attrs, Context context)
        {
            if (attrs == null) throw new Exception("You need to set attribut with this component <SectionBarLayout>");
            if (context == null) throw new Exception("You need to set context for <SectionBarLayout>");
            _context = context;

            // récupération des attributs
            var attributArray = _context.ObtainStyledAttributes(attrs, Resource.Styleable.SectionBarLayout);
            var titleValue = attributArray.GetString(Resource.Styleable.SectionBarLayout_sectionBarTitle);

            // initialisation du layout
            SetBackgroundColor(AccessResources.Instance.ColorLayoutBackgroundSectionBar());

            // initialisation du titre 
            var titleTextView = new TextView(Context);
            titleTextView.Text = titleValue;
            titleTextView.SetTextColor(AccessResources.Instance.ColorTextColorTitle());
            titleTextView.SetTextSize(ComplexUnitType.Dip, 16);
            var titleLayoutParam = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            titleLayoutParam.LeftMargin = AccessResources.Instance.SizeOf15Dip();
            titleLayoutParam.AddRule(LayoutRules.CenterVertical);
            titleTextView.LayoutParameters = titleLayoutParam;

            // ajout du titre dans la vue
            AddView(titleTextView);
        }

        #endregion
    }
}