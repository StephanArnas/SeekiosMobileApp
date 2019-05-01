using System;
using Android.App;
using Android.Support.V4.App;
using SeekiosApp.Droid.View.FragmentView;
using SeekiosApp.ViewModel;

namespace SeekiosApp.Droid.CustomComponents.Adapter
{
    class MapBaseBottomLayoutAdapter : FragmentPagerAdapter
    {
        Android.Support.V4.App.Fragment _modeMetaDataFragment;
        Android.Support.V4.App.Fragment _locationHistoryFragment;

        public MapBaseBottomLayoutAdapter(Android.Support.V4.App.FragmentManager fm, Android.Support.V4.App.Fragment modeMetaDataFragment, MapViewModelBase maBase) : base(fm)
        {
            _modeMetaDataFragment = modeMetaDataFragment;
            _locationHistoryFragment = new LocationHistoryFragment(maBase);
        }

        public override int Count
        {
            get
            {
                return _modeMetaDataFragment == null ? 1 : 2;
            }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            if (_modeMetaDataFragment == null) position++;

            BottomFragment fragment = (BottomFragment)position;
            switch ((BottomFragment)position)
            {
                case BottomFragment.ModeMetaData:
                default:
                    return _modeMetaDataFragment;
                case BottomFragment.LocationHistory:
                    return _locationHistoryFragment;
                    
            }
        }
    }

    public enum BottomFragment
    {
        ModeMetaData = 0,
        LocationHistory = 1
    }
}