using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using SeekiosApp.Interfaces;
using SeekiosApp.Enum;

namespace SeekiosApp.Droid.Services
{
    public class AppCompatDialogService : IDialogService
    {
        public Task ShowError(string message
            , string title
            , string buttonText
            , Action afterHideCallback)
        {
            var afterHideCallbackWithResponse = (Action<bool>)(r =>
            {
                if (afterHideCallback == null)
                    return;
                afterHideCallback();
                afterHideCallback = null;
            });

            var dialog = CreateDialog(message, title, buttonText, null, afterHideCallbackWithResponse);
            dialog.Dialog.Show();
            return dialog.Tcs.Task;
        }

        public Task ShowError(Exception error
            , string title
            , string buttonText
            , Action afterHideCallback)
        {
            var afterHideCallbackWithResponse = (Action<bool>)(r =>
            {
                if (afterHideCallback == null)
                    return;
                afterHideCallback();
                afterHideCallback = null;
            });

            var dialog = CreateDialog(error.Message, title, buttonText, null, afterHideCallbackWithResponse);
            dialog.Dialog.Show();
            return dialog.Tcs.Task;
        }

        public Task ShowMessage(string message
            , string title)
        {
            var dialog = CreateDialog(message, title);
            dialog.Dialog.Show();
            return dialog.Tcs.Task;
        }

        public Task ShowMessage(string message
            , string title
            , string buttonText
            , Action afterHideCallback)
        {
            var afterHideCallbackWithResponse = (Action<bool>)(r =>
            {
                if (afterHideCallback == null)
                    return;
                afterHideCallback();
                afterHideCallback = null;
            });

            var dialog = CreateDialog(message, title, buttonText, null, afterHideCallbackWithResponse);
            dialog.Dialog.Show();
            return dialog.Tcs.Task;
        }

        public Task<bool> ShowMessage(string message
            , string title
            , string buttonConfirmText
            , string buttonCancelText
            , Action<bool> afterHideCallback)
        {
            var afterHideCallbackWithResponse = (Action<bool>)(r =>
            {
                if (afterHideCallback == null)
                    return;
                afterHideCallback(r);
                afterHideCallback = null;
            });

            var dialog = CreateDialog(message, title, buttonConfirmText, buttonCancelText ?? "Cancel", afterHideCallbackWithResponse);
            dialog.Dialog.Show();
            return dialog.Tcs.Task;
        }

        public Task ShowMessageBox(string message
            , string title)
        {
            return ShowMessage(message, title);
        }

        private static AlertDialogInfo CreateDialog(string content
            , string title
            , string okText = null
            , string cancelText = null
            , Action<bool> afterHideCallbackWithResponse = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            var builder = new AlertDialog.Builder(AppCompatActivityBase.CurrentActivity, Resource.Style.Theme_AppCompat_Light_Dialog_Alert);
            builder.SetMessage(content);
            builder.SetTitle(title);
            var dialog = (AlertDialog)null;
            builder.SetPositiveButton(okText ?? "OK", (d, index) =>
            {
                tcs.TrySetResult(true);
                if (dialog != null)
                {
                    dialog.Dismiss();
                    dialog.Dispose();
                }
                if (afterHideCallbackWithResponse == null)
                    return;
                afterHideCallbackWithResponse(true);
            });

            if (cancelText != null)
            {
                builder.SetNegativeButton(cancelText, (d, index) =>
                {
                    tcs.TrySetResult(false);
                    if (dialog != null)
                    {
                        dialog.Dismiss();
                        dialog.Dispose();
                    }
                    if (afterHideCallbackWithResponse == null)
                        return;
                    afterHideCallbackWithResponse(false);
                });
            }

            builder.SetOnDismissListener(new OnDismissListener(() =>
            {
                tcs.TrySetResult(false);
                if (afterHideCallbackWithResponse == null)
                    return;
                afterHideCallbackWithResponse(false);
            }));

            dialog = builder.Create();

            return new AlertDialogInfo
            {
                Dialog = dialog,
                Tcs = tcs
            };
        }

