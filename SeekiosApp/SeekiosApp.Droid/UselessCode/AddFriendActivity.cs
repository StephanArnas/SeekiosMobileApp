//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using SeekiosApp.Enum;
//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using SeekiosApp.Droid.Helper;
//using SeekiosApp.Droid.CustomComponents;
//using XamSvg;
//using SeekiosApp.Droid.Services;
//using SeekiosApp.Model.DTO;
//using System.Threading.Tasks;
//using SeekiosApp.Droid.CustomComponents.Adapter;
//using static Android.Views.View;

//namespace SeekiosApp.Droid.View
//{
//    //[Activity(Theme = "@style/Theme.Community")]
//    //public class AddFriendActivity : AppCompatActivityBase
//    //{

//    //    #region ===== Attributs ===================================================================

//    //    /// <summary>Utilisateur sélectionné dans la liste</summary>
//    //    private ShortUserDTO _selectedUser;

//    //    ///<summary>Option de recherche sélectionnée</summary>
//    //    private Enum.SearchOptionEnum _optionSelected = SearchOptionEnum.ByName; // par défaut c'est l'option Byname qui est sélectionnée

//    //    /// <summary>List de string représentants le nom et le prénom de chaque user retrouvé lors de la recherche d'un ami</summary>
//    //    private List<ShortUserDTO> _userList = null;

//    //    ///<summary></summary>
//    //    private AddFriendAdapter _listAdapter = null;

//    //    /// <summary>Timer for the research : we start a research only if the user didn't enter more text after 1 second</summary>
//    //    private System.Timers.Timer _timer;

//    //    /// <summary></summary>
//    //    private int _countSeconds;

//    //    /// <summary></summary>
//    //    private string _oldText = string.Empty;

//    //    #endregion

//    //    #region ===== Propriétés ==================================================================

//    //    /// <summary>TextView qui sert à afficher les contacts par autocomplete</summary>
//    //    public AutoCompleteTextView AutoCompleteTextView { get; set; }

//    //    /// <summary>TextView pour ajouter un ami par email</summary>
//    //    public RadioGroup SearchOptionRadioGroup { get; set; }

//    //    /// <summary>ListView to display search results</summary>
//    //    public ListView SearchListView { get; set; }

//    //    /// <summary>Layout to display when the listview is empty</summary>
//    //    public LinearLayout EmptySearchLayout { get; set; }

//    //    /// <summary>empty list text</summary>
//    //    public TextView EmptyListTextView { get; set; }

//    //    #endregion

//    //    #region ===== Cycle De Vie ================================================================

//    //    /// <summary>
//    //    /// Création de la page
//    //    /// </summary>
//    //    protected override void OnCreate(Bundle bundle)
//    //    {
//    //        SetContentView(Resource.Layout.AddFriendLayout);
//    //        base.OnCreate(bundle); //, Resource.Layout.AddFriendLayout, true);

//    //        GetObjectsFromView();
//    //        SetDataToView();

//    //        if (ToolbarPage != null)
//    //        {
//    //            SetSupportActionBar(ToolbarPage);
//    //            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
//    //        }
//    //    }

//    //    /// <summary>
//    //    /// Reprise de la page
//    //    /// </summary>
//    //    protected override void OnResume()
//    //    {
//    //        AutoCompleteTextView.AfterTextChanged += OnAutoCompleteTextViewAfterTextChanged;
//    //        SearchOptionRadioGroup.CheckedChange += OnSearchOptionRadioGroupCheckedChange;
//    //        App.Locator.AddFriend.PropertyChanged += OnPictureLoaded;
//    //        AutoCompleteTextView.KeyPress += AutoCompleteTextView_KeyPress;
//    //        ClearListView();
//    //        base.OnResume();
//    //    }


//    //    /// <summary>
//    //    /// Suspension de la page
//    //    /// </summary>
//    //    protected override void OnPause()
//    //    {
//    //        AutoCompleteTextView.AfterTextChanged -= OnAutoCompleteTextViewAfterTextChanged;
//    //        SearchOptionRadioGroup.CheckedChange -= OnSearchOptionRadioGroupCheckedChange;
//    //        App.Locator.AddFriend.PropertyChanged -= OnPictureLoaded;
//    //        AutoCompleteTextView.KeyPress -= AutoCompleteTextView_KeyPress;
//    //        base.OnPause();
//    //    }

//    //    /// <summary>
//    //    /// Suppression de la page
//    //    /// </summary>
//    //    protected override void OnDestroy()
//    //    {
//    //        base.OnDestroy();
//    //    }

