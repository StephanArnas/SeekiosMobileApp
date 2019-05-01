using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using SeekiosApp.iOS.Views;

namespace SeekiosApp.iOS.Views.TableSources
{
    public class TutorialBackgroundSource : UIPageViewControllerDataSource
    {
        #region ===== Attributs ===================================================================

        private TutorialBackgroundView _parentViewController;
        private List<string> _PageTitles;

        #endregion

        #region ====== Constructor ================================================================

        public TutorialBackgroundSource(UIViewController parentViewController, List<string> pageTitles)
        {
            _parentViewController = parentViewController as TutorialBackgroundView;
            _PageTitles = pageTitles;
        }

        #endregion

        #region ===== Public Overrides Methodes ===================================================

        public override UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            var vc = referenceViewController as TutorialForegroundView;
            var index = vc.PageIndex;

            _parentViewController.UpdateSkipButton();

            if (index == 0) return null;
            else
            {
                index--;
                return _parentViewController.ViewControllerAtIndex(index);
            }
        }

        public override UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            var vc = referenceViewController as TutorialForegroundView;
            var index = vc.PageIndex;
            index++;

            _parentViewController.UpdateSkipButton();

            if (index == _PageTitles.Count) return null;
            else return _parentViewController.ViewControllerAtIndex(index);
        }

        public override nint GetPresentationCount(UIPageViewController pageViewController)
        {
            return _PageTitles.Count;
        }

        public override nint GetPresentationIndex(UIPageViewController pageViewController)
        {
            return _parentViewController.CurrentPage;
        }
        
        #endregion
    }
}
