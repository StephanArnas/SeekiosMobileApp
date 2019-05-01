using System;
using System.Collections.Generic;
using UIKit;

namespace SeekiosApp.iOS
{
    public class CountryCodePickerView : UIPickerViewModel
    {
        #region ===== Properties ==================================================================

        public List<Tuple<string, string>> ValuesPicker { get; set; }
        public event EventHandler<PickerChangedEventArgs> PickerChanged;
        public event EventHandler CountryCodeSelected;

        #endregion

        #region ===== Constructor =================================================================

        public CountryCodePickerView()
        {
            ValuesPicker = new List<Tuple<string, string>>();
            ValuesPicker.Add(new Tuple<string, string>("+31", "NL"));
            ValuesPicker.Add(new Tuple<string, string>("+32", "BE"));
            ValuesPicker.Add(new Tuple<string, string>("+33", "FR"));
            ValuesPicker.Add(new Tuple<string, string>("+34", "ES"));
            ValuesPicker.Add(new Tuple<string, string>("+41", "SE"));
            ValuesPicker.Add(new Tuple<string, string>("+44", "GB"));
            ValuesPicker.Add(new Tuple<string, string>("+45", "DK"));
            ValuesPicker.Add(new Tuple<string, string>("+46", "SE"));
            ValuesPicker.Add(new Tuple<string, string>("+59", "IT"));
            ValuesPicker.Add(new Tuple<string, string>("+213", "DE"));
            ValuesPicker.Add(new Tuple<string, string>("+351", "PT"));
            ValuesPicker.Add(new Tuple<string, string>("+352", "LU"));
        }

        #endregion

        #region ===== Private Methodes ============================================================

        private void OnCountryCodeSelected()
        {
            CountryCodeSelected?.Invoke(this, EventArgs.Empty);
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