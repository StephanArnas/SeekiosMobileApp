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

namespace SeekiosApp.Droid.CustomComponents.Adapter
{
    public class ListTutorialAdapter : BaseAdapter<int>
    {
        #region ===== Attributs ===================================================================

        private Activity _context;
        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();
        private int[] _titleList = new int[]
        {
            Resource.String.listTutorial_item1,
            Resource.String.listTutorial_item3,
            Resource.String.listTutorial_item4,
            Resource.String.listTutorial_item5
        };
        private int[] _imageList = new int[]
        {
            Resource.Drawable.Help2,
            Resource.Drawable.PowerSaving,
            Resource.Drawable.Credit,
            Resource.Drawable.Help
        };

        #endregion

        #region ===== Constructeur(s) =============================================================

        public ListTutorialAdapter(Activity context) : base()
        {
            _context = context;
        }

        #endregion

        #region ===== ListAdapter =================================================================

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int this[int position]
        {
            get { return _titleList[position]; }
        }

        public override int Count
        {
            get { return _titleList.Length; }
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
        {
            var view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
            {
                view = _context.LayoutInflater.Inflate(Resource.Layout.ListTutorialRow, null);
            }
            view.FindViewById<TextView>(Resource.Id.listTutorial_text).SetText(_titleList[position]);
            view.FindViewById<XamSvg.SvgImageView>(Resource.Id.listTutorial_image).SetSvg(_context, _imageList[position]);
            return view;
        }


        #endregion
    }
}