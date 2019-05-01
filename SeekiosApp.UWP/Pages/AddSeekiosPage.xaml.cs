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

namespace SeekiosApp.UWP.Pages
{
    public sealed partial class AddSeekiosPage : Page
    {
        #region ===== Constructor =================================================================

        public AddSeekiosPage()
        {
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 98, 218, 115);
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(100, 98, 218, 115);
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Title = "seekios > Ajouter un seekios";
            InitializeComponent();
        }

        #endregion

        #region ====== Life Cycle =================================================================

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            SetDataAndStyleToView();
        }

        #endregion

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested -= App_BackRequested;
        }

        #region ===== Initialisze View ============================================================

        public void SetDataAndStyleToView()
        {

        }

        #endregion

        #region ===== Event =======================================================================

        private void App_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null) return;

            // Navigate back if possible, and if the event has not already been handled
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        #endregion
    }
}
