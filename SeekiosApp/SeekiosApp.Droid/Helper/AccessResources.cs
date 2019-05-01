using Android.Content;
using Android.Util;

namespace SeekiosApp.Droid.Helper
{
    public class AccessResources
    {
        #region ===== Attributs ===================================================================

        private int _height200dip = 0;
        private int _height70dip = 0;
        private int _height60dip = 0;
        private int _height50dip = 0;
        private int _height30dip = 0;
        private int _height20dip = 0;
        private int _height15dip = 0;
        private int _height10dip = 0;
        private int _height5dip = 0;

        private Android.Graphics.Color _colorLayoutBackgroundSectionBar;
        private Android.Graphics.Color _colorLayoutBackgroundSecondary;
        private Android.Graphics.Color _colorTextColorContent;
        private Android.Graphics.Color _colorTextColorTitle;
        private Android.Graphics.Color _colorTextColorHint;
        
        private static Context _context = null;
        private static AccessResources _accessResourceInstance = null;

        #endregion

        #region ===== Propriétés ==================================================================

        public static AccessResources Instance
        {
            get
            {
                return _accessResourceInstance;
            }
        }

        #endregion

        #region ===== Constructeur ================================================================

        public AccessResources()
        {
            _height200dip = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 200, _context.Resources.DisplayMetrics);
            _height70dip = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 70, _context.Resources.DisplayMetrics);
            _height60dip = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 60, _context.Resources.DisplayMetrics);
            _height50dip = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 50, _context.Resources.DisplayMetrics);
            _height30dip = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 30, _context.Resources.DisplayMetrics);
            SizeOf25Dip = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 25, _context.Resources.DisplayMetrics);
            _height20dip = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 20, _context.Resources.DisplayMetrics);
            _height15dip = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 15, _context.Resources.DisplayMetrics);
            _height10dip = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 10, _context.Resources.DisplayMetrics);
            _height5dip = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 5, _context.Resources.DisplayMetrics);
            SizeOf80Dip = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 80, _context.Resources.DisplayMetrics);
            SizeOf90Dip = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 90, _context.Resources.DisplayMetrics);
            SizeOf100Dip = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 100, _context.Resources.DisplayMetrics);
            SizeOf110Dip = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 110, _context.Resources.DisplayMetrics);
            SizeOf120Dip = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 120, _context.Resources.DisplayMetrics);
            
            TypedValue typedValue = new TypedValue();

            _context.Theme.ResolveAttribute(Resource.Attribute.layoutBackgroundSectionBar, typedValue, false);
            _colorLayoutBackgroundSectionBar = _context.Resources.GetColor(typedValue.Data);

            _context.Theme.ResolveAttribute(Resource.Attribute.layoutBackgroundSecondly, typedValue, false);
            _colorLayoutBackgroundSecondary = _context.Resources.GetColor(typedValue.Data);

            _context.Theme.ResolveAttribute(Resource.Attribute.textColorContent, typedValue, false);
            _colorTextColorContent = _context.Resources.GetColor(typedValue.Data);

            _context.Theme.ResolveAttribute(Resource.Attribute.textColorTitle, typedValue, false);
            _colorTextColorTitle = _context.Resources.GetColor(typedValue.Data);

            _context.Theme.ResolveAttribute(Resource.Attribute.textColorHint, typedValue, false);
            _colorTextColorHint = _context.Resources.GetColor(typedValue.Data);
        }

        #endregion

        #region ===== Initialisation ==============================================================

        public static void CreateInstance(Context context)
        {
            _context = context;
            _accessResourceInstance = new AccessResources();
        }

        #endregion

        #region ===== Getter Size =================================================================

        public int SizeOf25Dip { get; private set; }
        public int SizeOf80Dip { get; private set; }
        public int SizeOf90Dip { get; private set; }
        public int SizeOf100Dip { get; private set; }
        public int SizeOf110Dip { get; private set; }
        public int SizeOf120Dip { get; private set; }

        public int SizeOf200Dip()
        {
            return _height200dip;
        }

        public int SizeOf70Dip()
        {
            return _height70dip;
        }

        public int SizeOf60Dip()
        {
            return _height60dip;
        }

        public int SizeOf50Dip()
        {
            return _height50dip;
        }

        public int SizeOf30Dip()
        {
            return _height30dip;
        }

        public int SizeOf20Dip()
        {
            return _height20dip;
        }

        public int SizeOf15Dip()
        {
            return _height15dip;
        }

        public int SizeOf10Dip()
        {
            return _height10dip;
        }

        public int SizeOf5Dip()
        {
            return _height5dip;
        }

        #endregion

        #region ===== Getter Color ================================================================

        public Android.Graphics.Color ColorLayoutBackgroundSectionBar()
        {
            return _colorLayoutBackgroundSectionBar;
        }

        public Android.Graphics.Color ColorLayoutBackgroundSecondary()
        {
            return _colorLayoutBackgroundSecondary;
        }

        public Android.Graphics.Color ColorTextColorContent()
        {
            return _colorTextColorContent;
        }

        public Android.Graphics.Color ColorTextColorTitle()
        {
            return _colorTextColorTitle;
        }

        public Android.Graphics.Color ColorTextColorHint()
        {
            return _colorTextColorHint;
        }

        #endregion
    }
}