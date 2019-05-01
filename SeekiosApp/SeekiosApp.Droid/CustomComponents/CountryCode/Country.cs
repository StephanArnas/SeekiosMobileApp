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
using Org.XmlPull.V1;
using Java.IO;
using System.Xml;

namespace SeekiosApp.Droid.CustomComponents.CountryCode
{
    public class Country
    {
        static string TAG = "Class Country";

        public string NameCode { get; set; }
        public string PhoneCode { get; set; }
        public string Name { get; set; }

        public Country()
        {

        }

        public Country(string nameCode, string phoneCode, string name)
        {
            this.NameCode = nameCode;
            this.PhoneCode = phoneCode;
            this.Name = name;
        }

        public static List<Country> ReadXMLofCountries(Context context)
        {
            List<Country> countries = new List<Country>();
            try
            {
                var ins = context.Resources.OpenRawResource(Resource.Raw.countries);
                using (XmlReader reader = XmlReader.Create(ins))
                {
                    reader.MoveToContent();
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            Country country = new Country();
                            if (reader.Name == "code")
                            {
                                country.NameCode = reader.ReadString();
                            }
                            if (reader.Name == "phoneCode")
                            {
                                country.PhoneCode = reader.ReadString();
                            }
                            if (reader.Name == "name")
                            {
                                country.Name = reader.ReadString();
                            }
                            countries.Add(country);
                        }
                    }
                }
            }
            catch
            {

            }
            return countries;
        }

        /// <summary>
        /// Search a country which matches @param code.
        /// </summary>
        /// <param name="language">preferredCountries is list of preference countries.</param>
        /// <param name="preferredCountries">preferredCountries is list of preference countries.</param>
        /// <param name="code">phone code. i.e "91" or "1"</param>
        /// <returns>
        /// Country that has phone code as @param code.
        /// or returns null if no country matches given code.
        /// if same code (e.g. +1) available for more than one country ( US, canada) , this function will return preferred country.
        /// </returns>
        private static Country GetCountryForCode(CountryCodePicker.Language language, List<Country> preferredCountries, string code)
        {
            // check in preferred countries
            if (preferredCountries != null && preferredCountries.Count > 0)
            {
                foreach (var country in preferredCountries)
                {
                    if (country.PhoneCode.Equals(code))
                    {
                        return country;
                    }
                }
            }
            foreach (var country in GetLibraryMasterCountryList(language))
            {
                if (country.PhoneCode.Equals(code))
                {
                    return country;
                }
            }
            return null;
        }

        public static List<Country> GetCustomMasterCountryList(CountryCodePicker codePicker)
        {
            codePicker.RefreshCustomMasterList();
            if (codePicker.CustomMasterCountriesList != null && codePicker.CustomMasterCountriesList.Count > 0)
            {
                return codePicker.CustomMasterCountriesList;
            }
            else
            {
                return GetLibraryMasterCountryList(codePicker.CustomLanguage);
            }
        }

        /// <summary>
        /// Search a country which matches @param nameCode.
        /// </summary>
        /// <param name="customMasterCountriesList">customMasterCountriesList</param>
        /// <param name="language">nameCode country name code. i.e US or us or Au. See countries.xml for all code names.</param>
        /// <param name="nameCode">nameCode</param>
        /// <returns>Country that has phone code as @param code.</returns>
        public static Country GetCountryForNameCodeFromCustomMasterList(List<Country> customMasterCountriesList, CountryCodePicker.Language language, String nameCode)
        {
            if (customMasterCountriesList == null || customMasterCountriesList.Count == 0)
            {
                return GetCountryForNameCodeFromLibraryMasterList(language, nameCode);
            }
            else
            {
                foreach (var country in customMasterCountriesList)
                {
                    if (country.NameCode.Equals(nameCode, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return country;
                    }
                }
            }
            return null;
        }

        public static Country GetCountryForNameCodeFromLibraryMasterList(CountryCodePicker.Language language, string nameCode)
        {
            List<Country> countries = Country.GetLibraryMasterCountryList(language);
            foreach (var country in countries)
            {
                if (country.NameCode.Equals(nameCode, StringComparison.CurrentCultureIgnoreCase))
                {
                    return country;
                }
            }
            return null;
        }

        public static Country GetCountryForNameCodeTrial(string nameCode, Context context)
        {
            List<Country> countries = Country.ReadXMLofCountries(context);
            foreach (var country in countries)
            {
                if (country.NameCode.Equals(nameCode, StringComparison.CurrentCultureIgnoreCase))
                {
                    return country;
                }
            }
            return null;
        }

        public static Country GetCountryForCode(CountryCodePicker.Language language, List<Country> preferredCountries, int code)
        {
            return GetCountryForCode(language, preferredCountries, code.ToString());
        }

        public static Country GetCountryForNumber(CountryCodePicker.Language language, List<Country> preferredCountries, String fullNumber)
        {
            int firstDigit;
            if (fullNumber.Length != 0)
            {
                if (fullNumber[0] == '+')
                {
                    firstDigit = 1;
                }
                else
                {
                    firstDigit = 0;
                }
                Country country = null;
                for (int i = firstDigit; i < firstDigit + 4; i++)
                {
                    String code = fullNumber.Substring(firstDigit, i);
                    country = Country.GetCountryForCode(language, preferredCountries, code);
                    if (country != null)
                    {
                        return country;
                    }
                }
            }
            return null;
        }

        public bool IsEligibleForQuery(String query)
        {
            query = query.ToLower();
            return Name.ToLower().Contains(query) || NameCode.ToLower().Contains(query) || PhoneCode.ToLower().Contains(query);
        }

        public static List<Country> GetLibraryMasterCountryList(CountryCodePicker.Language language)
        {
            switch (language)
            {
                //case CountryCodePicker.Language.ARABIC:
                //    return GetLibraryMasterCountriesArabic();
                //case CountryCodePicker.Language.BENGALI:
                //    return GetLibraryMasterCountriesBengali();
                //case CountryCodePicker.Language.CHINESE:
                //    return GetLibraryMasterCountriesChinese();
                case CountryCodePicker.Language.ENGLISH:
                    return GetLibraryMasterCountriesEnglish();
                case CountryCodePicker.Language.FRENCH:
                    return GetLibraryMasterCountriesFrench();
                case CountryCodePicker.Language.GERMAN:
                    return GetLibraryMasterCountriesGerman();
                //case CountryCodePicker.Language.GUJARATI:
                //    return GetLibraryMasterCountriesGujarati();
                //case CountryCodePicker.Language.HINDI:
                //    return GetLibraryMasterCountriesHindi();
                //case CountryCodePicker.Language.JAPANESE:
                //    return GetLibraryMasterCountriesJapanese();
                //case CountryCodePicker.Language.JAVANESE:
                //    return GetLibraryMasterCountriesJavanese();
                //case CountryCodePicker.Language.PORTUGUESE:
                //    return GetLibraryMasterCountriesPortuguese();
                //case CountryCodePicker.Language.RUSSIAN:
                //    return GetLibraryMasterCountriesRussian();
                case CountryCodePicker.Language.SPANISH:
                    return GetLibraryMasterCountriesSpanish();
                default:
                    return GetLibraryMasterCountriesEnglish();
            }
        }

        public static List<Country> GetLibraryMasterCountriesFrench()
        {
            List<Country> countries = new List<Country>();
            countries.Add(new Country("af", "93", "Afghanistan"));
            countries.Add(new Country("al", "355", "Albanie"));
            countries.Add(new Country("dz", "213", "Algérie"));
            countries.Add(new Country("ad", "376", "Andorre"));
            countries.Add(new Country("ao", "244", "Angola"));
            countries.Add(new Country("aq", "672", "Antarctique"));
            countries.Add(new Country("ar", "54", "Argentine"));
            countries.Add(new Country("am", "374", "Arménie"));
            countries.Add(new Country("aw", "297", "Aruba"));
            countries.Add(new Country("au", "61", "Australie"));
            countries.Add(new Country("at", "43", "Autriche"));
            countries.Add(new Country("az", "994", "Azerbaïdjan"));
            countries.Add(new Country("bh", "973", "Bahreïn"));
            countries.Add(new Country("bd", "880", "Bangladesh"));
            countries.Add(new Country("by", "375", "Belarus"));
            countries.Add(new Country("be", "32", "Belgique"));
            countries.Add(new Country("bz", "501", "Belize"));
            countries.Add(new Country("bj", "229", "Bénin"));
            countries.Add(new Country("bt", "975", "Bhoutan"));
            countries.Add(new Country("bo", "591", "Bolivie, Etat plurinational de"));
            countries.Add(new Country("ba", "387", "Bosnie Herzégovine"));
            countries.Add(new Country("bw", "267", "Botswana"));
            countries.Add(new Country("br", "55", "Brésil"));
            countries.Add(new Country("bn", "673", "Brunei Darussalam"));
            countries.Add(new Country("bg", "359", "Bulgarie"));
            countries.Add(new Country("bf", "226", "Burkina Faso"));
            countries.Add(new Country("mm", "95", "Myanmar"));
            countries.Add(new Country("bi", "257", "Burundi"));
            countries.Add(new Country("kh", "855", "Cambodge"));
            countries.Add(new Country("cm", "237", "Cameroun"));
            countries.Add(new Country("ca", "1", "Canada"));
            countries.Add(new Country("cv", "238", "Cap-Vert"));
            countries.Add(new Country("cf", "236", "République centrafricaine"));
            countries.Add(new Country("td", "235", "Tchad"));
            countries.Add(new Country("cl", "56", "Chili"));
            countries.Add(new Country("cn", "86", "Chine"));
            countries.Add(new Country("cx", "61", "L'île de noël"));
            countries.Add(new Country("cc", "61", "Cocos (Keeling)"));
            countries.Add(new Country("co", "57", "Colombie"));
            countries.Add(new Country("km", "269", "Comores"));
            countries.Add(new Country("cg", "242", "Congo"));
            countries.Add(new Country("cd", "243", "Congo, La République Démocratique Du"));
            countries.Add(new Country("ck", "682", "les Îles Cook"));
            countries.Add(new Country("cr", "506", "Costa Rica"));
            countries.Add(new Country("hr", "385", "Croatie"));
            countries.Add(new Country("cu", "53", "Cuba"));
            countries.Add(new Country("cy", "357", "Chypre"));
            countries.Add(new Country("cz", "420", "République Tchèque"));
            countries.Add(new Country("dk", "45", "Danemark"));
            countries.Add(new Country("dj", "253", "Djibouti"));
            countries.Add(new Country("tl", "670", "Timor-leste"));
            countries.Add(new Country("ec", "593", "Equateur"));
            countries.Add(new Country("eg", "20", "Egypte"));
            countries.Add(new Country("sv", "503", "Le Salvador"));
            countries.Add(new Country("gq", "240", "Guinée Équatoriale"));
            countries.Add(new Country("er", "291", "Érythrée"));
            countries.Add(new Country("ee", "372", "Estonie"));
            countries.Add(new Country("et", "251", "Ethiopie"));
            countries.Add(new Country("fk", "500", "Îles Falkland (liste Malvinas)"));
            countries.Add(new Country("fo", "298", "Îles Féroé"));
            countries.Add(new Country("fj", "679", "Fidji"));
            countries.Add(new Country("fi", "358", "Finlande"));
            countries.Add(new Country("fr", "33", "France"));
            countries.Add(new Country("pf", "689", "Polynésie française"));
            countries.Add(new Country("ga", "241", "Gabon"));
            countries.Add(new Country("gm", "220", "Gambie"));
            countries.Add(new Country("ge", "995", "Géorgie"));
            countries.Add(new Country("de", "49", "Allemagne"));
            countries.Add(new Country("gh", "233", "Ghana"));
            countries.Add(new Country("gi", "350", "Gibraltar"));
            countries.Add(new Country("gr", "30", "Grèce"));
            countries.Add(new Country("gl", "299", "Groenland"));
            countries.Add(new Country("gt", "502", "Guatemala"));
            countries.Add(new Country("gn", "224", "Guinée"));
            countries.Add(new Country("gw", "245", "Guinée-Bissau"));
            countries.Add(new Country("gy", "592", "Guyane"));
            countries.Add(new Country("ht", "509", "Haïti"));
            countries.Add(new Country("hn", "504", "Honduras"));
            countries.Add(new Country("hk", "852", "Hong Kong"));
            countries.Add(new Country("hu", "36", "Hongrie"));
            countries.Add(new Country("in", "91", "Inde"));
            countries.Add(new Country("id", "62", "Indonésie"));
            countries.Add(new Country("ir", "98", "Iran (République islamique d"));
            countries.Add(new Country("iq", "964", "Irak"));
            countries.Add(new Country("ie", "353", "Irlande"));
            countries.Add(new Country("im", "44", "Isle Of Man"));
            countries.Add(new Country("il", "972", "Israël"));
            countries.Add(new Country("it", "39", "Italie"));
            countries.Add(new Country("ci", "225", "Côte D & apos; ivoire"));
            countries.Add(new Country("jp", "81", "Japon"));
            countries.Add(new Country("jo", "962", "Jordanie"));
            countries.Add(new Country("kz", "7", "Kazakhstan"));
            countries.Add(new Country("ke", "254", "Kenya"));
            countries.Add(new Country("ki", "686", "Kiribati"));
            countries.Add(new Country("kw", "965", "Koweit"));
            countries.Add(new Country("kg", "996", "Kirghizistan"));
            countries.Add(new Country("la", "856", "République démocratique; Lao People & apos"));
            countries.Add(new Country("lv", "371", "Lettonie"));
            countries.Add(new Country("lb", "961", "Liban"));
            countries.Add(new Country("ls", "266", "Lesotho"));
            countries.Add(new Country("lr", "231", "Libéria"));
            countries.Add(new Country("ly", "218", "Libye"));
            countries.Add(new Country("li", "423", "Liechtenstein"));
            countries.Add(new Country("lt", "370", "Lituanie"));
            countries.Add(new Country("lu", "352", "Luxembourg"));
            countries.Add(new Country("mo", "853", "Macao"));
            countries.Add(new Country("mk", "389", "Macédoine, Ex-République yougoslave de"));
            countries.Add(new Country("mg", "261", "Madagascar"));
            countries.Add(new Country("mw", "265", "Malawi"));
            countries.Add(new Country("my", "60", "Malaisie"));
            countries.Add(new Country("mv", "960", "Maldives"));
            countries.Add(new Country("ml", "223", "Mali"));
            countries.Add(new Country("mt", "356", "Malte"));
            countries.Add(new Country("mh", "692", "Iles Marshall"));
            countries.Add(new Country("mr", "222", "Mauritanie"));
            countries.Add(new Country("mu", "230", "Ile Maurice"));
            countries.Add(new Country("yt", "262", "Mayotte"));
            countries.Add(new Country("mx", "52", "Mexique"));
            countries.Add(new Country("fm", "691", "Micronésie, États fédérés de"));
            countries.Add(new Country("md", "373", "Moldova, République de"));
            countries.Add(new Country("mc", "377", "Monaco"));
            countries.Add(new Country("mn", "976", "Mongolie"));
            countries.Add(new Country("me", "382", "Monténégro"));
            countries.Add(new Country("ma", "212", "Maroc"));
            countries.Add(new Country("mz", "258", "Mozambique"));
            countries.Add(new Country("na", "264", "Namibie"));
            countries.Add(new Country("nr", "674", "Nauru"));
            countries.Add(new Country("np", "977", "Népal"));
            countries.Add(new Country("nl", "31", "Pays-Bas"));
            countries.Add(new Country("nc", "687", "Nouvelle Calédonie"));
            countries.Add(new Country("nz", "64", "nouvelle Zélande"));
            countries.Add(new Country("ni", "505", "Nicaragua"));
            countries.Add(new Country("ne", "227", "Niger"));
            countries.Add(new Country("ng", "234", "Nigeria"));
            countries.Add(new Country("nu", "683", "Niue"));
            countries.Add(new Country("kp", "850", "Corée"));
            countries.Add(new Country("no", "47", "Norvège"));
            countries.Add(new Country("om", "968", "Oman"));
            countries.Add(new Country("pk", "92", "Pakistan"));
            countries.Add(new Country("pw", "680", "Palau"));
            countries.Add(new Country("pa", "507", "Panama"));
            countries.Add(new Country("pg", "675", "Papouasie Nouvelle Guinée"));
            countries.Add(new Country("py", "595", "Paraguay"));
            countries.Add(new Country("pe", "51", "Pérou"));
            countries.Add(new Country("ph", "63", "Philippines"));
            countries.Add(new Country("pn", "870", "Pitcairn"));
            countries.Add(new Country("pl", "48", "Pologne"));
            countries.Add(new Country("pt", "351", "le Portugal"));
            countries.Add(new Country("pr", "1", "Porto Rico"));
            countries.Add(new Country("qa", "974", "Qatar"));
            countries.Add(new Country("ro", "40", "Roumanie"));
            countries.Add(new Country("ru", "7", "Fédération Russe"));
            countries.Add(new Country("rw", "250", "Rwanda"));
            countries.Add(new Country("bl", "590", "Saint Barthélemy"));
            countries.Add(new Country("ws", "685", "Samoa"));
            countries.Add(new Country("sm", "378", "Saint Marin"));
            countries.Add(new Country("st", "239", "Sao Tomé-et-Principe"));
            countries.Add(new Country("sa", "966", "Arabie Saoudite"));
            countries.Add(new Country("sn", "221", "Sénégal"));
            countries.Add(new Country("rs", "381", "Serbie"));
            countries.Add(new Country("sc", "248", "les Seychelles"));
            countries.Add(new Country("sl", "232", "Sierra Leone"));
            countries.Add(new Country("sg", "65", "Singapour"));
            countries.Add(new Country("sk", "421", "Slovaquie"));
            countries.Add(new Country("si", "386", "Slovénie"));
            countries.Add(new Country("sb", "677", "Les îles Salomon"));
            countries.Add(new Country("so", "252", "Somalie"));
            countries.Add(new Country("za", "27", "Afrique du Sud"));
            countries.Add(new Country("kr", "82", "Corée, République de"));
            countries.Add(new Country("es", "34", "l'Espagne"));
            countries.Add(new Country("lk", "94", "Sri Lanka"));
            countries.Add(new Country("sh", "290", "Sainte-Hélène, Ascension et Tristan da Cunha"));
            countries.Add(new Country("pm", "508", "Saint-Pierre-et-Miquelon"));
            countries.Add(new Country("sd", "249", "Soudan"));
            countries.Add(new Country("sr", "597", "Suriname"));
            countries.Add(new Country("sz", "268", "Swaziland"));
            countries.Add(new Country("se", "46", "Suède"));
            countries.Add(new Country("ch", "41", "Suisse"));
            countries.Add(new Country("sy", "963", "République arabe syrienne"));
            countries.Add(new Country("tw", "886", "Taiwan, Province de Chine"));
            countries.Add(new Country("tj", "992", "Tadjikistan"));
            countries.Add(new Country("tz", "255", "Tanzanie, République-Unie de"));
            countries.Add(new Country("th", "66", "Thaïlande"));
            countries.Add(new Country("tg", "228", "Aller"));
            countries.Add(new Country("tk", "690", "Tokelau"));
            countries.Add(new Country("to", "676", "Tonga"));
            countries.Add(new Country("tn", "216", "Tunisie"));
            countries.Add(new Country("tr", "90", "dinde"));
            countries.Add(new Country("tm", "993", "Turkménistan"));
            countries.Add(new Country("tv", "688", "Tuvalu"));
            countries.Add(new Country("ae", "971", "Emirats Arabes Unis"));
            countries.Add(new Country("ug", "256", "Ouganda"));
            countries.Add(new Country("gb", "44", "Royaume-Uni"));
            countries.Add(new Country("ua", "380", "Ukraine"));
            countries.Add(new Country("uy", "598", "Uruguay"));
            countries.Add(new Country("us", "1", "États Unis"));
            countries.Add(new Country("uz", "998", "Ouzbékistan"));
            countries.Add(new Country("vu", "678", "Vanuatu"));
            countries.Add(new Country("va", "39", "Saint-Siège (Cité du Vatican)"));
            countries.Add(new Country("ve", "58", "Venezuela, République bolivarienne du"));
            countries.Add(new Country("vn", "84", "Viet Nam"));
            countries.Add(new Country("wf", "681", "Wallis et Futuna"));
            countries.Add(new Country("ye", "967", "Yémen"));
            countries.Add(new Country("zm", "260", "Zambie"));
            countries.Add(new Country("zw", "263", "Zimbabwe"));
            return countries;
        }

        public static List<Country> GetLibraryMasterCountriesEnglish()
        {
            List<Country> countries = new List<Country>();
            countries.Add(new Country("af", "93", "Afghanistan"));
            countries.Add(new Country("al", "355", "Albania"));
            countries.Add(new Country("dz", "213", "Algeria"));
            countries.Add(new Country("ad", "376", "Andorra"));
            countries.Add(new Country("ao", "244", "Angola"));
            countries.Add(new Country("aq", "672", "Antarctica"));
            countries.Add(new Country("ar", "54", "Argentina"));
            countries.Add(new Country("am", "374", "Armenia"));
            countries.Add(new Country("aw", "297", "Aruba"));
            countries.Add(new Country("au", "61", "Australia"));
            countries.Add(new Country("at", "43", "Austria"));
            countries.Add(new Country("az", "994", "Azerbaijan"));
            countries.Add(new Country("bh", "973", "Bahrain"));
            countries.Add(new Country("bd", "880", "Bangladesh"));
            countries.Add(new Country("by", "375", "Belarus"));
            countries.Add(new Country("be", "32", "Belgium"));
            countries.Add(new Country("bz", "501", "Belize"));
            countries.Add(new Country("bj", "229", "Benin"));
            countries.Add(new Country("bt", "975", "Bhutan"));
            countries.Add(new Country("bo", "591", "Bolivia, Plurinational State Of"));
            countries.Add(new Country("ba", "387", "Bosnia And Herzegovina"));
            countries.Add(new Country("bw", "267", "Botswana"));
            countries.Add(new Country("br", "55", "Brazil"));
            countries.Add(new Country("bn", "673", "Brunei Darussalam"));
            countries.Add(new Country("bg", "359", "Bulgaria"));
            countries.Add(new Country("bf", "226", "Burkina Faso"));
            countries.Add(new Country("mm", "95", "Myanmar"));
            countries.Add(new Country("bi", "257", "Burundi"));
            countries.Add(new Country("kh", "855", "Cambodia"));
            countries.Add(new Country("cm", "237", "Cameroon"));
            countries.Add(new Country("ca", "1", "Canada"));
            countries.Add(new Country("cv", "238", "Cape Verde"));
            countries.Add(new Country("cf", "236", "Central African Republic"));
            countries.Add(new Country("td", "235", "Chad"));
            countries.Add(new Country("cl", "56", "Chile"));
            countries.Add(new Country("cn", "86", "China"));
            countries.Add(new Country("cx", "61", "Christmas Island"));
            countries.Add(new Country("cc", "61", "Cocos (keeling) Islands"));
            countries.Add(new Country("co", "57", "Colombia"));
            countries.Add(new Country("km", "269", "Comoros"));
            countries.Add(new Country("cg", "242", "Congo"));
            countries.Add(new Country("cd", "243", "Congo, The Democratic Republic Of The"));
            countries.Add(new Country("ck", "682", "Cook Islands"));
            countries.Add(new Country("cr", "506", "Costa Rica"));
            countries.Add(new Country("hr", "385", "Croatia"));
            countries.Add(new Country("cu", "53", "Cuba"));
            countries.Add(new Country("cy", "357", "Cyprus"));
            countries.Add(new Country("cz", "420", "Czech Republic"));
            countries.Add(new Country("dk", "45", "Denmark"));
            countries.Add(new Country("dj", "253", "Djibouti"));
            countries.Add(new Country("tl", "670", "Timor-leste"));
            countries.Add(new Country("ec", "593", "Ecuador"));
            countries.Add(new Country("eg", "20", "Egypt"));
            countries.Add(new Country("sv", "503", "El Salvador"));
            countries.Add(new Country("gq", "240", "Equatorial Guinea"));
            countries.Add(new Country("er", "291", "Eritrea"));
            countries.Add(new Country("ee", "372", "Estonia"));
            countries.Add(new Country("et", "251", "Ethiopia"));
            countries.Add(new Country("fk", "500", "Falkland Islands (malvinas)"));
            countries.Add(new Country("fo", "298", "Faroe Islands"));
            countries.Add(new Country("fj", "679", "Fiji"));
            countries.Add(new Country("fi", "358", "Finland"));
            countries.Add(new Country("fr", "33", "France"));
            countries.Add(new Country("pf", "689", "French Polynesia"));
            countries.Add(new Country("ga", "241", "Gabon"));
            countries.Add(new Country("gm", "220", "Gambia"));
            countries.Add(new Country("ge", "995", "Georgia"));
            countries.Add(new Country("de", "49", "Germany"));
            countries.Add(new Country("gh", "233", "Ghana"));
            countries.Add(new Country("gi", "350", "Gibraltar"));
            countries.Add(new Country("gr", "30", "Greece"));
            countries.Add(new Country("gl", "299", "Greenland"));
            countries.Add(new Country("gt", "502", "Guatemala"));
            countries.Add(new Country("gn", "224", "Guinea"));
            countries.Add(new Country("gw", "245", "Guinea-bissau"));
            countries.Add(new Country("gy", "592", "Guyana"));
            countries.Add(new Country("ht", "509", "Haiti"));
            countries.Add(new Country("hn", "504", "Honduras"));
            countries.Add(new Country("hk", "852", "Hong Kong"));
            countries.Add(new Country("hu", "36", "Hungary"));
            countries.Add(new Country("in", "91", "India"));
            countries.Add(new Country("id", "62", "Indonesia"));
            countries.Add(new Country("ir", "98", "Iran, Islamic Republic Of"));
            countries.Add(new Country("iq", "964", "Iraq"));
            countries.Add(new Country("ie", "353", "Ireland"));
            countries.Add(new Country("im", "44", "Isle Of Man"));
            countries.Add(new Country("il", "972", "Israel"));
            countries.Add(new Country("it", "39", "Italy"));
            countries.Add(new Country("ci", "225", "Côte D&apos;ivoire"));
            countries.Add(new Country("jp", "81", "Japan"));
            countries.Add(new Country("jo", "962", "Jordan"));
            countries.Add(new Country("kz", "7", "Kazakhstan"));
            countries.Add(new Country("ke", "254", "Kenya"));
            countries.Add(new Country("ki", "686", "Kiribati"));
            countries.Add(new Country("kw", "965", "Kuwait"));
            countries.Add(new Country("kg", "996", "Kyrgyzstan"));
            countries.Add(new Country("la", "856", "Lao People&apos;s Democratic Republic"));
            countries.Add(new Country("lv", "371", "Latvia"));
            countries.Add(new Country("lb", "961", "Lebanon"));
            countries.Add(new Country("ls", "266", "Lesotho"));
            countries.Add(new Country("lr", "231", "Liberia"));
            countries.Add(new Country("ly", "218", "Libya"));
            countries.Add(new Country("li", "423", "Liechtenstein"));
            countries.Add(new Country("lt", "370", "Lithuania"));
            countries.Add(new Country("lu", "352", "Luxembourg"));
            countries.Add(new Country("mo", "853", "Macao"));
            countries.Add(new Country("mk", "389", "Macedonia, The Former Yugoslav Republic Of"));
            countries.Add(new Country("mg", "261", "Madagascar"));
            countries.Add(new Country("mw", "265", "Malawi"));
            countries.Add(new Country("my", "60", "Malaysia"));
            countries.Add(new Country("mv", "960", "Maldives"));
            countries.Add(new Country("ml", "223", "Mali"));
            countries.Add(new Country("mt", "356", "Malta"));
            countries.Add(new Country("mh", "692", "Marshall Islands"));
            countries.Add(new Country("mr", "222", "Mauritania"));
            countries.Add(new Country("mu", "230", "Mauritius"));
            countries.Add(new Country("yt", "262", "Mayotte"));
            countries.Add(new Country("mx", "52", "Mexico"));
            countries.Add(new Country("fm", "691", "Micronesia, Federated States Of"));
            countries.Add(new Country("md", "373", "Moldova, Republic Of"));
            countries.Add(new Country("mc", "377", "Monaco"));
            countries.Add(new Country("mn", "976", "Mongolia"));
            countries.Add(new Country("me", "382", "Montenegro"));
            countries.Add(new Country("ma", "212", "Morocco"));
            countries.Add(new Country("mz", "258", "Mozambique"));
            countries.Add(new Country("na", "264", "Namibia"));
            countries.Add(new Country("nr", "674", "Nauru"));
            countries.Add(new Country("np", "977", "Nepal"));
            countries.Add(new Country("nl", "31", "Netherlands"));
            countries.Add(new Country("nc", "687", "New Caledonia"));
            countries.Add(new Country("nz", "64", "New Zealand"));
            countries.Add(new Country("ni", "505", "Nicaragua"));
            countries.Add(new Country("ne", "227", "Niger"));
            countries.Add(new Country("ng", "234", "Nigeria"));
            countries.Add(new Country("nu", "683", "Niue"));
            countries.Add(new Country("kp", "850", "Korea"));
            countries.Add(new Country("no", "47", "Norway"));
            countries.Add(new Country("om", "968", "Oman"));
            countries.Add(new Country("pk", "92", "Pakistan"));
            countries.Add(new Country("pw", "680", "Palau"));
            countries.Add(new Country("pa", "507", "Panama"));
            countries.Add(new Country("pg", "675", "Papua New Guinea"));
            countries.Add(new Country("py", "595", "Paraguay"));
            countries.Add(new Country("pe", "51", "Peru"));
            countries.Add(new Country("ph", "63", "Philippines"));
            countries.Add(new Country("pn", "870", "Pitcairn"));
            countries.Add(new Country("pl", "48", "Poland"));
            countries.Add(new Country("pt", "351", "Portugal"));
            countries.Add(new Country("pr", "1", "Puerto Rico"));
            countries.Add(new Country("qa", "974", "Qatar"));
            countries.Add(new Country("ro", "40", "Romania"));
            countries.Add(new Country("ru", "7", "Russian Federation"));
            countries.Add(new Country("rw", "250", "Rwanda"));
            countries.Add(new Country("bl", "590", "Saint Barthélemy"));
            countries.Add(new Country("ws", "685", "Samoa"));
            countries.Add(new Country("sm", "378", "San Marino"));
            countries.Add(new Country("st", "239", "Sao Tome And Principe"));
            countries.Add(new Country("sa", "966", "Saudi Arabia"));
            countries.Add(new Country("sn", "221", "Senegal"));
            countries.Add(new Country("rs", "381", "Serbia"));
            countries.Add(new Country("sc", "248", "Seychelles"));
            countries.Add(new Country("sl", "232", "Sierra Leone"));
            countries.Add(new Country("sg", "65", "Singapore"));
            countries.Add(new Country("sk", "421", "Slovakia"));
            countries.Add(new Country("si", "386", "Slovenia"));
            countries.Add(new Country("sb", "677", "Solomon Islands"));
            countries.Add(new Country("so", "252", "Somalia"));
            countries.Add(new Country("za", "27", "South Africa"));
            countries.Add(new Country("kr", "82", "Korea, Republic Of"));
            countries.Add(new Country("es", "34", "Spain"));
            countries.Add(new Country("lk", "94", "Sri Lanka"));
            countries.Add(new Country("sh", "290", "Saint Helena, Ascension And Tristan Da Cunha"));
            countries.Add(new Country("pm", "508", "Saint Pierre And Miquelon"));
            countries.Add(new Country("sd", "249", "Sudan"));
            countries.Add(new Country("sr", "597", "Suriname"));
            countries.Add(new Country("sz", "268", "Swaziland"));
            countries.Add(new Country("se", "46", "Sweden"));
            countries.Add(new Country("ch", "41", "Switzerland"));
            countries.Add(new Country("sy", "963", "Syrian Arab Republic"));
            countries.Add(new Country("tw", "886", "Taiwan, Province Of China"));
            countries.Add(new Country("tj", "992", "Tajikistan"));
            countries.Add(new Country("tz", "255", "Tanzania, United Republic Of"));
            countries.Add(new Country("th", "66", "Thailand"));
            countries.Add(new Country("tg", "228", "Togo"));
            countries.Add(new Country("tk", "690", "Tokelau"));
            countries.Add(new Country("to", "676", "Tonga"));
            countries.Add(new Country("tn", "216", "Tunisia"));
            countries.Add(new Country("tr", "90", "Turkey"));
            countries.Add(new Country("tm", "993", "Turkmenistan"));
            countries.Add(new Country("tv", "688", "Tuvalu"));
            countries.Add(new Country("ae", "971", "United Arab Emirates"));
            countries.Add(new Country("ug", "256", "Uganda"));
            countries.Add(new Country("gb", "44", "United Kingdom"));
            countries.Add(new Country("ua", "380", "Ukraine"));
            countries.Add(new Country("uy", "598", "Uruguay"));
            countries.Add(new Country("us", "1", "United States"));
            countries.Add(new Country("uz", "998", "Uzbekistan"));
            countries.Add(new Country("vu", "678", "Vanuatu"));
            countries.Add(new Country("va", "39", "Holy See (vatican City State)"));
            countries.Add(new Country("ve", "58", "Venezuela, Bolivarian Republic Of"));
            countries.Add(new Country("vn", "84", "Viet Nam"));
            countries.Add(new Country("wf", "681", "Wallis And Futuna"));
            countries.Add(new Country("ye", "967", "Yemen"));
            countries.Add(new Country("zm", "260", "Zambia"));
            countries.Add(new Country("zw", "263", "Zimbabwe"));
            return countries;
        }

        public static List<Country> GetLibraryMasterCountriesSpanish()
        {
            List<Country> countries = new List<Country>();
            countries.Add(new Country("af", "93", "Afganistán"));
            countries.Add(new Country("al", "355", "Albania"));
            countries.Add(new Country("dz", "213", "Argelia"));
            countries.Add(new Country("ad", "376", "Andorra"));
            countries.Add(new Country("ao", "244", "Angola"));
            countries.Add(new Country("aq", "672", "Antártida"));
            countries.Add(new Country("ar", "54", "Argentina"));
            countries.Add(new Country("am", "374", "Armenia"));
            countries.Add(new Country("aw", "297", "Aruba"));
            countries.Add(new Country("au", "61", "Australia"));
            countries.Add(new Country("at", "43", "Austria"));
            countries.Add(new Country("az", "994", "Azerbaiyán"));
            countries.Add(new Country("bh", "973", "Bahrein"));
            countries.Add(new Country("bd", "880", "Bangladesh"));
            countries.Add(new Country("by", "375", "Belarús"));
            countries.Add(new Country("be", "32", "Bélgica"));
            countries.Add(new Country("bz", "501", "Belice"));
            countries.Add(new Country("bj", "229", "Benin"));
            countries.Add(new Country("bt", "975", "Bhután"));
            countries.Add(new Country("bo", "591", "Bolivia, Estado Plurinacional de"));
            countries.Add(new Country("ba", "387", "Bosnia y Herzegovina"));
            countries.Add(new Country("bw", "267", "Botswana"));
            countries.Add(new Country("br", "55", "Brasil"));
            countries.Add(new Country("bn", "673", "Brunei Darussalam"));
            countries.Add(new Country("bg", "359", "Bulgaria"));
            countries.Add(new Country("bf", "226", "Burkina Faso"));
            countries.Add(new Country("mm", "95", "Myanmar"));
            countries.Add(new Country("bi", "257", "Burundi"));
            countries.Add(new Country("kh", "855", "Camboya"));
            countries.Add(new Country("cm", "237", "Camerún"));
            countries.Add(new Country("ca", "1", "Canadá"));
            countries.Add(new Country("cv", "238", "Cabo Verde"));
            countries.Add(new Country("cf", "236", "República Centroafricana"));
            countries.Add(new Country("td", "235", "Chad"));
            countries.Add(new Country("cl", "56", "Chile"));
            countries.Add(new Country("cn", "86", "China"));
            countries.Add(new Country("cx", "61", "Isla de Navidad"));
            countries.Add(new Country("cc", "61", "Cocos (Keeling)"));
            countries.Add(new Country("co", "57", "Colombia"));
            countries.Add(new Country("km", "269", "Comoras"));
            countries.Add(new Country("cg", "242", "Congo"));
            countries.Add(new Country("cd", "243", "Congo, La República Democrática Del"));
            countries.Add(new Country("ck", "682", "Islas Cook"));
            countries.Add(new Country("cr", "506", "Costa Rica"));
            countries.Add(new Country("hr", "385", "Croacia"));
            countries.Add(new Country("cu", "53", "Cuba"));
            countries.Add(new Country("cy", "357", "Chipre"));
            countries.Add(new Country("cz", "420", "República Checa"));
            countries.Add(new Country("dk", "45", "Dinamarca"));
            countries.Add(new Country("dj", "253", "Djibouti"));
            countries.Add(new Country("tl", "670", "Timor Oriental"));
            countries.Add(new Country("ec", "593", "Ecuador"));
            countries.Add(new Country("eg", "20", "Egipto"));
            countries.Add(new Country("sv", "503", "El Salvador"));
            countries.Add(new Country("gq", "240", "Guinea Ecuatorial"));
            countries.Add(new Country("er", "291", "Eritrea"));
            countries.Add(new Country("ee", "372", "Estonia"));
            countries.Add(new Country("et", "251", "Etiopía"));
            countries.Add(new Country("fk", "500", "Islas Malvinas (Falkland)"));
            countries.Add(new Country("fo", "298", "Islas Faroe"));
            countries.Add(new Country("fj", "679", "Fiji"));
            countries.Add(new Country("fi", "358", "Finlandia"));
            countries.Add(new Country("fr", "33", "Francia"));
            countries.Add(new Country("pf", "689", "Polinesia francés"));
            countries.Add(new Country("ga", "241", "Gabón"));
            countries.Add(new Country("gm", "220", "Gambia"));
            countries.Add(new Country("ge", "995", "Georgia"));
            countries.Add(new Country("de", "49", "Alemania"));
            countries.Add(new Country("gh", "233", "Ghana"));
            countries.Add(new Country("gi", "350", "Gibraltar"));
            countries.Add(new Country("gr", "30", "Grecia"));
            countries.Add(new Country("gl", "299", "Tierra Verde"));
            countries.Add(new Country("gt", "502", "Guatemala"));
            countries.Add(new Country("gn", "224", "Guinea"));
            countries.Add(new Country("gw", "245", "Guinea-Bissau"));
            countries.Add(new Country("gy", "592", "Guayana"));
            countries.Add(new Country("ht", "509", "Haití"));
            countries.Add(new Country("hn", "504", "Honduras"));
            countries.Add(new Country("hk", "852", "Hong Kong"));
            countries.Add(new Country("hu", "36", "Hungría"));
            countries.Add(new Country("in", "91", "India"));
            countries.Add(new Country("id", "62", "Indonesia"));
            countries.Add(new Country("ir", "98", "Irán (República Islámica de"));
            countries.Add(new Country("iq", "964", "Irak"));
            countries.Add(new Country("ie", "353", "Irlanda"));
            countries.Add(new Country("im", "44", "Isla de Man"));
            countries.Add(new Country("il", "972", "Israel"));
            countries.Add(new Country("it", "39", "Italia"));
            countries.Add(new Country("ci", "225", "Côte D & apos; Marfil"));
            countries.Add(new Country("jp", "81", "Japón"));
            countries.Add(new Country("jo", "962", "Jordán"));
            countries.Add(new Country("kz", "7", "Kazajstán"));
            countries.Add(new Country("ke", "254", "Kenia"));
            countries.Add(new Country("ki", "686", "Kiribati"));
            countries.Add(new Country("kw", "965", "Kuwait"));
            countries.Add(new Country("kg", "996", "Kirguistán"));
            countries.Add(new Country("la", "856", "República Democrática s; Lao y apos"));
            countries.Add(new Country("lv", "371", "Letonia"));
            countries.Add(new Country("lb", "961", "Líbano"));
            countries.Add(new Country("ls", "266", "Lesoto"));
            countries.Add(new Country("lr", "231", "Liberia"));
            countries.Add(new Country("ly", "218", "Libia"));
            countries.Add(new Country("li", "423", "Liechtenstein"));
            countries.Add(new Country("lt", "370", "Lituania"));
            countries.Add(new Country("lu", "352", "Luxemburgo"));
            countries.Add(new Country("mo", "853", "macao"));
            countries.Add(new Country("mk", "389", "Macedonia, Ex-República Yugoslava de"));
            countries.Add(new Country("mg", "261", "Madagascar"));
            countries.Add(new Country("mw", "265", "Malawi"));
            countries.Add(new Country("my", "60", "Malasia"));
            countries.Add(new Country("mv", "960", "Maldivas"));
            countries.Add(new Country("ml", "223", "mali"));
            countries.Add(new Country("mt", "356", "Malta"));
            countries.Add(new Country("mh", "692", "Islas Marshall"));
            countries.Add(new Country("mr", "222", "Mauritania"));
            countries.Add(new Country("mu", "230", "Mauricio"));
            countries.Add(new Country("yt", "262", "Mayotte"));
            countries.Add(new Country("mx", "52", "Méjico"));
            countries.Add(new Country("fm", "691", "Micronesia, Estados Federados de"));
            countries.Add(new Country("md", "373", "Moldavia"));
            countries.Add(new Country("mc", "377", "Mónaco"));
            countries.Add(new Country("mn", "976", "Mongolia"));
            countries.Add(new Country("me", "382", "Montenegro"));
            countries.Add(new Country("ma", "212", "Marruecos"));
            countries.Add(new Country("mz", "258", "Mozambique"));
            countries.Add(new Country("na", "264", "Namibia"));
            countries.Add(new Country("nr", "674", "Nauru"));
            countries.Add(new Country("np", "977", "Nepal"));
            countries.Add(new Country("nl", "31", "Países Bajos"));
            countries.Add(new Country("nc", "687", "Nueva Caledonia"));
            countries.Add(new Country("nz", "64", "Nueva Zelanda"));
            countries.Add(new Country("ni", "505", "Nicaragua"));
            countries.Add(new Country("ne", "227", "Níger"));
            countries.Add(new Country("ng", "234", "Nigeria"));
            countries.Add(new Country("nu", "683", "Niue"));
            countries.Add(new Country("kp", "850", "República de s; Corea, Personas & apos Democrática"));
            countries.Add(new Country("no", "47", "Noruega"));
            countries.Add(new Country("om", "968", "Omán"));
            countries.Add(new Country("pk", "92", "Pakistán"));
            countries.Add(new Country("pw", "680", "Palau"));
            countries.Add(new Country("pa", "507", "Panamá"));
            countries.Add(new Country("pg", "675", "Papúa Nueva Guinea"));
            countries.Add(new Country("py", "595", "Paraguay"));
            countries.Add(new Country("pe", "51", "Perú"));
            countries.Add(new Country("ph", "63", "Filipinas"));
            countries.Add(new Country("pn", "870", "Pitcairn"));
            countries.Add(new Country("pl", "48", "Polonia"));
            countries.Add(new Country("pt", "351", "Portugal"));
            countries.Add(new Country("pr", "1", "Puerto Rico"));
            countries.Add(new Country("qa", "974", "Katar"));
            countries.Add(new Country("ro", "40", "Rumania"));
            countries.Add(new Country("ru", "7", "Federación Rusa"));
            countries.Add(new Country("rw", "250", "Ruanda"));
            countries.Add(new Country("bl", "590", "San Bartolomé"));
            countries.Add(new Country("ws", "685", "Samoa"));
            countries.Add(new Country("sm", "378", "San Marino"));
            countries.Add(new Country("st", "239", "Santo Tomé y Príncipe"));
            countries.Add(new Country("sa", "966", "Arabia Saudita"));
            countries.Add(new Country("sn", "221", "Senegal"));
            countries.Add(new Country("rs", "381", "Serbia"));
            countries.Add(new Country("sc", "248", "Seychelles"));
            countries.Add(new Country("sl", "232", "Sierra Leona"));
            countries.Add(new Country("sg", "65", "Singapur"));
            countries.Add(new Country("sk", "421", "Eslovaquia"));
            countries.Add(new Country("si", "386", "Eslovenia"));
            countries.Add(new Country("sb", "677", "Islas Salomón"));
            countries.Add(new Country("so", "252", "Somalia"));
            countries.Add(new Country("za", "27", "Sudáfrica"));
            countries.Add(new Country("kr", "82", "Corea"));
            countries.Add(new Country("es", "34", "España"));
            countries.Add(new Country("lk", "94", "Sri Lanka"));
            countries.Add(new Country("sh", "290", "Santa Helena, Ascensión y Tristán da Cunha"));
            countries.Add(new Country("pm", "508", "San Pedro y Miquelón"));
            countries.Add(new Country("sd", "249", "Sudán"));
            countries.Add(new Country("sr", "597", "Surinam"));
            countries.Add(new Country("sz", "268", "Swazilandia"));
            countries.Add(new Country("se", "46", "Suecia"));
            countries.Add(new Country("ch", "41", "Suiza"));
            countries.Add(new Country("sy", "963", "República Árabe Siria"));
            countries.Add(new Country("tw", "886", "Taiwan, provincia de China"));
            countries.Add(new Country("tj", "992", "Tayikistán"));
            countries.Add(new Country("tz", "255", "Tanzania, República Unida de"));
            countries.Add(new Country("th", "66", "Tailandia"));
            countries.Add(new Country("tg", "228", "Ir"));
            countries.Add(new Country("tk", "690", "Tokelau"));
            countries.Add(new Country("to", "676", "Tonga"));
            countries.Add(new Country("tn", "216", "Túnez"));
            countries.Add(new Country("tr", "90", "Turquía"));
            countries.Add(new Country("tm", "993", "Turkmenistán"));
            countries.Add(new Country("tv", "688", "Tuvalu"));
            countries.Add(new Country("ae", "971", "Emiratos Árabes Unidos"));
            countries.Add(new Country("ug", "256", "Uganda"));
            countries.Add(new Country("gb", "44", "Reino Unido"));
            countries.Add(new Country("ua", "380", "Ucrania"));
            countries.Add(new Country("uy", "598", "Uruguay"));
            countries.Add(new Country("us", "1", "Estados Unidos"));
            countries.Add(new Country("uz", "998", "Uzbekistán"));
            countries.Add(new Country("vu", "678", "Vanuatu"));
            countries.Add(new Country("va", "39", "Santa Sede (Ciudad del Vaticano)"));
            countries.Add(new Country("ve", "58", "Venezuela, República Bolivariana de"));
            countries.Add(new Country("vn", "84", "Vietnam"));
            countries.Add(new Country("wf", "681", "Wallis y Futuna"));
            countries.Add(new Country("ye", "967", "Yemen"));
            countries.Add(new Country("zm", "260", "Zambia"));
            countries.Add(new Country("zw", "263", "Zimbabue"));
            return countries;
        }

        public static List<Country> GetLibraryMasterCountriesGerman()
        {
            List<Country> countries = new List<Country>();
            countries.Add(new Country("af", "93", "Afghanistan"));
            countries.Add(new Country("al", "355", "Albanien"));
            countries.Add(new Country("dz", "213", "Algerien"));
            countries.Add(new Country("ad", "376", "Andorra"));
            countries.Add(new Country("ao", "244", "Angola"));
            countries.Add(new Country("aq", "672", "Antarktika"));
            countries.Add(new Country("ar", "54", "Argentinien"));
            countries.Add(new Country("am", "374", "Armenien"));
            countries.Add(new Country("aw", "297", "Aruba"));
            countries.Add(new Country("au", "61", "Australien"));
            countries.Add(new Country("at", "43", "Österreich"));
            countries.Add(new Country("az", "994", "Aserbaidschan"));
            countries.Add(new Country("bh", "973", "Bahrein"));
            countries.Add(new Country("bd", "880", "Bangladesch"));
            countries.Add(new Country("by", "375", "Weißrussland"));
            countries.Add(new Country("be", "32", "Belgien"));
            countries.Add(new Country("bz", "501", "Belize"));
            countries.Add(new Country("bj", "229", "Benin"));
            countries.Add(new Country("bt", "975", "Bhutan"));
            countries.Add(new Country("bo", "591", "Bolivien, Plurinationaler Staat"));
            countries.Add(new Country("ba", "387", "Bosnien und Herzegowina"));
            countries.Add(new Country("bw", "267", "Botswana"));
            countries.Add(new Country("br", "55", "Brasilien"));
            countries.Add(new Country("bn", "673", "Brunei Darussalam"));
            countries.Add(new Country("bg", "359", "Bulgarien"));
            countries.Add(new Country("bf", "226", "Burkina Faso"));
            countries.Add(new Country("mm", "95", "Myanmar"));
            countries.Add(new Country("bi", "257", "Burundi"));
            countries.Add(new Country("kh", "855", "Kambodscha"));
            countries.Add(new Country("cm", "237", "Kamerun"));
            countries.Add(new Country("ca", "1", "Kanada"));
            countries.Add(new Country("cv", "238", "Kap Verde"));
            countries.Add(new Country("cf", "236", "Zentralafrikanische Republik"));
            countries.Add(new Country("td", "235", "Tschad"));
            countries.Add(new Country("cl", "56", "Chile"));
            countries.Add(new Country("cn", "86", "China"));
            countries.Add(new Country("cx", "61", "Weihnachtsinsel"));
            countries.Add(new Country("cc", "61", "Cocos (Keeling) Islands"));
            countries.Add(new Country("co", "57", "Kolumbien"));
            countries.Add(new Country("km", "269", "Komoren"));
            countries.Add(new Country("cg", "242", "Kongo"));
            countries.Add(new Country("cd", "243", "Kongo, Demokratische Republik der"));
            countries.Add(new Country("ck", "682", "Cookinseln"));
            countries.Add(new Country("cr", "506", "Costa Rica"));
            countries.Add(new Country("hr", "385", "Kroatien"));
            countries.Add(new Country("cu", "53", "Kuba"));
            countries.Add(new Country("cy", "357", "Zypern"));
            countries.Add(new Country("cz", "420", "Tschechien"));
            countries.Add(new Country("dk", "45", "Dänemark"));
            countries.Add(new Country("dj", "253", "Dschibuti"));
            countries.Add(new Country("tl", "670", "Timor-Leste"));
            countries.Add(new Country("ec", "593", "Ecuador"));
            countries.Add(new Country("eg", "20", "Ägypten"));
            countries.Add(new Country("sv", "503", "El Salvador"));
            countries.Add(new Country("gq", "240", "Äquatorialguinea"));
            countries.Add(new Country("er", "291", "Eritrea"));
            countries.Add(new Country("ee", "372", "Estland"));
            countries.Add(new Country("et", "251", "Äthiopien"));
            countries.Add(new Country("fk", "500", "Falklandinseln (Malvinas)"));
            countries.Add(new Country("fo", "298", "Färöer Inseln"));
            countries.Add(new Country("fj", "679", "Fidschi"));
            countries.Add(new Country("fi", "358", "Finnland"));
            countries.Add(new Country("fr", "33", "Frankreich"));
            countries.Add(new Country("pf", "689", "Französisch Polynesien"));
            countries.Add(new Country("ga", "241", "Gabun"));
            countries.Add(new Country("gm", "220", "Gambia"));
            countries.Add(new Country("ge", "995", "Georgia"));
            countries.Add(new Country("de", "49", "Deutschland"));
            countries.Add(new Country("gh", "233", "Ghana"));
            countries.Add(new Country("gi", "350", "Gibraltar"));
            countries.Add(new Country("gr", "30", "Griechenland"));
            countries.Add(new Country("gl", "299", "Grönland"));
            countries.Add(new Country("gt", "502", "Guatemala"));
            countries.Add(new Country("gn", "224", "Guinea"));
            countries.Add(new Country("gw", "245", "Guinea-Bissaus"));
            countries.Add(new Country("gy", "592", "Guyana"));
            countries.Add(new Country("ht", "509", "Haiti"));
            countries.Add(new Country("hn", "504", "Honduras"));
            countries.Add(new Country("hk", "852", "Hongkong"));
            countries.Add(new Country("hu", "36", "Ungarn"));
            countries.Add(new Country("in", "91", "Indien"));
            countries.Add(new Country("id", "62", "Indonesien"));
            countries.Add(new Country("ir", "98", "Iran, Islamische Republik"));
            countries.Add(new Country("iq", "964", "Irak"));
            countries.Add(new Country("ie", "353", "Irland"));
            countries.Add(new Country("im", "44", "Isle of Man"));
            countries.Add(new Country("il", "972", "Israel"));
            countries.Add(new Country("it", "39", "Italien"));
            countries.Add(new Country("ci", "225", "Côte D & apos; ivoire"));
            countries.Add(new Country("jp", "81", "Japan"));
            countries.Add(new Country("jo", "962", "Jordanien"));
            countries.Add(new Country("kz", "7", "Kasachstan"));
            countries.Add(new Country("ke", "254", "Kenia"));
            countries.Add(new Country("ki", "686", "Kiribati"));
            countries.Add(new Country("kw", "965", "Kuwait"));
            countries.Add(new Country("kg", "996", "Kirgisistan"));
            countries.Add(new Country("la", "856", "Lao Menschen & apos; s Demokratischen Republik"));
            countries.Add(new Country("lv", "371", "Lettland"));
            countries.Add(new Country("lb", "961", "Libanon"));
            countries.Add(new Country("ls", "266", "Lesotho"));
            countries.Add(new Country("lr", "231", "Liberia"));
            countries.Add(new Country("ly", "218", "Libyen"));
            countries.Add(new Country("li", "423", "Liechtenstein"));
            countries.Add(new Country("lt", "370", "Litauen"));
            countries.Add(new Country("lu", "352", "Luxemburg"));
            countries.Add(new Country("mo", "853", "Macao"));
            countries.Add(new Country("mk", "389", "Mazedonien, die ehemalige jugoslawische Republik"));
            countries.Add(new Country("mg", "261", "Madagaskar"));
            countries.Add(new Country("mw", "265", "Malawi"));
            countries.Add(new Country("my", "60", "Malaysia"));
            countries.Add(new Country("mv", "960", "Malediven"));
            countries.Add(new Country("ml", "223", "Mali"));
            countries.Add(new Country("mt", "356", "Malta"));
            countries.Add(new Country("mh", "692", "Marshallinseln"));
            countries.Add(new Country("mr", "222", "Mauretanien"));
            countries.Add(new Country("mu", "230", "Mauritius"));
            countries.Add(new Country("yt", "262", "Mayotte"));
            countries.Add(new Country("mx", "52", "Mexiko"));
            countries.Add(new Country("fm", "691", "Mikronesien, Föderierte Staaten von"));
            countries.Add(new Country("md", "373", "Moldawien, Republik"));
            countries.Add(new Country("mc", "377", "Monaco"));
            countries.Add(new Country("mn", "976", "Mongolei"));
            countries.Add(new Country("me", "382", "Montenegro"));
            countries.Add(new Country("ma", "212", "Marokko"));
            countries.Add(new Country("mz", "258", "Mosambik"));
            countries.Add(new Country("na", "264", "Namibia"));
            countries.Add(new Country("nr", "674", "Nauru"));
            countries.Add(new Country("np", "977", "Nepal"));
            countries.Add(new Country("nl", "31", "Niederlande"));
            countries.Add(new Country("nc", "687", "Neu-Kaledonien"));
            countries.Add(new Country("nz", "64", "Neuseeland"));
            countries.Add(new Country("ni", "505", "Nicaragua"));
            countries.Add(new Country("ne", "227", "Niger"));
            countries.Add(new Country("ng", "234", "Nigeria"));
            countries.Add(new Country("nu", "683", "Niue"));
            countries.Add(new Country("kp", "850", "Korea"));
            countries.Add(new Country("no", "47", "Norwegen"));
            countries.Add(new Country("om", "968", "Oman"));
            countries.Add(new Country("pk", "92", "Pakistan"));
            countries.Add(new Country("pw", "680", "Palau"));
            countries.Add(new Country("pa", "507", "Panama"));
            countries.Add(new Country("pg", "675", "Papua-Neuguinea"));
            countries.Add(new Country("py", "595", "Paraguay"));
            countries.Add(new Country("pe", "51", "Peru"));
            countries.Add(new Country("ph", "63", "Philippinen"));
            countries.Add(new Country("pn", "870", "Pitcairn"));
            countries.Add(new Country("pl", "48", "Polen"));
            countries.Add(new Country("pt", "351", "Portugal"));
            countries.Add(new Country("pr", "1", "Puerto Rico"));
            countries.Add(new Country("qa", "974", "Katar"));
            countries.Add(new Country("ro", "40", "Rumänien"));
            countries.Add(new Country("ru", "7", "Russische Föderation"));
            countries.Add(new Country("rw", "250", "Ruanda"));
            countries.Add(new Country("bl", "590", "saint Barthélemy"));
            countries.Add(new Country("ws", "685", "Samoa"));
            countries.Add(new Country("sm", "378", "San Marino"));
            countries.Add(new Country("st", "239", "Sao Tome und Principe"));
            countries.Add(new Country("sa", "966", "Saudi Arabien"));
            countries.Add(new Country("sn", "221", "Senegal"));
            countries.Add(new Country("rs", "381", "Serbien"));
            countries.Add(new Country("sc", "248", "Seychellen"));
            countries.Add(new Country("sl", "232", "Sierra Leone"));
            countries.Add(new Country("sg", "65", "Singapur"));
            countries.Add(new Country("sk", "421", "Slowakei"));
            countries.Add(new Country("si", "386", "Slowenien"));
            countries.Add(new Country("sb", "677", "Salomon-Inseln"));
            countries.Add(new Country("so", "252", "Somalia"));
            countries.Add(new Country("za", "27", "Südafrika"));
            countries.Add(new Country("kr", "82", "Korea, Republik von"));
            countries.Add(new Country("es", "34", "Spanien"));
            countries.Add(new Country("lk", "94", "Sri Lanka"));
            countries.Add(new Country("sh", "290", "St. Helena, Ascension und Tristan da Cunha"));
            countries.Add(new Country("pm", "508", "St. Pierre und Miquelon"));
            countries.Add(new Country("sd", "249", "Sudan"));
            countries.Add(new Country("sr", "597", "Suriname"));
            countries.Add(new Country("sz", "268", "Swasiland"));
            countries.Add(new Country("se", "46", "Schweden"));
            countries.Add(new Country("ch", "41", "Schweiz"));
            countries.Add(new Country("sy", "963", "Syrische Arabische Republik"));
            countries.Add(new Country("tw", "886", "Taiwan, Province of China"));
            countries.Add(new Country("tj", "992", "Tadschikistan"));
            countries.Add(new Country("tz", "255", "Tansania, Vereinigte Republik"));
            countries.Add(new Country("th", "66", "Thailand"));
            countries.Add(new Country("tg", "228", "Gehen"));
            countries.Add(new Country("tk", "690", "Tokelau"));
            countries.Add(new Country("to", "676", "Tonga"));
            countries.Add(new Country("tn", "216", "Tunesien"));
            countries.Add(new Country("tr", "90", "Truthahn"));
            countries.Add(new Country("tm", "993", "Turkmenistan"));
            countries.Add(new Country("tv", "688", "Tuvalu"));
            countries.Add(new Country("ae", "971", "Vereinigte Arabische Emirate"));
            countries.Add(new Country("ug", "256", "Uganda"));
            countries.Add(new Country("gb", "44", "Großbritannien"));
            countries.Add(new Country("ua", "380", "Ukraine"));
            countries.Add(new Country("uy", "598", "Uruguay"));
            countries.Add(new Country("us", "1", "Vereinigte Staaten"));
            countries.Add(new Country("uz", "998", "Usbekistan"));
            countries.Add(new Country("vu", "678", "Vanuatu"));
            countries.Add(new Country("va", "39", "Heiliger Stuhl (Vatikanstadt)"));
            countries.Add(new Country("ve", "58", "Venezuela, Bolivarische Republik"));
            countries.Add(new Country("vn", "84", "Vietnam"));
            countries.Add(new Country("wf", "681", "Wallis und Futuna"));
            countries.Add(new Country("ye", "967", "Jemen"));
            countries.Add(new Country("zm", "260", "Sambia"));
            countries.Add(new Country("zw", "263", "Simbabwe"));
            return countries;
        }
    }
}