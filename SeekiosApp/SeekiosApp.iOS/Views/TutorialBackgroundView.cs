using CoreGraphics;
using Foundation;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using SeekiosApp.iOS.Views;
using SeekiosApp.iOS.Views.TableSources;
using SeekiosApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace SeekiosApp.iOS
{
    public partial class TutorialBackgroundView : BaseViewController
    {
        #region ===== Attributs ===================================================================

        private UIPageViewController _pageViewController;
        private List<string> _pageTitles;
        private List<string> _images;

        #endregion

        #region ===== Properties ==================================================================

        public bool IsTutoFinished { get; private set; }
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                if (_currentPage == 2) IsTutoFinished = true;
                else IsTutoFinished = false;
                if (_currentPage < 0) _currentPage = 0;
            }
        }
        private int _currentPage = 0;

        #endregion

        #region ====== Constructor ================================================================

        public TutorialBackgroundView(IntPtr handle) : base(handle)
        {
            IsTutoFinished = false;
        }

        #endregion

        #region ====== Life Cycle =================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = Application.LocalizedString("GeneralHelp");
            if (!App.Locator.Login.GetSavedFirstLaunchTuto()) NavigationController.NavigationBarHidden = true;

            _pageTitles = new List<string> { Application.LocalizedString("IMEIAndPinHelp")
                , Application.LocalizedString("CreditsHelp")
                , Application.LocalizedString("FirstUseHelp")};
            _images = new List<string> { "TutoAddSeekios", "TutoCredits", "TutoMode" };

            _pageViewController = Storyboard.InstantiateViewController("PageViewController") as UIPageViewController;
            _pageViewController.DataSource = new TutorialBackgroundSource(this, _pageTitles);

            var startVC = ViewControllerAtIndex(0) as TutorialForegroundView;
            var viewControllers = new UIViewController[] { startVC };

            _pageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);
            _pageViewController.View.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Size.Height - 42);
            AddChildViewController(_pageViewController);
            View.AddSubview(_pageViewController.View);
            _pageViewController.DidMoveToParentViewController(this);
            _pageViewController.WillTransition += PageViewController_WillTransition;

            ButtonSkip.TouchUpInside += ButtonSkip_TouchUpInside;
            ButtonSkip.SetTitle(Application.LocalizedString("SkipButton"), UIControlState.Normal);
        }

        private void PageViewController_WillTransition(object sender, UIPageViewControllerTransitionEventArgs e)
        {
            CurrentPage = ((TutorialForegroundView)e.PendingViewControllers[0]).PageIndex;
        }

        public UIViewController ViewControllerAtIndex(int index)
        {
            var vc = Storyboard.InstantiateViewController("TutorialForegroundView") as TutorialForegroundView;
            vc.Details = _pageTitles.ElementAt(index);
            vc.ImageFile = _images.ElementAt(index);
            vc.PageIndex = index;
            return vc;
        }

        public void UpdateSkipButton()
        {
            if (IsTutoFinished)
            {
                ButtonSkip.SetTitle(Application.LocalizedString("DoneButton"), UIControlState.Normal);
            }
            else ButtonSkip.SetTitle(Application.LocalizedString("SkipButton"), UIControlState.Normal);
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public override void SetDataAndStyleToView()
        {
            // round corner on the Skip button
            ButtonSkip.Layer.CornerRadius = 4;
            ButtonSkip.Layer.MasksToBounds = true;
            ButtonSkip.Layer.BorderWidth = 1;
            ButtonSkip.Layer.BorderColor = UIColor.FromRGBA(98, 218, 115, 255).CGColor;
            ButtonSkip.ContentEdgeInsets = new UIEdgeInsets(5, 10, 5, 10);

            UIPageControl.Appearance.CurrentPageIndicatorTintColor = UIColor.FromRGB(98, 218, 115);
            UIPageControl.Appearance.PageIndicatorTintColor = UIColor.FromRGB(184, 184, 184);
        }

        #endregion

        #region ===== Event =======================================================================

        void ButtonSkip_TouchUpInside(object sender, EventArgs e)
        {
            if (IsTutoFinished)
            {
                if (!App.Locator.Login.GetSavedFirstLaunchTuto())
                {
                    App.Locator.Login.SaveFirstLaunchTuto();
                    App.Locator.ListSeekios.GoToSeekios();
                }
                else GoBack(true);
            }
            else
            {
                if (CurrentPage < 3)
                {
                    CurrentPage++;
                    var startVC = ViewControllerAtIndex(CurrentPage) as TutorialForegroundView;
                    var viewControllers = new UIViewController[] { startVC };
                    _pageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);
                    UpdateSkipButton();
                }
            }
        }

        #endregion
    }
}