//    //    #endregion

//    //    #region ===== ActionBAr ===================================================================

//    //    public override bool OnOptionsItemSelected(IMenuItem item)
//    //    {
//    //        Finish();
//    //        return base.OnOptionsItemSelected(item);
//    //    }

//    //    #endregion

//    //    #region ===== Initialisation Vue ==========================================================

//    //    /// <summary>
//    //    /// Initialise les objets de la vue
//    //    /// </summary>
//    //    private void GetObjectsFromView()
//    //    {
//    //        AutoCompleteTextView = FindViewById<AutoCompleteTextView>(Resource.Id.addFriend_autoCompleteTextView);
//    //        SearchOptionRadioGroup = FindViewById<RadioGroup>(Resource.Id.addFriend_radioGroup);
//    //        ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
//    //        SearchListView = FindViewById<ListView>(Resource.Id.addFriend_listView);
//    //        EmptySearchLayout = FindViewById<LinearLayout>(Resource.Id.addFriend_emptyLayout);
//    //        EmptyListTextView = FindViewById<TextView>(Resource.Id.emptyList_text);
//    //    }

//    //    /// <summary>
//    //    /// Initialise la vue
//    //    /// </summary>
//    //    private void SetDataToView()
//    //    {
//    //        ToolbarPage.SetTitle(Resource.String.addFriend_title);
//    //        ToolbarPage.SetBackgroundColor(Resources.GetColor(Resource.Color.communityDarkBlue));
//    //        AutoCompleteTextView.SetTextColor(Resources.GetColor(Resource.Color.textColorContent));
//    //        AutoCompleteTextView.SetCompletionHint("Search");

//    //        InitialiseListView();
//    //    }

//    //    /// <summary>
//    //    /// Initialise the ListView
//    //    /// </summary>
//    //    private void InitialiseListView()
//    //    {
//    //        _listAdapter = new AddFriendAdapter(this);
//    //        SearchListView.EmptyView = EmptySearchLayout;
//    //        SearchListView.Adapter = _listAdapter;
//    //        SearchListView.ChoiceMode = ChoiceMode.Single;
//    //        SearchListView.ItemsCanFocus = true;
//    //    }

//    //    #endregion

//    //    #region ===== Méthodes publiques ==========================================================

//    //    #endregion

//    //    #region ===== Méthodes privées ============================================================

//    //    /// <summary>
//    //    /// Clear the listview if no results are found
//    //    /// </summary>
//    //    private void ClearListView()
//    //    {
//    //        //clear the listview if no results are found
//    //        App.Locator.AddFriend.FriendSearchList.Clear();
//    //        EmptyListTextView.Text = Resources.GetString(Resource.String.addFriend_emptySearch);
//    //        _listAdapter.NotifyDataSetChanged();
//    //    }

//    //    #endregion

//    //    #region ===== Evènements ==================================================================

//    //    /// <summary>
//    //    /// Evenement lorsque le text change dans le textview de recherche
//    //    /// </summary>
//    //    /// <param name="sender"></param>
//    //    /// <param name="e"></param>
//    //    private void OnAutoCompleteTextViewAfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
//    //    {
//    //        _timer = new System.Timers.Timer();
//    //        //Trigger event every 1 seconds
//    //        _timer.Interval = 1500;
//    //        _timer.Elapsed += OnTimedEvent;
//    //        //count down 2 seconds : un seul tick d'une seconde
//    //        _countSeconds = 2;

//    //        _timer.Enabled = true;
//    //        _timer.AutoReset = false;
//    //    }

//    //    /// <summary>
//    //    /// Key pressed handler
//    //    /// </summary>
//    //    /// <param name="sender"></param>
//    //    /// <param name="e"></param>
//    //    private void AutoCompleteTextView_KeyPress(object sender, KeyEventArgs e)
//    //    {
//    //        if (e.KeyCode == Keycode.Enter || e.KeyCode == Keycode.NumpadEnter)
//    //        {
//    //            Android.Views.InputMethods.InputMethodManager imm = (Android.Views.InputMethods.InputMethodManager)GetSystemService(InputMethodService);
//    //            imm.HideSoftInputFromWindow(AutoCompleteTextView.WindowToken, 0);
//    //        }
//    //        else e.Handled = false; 
//    //    }

