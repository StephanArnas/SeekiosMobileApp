using Foundation;
using System;
using UIKit;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;

namespace SeekiosApp.iOS
{
	public partial class RechargeCreditsView : UITabBarController
    {
        public RechargeCreditsView (IntPtr handle) : base (handle)
        {
			
        }

		public override void ViewWillDisappear(bool animated)
		{
            Title = "Crédits";
			base.ViewWillDisappear(animated);
   //         var navigation = (SimpleIoc.Default.GetInstance<INavigationService>() as Services.NavigationService);
			//navigation.RemoveLastEntryNavigationStack();
		}
    }
}