        public Task<bool> ShowChangeModePopup(string titlePopup
            , string powerSagingText
            , int oldMode
            , int newMode
            , bool isInPowerSaving)
        {
            var tcs = new TaskCompletionSource<bool>();

            int oldModeImage = 0;
            int oldModeText = 0;

            int newModeImage = 0;
            int newModeText = 0;

            if (oldMode == (int)ModeDefinitionEnum.ModeTracking)
            {
                oldModeImage = Resource.Drawable.ModeTracking;
                oldModeText = Resource.String.modeDefinition_tracking;
            }
            else if (oldMode == (int)ModeDefinitionEnum.ModeZone)
            {
                oldModeImage = Resource.Drawable.ModeZone;
                oldModeText = Resource.String.modeDefinition_zone;
            }
            else if (oldMode == (int)ModeDefinitionEnum.ModeDontMove)
            {
                oldModeImage = Resource.Drawable.ModeDontMove;
                oldModeText = Resource.String.modeDefinition_dontmove;
            }

            if (newMode == (int)ModeDefinitionEnum.ModeTracking)
            {
                newModeImage = Resource.Drawable.ModeTracking;
                newModeText = Resource.String.modeDefinition_tracking;
            }
            else if (newMode == (int)ModeDefinitionEnum.ModeZone)
            {
                newModeImage = Resource.Drawable.ModeZone;
                newModeText = Resource.String.modeDefinition_zone;
            }
            else if (newMode == (int)ModeDefinitionEnum.ModeDontMove)
            {
                newModeImage = Resource.Drawable.ModeDontMove;
                newModeText = Resource.String.modeDefinition_dontmove;
            }

            AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(AppCompatActivityBase.CurrentActivity
                , Resource.Style.Theme_AppCompat_Light_Dialog_Alert);
            AlertDialog alertDialog = null;

            var view = AppCompatActivityBase.CurrentActivity.LayoutInflater.Inflate(Resource.Layout.ChangeModePopup, null);
            var previousModeSvg = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.previousMode);
            var nextModeSvg = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.nextMode);
            var previousModeText = view.FindViewById<TextView>(Resource.Id.previousModeText);
            var nextModeText = view.FindViewById<TextView>(Resource.Id.nextModeText);
            var titleText = view.FindViewById<TextView>(Resource.Id.title);
            if (isInPowerSaving)
            {
                var powerSavingText = view.FindViewById<TextView>(Resource.Id.changeModePopup_powerSaving);
                powerSavingText.Text = powerSagingText;
                powerSavingText.Visibility = ViewStates.Visible;
                nextModeSvg.SetSvg(AppCompatActivityBase.CurrentActivity, newModeImage, "62da73=c8c8c8");
            }
            else nextModeSvg.SetSvg(AppCompatActivityBase.CurrentActivity, newModeImage);

            titleText.Text = AppCompatActivityBase.CurrentActivity.Resources.GetString(Resource.String.popup_configModeTitle);

            previousModeSvg.SetSvg(AppCompatActivityBase.CurrentActivity, oldModeImage);
            previousModeText.SetText(oldModeText);
            nextModeText.SetText(newModeText);

            alertDialogBuilder.SetView(view);

            alertDialogBuilder.SetPositiveButton(AppCompatActivityBase.CurrentActivity.Resources.GetString(Resource.String.popup_configModeAccept)
                , (senderAlert, args) => { tcs.SetResult(true); });
            alertDialogBuilder.SetNegativeButton(AppCompatActivityBase.CurrentActivity.Resources.GetString(Resource.String.popup_configModeCancel)
                , (senderAlert, args) => { tcs.SetResult(false); });

            alertDialog = alertDialogBuilder.Create();
            alertDialog.Show();

            return tcs.Task;
        }

        public void ShowLoadingLayout()
        {
            if (AppCompatActivityBase.CurrentActivity == null) throw new Exception("CurrentViewController can not be null in DialogService");
            AppCompatActivityBase.CurrentActivity.LoadingLayout.Visibility = ViewStates.Visible;
        }

        public void HideLoadingLayout()
        {
            if (AppCompatActivityBase.CurrentActivity == null) throw new Exception("CurrentViewController can not be null in DialogService");
            AppCompatActivityBase.CurrentActivity.LoadingLayout.Visibility = ViewStates.Gone;
        }

        public Task ShowPopupCredit(string title, string message)
        {
            var tcs = new TaskCompletionSource<bool>();

            AlertDialog.Builder alertDialogBuilder = new AlertDialog.Builder(AppCompatActivityBase.CurrentActivity
                , Resource.Style.Theme_AppCompat_Light_Dialog_Alert);
            AlertDialog alertDialog = null;

            var view = AppCompatActivityBase.CurrentActivity.LayoutInflater.Inflate(Resource.Layout.PopupReloadCreditMonthly, null);
            var titleTextView = view.FindViewById<TextView>(Resource.Id.title);
            var contentTextView = view.FindViewById<TextView>(Resource.Id.popupReloadCreditMonthly_content);

            titleTextView.Text = title;
            contentTextView.Text = message;

            alertDialogBuilder.SetView(view);
            alertDialogBuilder.SetPositiveButton(AppCompatActivityBase.CurrentActivity.Resources.GetString(Resource.String.map_closePopup)
                , (senderAlert, args) => { tcs.SetResult(true); });

            alertDialog = alertDialogBuilder.Create();
            alertDialog.Show();

            return tcs.Task;
        }

        private struct AlertDialogInfo
        {
            public AlertDialog Dialog;
            public TaskCompletionSource<bool> Tcs;
        }

        private sealed class OnDismissListener : Java.Lang.Object, IDialogInterfaceOnDismissListener
        {
            private readonly Action _action;

            public OnDismissListener(Action action)
            {
                _action = action;
            }

            public void OnDismiss(IDialogInterface dialog)
            {
                _action();
            }
        }
    }
}