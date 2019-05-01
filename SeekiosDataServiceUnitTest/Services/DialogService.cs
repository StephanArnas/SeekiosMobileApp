using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosDataServiceUnitTest.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
        {
            return Task.FromResult<object>(null);
        }

        public Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
        {
            return Task.FromResult<object>(null);
        }

        public Task ShowMessage(string message, string title)
        {            
            return Task.FromResult<object>(null);
        }

        public Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
        {
            return Task.FromResult<object>(null);
        }

        public Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback)
        {
            return Task.FromResult(true);
        }

        public Task ShowMessageBox(string message, string title)
        {
            return Task.FromResult<object>(null);
        }
    }
}
