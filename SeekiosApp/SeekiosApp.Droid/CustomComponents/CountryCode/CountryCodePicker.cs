using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Graphics;
using Java.Util;

namespace SeekiosApp.Droid.CustomComponents.CountryCode
{
    public class CountryCodePicker : RelativeLayout
    {
        public int LANGUAGE_ARABIC = 1;
        public int LANGUAGE_BENGALI = 2;
        public int LANGUAGE_CHINESE = 3;
        public int LANGUAGE_ENGLISH = 4;
        public int LANGUAGE_FRENCH = 5;
        public int LANGUAGE_GERMAN = 6;
        public int LANGUAGE_GUJARATI = 7;
        public int LANGUAGE_HINDI = 8;
        public int LANGUAGE_JAPANESE = 9;
        public int LANGUAGE_JAVANESE = 10;
        public int LANGUAGE_PORTUGUESE = 11;
        public int LANGUAGE_RUSSIAN = 12;
        public int LANGUAGE_SPANISH = 13;

        static string TAG = "CCP";
        static int LIB_DEFAULT_COUNTRY_CODE = 91;
        int defaultCountryCode;
        string defaultCountryNameCode;

        public Context context { get; set; }
        public Android.Views.View HolderView { get; set; }
        public LayoutInflater mInflater { get; set; }
        public TextView SelectedCountryTextView { get; set; }
        public EditText RegisteredCarrierNumberEditText { get; set; }
        public RelativeLayout HolderLayout { get; set; }
        public ImageView ArrowImageView { get; set; }
        public Country SelectedCountry { get; set; }
        public Country DefaultCountry { get; set; }
        public CountryCodePicker CodePicker { get; set; }
        bool hideNameCode = false;
        public List<Country> PreferredCountries { get; set; }
        string countryPreference;
        public List<Country> CustomMasterCountriesList { get; set; }
        string CustomMasterCountries { get; set; }
        public Language CustomLanguage = Language.ENGLISH;
        public bool KeyboardAutoPopOnSearch { get; set; }

        public event EventHandler CountryCodeChanged;

        public CountryCodePicker(Context context) : base(context)
        {
            KeyboardAutoPopOnSearch = true;
            this.context = context;
            Init(null);
        }

        public CountryCodePicker(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            KeyboardAutoPopOnSearch = true;
            this.context = context;
            Init(attrs);
        }

        public CountryCodePicker(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            KeyboardAutoPopOnSearch = true;
            this.context = context;
            Init(attrs);
        }

        private void Init(IAttributeSet attrs)
        {
            Log.Debug(TAG, "Initialization of CCP");
            mInflater = LayoutInflater.From(context);
            HolderView = mInflater.Inflate(Resource.Layout.LayoutCodePicker, this, true);
            SelectedCountryTextView = (TextView)HolderView.FindViewById(Resource.Id.textView_selectedCountry);
            HolderLayout = (RelativeLayout)HolderView.FindViewById(Resource.Id.relative_countryCodeHolder);
            ArrowImageView = (ImageView)HolderView.FindViewById(Resource.Id.imageView_arrow);
            CodePicker = this;

            ApplyCustomProperty(attrs);
            HolderLayout.Click += (o, e) =>
            {
                CountryCodeDialog.OpenCountryCodeDialog(CodePicker);
            };
        }

