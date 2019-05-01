using SeekiosApp.Properties;

namespace SeekiosApp.Helper
{
    public static class ErrorMessageHelper
    {
        #region LOGIN

        public static void HandlerLoginError(ref string title, ref string content, string error)
        {
            title = Resources.AuthenticationFailedTitle;

            switch (error)
            {
                default:
                    content = Resources.AuthenticationFailed;
                    break;
                case "0x0000":
                case "0x0001":
                case "0x0002":
                    content = Resources.UnauthorizedAccess;
                    break;
                case "0x0003":
                    content = Resources.EmailNotFound;
                    break;
                case "0x0004":
                    content = Resources.BadPassword;
                    break;
            }
        }

        #endregion

        #region ENVIRONMENT

        public static void HandleUserEnvironmentError(ref string title, ref string content, string error)
        {
            title = Resources.AppNeedsUpdateTitle;

            switch (error)
            {
                default:
                    content = Resources.AppNeedsUpdate;
                    break;
                case "0x0102":
                case "0x0103":
                    content = Resources.AppNeedsUpdate;
                    break;
            }
        }

        #endregion

        #region USER

        public static void HandleInsertUserError(ref string title, ref string content, string error)
        {
            switch (error)
            {
                default:
                    content = Resources.CreateAccountFailedText;
                    title = Resources.CreateAccountFailedTitle;
                    break;
                case "0x0120":
                    content = Resources.EmailEmptyContent;
                    title = Resources.EmailErrorTitle;
                    break;
                case "0x0121":
                    content = Resources.EmailSyntaxErrorContent;
                    title = Resources.EmailErrorTitle;
                    break;
                case "0x0122":
                    content = Resources.EmptyPassword;
                    title = Resources.PasswordError;
                    break;
                case "0x0123":
                    content = Resources.PasswordTooShort;
                    title = Resources.InputError;
                    break;
                case "0x0124":
                    content = Resources.EmptyFirstName;
                    title = Resources.InputError;
                    break;
                case "0x0125":
                    content = Resources.EmptyLastName;
                    title = Resources.InputError;
                    break;
                case "0x0126":
                    content = Resources.CountryCodeError;
                    title = Resources.InputError;
                    break;
                case "0x0127":
                    content = Resources.AccountAlreadyExistsText;
                    title = Resources.AccountAlreadyExistsTitle;
                    break;
            }
        }

        public static void HandleUpdateUserError(ref string title, ref string content, string error)
        {
            title = Resources.UpdateUserError;
            switch (error)
            {
                default:
                    content = Resources.UpdateUserFailed;
                    break;
                case "0x0130":
                    content = Resources.EmailEmptyContent;
                    break;
                case "0x0131":
                    content = Resources.EmailSyntaxErrorContent;
                    break;
                case "0x0132":
                    content = Resources.EmailErrorTitle;
                    break;
                case "0x0133":
                    content = Resources.PasswordError;
                    break;
                case "0x0134":
                    content = Resources.PasswordTooShort;
                    break;
                case "0x0135":
                    content = Resources.EmptyFirstName;
                    break;
                case "0x0136":
                    content = Resources.EmptyLastName;
                    break;
                case "0x0137":
                    content = Resources.CountryCodeError;
                    break;
            }
        }

        public static void HandleAskNewPasswordError(ref string title, ref string content, int? errorCode)
        {
            if (errorCode == null) errorCode = 0;

            switch (errorCode)
            {
                default:
                case 0:
                    content = Resources.UnexpectedError;
                    title = Resources.UnexpectedErrorTitle;
                    break;
                case 1:
                    content = Resources.ResetPasswordRequestSentText;
                    title = Resources.ResetPasswordRequestSentTitle;
                    break;
                case -1:
                    content = Resources.EmailDoesntExistsText;
                    title = Resources.EmailDoesntExistsTitle;
                    break;
                case -2:
                    content = Resources.EmailSyntaxErrorContent;
                    title = Resources.EmailErrorTitle;
                    break;
            }
        }

        #endregion

        #region SEEKIOS