//    //    /// <summary>
//    //    /// Change la valeur de l'option sélectionnée quand on change l'att des radio button
//    //    /// </summary>
//    //    /// <param name="sender"></param>
//    //    /// <param name="e"></param>
//    //    private void OnSearchOptionRadioGroupCheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
//    //    {
//    //        switch (e.CheckedId)
//    //        {
//    //            default:
//    //                break;
//    //            case Resource.Id.addFriend_nameRadioButton:
//    //                _optionSelected = SearchOptionEnum.ByName;
//    //                break;
//    //            case Resource.Id.addFriend_emailRadioButton:
//    //                _optionSelected = SearchOptionEnum.ByEmail;
//    //                break;
//    //            case Resource.Id.addFriend_phoneRadioButton:
//    //                _optionSelected = SearchOptionEnum.ByPhone;
//    //                break;
//    //        }
//    //        AutoCompleteTextView.Text = string.Empty;
//    //        ClearListView();
//    //    }

//    //    /// <summary>
//    //    /// Event fired when a picture is loaded
//    //    /// </summary>
//    //    /// <param name="sender"></param>
//    //    /// <param name="e"></param>
//    //    private void OnPictureLoaded(object sender, System.ComponentModel.PropertyChangedEventArgs e)
//    //    {
//    //        if (e.PropertyName == "PictureLoaded")
//    //            _listAdapter.NotifyDataSetChanged();
//    //        if (e.PropertyName == "RequestSent")
//    //        {
//    //            Toast.MakeText(this, Resource.String.addFriend_requestSent, ToastLength.Short).Show();
//    //            _listAdapter.NotifyDataSetChanged();
//    //        }
//    //        if (e.PropertyName == "RequestNotSent")
//    //            Toast.MakeText(this, Resource.String.addFriend_requestNotSent, ToastLength.Short).Show();
//    //    }

//    //    /// <summary>
//    //    /// Event fired every 1.5 seconds when entering text in the AutoCompleteTextView
//    //    /// </summary>
//    //    /// <param name="sender"></param>
//    //    /// <param name="e"></param>
//    //    private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
//    //    {
//    //        while (_countSeconds > 0)
//    //        {
//    //            _countSeconds--;
//    //            if (_countSeconds < 1)
//    //            {
//    //                //Update visual representation here
//    //                //Remember to do it on UI thread
//    //                RunOnUiThread(async () =>
//    //                {
//    //                    var text = AutoCompleteTextView.Text;
//    //                    // si on a au moins 3 caratères dans la recherche
//    //                    if (AutoCompleteTextView.Text.Length > 2)
//    //                    {
//    //                        switch (_optionSelected)
//    //                        {
//    //                            default:
//    //                                break;
//    //                            case SearchOptionEnum.ByName:
//    //                                var result = await App.Locator.AddFriend.GetUsersByNameSearch(AutoCompleteTextView.Text);
//    //                                if (result.Count > 0)
//    //                                {
//    //                                    App.Locator.AddFriend.FriendSearchList = result;
//    //                                    App.Locator.AddFriend.GetPictureByUser();
//    //                                    _listAdapter.NotifyDataSetChanged();
//    //                                }
//    //                                else
//    //                                {
//    //                                    if (SearchListView.Count > 0) { ClearListView(); }
//    //                                    EmptyListTextView.Text = Resources.GetString(Resource.String.addFriend_searchNoResult);
//    //                                }
//    //                                break;
//    //                            case SearchOptionEnum.ByEmail:
//    //                                var emailresult = await App.Locator.AddFriend.GetUserByEmail(AutoCompleteTextView.Text);
//    //                                if (emailresult != null)
//    //                                {
//    //                                    ClearListView();
//    //                                    App.Locator.AddFriend.FriendSearchList.Add(emailresult);
//    //                                    App.Locator.AddFriend.GetPictureByUser();
//    //                                    _listAdapter.NotifyDataSetChanged();
//    //                                }
//    //                                else ClearListView();
//    //                                break;
//    //                            case SearchOptionEnum.ByPhone:
//    //                                /*var phoneResult = await App.Locator.AddFriend.GetUserByPhone(AutoCompleteTextView.Text);
//    //                                if (phoneResult != null)
//    //                                {
//    //                                    ClearListView();
//    //                                    App.Locator.AddFriend.FriendSearchList.Add(phoneResult);
//    //                                    App.Locator.AddFriend.GetPictureByUser();
//    //                                    _listAdapter.NotifyDataSetChanged();
//    //                                }
//    //                                else ClearListView();*/
//    //                                break;
//    //                        }
//    //                    }
//    //                    // moins de 3 caractères, on nettoie la vue
//    //                    else
//    //                    {
//    //                        ClearListView();
//    //                    }
//    //                });
//    //            }
//    //        }
//    //        _timer.Stop();
//    //    }


//    //    #endregion
//    //}
//}