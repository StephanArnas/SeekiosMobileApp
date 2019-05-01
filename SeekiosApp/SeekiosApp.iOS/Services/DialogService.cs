// ****************************************************************************
// <copyright file="DialogService.cs" company="GalaSoft Laurent Bugnion">
// Copyright � GalaSoft Laurent Bugnion 2009-2016
// </copyright>
// ****************************************************************************
// <author>Laurent Bugnion</author>
// <email>laurent@galasoft.ch</email>
// <date>02.10.2014</date>
// <project>GalaSoft.MvvmLight</project>
// <web>http://www.mvvmlight.net</web>
// <license>
// See license.txt in this solution or http://www.galasoft.ch/license_MIT.txt
// </license>
// ****************************************************************************

using CoreGraphics;
using SeekiosApp.Enum;
using SeekiosApp.Interfaces;
using SeekiosApp.iOS;
using SeekiosApp.iOS.Helper;
using System;
using System.Threading.Tasks;
using UIKit;

namespace Seekios.iOS.Services
{
    /// <summary>
    /// An implementation of <see cref="IDialogService"/> allowing
    /// to display simple dialogs to the user. Note that this class
    /// uses the built in Windows Phone dialogs which may or may not
    /// be sufficient for your needs. Using this class is easy
    /// but feel free to develop your own IDialogService implementation
    /// if needed.
    /// </summary>
    ////[ClassInfo(typeof(IDialogService))]
    public class DialogService : IDialogService
    {
        public static UIViewController CurrentViewController { get; set; }

        /// <summary>
        /// Displays information about an error.
        /// </summary>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="buttonText">The text shown in the only button
        /// in the dialog box. If left null, the text "OK" will be used.</param>
        /// <param name="afterHideCallback">A callback that should be executed after
        /// the dialog box is closed by the user.</param>
        /// <returns>A Task allowing this async method to be awaited.</returns>
        /// <remarks>Displaying dialogs in iOS is synchronous. As such,
        /// this method will be executed synchronously even though it can be awaited
        /// for cross-platform compatibility purposes.</remarks>
        public Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
        {
            var tcs = new TaskCompletionSource<bool>();

            var av = new UIAlertView(
                title,
                message,
                null,
                buttonText,
                null);

            av.Dismissed += (s, e) =>
            {
                if (afterHideCallback != null)
                {
                    afterHideCallback();
                }

                tcs.SetResult(true);
            };

            av.Show();
            return tcs.Task;
        }

        /// <summary>
        /// Displays information about an error.
        /// </summary>
        /// <param name="error">The exception of which the message must be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="buttonText">The text shown in the only button
        /// in the dialog box. If left null, the text "OK" will be used.</param>
        /// <param name="afterHideCallback">A callback that should be executed after
        /// the dialog box is closed by the user.</param>
        /// <returns>A Task allowing this async method to be awaited.</returns>
        /// <remarks>Displaying dialogs in iOS is synchronous. As such,
        /// this method will be executed synchronously even though it can be awaited
        /// for cross-platform compatibility purposes.</remarks>
        public Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
        {
            var tcs = new TaskCompletionSource<bool>();

            var av = new UIAlertView(
                title,
                error.Message,
                null,
                buttonText,
                null);

            av.Dismissed += (s, e) =>
            {
                if (afterHideCallback != null)
                {
                    afterHideCallback();
                }

                tcs.SetResult(true);
            };

            av.Show();
            return tcs.Task;
        }

        /// <summary>
        /// Displays information to the user. The dialog box will have only
        /// one button with the text "OK".
        /// </summary>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <returns>A Task allowing this async method to be awaited.</returns>
        /// <remarks>Displaying dialogs in Android is synchronous. As such,
        /// this method will be executed synchronously even though it can be awaited
        /// for cross-platform compatibility purposes.</remarks>
        public Task ShowMessage(string message, string title)
        {
            var tcs = new TaskCompletionSource<bool>();

            var av = new UIAlertView(
                title,
                message,
                null,
                "OK",
                null);

            av.Dismissed += (s, e) => tcs.SetResult(true);
            av.Show();
            return tcs.Task;
        }

