using Foundation;
using System;
using UIKit;
using SeekiosApp.iOS.Views;
using SeekiosApp.Model.DTO;
using System.Collections.Generic;
using SeekiosApp.iOS.Helper;
using System.Threading.Tasks;

namespace SeekiosApp.iOS
{
	public partial class TransactionHistoricView : BaseViewController
	{
		#region ====== Constructor ================================================================

		public TransactionHistoricView(IntPtr handle) : base(handle) { }

		#endregion

		#region ====== Life Cycle =================================================================

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
		}

		#endregion

		#region ===== Initialisze View ============================================================

		public override void SetDataAndStyleToView()
		{
			Title = Application.LocalizedString("TransactionTitle");
            NoCreditLabel.Text = Application.LocalizedString("TransactionNoCredit");
            // display the loading layout
            var alertLoading = AlertControllerHelper.ShowAlertLoading();
            PresentViewController(alertLoading, true, async () => 
            {
                await App.Locator.TransactionHistoric.GetTransactionHistoricByUser();
                Tableview.Source = new HistoriqueSource(this);
                Tableview.Hidden = false;
                RefreshTable();
                DismissViewController(false, null);
                alertLoading.DismissViewController(false, null);
            });
        }

		#endregion

		#region ====== Private Methodes ===========================================================

		public void RefreshTable()
		{
			if (App.Locator.TransactionHistoric.LsOperation.Count > 0)
			{
				Tableview.Hidden = false;
				Tableview.ReloadData();
			}
			else
			{
				Tableview.Hidden = true;
			}
		}

        #endregion
    }
}