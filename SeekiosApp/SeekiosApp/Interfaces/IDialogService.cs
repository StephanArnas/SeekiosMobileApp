﻿using SeekiosApp.Enum;
using System;
using System.Threading.Tasks;

namespace SeekiosApp.Interfaces
{
    public interface IDialogService
    {
        Task ShowError(string message
            , string title
            , string buttonText
            , Action afterHideCallback);
        Task ShowError(Exception error
            , string title
            , string buttonText
            , Action afterHideCallback);
        Task ShowMessage(string message
            , string title);
        Task ShowMessage(string message
            , string title
            , string buttonText
            , Action afterHideCallback);
        Task<bool> ShowMessage(string message
            , string title
            , string buttonConfirmText
            , string buttonCancelText
            , Action<bool> afterHideCallback);
        Task ShowMessageBox(string message
            , string title);
        Task<bool> ShowChangeModePopup(string titlePopup
            , string powerSagingText
            , int oldMode
            , int newMode
            , bool isInPowerSaving);
        void ShowLoadingLayout();
        void HideLoadingLayout();

        Task ShowPopupCredit(string title, string message);
    }
}