        /// <summary>
        /// Displays information to the user. The dialog box will have only
        /// one button.
        /// </summary>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="buttonText">The text shown in the only button
        /// in the dialog box. If left null, the text "OK" will be used.</param>
        /// <param name="afterHideCallback">A callback that should be executed after
        /// the dialog box is closed by the user.</param>
        /// <returns>A Task allowing this async method to be awaited.</returns>
        /// <remarks>Displaying dialogs in Android is synchronous. As such,
        /// this method will be executed synchronously even though it can be awaited
        /// for cross-platform compatibility purposes.</remarks>
        public Task ShowMessage(
            string message,
            string title,
            string buttonText,
            Action afterHideCallback)
        {
            var tcs = new TaskCompletionSource<bool>();

            var av = new UIAlertView(
                title,
                message,
                null,
                buttonText,
                null);

            av.Dismissed += (s, e) =>
            {
                if (afterHideCallback != null)
                {
                    afterHideCallback();
                }

                tcs.SetResult(true);
            };

            av.Show();
            return tcs.Task;
        }

        /// <summary>
        /// Displays information to the user. The dialog box will have only
        /// one button.
        /// </summary>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="buttonConfirmText">The text shown in the "confirm" button
        /// in the dialog box. If left null, the text "OK" will be used.</param>
        /// <param name="buttonCancelText">The text shown in the "cancel" button
        /// in the dialog box. If left null, the text "Cancel" will be used.</param>
        /// <param name="afterHideCallback">A callback that should be executed after
        /// the dialog box is closed by the user. The callback method will get a boolean
        /// parameter indicating if the "confirm" button (true) or the "cancel" button
        /// (false) was pressed by the user.</param>
        /// <returns>A Task allowing this async method to be awaited. The task will return
        /// true or false depending on the dialog result.</returns>
        /// <remarks>Displaying dialogs in Android is synchronous. As such,
        /// this method will be executed synchronously even though it can be awaited
        /// for cross-platform compatibility purposes.</remarks>
        public Task<bool> ShowMessage(
            string message,
            string title,
            string buttonConfirmText,
            string buttonCancelText,
            Action<bool> afterHideCallback)
        {
            var tcs = new TaskCompletionSource<bool>();

            var av = new UIAlertView(
                title,
                message,
                null,
                buttonCancelText,
                buttonConfirmText);

            av.Dismissed += (s, e) =>
            {
                if (afterHideCallback != null)
                {
                    afterHideCallback(e.ButtonIndex > 0);
                }

                tcs.SetResult(e.ButtonIndex > 0);
            };

            av.Show();
            return tcs.Task;
        }

        /// <summary>
        /// Displays information to the user in a simple dialog box. The dialog box will have only
        /// one button with the text "OK". This method should be used for debugging purposes.
        /// </summary>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <returns>A Task allowing this async method to be awaited.</returns>
        /// <remarks>Displaying dialogs in Android is synchronous. As such,
        /// this method will be executed synchronously even though it can be awaited
        /// for cross-platform compatibility purposes.</remarks>
        public Task ShowMessageBox(string message, string title)
        {
            var tcs = new TaskCompletionSource<bool>();

            var av = new UIAlertView(
                title,
                message,
                null,
                "OK",
                null);

            av.Dismissed += (s, e) => tcs.SetResult(true);
            av.Show();
            return tcs.Task;
        }

