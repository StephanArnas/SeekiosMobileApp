using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Plugin.DeviceInfo;

namespace SeekiosApp.UWP
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 240, 240, 240);
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(100, 240, 240, 240);
            Email.Text = "seekios.dev.tot@gmail.com";
            Password.Password = "12345678";
            SeekiosApp.App.Locator.Login.PropertyChanged += OnLoginPropertyChanged;
        }

        /// <summary>
        /// Raised when boolean changed in the LoginViewModel
        /// </summary>
        private void OnLoginPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsLoading")
            {
                var value = !SeekiosApp.App.Locator.Login.IsLoading;
                var alpha = SeekiosApp.App.Locator.Login.IsLoading ? 50 : 200;
                ConnectionProgressRing.Visibility = SeekiosApp.App.Locator.Login.IsLoading ? Visibility.Visible : Visibility.Collapsed;
                //ConnectFacebook.Enabled = valueVisibility
                //ConnectFacebook.Clickable = value;
                //ConnectFacebook.SetBackgroundColor(Color.Argb(alpha, 65, 93, 174));
                //GoToForgetPassword.Enabled = value;
                //GoToForgetPassword.Visibility = !App.Locator.Login.IsLoading ? ViewStates.Visible : ViewStates.Invisible;
                //GoToCreateAccount.Enabled = value;
                //GoToCreateAccount.Visibility = !App.Locator.Login.IsLoading ? ViewStates.Visible : ViewStates.Invisible;
                //GoToWhatIsSeekiosLayout.Enabled = value;
                //GoToWhatIsSeekiosLayout.Visibility = !App.Locator.Login.IsLoading ? ViewStates.Visible : ViewStates.Invisible; ;
                Email.IsEnabled = value;
                //Email.SetBackgroundColor(Color.Argb(alpha, 255, 255, 255));
                Password.IsEnabled = value;
                //Password.SetBackgroundColor(Color.Argb(alpha, 255, 255, 255));
                //GoToCreateAccountLayout.Clickable = value;
                //GoToForgetPasswordLayout.Clickable = value;
                //ConnectButton.SetBackgroundColor(Color.Argb(alpha, 98, 218, 115));
                ConnectButton.IsEnabled = value;
                CreateAccountButton.IsEnabled = value;
                ForgetPasswordButton.IsEnabled = value;
            }
        }

        private async void ConnectButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SeekiosApp.App.Locator.Login.Email = Email.Text;
            SeekiosApp.App.Locator.Login.Password = Password.Password;

            //if (!Email.Text.IsEmail())
            //{
            //    Email.Error = Resources.GetString(Resource.String.createAccount_errorEmail);
            //    return;
            //}

            ConnectionProgressRing.IsActive = true;
            await SeekiosApp.App.Locator.Login.Connect(CrossDeviceInfo.Current.Model
                , CrossDeviceInfo.Current.Platform.ToString()
                , CrossDeviceInfo.Current.Version
                , CrossDeviceInfo.Current.Id
                , Windows.Globalization.Language.CurrentInputMethodLanguageTag);
            ConnectionProgressRing.IsActive = false;
        }
    }
}
