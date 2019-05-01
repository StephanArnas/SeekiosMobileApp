// Camera.cs: Support code for taking pictures
//
// Copyright 2010 Miguel de Icaza
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using UIKit;
using Foundation;

namespace SeekiosApp.iOS.Helper
{
    // A static class that will reuse the UIImagePickerController
    // as iPhoneOS has a crash if multiple UIImagePickerController are created
    //   http://stackoverflow.com/questions/487173
    public static class CameraHelper
    {
        private static UIImagePickerController _picker;
        private static Action<NSDictionary> _callback;

        /// <summary>
        /// Init this instance.
        /// </summary>
        static void Init()
        {
            if (_picker != null) return;
            _picker = new UIImagePickerController();
            _picker.Delegate = new CameraDelegate();
        }


        /// <summary>
        /// Camera delegate.
        /// </summary>
        class CameraDelegate : UIImagePickerControllerDelegate
        {
            public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
            {
                var cb = _callback;
                _callback = null;

                picker.DismissModalViewController(true);
                //picker.DismissModalViewControllerAnimated (true);
                cb(info);
            }

            public override void FinishedPickingImage(UIImagePickerController picker, UIImage image, NSDictionary editingInfo)
            {
                var cb = _callback;
                _callback = null;

                picker.DismissModalViewController(true);
                //picker.DismissModalViewControllerAnimated (true);
                cb(editingInfo);
            }
        }

        /// <summary>
        /// Takes the picture.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="callback">Callback.</param>
        public static void TakePicture(UIViewController parent, Action<NSDictionary> callback)
        {
            Init();
            _picker.AllowsEditing = true;
            _picker.SourceType = UIImagePickerControllerSourceType.Camera;
            _callback = callback;
            parent.PresentModalViewController(_picker, true);
        }

        /// <summary>
        /// Selects the picture.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="callback">Callback.</param>
        public static void SelectPicture(UIViewController parent, Action<NSDictionary> callback)
        {
            try
            {
                Init();
                _picker.AllowsEditing = true;
                _picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                _callback = callback;
                parent.PresentModalViewController(_picker, true);
            }
            catch (Exception ex)
            {
                new UIAlertView(ex.Message, ex.StackTrace, null, Application.LocalizedString("OK"), null).Show();
            }
        }
    }
}