        public static void HandleInsertSeekiosError(ref string title, ref string content, string error)
        {
            title = Resources.InsertSeekiosError;
            switch (error)
            {
                default:
                    content = Resources.UnexpectedErrorTitle;
                    break;
                case "0x0220":
                    content = Resources.SeekiosNameEmpty;
                    break;
                case "0x0221":
                case "0x0222":
                case "0x0223":
                case "0x0224":
                case "0x0225":
                    content = Resources.AddSeekiosIMEIOrPinError;
                    break;
                case "0x0226":
                    content = Resources.AddSeekiosAlreadyUsed;
                    break;
                case "0x0227":
                    content = Resources.AddSeekiosIMEIOrPinError;
                    break;
            }
        }

        public static void HandleDeletSeekiosError(ref string title, ref string content, string error)
        {
            //by default return an unique message, I think it's not worth to be precise with the customer
            title = Resources.MySeekios_DeleteSeekios_Title;

            switch (error)
            {
                default:
                case "0x0240":
                case "0x0241":
                case "0x0242":
                case "0x0243":
                    content = Resources.MySeekios_DeleteSeekios_Content;
                    break;
            }
        }

        public static void HandleRefreshSeekiosLocationError(ref string title, ref string content, string error)
        {
            title = Resources.RefreshSeekiosFailedTitle;

            switch (error)
            {
                default:
                    content = Resources.SomethingWentWrong;
                    break;
                case "0x0250":
                    content = Resources.SomethingWentWrong;
                    break;
                case "0x0251":
                    content = Resources.NoMoreCredits;
                    break;
                case "0x0252":
                case "0x0253":
                case "0x0254":
                    content = Resources.SomethingWentWrong;
                    break;
            }
        }

        public static void HandleRefreshSeekiosBatteryError(ref string title, ref string content, string error)
        {
            title = Resources.BatteryUpdateTitle;

            switch (error)
            {
                default:
                    content = Resources.SomethingWentWrong;
                    break;
                case "0x0260":
                    content = Resources.SomethingWentWrong;
                    break;
                case "0x0261":
                    content = Resources.NoMoreCredits;
                    break;
                case "0x0262":
                case "0x0263":
                case "0x0264":
                    content = Resources.SomethingWentWrong;
                    break;
            }
        }

        #endregion

        #region LOCATIONS

        public static void HandleLocationsError(ref string title, ref string content, string error)
        {
            title = Resources.LocationsErrorTitle;

            switch (error)
            {
                default:
                    content = Resources.UnexpectedErrorTitle;
                    break;
                case "0x0350":
                    content = Resources.UnexpectedErrorTitle;
                    break;
                case "0x0351":
                    content = Resources.LowerDateError;
                    break;
                case "0x0352":
                    content = Resources.UpperDateError;
                    break;
                case "0x0353":
                case "0x0354":
                    content = Resources.UnexpectedErrorTitle;
                    break;
            }
        }

        public static void HandleLowerAndUpperDateError(ref string title, ref string content, string error)
        {
            title = Resources.LowerAndUpperDateErrorTitle;

            switch (error)
            {
                case "0x0360":
                case "0x0361":
                case "0x0362":
                default:
                    content = Resources.SeekiosUnavailable;
                    break;
            }
        }

        #endregion

        #region MODES

        public static void HandleInsertModeTrackingError(ref string title, ref string content, string error)
        {
            title = Resources.ModeNotConfiguredTitle;

            switch (error)
            {
                default:
                    content = Resources.UnexpectedErrorTitle;
                    break;
                case "0x0460":
                case "0x0461":
                case "0x0462":
                case "0x0463":
                    content = Resources.UnexpectedErrorTitle;
                    break;
                case "0x0464":
                    content = Resources.UnauthorizedAccessTitle;
                    break;
                case "0x0465":
                    content = Resources.SomethingWentWrong;
                    break;
                case "0x0466":
                    content = Resources.UnexpectedErrorTitle;
                    break;
                case "0x0467":
                    content = Resources.NoMoreCredits;
                    break;
            }
        }