        private void ApplyCustomProperty(IAttributeSet attrs)
        {
            Log.Debug(TAG, "Applying custom property");
            var a = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.CountryCodePicker, 0, 0);
            try
            {
                // set hide nameCode. If someone wants only phone code to avoid name collision for same country phone code.
                hideNameCode = a.GetBoolean(Resource.Styleable.CountryCodePicker_hideNameCode, false);

                // set autopop keyboard
                KeyboardAutoPopOnSearch = (a.GetBoolean(Resource.Styleable.CountryCodePicker_keyboardAutoPopOnSearch, true));

                // set custom language is specified, then set it as custom
                int attrLanguage = LANGUAGE_ENGLISH;
                if (a.HasValue(Resource.Styleable.CountryCodePicker_ccpLanguage))
                {
                    attrLanguage = a.GetInt(Resource.Styleable.CountryCodePicker_ccpLanguage, 1);
                }
                CustomLanguage = GetLanguageEnum(attrLanguage);

                // set custom master list
                CustomMasterCountries = a.GetString(Resource.Styleable.CountryCodePicker_customMasterCountries);
                RefreshCustomMasterList();

                // set preference
                countryPreference = a.GetString(Resource.Styleable.CountryCodePicker_countryPreference);
                RefreshPreferredCountries();

                // set default country
                var lang = Locale.Default.DisplayLanguage;
                defaultCountryNameCode = a.GetString(Resource.Styleable.CountryCodePicker_defaultNameCode);
                bool usingNameCode = false;
                if (defaultCountryNameCode != null && defaultCountryNameCode.Length != 0)
                {
                    if (Country.GetCountryForNameCodeFromLibraryMasterList(CustomLanguage, defaultCountryNameCode) != null)
                    {
                        usingNameCode = true;
                        DefaultCountry = Country.GetCountryForNameCodeFromLibraryMasterList(CustomLanguage, defaultCountryNameCode);
                        SetSelectedCountry(DefaultCountry);
                    }
                }

                // set default country is not set using name code.
                if (!usingNameCode)
                {
                    int defaultCountryCode = a.GetInteger(Resource.Styleable.CountryCodePicker_defaultCode, -1);

                    // if invalid country is set using xml, it will be replaced with LIB_DEFAULT_COUNTRY_CODE
                    if (Country.GetCountryForCode(CustomLanguage, PreferredCountries, defaultCountryCode) == null)
                    {
                        defaultCountryCode = LIB_DEFAULT_COUNTRY_CODE;
                    }
                    SetDefaultCountryUsingPhoneCode(defaultCountryCode);
                    SetSelectedCountry(DefaultCountry);
                }

                // set color
                SelectedCountryTextView.SetTextColor(Color.ParseColor("#666666"));
                ArrowImageView.SetColorFilter(Color.ParseColor("#666666"), PorterDuff.Mode.SrcIn);
                // set text size
                SelectedCountryTextView.SetTextSize(ComplexUnitType.Dip, 18);

                // set arrow arrow size is explicitly defined
                int arrowSize = a.GetDimensionPixelSize(Resource.Styleable.CountryCodePicker_arrowSize, 0);
                if (arrowSize > 0)
                {
                    SetArrowSize(arrowSize);
                }
            }
            catch (Exception e)
            {
                SelectedCountryTextView.Text = e.Message;
            }
            finally
            {
                a.Recycle();
            }
        }

        private void SetArrowSize(int arrowSize)
        {
            if (arrowSize > 0)
            {
                LayoutParams param = (LayoutParams)ArrowImageView.LayoutParameters;
                param.Width = arrowSize;
                param.Height = arrowSize;
                ArrowImageView.LayoutParameters = param;
            }
        }

        public void SetCountryForPhoneCode(int countryCode)
        {
            Country country = Country.GetCountryForCode(CustomLanguage, PreferredCountries, countryCode);
            if (country == null)
            {
                if (DefaultCountry == null)
                {
                    DefaultCountry = Country.GetCountryForCode(CustomLanguage, PreferredCountries, defaultCountryCode);
                }
                SetSelectedCountry(DefaultCountry);
            }
            else
            {
                SetSelectedCountry(country);
            }
        }

        public void RefreshPreferredCountries()
        {
            if (countryPreference == null || countryPreference.Length == 0)
            {
                PreferredCountries = null;
            }
            else
            {
                List<Country> localCountryList = new List<Country>();
                foreach (var nameCode in countryPreference.Split(','))
                {
                    Country country = Country.GetCountryForNameCodeFromCustomMasterList(CustomMasterCountriesList, CustomLanguage, nameCode);
                    if (country != null)
                    {
                        if (!IsAlreadyInList(country, localCountryList))
                        { //to avoid duplicate entry of country
                            localCountryList.Add(country);
                        }
                    }
                }

                if (localCountryList.Count == 0)
                {
                    PreferredCountries = null;
                }
                else
                {
                    PreferredCountries = localCountryList;
                }
            }
        }