        public Task<bool> ShowChangeModePopup(string titlePopup
            , string powerSagingText
            , int oldMode
            , int newMode
            , bool isInPowerSaving)
        {
            if (CurrentViewController == null) throw new Exception("CurrentViewController can not be null in DialogService");

            var tcs = new TaskCompletionSource<bool>();

            var actionSheetAlert = UIAlertController.Create(titlePopup + (!isInPowerSaving ? "\n\n\n\n\n\n\n\n" : "\n\n\n\n\n\n\n\n\n\n\n")
                , string.Empty
                , UIAlertControllerStyle.Alert);

            string newModeText = string.Empty;
            string newModeImage = string.Empty;
            string oldModeText = string.Empty;
            string oldModeImage = string.Empty;

            if (newMode == (int)ModeDefinitionEnum.ModeTracking)
            {
                newModeText = Application.LocalizedString("ModeTracking");
                newModeImage = "ModeTracking";
            }
            else if (newMode == (int)ModeDefinitionEnum.ModeDontMove)
            {
                newModeText = Application.LocalizedString("ModeDontMove");
                newModeImage = "ModeDontMove";
            }
            else if (newMode == (int)ModeDefinitionEnum.ModeZone)
            {
                newModeText = Application.LocalizedString("ModeZone");
                newModeImage = "ModeZone";
            }

            if (oldMode == (int)ModeDefinitionEnum.ModeTracking)
            {
                oldModeText = Application.LocalizedString("ModeTracking");
                oldModeImage = "ModeTracking";
            }
            else if (oldMode == (int)ModeDefinitionEnum.ModeDontMove)
            {
                oldModeText = Application.LocalizedString("ModeDontMove");
                oldModeImage = "ModeDontMove";
            }
            else if (oldMode == (int)ModeDefinitionEnum.ModeZone)
            {
                oldModeText = Application.LocalizedString("ModeZone");
                oldModeImage = "ModeZone";
            }

            var image1 = new UIImageView();
            image1.Image = UIImage.FromBundle(oldModeImage);
            image1.Frame = new CGRect(35, 100, 50, 50);
            actionSheetAlert.Add(image1);

            var imageArrow = new UIImageView();
            imageArrow.Image = UIImage.FromBundle("Arrow2");
            imageArrow.Frame = new CGRect(120, 110, 30, 30);
            imageArrow.Transform = CGAffineTransform.MakeRotation((float)Math.PI / 2);
            actionSheetAlert.Add(imageArrow);

            var image2 = new UIImageView();
            image2.Image = UIImage.FromBundle(newModeImage).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            image2.Frame = new CGRect(185, 100, 50, 50);
            if (isInPowerSaving) image2.TintColor = UIColor.FromRGB(200, 200, 200);
            else image2.TintColor = UIColor.FromRGB(98, 218, 115);
            actionSheetAlert.Add(image2);

            var oldModeLabel1 = new UILabel();
            oldModeLabel1.Text = Application.LocalizedString("ModePopupOldMode");
            oldModeLabel1.Font = UIFont.FromName("HelveticaNeue", 12);
            oldModeLabel1.Frame = new CGRect(25, 50, 80, 40);
            oldModeLabel1.TextAlignment = UITextAlignment.Center;
            oldModeLabel1.TextColor = UIColor.FromRGB(153, 153, 153);
            oldModeLabel1.BackgroundColor = UIColor.Clear;
            oldModeLabel1.Lines = 2;
            actionSheetAlert.Add(oldModeLabel1);

            var newModeLabel2 = new UILabel();
            newModeLabel2.Text = Application.LocalizedString("ModePopupNewMode");
            newModeLabel2.Font = UIFont.FromName("HelveticaNeue", 12);
            newModeLabel2.Frame = new CGRect(175, 50, 80, 40);
            newModeLabel2.TextAlignment = UITextAlignment.Center;
            newModeLabel2.TextColor = UIColor.FromRGB(153, 153, 153);
            newModeLabel2.BackgroundColor = UIColor.Clear;
            newModeLabel2.Lines = 2;
            actionSheetAlert.Add(newModeLabel2);

            var oldMode1 = new UILabel();
            oldMode1.Text = oldModeText;
            oldMode1.Font = UIFont.FromName("HelveticaNeue", 12);
            oldMode1.Frame = new CGRect(25, 170, 70, 40);
            oldMode1.TextAlignment = UITextAlignment.Center;
            oldMode1.TextColor = UIColor.FromRGB(51, 51, 51);
            oldMode1.BackgroundColor = UIColor.Clear;
            oldMode1.Lines = 2;
            actionSheetAlert.Add(oldMode1);

            var newMode2 = new UILabel();
            newMode2.Text = newModeText;
            newMode2.Font = UIFont.FromName("HelveticaNeue", 12);
            newMode2.Frame = new CGRect(175, 170, 70, 40);
            newMode2.TextAlignment = UITextAlignment.Center;
            newMode2.TextColor = UIColor.FromRGB(51, 51, 51);
            newMode2.BackgroundColor = UIColor.Clear;
            newMode2.Lines = 2;
            actionSheetAlert.Add(newMode2);

            if (isInPowerSaving)
            {
                var powerSaving = new UILabel();
                powerSaving.Text = powerSagingText;
                powerSaving.Font = UIFont.FromName("HelveticaNeue", 12);
                powerSaving.Frame = new CGRect(10, 220, 250, 60);
                powerSaving.TextAlignment = UITextAlignment.Center;
                powerSaving.TextColor = UIColor.FromRGB(255, 76, 46);
                powerSaving.BackgroundColor = UIColor.Clear;
                powerSaving.Lines = 4;
                actionSheetAlert.Add(powerSaving);
            }

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Yes")
                , UIAlertActionStyle.Default
                , (action) => { tcs.SetResult(true); }));

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("No")
                , UIAlertActionStyle.Cancel
                , (action) => { tcs.SetResult(false); }));

            CurrentViewController.PresentViewController(actionSheetAlert, true, null);

            return tcs.Task;
        }

        public void ShowLoadingLayout()
        {
            if (CurrentViewController == null) throw new Exception("CurrentViewController can not be null in DialogService");
            CurrentViewController.PresentViewController(AlertControllerHelper.ShowAlertLoading(), true, null);
        }

        public void HideLoadingLayout()
        {
            if (CurrentViewController == null) throw new Exception("CurrentViewController can not be null in DialogService");
            CurrentViewController.DismissViewController(true, null);
        }

        public Task ShowPopupCredit(string title, string message)
        {
            if (CurrentViewController == null) throw new Exception("CurrentViewController can not be null in DialogService");

            var tcs = new TaskCompletionSource<bool>();

            var actionSheetAlert = UIAlertController.Create(title + "\n\n\n\n\n\n\n\n" 
                , string.Empty
                , UIAlertControllerStyle.Alert);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Close")
                , UIAlertActionStyle.Default
                , (action) => { tcs.SetResult(true); }));

            var iconCredit = new UIImageView();
            iconCredit.Image = UIImage.FromBundle("IconAddCredits");
            iconCredit.Frame = new CGRect(95, 90, 80, 80);
            actionSheetAlert.Add(iconCredit);

            var content = new UILabel();
            content.Text = message;
            content.Font = UIFont.FromName("HelveticaNeue", 14);
            content.Frame = new CGRect(20, 170, 250, 100);
            content.TextAlignment = UITextAlignment.Left;
            content.TextColor = UIColor.FromRGB(51, 51, 51);
            content.BackgroundColor = UIColor.Clear;
            content.Lines = 5;
            actionSheetAlert.Add(content);

            CurrentViewController.PresentViewController(actionSheetAlert, true, null);
            
            return tcs.Task;
        }

        public Task ShowPopupAppUpdates(string title)
        {
            if (CurrentViewController == null) throw new Exception("CurrentViewController can not be null in DialogService");

            var tcs = new TaskCompletionSource<bool>();

            var actionSheetAlert = UIAlertController.Create(title + "\n\n\n\n\n\n\n\n"
                , string.Empty
                , UIAlertControllerStyle.Alert);

            actionSheetAlert.AddAction(UIAlertAction.Create(Application.LocalizedString("Close")
                , UIAlertActionStyle.Default
                , (action) => { tcs.SetResult(true); }));

            CurrentViewController.PresentViewController(actionSheetAlert, true, null);

            return tcs.Task;
        }
    }
}