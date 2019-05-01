using SeekiosApp.iOS.ModeZone;
using SeekiosApp.ViewModel;
using System;
using UIKit;

namespace SeekiosApp.iOS.Views.CustomComponents.CustomPicker
{
    public class RefreshPositionPickerView : UIPickerView
    {
        public RefreshPositionPickerViewModel PickerViewModel { get; set; }

        public UIToolbar Toolbar { get; set; }

        public UITextField TextField { get; set; }

        public RefreshPositionPickerView()
        {
            PickerViewModel = new RefreshPositionPickerViewModel();
            Model = PickerViewModel;
            ShowSelectionIndicator = true;
            BackgroundColor = UIColor.White;
        }

        public void InitEventPickerView(UITextField textField)
        {
            TextField = textField;
            TextField.TouchDown += SetPickerValue_Changed;
        }

        public void InitDefaultValue(int defaultValue = 8)
        {
            TextField.Text = PickerViewModel.ValuesPicker[defaultValue];
            Select(PickerViewModel.ValuesPicker.IndexOf(TextField.Text), 0, true);
            MapViewModelBase.RefreshTime = SeekiosApp.Helper.SpinnerHelper.GetValueSpinner(defaultValue);
        }

        public UIToolbar GetToolbar(Action callback)
        {
            Toolbar = new UIToolbar();
            Toolbar.BarStyle = UIBarStyle.Default;
            Toolbar.Translucent = true;
            Toolbar.SizeToFit();
            //Toolbar.BackgroundColor = UIColor.FromRGB(98, 218, 115);

            var doneButton = new UIBarButtonItem(Application.LocalizedString("Validate"), UIBarButtonItemStyle.Done, (s, e) =>
            {
                callback.Invoke();
            });
            Toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);

            return Toolbar;
        }

        public UIToolbar GetToolbar(UIView views)
        {
            if (TextField == null) throw new Exception("You must initialize TextField in the contructor.");
            if (views == null) throw new Exception("The parameter view can not be null.");

            Toolbar = new UIToolbar();
            Toolbar.BarStyle = UIBarStyle.Default;
            Toolbar.Translucent = true;
            Toolbar.SizeToFit();

            var doneButton = new UIBarButtonItem(Application.LocalizedString("Validate"), UIBarButtonItemStyle.Done, (s, e) =>
            {
                foreach (var view in views.Subviews)
                {
                    if (view.IsFirstResponder)
                    {
                        TextField.Text = PickerViewModel.ValuesPicker[(int)SelectedRowInComponent(0)].ToString();
                        TextField.ResignFirstResponder();
                    }
                }
            });
            Toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);

            return Toolbar;
        }

        public UIToolbar GetToolbar(UIView views, UIButton label1, UISwitch sw)
        {
            if (TextField == null) throw new Exception("You must initialize TextField in the contructor.");
            if (views == null) throw new Exception("The parameter view can not be null.");

            Toolbar = new UIToolbar();
            Toolbar.BarStyle = UIBarStyle.Default;
            Toolbar.Translucent = true;
            Toolbar.SizeToFit();

            var doneButton = new UIBarButtonItem(Application.LocalizedString("Validate"), UIBarButtonItemStyle.Done, (s, e) =>
            {
                foreach (var view in views.Subviews)
                {
                    if (view.IsFirstResponder)
                    {
                        if (App.Locator.DetailSeekios.SeekiosSelected.VersionEmbedded_idversionEmbedded >= (int)Enum.VersionEmbeddedEnum.V1007)
                        {
                            TextField.Text = PickerViewModel.ValuesPicker[(int)SelectedRowInComponent(0)].ToString();
                            if (SelectedRowInComponent(0) < 7)
                            {
                                label1.SetTitle(Application.LocalizedString("DMTitlePowerSavingWithSpace") + " (>=30 min)", UIControlState.Normal);
                                sw.On = false;
                                sw.Enabled = false;
                            }
                            else
                            {
                                label1.SetTitle(Application.LocalizedString("DMTitlePowerSavingWithSpace"), UIControlState.Normal);
                                sw.Enabled = true;
                            }
                            TextField.ResignFirstResponder();
                        }
                    }
                    else
                    {
                        TextField.Text = PickerViewModel.ValuesPicker[(int)SelectedRowInComponent(0)].ToString();
                        TextField.ResignFirstResponder();
                    }
                }
            });
            Toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);

            return Toolbar;
        }

        public void SetPickerValue_Changed(object sender, EventArgs e)
        {
            if (TextField == null) throw new Exception("You must initialize TextField in the contructor.");
            Select(PickerViewModel.ValuesPicker.IndexOf(TextField.Text), 0, true);
        }
    }
}