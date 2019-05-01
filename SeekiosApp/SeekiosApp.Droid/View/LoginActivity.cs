using Android.Widget;
using Android.App;
using Android.OS;
using Android.Views;
using SeekiosApp.Droid.Helper;
using SeekiosApp.Droid.Services;
using System;
using Android.Content;
using Android.Graphics;
using SeekiosApp.Extension;
using Android.Text;
using Android.Text.Style;
using SeekiosApp.Droid.CustomComponents;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class LoginActivity : AppCompatActivityBase/*, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback*/
    {
        #region ===== Properties ==================================================================

        public static MyTimer timer;

        #region LOGIN

        /// <summary>Image de chargement</summary>
        public ProgressBar LoaderImage { get; private set; }

        /// <summary>Page de fond</summary>
        public ImageView BackgroundImage { get; private set; }

        /// <summary>Adresse email de l'utilisateur</summary>
        public EditText Email { get; private set; }

        /// <summary>Mot de passe de l'utilsateur</summary>
        public EditText Password { get; private set; }

        /// <summary>Button de connexion</summary>
        public Button Connect { get; set; }

        /// <summary>Button pour se connecter avec Facebook</summary>
        //public LoginButton ConnectFacebook { get; set; }

        /// <summary>Button pour se connecter avec Twitter</summary>
        public LinearLayout ConnectTwitter { get; set; }

        /// <summary>Button pour se connecter avec GooglePlus</summary>
        public LinearLayout ConnectGooglePlus { get; set; }

        /// <summary>Layout du bouton create account</summary>
        public LinearLayout GoToCreateAccountLayout { get; set; }

        /// <summary>Layout du bouton password forgotten</summary>
        public LinearLayout GoToForgetPasswordLayout { get; set; }

        /// <summary>Lien pour aller vers la page de Creation de compte</summary>
        public TextView GoToCreateAccount { get; set; }

        /// <summary>Lien pour aller vers la page Forget Password</summary>
        public TextView GoToForgetPassword { get; set; }

        /// <summary>Layout de la connexion de l'utilisateur</summary>
        public LinearLayout LoginLinearLayout { get; set; }
        public LinearLayout GoToWhatIsSeekiosLayout { get; set; }

        #endregion

        #region CREATION D'UN COMPTE

        /// <summary>Prénom de l'utilisateur</summary>
        public EditText FirstNameEditText { get; private set; }

        /// <summary>Nom de l'utilisateur</summary>
        public EditText LastNameEditText { get; private set; }

        /// <summary>Email de l'utilisateur</summary>
        public EditText EmailEditText { get; private set; }

        /// <summary>Mot de passe de l'utilisateur</summary>
        public EditText PasswordEditText { get; private set; }

        /// <summary>Confirmation du mot de passe</summary>
        public EditText ConfirmedPasswordEditText { get; private set; }

        /// <summary>Button de création de compte</summary>
        public Button CreateAccountButton { get; private set; }

        /// <summary>Layout de la creation d'un compte</summary>
        public LinearLayout CreateAccountLayout { get; set; }

        #endregion

        #region OUBLIE DU MOT DE PASSE

        /// <summary>Email de l'utilisateur</summary>
        public EditText ForgetPasswordEditText { get; private set; }

        /// <summary>Button de création de compte</summary>
        public Button ForgetPasswordButton { get; private set; }

        /// <summary>Layout de la creation d'un compte</summary>
        public LinearLayout ForgetPasswordLinearLayout { get; set; }

        /// <summary>Return button</summary>
        public LinearLayout ReturnButtonLayout { get; set; }

        #endregion

        #endregion

        #region ===== Attributs ===================================================================

        /// <summary></summary>
        //private ICallbackManager mCallBackManager = null;

        private Dialog _popupCGU = null;
        private TextView _stepInformationTextView;
        private TextView _titleTextView;
        private TextView _linkToCGUTextView;

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //FacebookSdk.SdkInitialize(this.ApplicationContext);
            //if (AccessToken.CurrentAccessToken != null)
            //{
            //    LoginManager.Instance.LogOut();
            //}

            SetContentView(Resource.Layout.LoginLayout);

            GetObjectsFromView();
            SetBackground();
            SetDataToView();
            //InitialiseFacebook();
            if (!App.Locator.Login.IsDeconnected && Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
            {
                AutoConnexion();
            }
        }

        protected override void OnResume()
        {
            //subscribe to connection state changed event
            OnConnectionStateChanged += UpdateUIOnConnectionStateChanged;

            base.OnResume();
            App.Locator.Login.PropertyChanged += OnLoginPropertyChanged;
            GoToCreateAccountLayout.Click += HideLoginLayoutToCreateAccountClick;
            CreateAccountButton.Click += CreateAccountClick;
            GoToForgetPasswordLayout.Click += HideLoginLayoutToForgetPasswordClick;
            ForgetPasswordButton.Click += ForgetPasswordClick;
            Connect.Click += ConnectClick;
            GoToWhatIsSeekiosLayout.Click += GoToWhatIsSeekiosLayoutClick;

            ForgetPasswordEditText.TextChanged += OnForgetEmailTextChanged;

            Email.TextChanged += OnLoginEmailTextChanged;
            Password.TextChanged += OnLoginPasswordTextChanged;

            FirstNameEditText.TextChanged += OnUserFirstNameTextChanged;
            LastNameEditText.TextChanged += OnUserLastNameTextChanged;
            EmailEditText.TextChanged += OnUserEmailTextChanged;
            PasswordEditText.TextChanged += OnPasswordTextChanged;
            PasswordEditText.FocusChange += PasswordEditText_FocusChange;
            ConfirmedPasswordEditText.TextChanged += OnConfirmedPasswordTextChanged;

            ReturnButtonLayout.Click += ReturnButtonClick;
        }

        protected override void OnPause()
        {
            base.OnPause();
            App.Locator.Login.PropertyChanged -= OnLoginPropertyChanged;
            GoToCreateAccountLayout.Click -= HideLoginLayoutToCreateAccountClick;
            CreateAccountButton.Click -= CreateAccountClick;
            GoToForgetPasswordLayout.Click -= HideLoginLayoutToForgetPasswordClick;
            ForgetPasswordButton.Click -= ForgetPasswordClick;
            Connect.Click -= ConnectClick;
            GoToWhatIsSeekiosLayout.Click -= GoToWhatIsSeekiosLayoutClick;

            ForgetPasswordEditText.TextChanged -= OnForgetEmailTextChanged;

            Email.TextChanged -= OnLoginEmailTextChanged;
            Password.TextChanged -= OnLoginPasswordTextChanged;

            FirstNameEditText.TextChanged -= OnUserFirstNameTextChanged;
            LastNameEditText.TextChanged -= OnUserLastNameTextChanged;
            EmailEditText.TextChanged -= OnUserEmailTextChanged;
            PasswordEditText.TextChanged -= OnPasswordTextChanged;
            PasswordEditText.FocusChange -= PasswordEditText_FocusChange;
            ConfirmedPasswordEditText.TextChanged -= OnConfirmedPasswordTextChanged;

            ReturnButtonLayout.Click -= ReturnButtonClick;

            //subscribe to connection state changed event
            OnConnectionStateChanged -= UpdateUIOnConnectionStateChanged;
        }

        protected override void OnStop()
        {
            base.OnStop();
            App.Locator.Login.IsLoading = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnBackPressed()
        {
            if (LoginLinearLayout.Visibility == ViewStates.Visible)
            {
                return;
            }
            CreateAccountLayout.Visibility = ViewStates.Gone;
            LoginLinearLayout.Visibility = ViewStates.Visible;
            ForgetPasswordLinearLayout.Visibility = ViewStates.Gone;
            ReturnButtonLayout.Visibility = ViewStates.Gone;
            GoToForgetPasswordLayout.Visibility = ViewStates.Visible;
            GoToCreateAccountLayout.Visibility = ViewStates.Visible;
            GoToWhatIsSeekiosLayout.Visibility = ViewStates.Visible;
        }

        #endregion

        #region ===== Initialisze View ============================================================

        private void GetObjectsFromView()
        {
            // LOGIN
            Email = FindViewById<EditText>(Resource.Id.login_email);
            Password = FindViewById<EditText>(Resource.Id.login_password);
            Connect = FindViewById<Button>(Resource.Id.login_connect);
            //ConnectFacebook = FindViewById<LoginButton>(Resource.Id.login_connectFacebook);
            GoToCreateAccount = FindViewById<TextView>(Resource.Id.login_createAccount);
            GoToForgetPassword = FindViewById<TextView>(Resource.Id.login_forgetPassword);
            LoaderImage = FindViewById<ProgressBar>(Resource.Id.login_loaderImg);
            BackgroundImage = FindViewById<ImageView>(Resource.Id.login_backgroundActivity);
            LoginLinearLayout = FindViewById<LinearLayout>(Resource.Id.login_layout);
            GoToCreateAccountLayout = FindViewById<LinearLayout>(Resource.Id.login_createAccountLayout);
            GoToForgetPasswordLayout = FindViewById<LinearLayout>(Resource.Id.login_forgetPasswordLayout);
            GoToWhatIsSeekiosLayout = FindViewById<LinearLayout>(Resource.Id.login_whatIsSeekiosLayout);
            ReturnButtonLayout = FindViewById<LinearLayout>(Resource.Id.login_returnButton);
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);

            /// CREATION D'UN COMPTE
            CreateAccountLayout = FindViewById<LinearLayout>(Resource.Id.createAccount_layout);
            FirstNameEditText = FindViewById<EditText>(Resource.Id.createAccount_firstName);
            LastNameEditText = FindViewById<EditText>(Resource.Id.createAccount_lastName);
            EmailEditText = FindViewById<EditText>(Resource.Id.createAccount_emailAdress);
            PasswordEditText = FindViewById<EditText>(Resource.Id.createAccount_password);
            ConfirmedPasswordEditText = FindViewById<EditText>(Resource.Id.createAccount_confirmedPassword);
            CreateAccountButton = FindViewById<Button>(Resource.Id.createAccount_createAccountButton);

            /// OUBLIE DU MOT DE PASSE
            ForgetPasswordLinearLayout = FindViewById<LinearLayout>(Resource.Id.forgetPassword_layout);
            ForgetPasswordEditText = FindViewById<EditText>(Resource.Id.forgetPassword_emailAdress);
            ForgetPasswordButton = FindViewById<Button>(Resource.Id.forgetPassword_button);
        }

        private void SetDataToView()
        {
            Connect.Enabled = false;
            CreateAccountButton.Enabled = false;
            ForgetPasswordButton.Enabled = false;
            CreateAccountLayout.Visibility = ViewStates.Gone;
            ForgetPasswordLinearLayout.Visibility = ViewStates.Gone;
            ReturnButtonLayout.Visibility = ViewStates.Gone;
            //ConnectFacebook.SetBackgroundColor(Color.Argb(200, 65, 93, 174));
        }

        private void SetBackground()
        {
            string backgroundName = App.Locator.Login.GetRamdomImageName();
            int resID = Resources.GetIdentifier(backgroundName.ToLower(), "drawable", PackageName);
            // nécessaire pour l'affichage de l'image sous Android 6 sinon image noir
            BackgroundImage.SetLayerType(LayerType.Software, null);
            BackgroundImage.SetImageResource(resID);
        }

        #endregion

        #region ===== Private Methodes ============================================================

        /// <summary>
        /// If the user have credential store in local
        /// Launch the auto connection
        /// </summary>
        private async void AutoConnexion()
        {
            LoaderImage.Visibility = ViewStates.Visible;
            Email.Visibility = ViewStates.Gone;
            Password.Visibility = ViewStates.Gone;
            GoToWhatIsSeekiosLayout.Visibility = ViewStates.Gone;
            Connect.Visibility = ViewStates.Gone;
            GoToCreateAccountLayout.Visibility = ViewStates.Gone;
            GoToForgetPasswordLayout.Visibility = ViewStates.Gone;

            if (!await App.Locator.Login.AutoConnect(DeviceInfoHelper.DeviceModel
                , DeviceInfoHelper.Platform
                , DeviceInfoHelper.Version
                , DeviceInfoHelper.GetDeviceUniqueId(this)
                , DeviceInfoHelper.CountryCode))
            {
                LoaderImage.Visibility = ViewStates.Gone;
                Email.Visibility = ViewStates.Visible;
                Password.Visibility = ViewStates.Visible;
                GoToWhatIsSeekiosLayout.Visibility = ViewStates.Visible;
                Connect.Visibility = ViewStates.Visible;
                GoToCreateAccountLayout.Visibility = ViewStates.Visible;
                GoToForgetPasswordLayout.Visibility = ViewStates.Visible;
            }
        }

        #endregion

        #region ===== Event =======================================================================

        /// <summary>
        /// Raised when boolean changed in the LoginViewModel
        /// </summary>
        private void OnLoginPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsLoading")
            {
                RunOnUiThread(() =>
                {
                    var value = !App.Locator.Login.IsLoading;
                    var alpha = App.Locator.Login.IsLoading ? 50 : 200;
                    LoaderImage.Visibility = App.Locator.Login.IsLoading ? ViewStates.Visible : ViewStates.Invisible;
                    //ConnectFacebook.Enabled = value;
                    //ConnectFacebook.Clickable = value;
                    //ConnectFacebook.SetBackgroundColor(Color.Argb(alpha, 65, 93, 174));
                    GoToForgetPassword.Enabled = value;
                    GoToForgetPassword.Visibility = !App.Locator.Login.IsLoading ? ViewStates.Visible : ViewStates.Invisible;
                    GoToCreateAccount.Enabled = value;
                    GoToCreateAccount.Visibility = !App.Locator.Login.IsLoading ? ViewStates.Visible : ViewStates.Invisible;
                    GoToWhatIsSeekiosLayout.Enabled = value;
                    GoToWhatIsSeekiosLayout.Visibility = !App.Locator.Login.IsLoading ? ViewStates.Visible : ViewStates.Invisible; ;
                    Email.Enabled = value;
                    Email.SetBackgroundColor(Color.Argb(alpha, 255, 255, 255));
                    Password.Enabled = value;
                    Password.SetBackgroundColor(Color.Argb(alpha, 255, 255, 255));
                    GoToCreateAccountLayout.Clickable = value;
                    GoToForgetPasswordLayout.Clickable = value;
                    Connect.SetBackgroundColor(Color.Argb(alpha, 98, 218, 115));
                    CreateAccountButton.Enabled = value;
                });
            }

            if (e.PropertyName == "CanConnect")
            {
                if (App.DeviceIsConnectedToInternet) Connect.Enabled = App.Locator.Login.CanConnect;
            }

            if (e.PropertyName == "CanCreateAccount")
            {
                if (App.DeviceIsConnectedToInternet) CreateAccountButton.Enabled = App.Locator.Login.CanCreateAccount;
            }

            if (e.PropertyName == "CanForgetPassword")
            {
                if (App.DeviceIsConnectedToInternet) ForgetPasswordButton.Enabled = App.Locator.Login.CanForgetPassword;
            }

            if (e.PropertyName == "Password" && App.Locator.Login.Password == string.Empty)
            {
                Password.Text = App.Locator.Login.Password;
            }
        }

        /// <summary>
        /// Connect the user
        /// </summary>
        private async void ConnectClick(object sender, EventArgs e)
        {
            // Send to DataService info of entered email and password
            App.Locator.Login.Email = Email.Text;
            App.Locator.Login.Password = Password.Text;

            if (!Email.Text.IsEmail())
            {
                Email.Error = Resources.GetString(Resource.String.createAccount_errorEmail);
                return;
            }

            // Hide Keyboard
            using (var imm = (Android.Views.InputMethods.InputMethodManager)GetSystemService(InputMethodService))
            {
                imm.HideSoftInputFromWindow(Connect.WindowToken, 0);
            }

            LoadingLayout.Visibility = ViewStates.Visible;

            await App.Locator.Login.Connect(DeviceInfoHelper.DeviceModel
                , DeviceInfoHelper.Platform
                , DeviceInfoHelper.Version
                , DeviceInfoHelper.GetDeviceUniqueId(this)
                , DeviceInfoHelper.CountryCode);

            LoadingLayout.Visibility = ViewStates.Invisible;
        }

        /// <summary>
        /// Button pour créer un compte
        /// </summary>
        private void CreateAccountClick(object sender, EventArgs e)
        {
            if (!EmailEditText.Text.IsEmail())
            {
                EmailEditText.Error = Resources.GetString(Resource.String.createAccount_errorEmail);
                return;
            }
            ShowCGUPopup();
        }

        private void ShowCGUPopup()
        {
            if (_popupCGU == null)
            {
                var refreshInProgressBuilder = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog);
                var inflater = (LayoutInflater)GetSystemService(LayoutInflaterService);
                var view = inflater.Inflate(Resource.Drawable.CGUPopup, null);

                //get the different elements
                //_stepInformationTextView = view.FindViewById<TextView>(Resource.Id.popupRefreshInProgress_stepText);
                //_titleTextView = view.FindViewById<TextView>(Resource.Id.popupRefreshinProgress_title);
                _linkToCGUTextView = view.FindViewById<TextView>(Resource.Id.popupcgu_linkwebcgu);

                _linkToCGUTextView.Click += OnGoToCGUURLButtonClick;

                //Android.Text.ISpanned cgu = Android.Text.Html.FromHtml("");
                //_stepInformationTextView.TextFormatted = cgu;
                //_stepInformationTextView.MovementMethod = new Android.Text.Method.ScrollingMovementMethod();

                //_stepInformationTextView.Text = Resources.GetString(Resource.String.map_TextStep3);

                //Cancel button
                refreshInProgressBuilder.SetNegativeButton(Resource.String.popupcgu_buttonDontAgree, (senderAlert, args) =>
                {
                    if (_popupCGU != null) { _popupCGU.Dismiss(); }
                });
                refreshInProgressBuilder.SetPositiveButton(Resource.String.popupcgu_buttonAgree, (senderAlert, args) =>
                {
                    App.Locator.Login.CreateAccount(FirstNameEditText.Text
                            , LastNameEditText.Text
                            , DeviceInfoHelper.DeviceModel
                            , DeviceInfoHelper.Platform
                            , DeviceInfoHelper.Version
                            , DeviceInfoHelper.GetDeviceUniqueId(this)
                            , DeviceInfoHelper.CountryCode).ContinueWith(c => { _popupCGU?.Dismiss(); });
                });
                refreshInProgressBuilder.SetView(view);
                _popupCGU = refreshInProgressBuilder.Create();
                _popupCGU.DismissEvent += _popupRefresh_DismissEvent;
            }
            _popupCGU.Show();
        }

        private void OnGoToCGUURLButtonClick(object sender, EventArgs e)
        {
            GoToCGUURL();
        }

        private void GoToCGUURL()
        {
            Android.Net.Uri uri = Android.Net.Uri.Parse("https://seekios.com/Home/CGU");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void _popupRefresh_DismissEvent(object sender, EventArgs e)
        {
            if (_popupCGU != null)
            {
                _linkToCGUTextView.Click -= OnGoToCGUURLButtonClick;
                _popupCGU.Dismiss();
                _popupCGU.Dispose();
                _popupCGU = null;
            }
        }

        /// <summary>
        /// Button Oublier mot de passe
        /// </summary>
        private void ForgetPasswordClick(object sender, EventArgs e)
        {
            if (!ForgetPasswordEditText.Text.IsEmail())
            {
                ForgetPasswordEditText.Error = Resources.GetString(Resource.String.createAccount_errorEmail);
                return;
            }
            RunOnUiThread(async () =>
            {
                //Hide Keyboard
                Android.Views.InputMethods.InputMethodManager imm = (Android.Views.InputMethods.InputMethodManager)GetSystemService(InputMethodService);
                imm.HideSoftInputFromWindow(Connect.WindowToken, 0);

                LoadingLayout.Visibility = ViewStates.Visible;
                await App.Locator.Login.ForgetPassword(ForgetPasswordEditText.Text);
                LoadingLayout.Visibility = ViewStates.Gone;
            });
        }

        /// <summary>
        /// Button pour aller sur la page Creation de compte
        /// </summary>
        private void HideLoginLayoutToCreateAccountClick(object sender, EventArgs e)
        {
            CreateAccountLayout.Visibility = ViewStates.Visible;
            LoginLinearLayout.Visibility = ViewStates.Gone;
            ForgetPasswordLinearLayout.Visibility = ViewStates.Gone;
            ReturnButtonLayout.Visibility = ViewStates.Visible;
            GoToForgetPasswordLayout.Visibility = ViewStates.Gone;
            GoToCreateAccountLayout.Visibility = ViewStates.Gone;
            GoToWhatIsSeekiosLayout.Visibility = ViewStates.Gone;
        }

        /// <summary>
        /// Button pour aller sur la page Mot de passe oublié
        /// </summary>
        private void HideLoginLayoutToForgetPasswordClick(object sender, EventArgs e)
        {
            CreateAccountLayout.Visibility = ViewStates.Gone;
            LoginLinearLayout.Visibility = ViewStates.Gone;
            ForgetPasswordLinearLayout.Visibility = ViewStates.Visible;
            ReturnButtonLayout.Visibility = ViewStates.Visible;
            GoToForgetPasswordLayout.Visibility = ViewStates.Gone;
            GoToCreateAccountLayout.Visibility = ViewStates.Gone;
            GoToWhatIsSeekiosLayout.Visibility = ViewStates.Gone;
        }

        /// <summary>
        /// Button pour revenir à la vue principale
        /// </summary>
        private void ReturnButtonClick(object sender, EventArgs e)
        {
            CreateAccountLayout.Visibility = ViewStates.Gone;
            LoginLinearLayout.Visibility = ViewStates.Visible;
            ForgetPasswordLinearLayout.Visibility = ViewStates.Gone;
            ReturnButtonLayout.Visibility = ViewStates.Gone;
            GoToForgetPasswordLayout.Visibility = ViewStates.Visible;
            GoToCreateAccountLayout.Visibility = ViewStates.Visible;
            GoToWhatIsSeekiosLayout.Visibility = ViewStates.Visible;
        }

        /// <summary>
        /// Evenement lorsque l'Edit Text de l'oublie du mot de passe change
        /// </summary>
        private void OnForgetEmailTextChanged(object sender, EventArgs e)
        {
            App.Locator.Login.ForgetPasswordEmail = ForgetPasswordEditText.Text;
        }

        /// <summary>
        /// Evenement lorsque l'Edit Text de l'email de la page Login change
        /// </summary>
        private void OnLoginEmailTextChanged(object sender, EventArgs e)
        {
            App.Locator.Login.Email = Email.Text;
        }

        /// <summary>
        /// Evenement lorsque l'Edit Text du password de la page Login change
        /// </summary>
        private void OnLoginPasswordTextChanged(object sender, EventArgs e)
        {
            App.Locator.Login.Password = Password.Text;
        }

        /// <summary>
        /// Evenement lorsque l'Edit Text du first name change
        /// </summary>
        private void OnUserFirstNameTextChanged(object sender, EventArgs e)
        {
            App.Locator.Login.UserFirstName = FirstNameEditText.Text;
        }

        /// <summary>
        /// Evenement lorsque l'Edit Text du last name change
        /// </summary>
        private void OnUserLastNameTextChanged(object sender, EventArgs e)
        {
            App.Locator.Login.UserLastName = LastNameEditText.Text;
        }

        /// <summary>
        /// Evenement lorsque l'Edit Text de l'email de la page CreateAccount change
        /// </summary>
        private void OnUserEmailTextChanged(object sender, EventArgs e)
        {
            App.Locator.Login.UserEmail = EmailEditText.Text;
        }

        /// <summary>
        /// Evenement lorsque l'Edit Text du password de la page CreateAccount change
        /// </summary>
        private void OnPasswordTextChanged(object sender, EventArgs e)
        {
            App.Locator.Login.UserPassword = PasswordEditText.Text;
            if (PasswordEditText.Text.Length < 8)
            {
                PasswordEditText.SetError(Resources.GetString(Resource.String.login_password_error), null);
                if (timer != null) timer.Cancel();
            }
            else
            {
                PasswordEditText.SetError(string.Empty, null);
                switch ((int)PasswordComplexityHelper.CheckStrength(PasswordEditText.Text))
                {
                    case (int)PasswordComplexityHelper.PasswordScore.Weak:
                        var spanString0 = Resources.GetString(Resource.String.login_password_weak);
                        var spanText0 = string.Format(spanString0, Resources.GetString(Resource.String.login_weak));

                        var resultTuple0 = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfStringInString(spanString0, Resources.GetString(Resource.String.login_weak));
                        var formattedinfoText0 = new SpannableString(spanText0);
                        formattedinfoText0.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.Red)), resultTuple0.Item1, resultTuple0.Item2, 0);

                        PasswordEditText.SetError(formattedinfoText0, null);
                        break;
                    case (int)PasswordComplexityHelper.PasswordScore.Medium:

                        var spanString = Resources.GetString(Resource.String.login_password_medium);
                        var spanText = string.Format(spanString, Resources.GetString(Resource.String.login_medium));

                        var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfStringInString(spanString, Resources.GetString(Resource.String.login_medium));
                        var formattedinfoText = new SpannableString(spanText);
                        formattedinfoText.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.Orange)), resultTuple.Item1, resultTuple.Item2, 0);

                        PasswordEditText.SetError(formattedinfoText, null);
                        break;
                    case (int)PasswordComplexityHelper.PasswordScore.Strong:
                    case (int)PasswordComplexityHelper.PasswordScore.VeryStrong:

                        var spanString1 = Resources.GetString(Resource.String.login_password_strong);
                        var spanText1 = string.Format(spanString1, Resources.GetString(Resource.String.login_strong));

                        var resultTuple1 = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfStringInString(spanString1, Resources.GetString(Resource.String.login_strong));
                        var formattedinfoText1 = new SpannableString(spanText1);
                        formattedinfoText1.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.MainColor)), resultTuple1.Item1, resultTuple1.Item2, 0);

                        PasswordEditText.SetError(formattedinfoText1, null);
                        break;
                }
                LaunchTimerToHideError();
            }
            if (string.IsNullOrEmpty(PasswordEditText.Text)) PasswordEditText.Error = null;
        }


        private void LaunchTimerToHideError()
        {
            if (timer == null) timer = new MyTimer(2000, 2000, PasswordEditText);
            timer.Cancel();
            timer.Start();
        }

        private void PasswordEditText_FocusChange(object sender, Android.Views.View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus && timer != null) timer.Cancel();
        }

        /// <summary>
        /// Evenement lorsque l'Edit Text de la confirmation du password de la page CreateAccount change
        /// </summary>
        private void OnConfirmedPasswordTextChanged(object sender, EventArgs e)
        {
            App.Locator.Login.UserConfirmedPassword = ConfirmedPasswordEditText.Text;
        }

        /// <summary>
        /// UpdateUI when connection state changed
        /// </summary>
        private void UpdateUIOnConnectionStateChanged(bool isConnected)
        {
            if (null != Connect) Connect.Enabled = isConnected;
            //if (null != ConnectFacebook) ConnectFacebook.Enabled = isConnected;
            if (null != CreateAccountButton) CreateAccountButton.Enabled = isConnected;
            if (null != ForgetPasswordButton) ForgetPasswordButton.Enabled = isConnected;
        }

        /// <summary>
        /// What Is Seekios action
        /// </summary>
        private void GoToWhatIsSeekiosLayoutClick(object sender, EventArgs e)
        {
            Android.Net.Uri uri = Android.Net.Uri.Parse("https://seekios.com/");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        #endregion

        #region ===== Facebook Login ==============================================================

        ///// <summary>
        ///// Initialisae the permission for Facebook, and register the call back
        ///// </summary>
        //private void InitialiseFacebook()
        //{
        //    ConnectFacebook.SetReadPermissions(new List<string> { "public_profile", "user_friends", "email" });
        //    mCallBackManager = CallbackManagerFactory.Create();
        //    ConnectFacebook.RegisterCallback(mCallBackManager, this);
        //}

        ///// <summary>
        ///// Call with the GraphRequest from the OnSuccess and allow to Connect to the App with Facebook Account
        ///// </summary>
        ///// <param name="user"></param>
        ///// <param name="graphResponse"></param>
        //public async void OnCompleted(JSONObject user, GraphResponse graphResponse)
        //{
        //    if (user != null)
        //    {
        //        ConnectFacebook.Enabled = false;

        //        FacebookResult facebookResult = JsonConvert.DeserializeObject<FacebookResult>(user.ToString());

        //        var email = facebookResult.Email;
        //        var firstname = facebookResult.FirstName;
        //        var lastname = facebookResult.LastName;
        //        var id = facebookResult.Id;

        //        Bitmap bitmap = null;
        //        string picture = string.Empty;
        //        var t = new Thread(new ThreadStart(() =>
        //        {
        //            bitmap = GetFacebookBitmap(id);

        //            using (var stream = new System.IO.MemoryStream())
        //            {
        //                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);

        //                var bytes = stream.ToArray();
        //                picture = Convert.ToBase64String(bytes);
        //            }
        //        }));
        //        t.Start();

        //        // TODO : change the process to wait the thread complete
        //        while (bitmap == null) { Thread.Sleep(500); }
        //        bitmap.Dispose();

        //        if (!string.IsNullOrEmpty(email))
        //        {
        //            try
        //            {
        //                //var dataService = (ServiceLocator.Current.GetInstance<IDataService>() as DataService);
        //                //PackageInfo packageInfo = PackageManager.GetPackageInfo(PackageName, 0);
        //                //string version = packageInfo.VersionName;

        //                //if (await dataService.IsSeekiosVersionApplicationNeedForceUpdate(version, ((int)PlateformeVersionEnum.Android).ToString()) == 1)
        //                //{
        //                //    App.Locator.Login.GoToNeedUpdate();
        //                //}
        //                //else
        //                //{
        //                // This method allows to connect to Facebook with email, password, first_name and last_name
        //                //if (!await App.Locator.Login.ConnectWithFacebook(email
        //                //    , firstname
        //                //    , lastname
        //                //    , id
        //                //    , picture
        //                //    , DeviceInfoHelper.DeviceModel
        //                //    , DeviceInfoHelper.Platform
        //                //    , DeviceInfoHelper.Version
        //                //    , DeviceInfoHelper.GetDeviceUniqueId(this)
        //                //    , DeviceInfoHelper.GetDeviceIp(this)
        //                //    , RegistrationIntentService.GCMRegistrationToken))
        //                //{
        //                //    PopupErrorFacebook();
        //                //}
        //                //}
        //            }
        //            catch (TimeoutException)
        //            {
        //                PopupErrorFacebook();
        //            }
        //        }
        //        else
        //        {
        //            PopupErrorFacebook();
        //        }
        //    }
        //    else
        //    {
        //        PopupErrorFacebook();
        //    }
        //}

        ///// <summary>
        ///// Pass on this method when you want to login with Facebook
        ///// This class represents an immutable access token for using Facebook APIs
        ///// </summary>
        ///// <param name="result"></param>
        //public void OnSuccess(Java.Lang.Object result)
        //{
        //    LoginResult loginResult = result as LoginResult;
        //    if (loginResult != null)
        //    {
        //        GraphRequest request = GraphRequest.NewMeRequest(loginResult.AccessToken, this);

        //        Bundle parameters = new Bundle();
        //        parameters.PutString("fields", "id,last_name,first_name,email");
        //        request.Parameters = parameters;
        //        request.ExecuteAsync();
        //    }
        //    Console.WriteLine(AccessToken.CurrentAccessToken.UserId);
        //}

        ///// <summary>
        ///// Cancel from the IFacebookCallback
        ///// </summary>
        //void IFacebookCallback.OnCancel()
        //{
        //    PopupErrorFacebook();
        //    Console.WriteLine("onCancel");
        //}

        ///// <summary>
        ///// Error from the IFacebookCallback
        ///// </summary>
        //void IFacebookCallback.OnError(FacebookException p0)
        //{
        //    PopupErrorFacebook();
        //    Console.WriteLine("onError");
        //    Log.Verbose("LoginActivity", p0.Cause == null ? "" : p0.Cause.ToString());
        //}

        ///// <summary>
        ///// From the IFacebookCallBack to call the OnActivityResult from the CallBackManager
        ///// </summary>
        ///// <param name="requestCode"></param>
        ///// <param name="resultCode"></param>
        ///// <param name="data"></param>
        //protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        //{
        //    base.OnActivityResult(requestCode, resultCode, data);
        //    mCallBackManager.OnActivityResult(requestCode, (int)resultCode, data);
        //}

        ///// <summary>
        ///// Get the Facebook User Id to get the URL for the User's picture
        ///// </summary>
        ///// <param name="userID"></param>
        ///// <returns></returns>
        //private Bitmap GetFacebookBitmap(string userID)
        //{
        //    URL imageURL = new URL("https://graph.facebook.com/" + userID + "/picture?type=large");
        //    return BitmapFactory.DecodeStream(imageURL.OpenConnection().InputStream);
        //}

        ///// <summary>
        ///// Popup which displays when there is an error with Facebook Connection
        ///// </summary>
        //private void PopupErrorFacebook()
        //{
        //    AlertDialog.Builder facebookAlertDialogBuilder = new AlertDialog.Builder(this);
        //    AlertDialog facebookAlertDialog = facebookAlertDialogBuilder.Create();
        //    facebookAlertDialog.SetTitle("Erreur avec la connexion Facebook");
        //    facebookAlertDialog.SetMessage("Veuillez réessayer de vous connecter");
        //    facebookAlertDialog.SetButton("OK", (senderAlert, args) =>
        //    {
        //        facebookAlertDialog.Cancel();
        //    });
        //    LoginManager.Instance.LogOut();
        //    facebookAlertDialog.Show();

        //    LoginManager.Instance.LogOut();
        //}

        #endregion
    }
}