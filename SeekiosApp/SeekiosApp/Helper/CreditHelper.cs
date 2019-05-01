namespace SeekiosApp.Helper
{
    public static class CreditHelper
    {
        private static int _refillDay = 16;

        public static string TotalCredits
        {
            get
            {
                int creditsOffered = 0;
                if (App.CurrentUserEnvironment.LsSeekios?.Count > 0)
                {
                    foreach (var seekios in App.CurrentUserEnvironment.LsSeekios)
                    {
                        creditsOffered += seekios.FreeCredit;
                    }
                }
                if ((App.CurrentUserEnvironment.User.RemainingRequest + creditsOffered) > 99999)
                {
                    return "+99999";
                }
                else return (App.CurrentUserEnvironment.User.RemainingRequest + creditsOffered).ToString();
            }
        }

        public static string CreditsOffered
        {
            get
            {
                int creditsOffered = 0;
                foreach (var seekios in App.CurrentUserEnvironment.LsSeekios)
                {
                    creditsOffered += seekios.FreeCredit;
                }
                return creditsOffered.ToString();
            }
        }

        public static int CreditsOfferedInteger
        {
            get
            {
                int creditsOffered = 0;
                foreach (var seekios in App.CurrentUserEnvironment.LsSeekios)
                {
                    creditsOffered += seekios.FreeCredit;
                }
                return creditsOffered;
            }
        }

        public static string CalculateNextReload()
        {
            return DateHelper.ComputeNextDayOfMonth(_refillDay).ToString("dd/MM/yyyy");
        }
    }
}