using SeekiosApp.ViewModel;
using System;
using System.Collections.Generic;
using UIKit;

namespace SeekiosApp.iOS.ModeZone
{
	public class RefreshPositionPickerViewModel : UIPickerViewModel
	{
        #region ===== Properties ==================================================================

        public IList<string> ValuesPicker { get; set; }
		public event EventHandler<PickerChangedEventArgs> PickerChanged;
		public event EventHandler MinutesSelected;

        #endregion

        #region ===== Constructor =================================================================

        public RefreshPositionPickerViewModel()
		{
            ValuesPicker = new List<string>();
            ValuesPicker.Add("1 min");
            ValuesPicker.Add("2 min");
            ValuesPicker.Add("3 min");
            ValuesPicker.Add("4 min");
            ValuesPicker.Add("5 min");
            ValuesPicker.Add("10 min");
            ValuesPicker.Add("15 min");
            ValuesPicker.Add("30 min");
            ValuesPicker.Add("1 h");
            ValuesPicker.Add("2 h");
            ValuesPicker.Add("3 h");
            ValuesPicker.Add("4 h");
            ValuesPicker.Add("5 h");
            ValuesPicker.Add("6 h");
            ValuesPicker.Add("12 h");
            ValuesPicker.Add("24 h");
        }

        #endregion

        #region ===== Private Methodes ============================================================

        private void OnMinutesSelected()
		{
            MinutesSelected?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ===== Public Overrides Methodes ===================================================

        public override nint GetComponentCount(UIPickerView picker)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return ValuesPicker.Count;
		}

		public override string GetTitle(UIPickerView picker, nint row, nint component)
		{
			return ValuesPicker[(int)row].ToString();
		}

		public override void Selected(UIPickerView picker, nint row, nint component)
		{
            PickerChanged?.Invoke(this, new PickerChangedEventArgs { SelectedValue = ValuesPicker[(int)row] });
            MapViewModelBase.RefreshTime = SeekiosApp.Helper.SpinnerHelper.GetValueSpinner((int)row);
        }

		public override nfloat GetComponentWidth(UIPickerView picker, nint component)
		{
			if (component == 0)
				return 220f;
			else
				return 30f;
		}

        #endregion

        #region ===== Custom Class ================================================================

        public class PickerChangedEventArgs : EventArgs
        {
            public object SelectedValue { get; set; }
        }

        #endregion
    }
}

