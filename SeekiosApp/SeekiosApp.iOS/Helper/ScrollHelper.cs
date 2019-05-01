using Foundation;
using System;
using System.Drawing;
using UIKit;

namespace SeekiosApp.iOS.Helper
{
    class ScrollHelper
    {
        public static bool ScrollOnHiddenUIElement(UIView view
            , UIScrollView scrollView
            , float bottom
            , NSNotification notification
            , UIInterfaceOrientation interfaceOrientation)
        {
            var landscape = interfaceOrientation == UIInterfaceOrientation.LandscapeLeft || interfaceOrientation == UIInterfaceOrientation.LandscapeRight;
            var viewHeight = (landscape ? scrollView.Frame.Width : view.Frame.Height);
            var nsKeyboardBounds = (NSValue)notification.UserInfo.ObjectForKey(UIKeyboard.BoundsUserInfoKey);
            var spaceFromNavbarToTopKeyboard = viewHeight - nsKeyboardBounds.RectangleFValue.Height - 44;
            var difference = spaceFromNavbarToTopKeyboard - bottom;

            if (difference < 0)
            {
                // perform the scrolling
                scrollView.ContentOffset = new PointF(0, (float)(difference * -1) + 30); // 30 is extra offset
                return true;
            }
            else
            {
                return false;
            }
        }

        public static nfloat ScrollOnHiddenUiElement(UIView view
            , float bottom
            , NSNotification notification
            , UIInterfaceOrientation interfaceOrientation)
        {
            var landscape = interfaceOrientation == UIInterfaceOrientation.LandscapeLeft || interfaceOrientation == UIInterfaceOrientation.LandscapeRight;
            var viewHeight = (landscape ? view.Frame.Width : view.Frame.Height);
            var spaceFromNavbarToTopKeyboard = viewHeight - UIKeyboard.FrameBeginFromNotification(notification).Height;
            return spaceFromNavbarToTopKeyboard - bottom - 30; // 30 is extra offset
        }
    }
}
