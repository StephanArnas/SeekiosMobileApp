using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using MapKit;
using SeekiosApp.iOS.ControlManager;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.SWRevealViewController;
using ObjCRuntime;

namespace SeekiosApp.iOS.Views.BaseView
{
    public abstract class BaseViewControllerMap : BaseViewController, IMKMapViewDelegate
    {
        #region ===== Attributs ===================================================================

        protected MapControlManager _mapControlManager = null;

        #endregion

        #region ===== Constructor =================================================================

        public BaseViewControllerMap(IntPtr handle) : base(handle) { }

		#endregion

		#region ===== Life Cycle ==================================================================

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
		}

		public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
			NavigationController.InteractivePopGestureRecognizer.Enabled = false;
			SetMapControlManager();
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
            if (_mapControlManager != null) _mapControlManager.Dispose();
            _mapControlManager = null;
        }

        #endregion

        #region ===== Interface MapViewDelegate ===================================================

        public IntPtr Handle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {

        }

        #endregion

        #region ===== Abstract Methodes ===========================================================

        public abstract void SetMapControlManager();

        #endregion
    }
}
