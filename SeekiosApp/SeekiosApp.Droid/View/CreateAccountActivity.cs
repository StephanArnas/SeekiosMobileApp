
using Android.App;
using Android.OS;
using Android.Views;
using SeekiosApp.Droid.Helper;
using SeekiosApp.Droid.CustomComponents;
using Android.Text;
using SeekiosApp.Droid.Services;
using System;
using Android.Widget;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class CreateAccountActivity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================

        #endregion

        #region ===== Propriétés =================================================================

        /// <summary>Prénom de l'utilisateur</summary>
        public ImageEditTextLayout FirstNameEditText { get; private set; }

        /// <summary>Nom de l'utilisateur</summary>
        public ImageEditTextLayout LastNameEditText { get; private set; }

        /// <summary>Email de l'utilisateur</summary>
        public ImageEditTextLayout EmailEditText { get; private set; }

        /// <summary>Mot de passe de l'utilisateur</summary>
        public ImageEditTextLayout PasswordEditText { get; private set; }

        /// <summary>Confirmation du mot de passe</summary>
        public ImageEditTextLayout ConfirmedPasswordEditText { get; private set; }

        /// <summary>Button de création de compte</summary>
        public ImageButtonLayout CreateAccountButton { get; private set; }

        /// <summary></summary>
        public ImageView BackgroundImage{ get; private set;}

        #endregion

        #region ===== Cycle De Vie ================================================================

        /// <summary>
        /// Création de la page
        /// </summary>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ThemeHelper.OnActivityCreateSetTheme(this);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CreateAccountLayout);

            GetObjectsFromView();
            SetBindings();
            SetDataToView();
        }

        /// <summary>
        /// Reprise de la page
        /// </summary>
        protected override void OnResume()
        {
            CreateAccountButton.Click += CreateAccountClick;
            base.OnResume();
        }

        /// <summary>
        /// Suspension de la page
        /// </summary>
        protected override void OnPause()
        {
            CreateAccountButton.Click -= CreateAccountClick;
            base.OnPause();
        }

        /// <summary>
        /// Suppression de la page
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        #endregion

        #region ===== ActionBar ===================================================================

        /// <summary>
        /// Sélection d'un bouton de l'action bar
        /// </summary>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                // ferme la fenêtre
                default:
                     Finish();
                    break;
            }
            return true;
        }

        #endregion

        #region ===== Initialisation Vue ==========================================================
        /// <summary>
        /// Récupère les objets de la vue
        /// </summary>
        private void GetObjectsFromView()
        {
            FirstNameEditText = FindViewById<ImageEditTextLayout>(Resource.Id.createAccount_firstName);
            LastNameEditText = FindViewById<ImageEditTextLayout>(Resource.Id.createAccount_lastName);
            EmailEditText = FindViewById<ImageEditTextLayout>(Resource.Id.createAccount_emailAdress);

            PasswordEditText = FindViewById<ImageEditTextLayout>(Resource.Id.createAccount_password);
            ConfirmedPasswordEditText = FindViewById<ImageEditTextLayout>(Resource.Id.createAccount_confirmedPassword);

            CreateAccountButton = FindViewById<ImageButtonLayout>(Resource.Id.createAccount_createAccountButton);

            BackgroundImage = FindViewById<ImageView>(Resource.Id.login_backgroundActivity);
        }

        /// <summary>
        /// Fais les liaisons entre les données du vue modèle et les objets de la vue
        /// </summary>
        private void SetBindings()
        {

        }

        /// <summary>
        /// Initialise les objets de la vue avec les données
        /// </summary>
        private void SetDataToView()
        {
            if (!Android.Util.Patterns.EmailAddress.Matcher(EmailEditText.EditText.Text).Matches())
            {
                EmailEditText.EditText.SetError(Resources.GetString(Resource.String.createAccount_errorEmail)
                    , Resources.GetDrawable(Resource.Drawable.FieldInvalid));
            }

            if(PasswordEditText.EditText.Text != ConfirmedPasswordEditText.EditText.Text)
            {
                EmailEditText.EditText.SetError(Resources.GetString(Resource.String.createAccount_errorPasswordNoCorresponding)
                    , Resources.GetDrawable(Resource.Drawable.FieldInvalid));
            }
        }

        /// <summary>
        /// Récupère l'image du ViewModel pour la mettre en fond de l'activité
        /// </summary>
        private void SetBackground()
        {
            string backgroundName = App.Locator.Login.GetRamdomImageName();
            int resID = Resources.GetIdentifier(backgroundName.ToLower(), "drawable", PackageName);
            // nécessaire pour l'affichage de l'image sous Android 6 sinon image noir
            BackgroundImage.SetLayerType(LayerType.Software, null);
            BackgroundImage.SetImageResource(resID);
        }

        #endregion

        #region ===== Méthode Privées =============================================================

        /// <summary>
        /// Configure les filtres sur les champs de saisie de données
        /// </summary>
        private void SetFilterEditText()
        {
            FirstNameEditText.EditText.InputType = InputTypes.TextVariationPersonName;
            LastNameEditText.EditText.InputType = InputTypes.TextVariationPersonName;
            EmailEditText.EditText.InputType = InputTypes.TextVariationEmailAddress;
            PasswordEditText.EditText.InputType = InputTypes.TextVariationPassword;
        }

        #endregion

        #region ===== Évènement ===================================================================

        private void CreateAccountClick(object sender, EventArgs e)
        {
            App.Locator.CreateAccount.CreateAccount(EmailEditText.EditText.Text
                            , PasswordEditText.EditText.Text
                            , FirstNameEditText.EditText.Text
                            , LastNameEditText.EditText.Text);
        }

        #endregion
    }
}