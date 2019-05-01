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
using Android.Support.V7.Widget;
using Android.Views.InputMethods;

namespace SeekiosApp.Droid.CustomComponents.CountryCode
{
    public class CountryCodeAdapter : Android.Support.V7.Widget.RecyclerView.Adapter
    {
        List<Country> filteredCountries = null, masterCountries = null;
        TextView textView_noResult;
        CountryCodePicker codePicker;
        LayoutInflater inflater;
        EditText editText_search;
        Dialog dialog;
        Context context;

        public CountryCodeAdapter(Context context, List<Country> countries, CountryCodePicker codePicker, EditText editText_search, TextView textView_noResult, Dialog dialog)
        {
            this.context = context;
            this.masterCountries = countries;
            this.codePicker = codePicker;
            this.dialog = dialog;
            this.textView_noResult = textView_noResult;
            this.editText_search = editText_search;
            this.inflater = LayoutInflater.From(context);
            SetTextWatcher();
            this.filteredCountries = GetFilteredCountries("");
        }

        private void SetTextWatcher()
        {
            if (this.editText_search != null)
            {
                this.editText_search.TextChanged += (v1, v2) =>
                {
                    ApplyQuery(v2.Text.ToString());
                };

                if (codePicker.KeyboardAutoPopOnSearch)
                {
                    InputMethodManager inputMethodManager = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
                    if (inputMethodManager != null)
                    {
                        inputMethodManager.ToggleSoftInput(InputMethodManager.ShowForced, 0);
                    }
                }
            }
        }

        private void ApplyQuery(String query)
        {
            textView_noResult.Visibility = ViewStates.Gone;
            query = query.ToLower();

            if (query.Count() > 0 && query[0] == '+')
            {
                query = query.Substring(1);
            }

            filteredCountries = GetFilteredCountries(query);

            if (filteredCountries.Count == 0)
            {
                textView_noResult.Visibility = ViewStates.Visible;
            }
            NotifyDataSetChanged();
        }

        private List<Country> GetFilteredCountries(string query)
        {
            List<Country> tempCountryList = new List<Country>();
            if (codePicker.PreferredCountries != null && codePicker.PreferredCountries.Count > 0)
            {
                foreach (var country in masterCountries)
                {
                    if (country.IsEligibleForQuery(query))
                    {
                        tempCountryList.Add(country);
                    }
                }
                if (tempCountryList.Count > 0)
                {
                    Country divider = null;
                    tempCountryList.Add(divider);
                }
            }
            foreach (var country in masterCountries)
            {
                if (country.IsEligibleForQuery(query))
                {
                    tempCountryList.Add(country);
                }
            }
            return tempCountryList;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var getholder = (CountryCodeViewHolder)holder;
            getholder.SetCountry(filteredCountries[position]);
            getholder.GetMainView().Click += (o, e) =>
            {
                codePicker.SetSelectedCountry(filteredCountries[holder.AdapterPosition]);
                dialog.Dismiss();
            };
        }

        public override int ItemCount
        {
            get
            {
                return filteredCountries.Count;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            Android.Views.View rootView = inflater.Inflate(Resource.Layout.LayoutRecyclerCountryTile, parent, false);
            CountryCodeViewHolder viewHolder = new CountryCodeViewHolder(rootView);
            return viewHolder;
        }
    }

    public class CountryCodeViewHolder : Android.Support.V7.Widget.RecyclerView.ViewHolder
    {
        RelativeLayout relativeLayout_main;
        TextView textView_name, textView_code;
        Android.Views.View divider;
        public CountryCodeViewHolder(Android.Views.View itemView) : base(itemView)
        {
            relativeLayout_main = (RelativeLayout)itemView;
            textView_name = (TextView)relativeLayout_main.FindViewById(Resource.Id.textView_countryName);
            textView_code = (TextView)relativeLayout_main.FindViewById(Resource.Id.textView_code);
            divider = (Android.Views.View)relativeLayout_main.FindViewById(Resource.Id.preferenceDivider);
        }

        public void SetCountry(Country country)
        {
            if (country != null)
            {
                divider.Visibility = ViewStates.Gone;
                textView_name.Visibility = ViewStates.Visible;
                textView_code.Visibility = ViewStates.Visible;
                textView_name.Text = country.Name + " (" + country.NameCode.ToUpper() + ")";
                textView_code.Text = "+" + country.PhoneCode;
            }
            else
            {
                divider.Visibility = ViewStates.Visible;
                textView_name.Visibility = ViewStates.Gone;
                textView_code.Visibility = ViewStates.Gone;
            }
        }

        public RelativeLayout GetMainView()
        {
            return relativeLayout_main;
        }
    }
}