        public static void HandleInsertModeZoneError(ref string title, ref string content, string error)
        {
            title = Resources.ModeNotConfiguredTitle;

            switch (error)
            {
                default:
                    content = Resources.UnexpectedErrorTitle;
                    break;
                case "0x0470":
                case "0x0471":
                case "0x0472":
                case "0x0473":
                    content = Resources.UnexpectedErrorTitle;
                    break;
                case "0x0474":
                    content = Resources.UnauthorizedAccessTitle;
                    break;
                case "0x0475":
                    content = Resources.SomethingWentWrong;
                    break;
                case "0x0476":
                    content = Resources.UnexpectedErrorTitle;
                    break;
                case "0x0477":
                    content = Resources.NoMoreCredits;
                    break;
            }
        }

        public static void HandleInsertModeDontMoveError(ref string title, ref string content, string error)
        {
            title = Resources.ModeNotConfiguredTitle;

            switch (error)
            {
                default:
                    content = Resources.UnexpectedErrorTitle;
                    break;
                case "0x0480":
                case "0x0481":
                case "0x0482":
                case "0x0483":
                    content = Resources.UnexpectedErrorTitle;
                    break;
                case "0x0484":
                    content = Resources.UnauthorizedAccessTitle;
                    break;
                case "0x0485":
                    content = Resources.SomethingWentWrong;
                    break;
                case "0x0486":
                    content = Resources.UnexpectedErrorTitle;
                    break;
                case "0x0487":
                    content = Resources.NoMoreCredits;
                    break;
            }
        }

        public static void HandleDeleteModeError(ref string title, ref string content, string error)
        {
            title = Resources.CannotDeleteModeTitle;

            switch (error)
            {
                default:
                    content = Resources.UnexpectedErrorTitle;
                    break;
                case "0x0500":
                case "0x0501":
                case "0x0502":
                case "0x0503":
                    content = Resources.CannotDeleteModeMessage;
                    break;
            }
        }

        #endregion

        #region ALERTS

        public static void HandleAlertByModeError(ref string title, ref string content, string error)
        {
            title = Resources.AlertErrorTitle;

            switch (error)
            {
                default:
                    content = Resources.UnexpectedErrorTitle;
                    break;
                case "0x0660":
                case "0x0661":
                case "0x0662":
                    content = Resources.UnexpectedErrorTitle;
                    break;
            }
        }

        public static void HandleAlertSOSHasBeenReadError(ref string title, ref string content, string error)
        {
            title = Resources.AlertErrorTitle;

            switch (error)
            {
                default:
                    content = Resources.UnexpectedError;
                    break;
                case "0x0670":
                case "0x0671":
                case "0x0672":
                case "0x0673":
                    content = Resources.UnexpectedError;
                    break;
            }
        }

        public static void HandleInsertAlertError(ref string title, ref string content, string error)
        {
            title = Resources.InsertAlertErrorTitle;

            switch (error)
            {
                default:
                    content = Resources.UnexpectedError;
                    break;
                case "0x0680":
                case "0x0681":
                case "0x0682":
                case "0x0683":
                    content = Resources.RecipientListNeeded;
                    break;
                case "0x0684":
                case "0x0685":
                    content = Resources.UnexpectedError;
                    break;
            }
        }

        public static void HandleUpdateAlertError(ref string title, ref string content, string error)
        {
            title = Resources.InsertAlertErrorTitle;

            switch (error)
            {
                default:
                    content = Resources.UnexpectedError;
                    break;
                case "0x0690":
                case "0x0691":
                case "0x0692":
                case "0x0693":
                case "0x0694":
                case "0x0695":
                    content = Resources.UnexpectedError;
                    break;
            }
        }

        #endregion

        #region CREDITS

        public static void HandleCreditPacksError(ref string title, ref string content, string error)
        {
            title = Resources.WrongLanguageTitle;

            switch (error)
            {
                default:
                    content = Resources.UnexpectedErrorTitle;
                    break;
                case "0x0800":
                    content = Resources.WrongLanguage;
                    break;
            }
        }

        #endregion
    }
}