        public void RefreshCustomMasterList()
        {
            if (CustomMasterCountries == null || CustomMasterCountries.Count() == 0)
            {
                CustomMasterCountriesList = null;
            }
            else
            {
                List<Country> localCountryList = new List<Country>();
                foreach (var nameCode in CustomMasterCountries.Split(','))
                {
                    Country country = Country.GetCountryForNameCodeFromLibraryMasterList(CustomLanguage, nameCode);
                    if (country != null)
                    {
                        if (!IsAlreadyInList(country, localCountryList))
                        { //to avoid duplicate entry of country
                            localCountryList.Add(country);
                        }
                    }
                }

                if (localCountryList.Count == 0)
                {
                    CustomMasterCountriesList = null;
                }
                else
                {
                    CustomMasterCountriesList = localCountryList;
                }
            }
        }

        private bool IsAlreadyInList(Country country, List<Country> countryList)
        {
            if (country != null && countryList != null)
            {
                foreach (var iterationCountry in countryList)
                {
                    if (iterationCountry.NameCode.Equals(country.NameCode, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private string SetectCarrierNumber(string fullNumber, Country country)
        {
            string carrierNumber;
            if (country == null || fullNumber == null)
            {
                carrierNumber = fullNumber;
            }
            else
            {
                int indexOfCode = fullNumber.IndexOf(country.PhoneCode);
                if (indexOfCode == -1)
                {
                    carrierNumber = fullNumber;
                }
                else
                {
                    carrierNumber = fullNumber.Substring(indexOfCode + country.PhoneCode.Count());
                }
            }
            return carrierNumber;
        }

        public void SetSelectedCountry(Country selectedCountry)
        {
            if (selectedCountry == null) return;
            SelectedCountry = selectedCountry;
            if (selectedCountry == null)
            {
                selectedCountry = Country.GetCountryForCode(CustomLanguage, PreferredCountries, defaultCountryCode);
            }
            if (!hideNameCode)
            {
                SelectedCountryTextView.Text = string.Format("({0}) +{1}", selectedCountry.NameCode.ToUpper(), selectedCountry.PhoneCode);
            }
            else
            {
                SelectedCountryTextView.Text = string.Format("({0})", selectedCountry.NameCode.ToUpper());
            }
            CountryCodeChanged?.Invoke(selectedCountry, EventArgs.Empty);
            Log.Debug(TAG, "Setting selected country:" + selectedCountry.Name);
        }

        public Language GetLanguageEnum(int language)
        {
            switch (language)
            {
                case 1:
                    return Language.ARABIC;
                case 2:
                    return Language.BENGALI;
                case 3:
                    return Language.CHINESE;
                case 4:
                    return Language.ENGLISH;
                case 5:
                    return Language.FRENCH;
                case 6:
                    return Language.GERMAN;
                case 7:
                    return Language.GUJARATI;
                case 8:
                    return Language.HINDI;
                case 9:
                    return Language.JAPANESE;
                case 10:
                    return Language.JAVANESE;
                case 11:
                    return Language.PORTUGUESE;
                case 12:
                    return Language.RUSSIAN;
                case 13:
                    return Language.SPANISH;
                default:
                    return Language.ENGLISH;
            }
        }

        public string GetSearchHintText()
        {
            switch (CustomLanguage)
            {
                case Language.ARABIC:
                    return "بحث";
                case Language.BENGALI:
                    return "অনুসন্ধান...";
                case Language.CHINESE:
                    return "搜索...";
                case Language.ENGLISH:
                    return "search...";
                case Language.FRENCH:
                    return "chercher ...";
                case Language.GERMAN:
                    return "Suche...";
                case Language.GUJARATI:
                    return "શોધ કરો ...";
                case Language.HINDI:
                    return "खोज करें ...";
                case Language.JAPANESE:
                    return "サーチ...";
                case Language.JAVANESE:
                    return "search ...";
                case Language.PORTUGUESE:
                    return "pesquisa ...";
                case Language.RUSSIAN:
                    return "поиск ...";
                case Language.SPANISH:
                    return "buscar ...";
                default:
                    return "Search...";
            }
        }

        public string GetDialogTitle()
        {
            switch (CustomLanguage)
            {
                case Language.ARABIC:
                    return "حدد الدولة";
                case Language.BENGALI:
                    return "দেশ নির্বাচন করুন";
                case Language.CHINESE:
                    return "选择国家";
                case Language.ENGLISH:
                    return "Select country";
                case Language.FRENCH:
                    return "Sélectionner le pays";
                case Language.GERMAN:
                    return "Land auswählen";
                case Language.GUJARATI:
                    return "દેશ પસંદ કરો";
                case Language.HINDI:
                    return "देश चुनिए";
                case Language.JAPANESE:
                    return "国を選択";
                case Language.JAVANESE:
                    return "Pilih negara";
                case Language.PORTUGUESE:
                    return "Selecione o pais";
                case Language.RUSSIAN:
                    return "Выберите страну";
                case Language.SPANISH:
                    return "Seleccionar país";
                default:
                    return "Select country";
            }
        }

        public string GetNoResultFoundText()
        {
            switch (CustomLanguage)
            {
                case Language.ARABIC:
                    return "يؤدي لم يتم العثور";
                case Language.BENGALI:
                    return "ফলাফল পাওয়া যায়নি";
                case Language.CHINESE:
                    return "结果未发现";
                case Language.ENGLISH:
                    return "result not found";
                case Language.FRENCH:
                    return "résulte pas trouvé";
                case Language.GERMAN:
                    return "Folge nicht gefunden";
                case Language.GUJARATI:
                    return "પરિણામ મળ્યું નથી";
                case Language.HINDI:
                    return "परिणाम नहीं मिला";
                case Language.JAPANESE:
                    return "結果として見つかりません。";
                case Language.JAVANESE:
                    return "kasil ora ketemu";
                case Language.PORTUGUESE:
                    return "resultar não encontrado";
                case Language.RUSSIAN:
                    return "результат не найден";
                case Language.SPANISH:
                    return "como resultado que no se encuentra";
                default:
                    return "No result found";
            }
        }

        public void SetDefaultCountryUsingPhoneCode(int defaultCountryCode)
        {
            Country defaultCountry = Country.GetCountryForCode(CustomLanguage, PreferredCountries, defaultCountryCode); //xml stores data in string format, but want to allow only numeric value to country code to user.
            if (defaultCountry == null)
            { //if no correct country is found
                Log.Debug(TAG, "No country for code " + defaultCountryCode + " is found");
            }
            else
            { //if correct country is found, set the country
                this.defaultCountryCode = defaultCountryCode;
                DefaultCountry = defaultCountry;
            }
        }

        public void SetDefaultCountryUsingNameCode(string defaultCountryNameCode)
        {
            Country defaultCountry = Country.GetCountryForNameCodeFromLibraryMasterList(CustomLanguage, defaultCountryNameCode); //xml stores data in string format, but want to allow only numeric value to country code to user.
            if (defaultCountry == null)
            { //if no correct country is found
                Log.Debug(TAG, "No country for nameCode " + defaultCountryNameCode + " is found");
            }
            else
            { //if correct country is found, set the country
                this.defaultCountryNameCode = defaultCountry.NameCode;
                DefaultCountry = defaultCountry;
            }
        }

        public int GetDefaultCountryCodeAsInt()
        {
            int code = 0;
            try
            {
                code = int.Parse(DefaultCountry.PhoneCode);
            }
            catch (Exception e)
            {

            }
            return 0;
        }

        //add an entry for your language in attrs.xml's <attr name="language" format="enum"> enum.
        //add getMasterListForLanguage() to Country.java
        //add here so that language can be set programmatically
        public enum Language
        {
            ARABIC, BENGALI, CHINESE, ENGLISH, FRENCH, GERMAN, GUJARATI, HINDI, JAPANESE, JAVANESE, PORTUGUESE, RUSSIAN, SPANISH
        }
    }
}