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
using SeekiosApp.Droid.CustomComponents.CountryCode;

namespace SeekiosApp.Droid.CustomComponents.CountryCode
{
    public class CountryCodeDialog
    {
        public static void OpenCountryCodeDialog(CountryCodePicker codePicker)
        {
            Context context = codePicker.Context;

            Dialog dialog = new Dialog(context, Resource.Style.Theme_AppCompat_Light_Dialog);

            codePicker.RefreshCustomMasterList();
            codePicker.RefreshPreferredCountries();
            List<Country> masterCountries = Country.GetCustomMasterCountryList(codePicker);
            //dialog.RequestWindowFeature(WindowFeatures.NoTitle);
            dialog.Window.SetContentView(Resource.Layout.LayoutPickerDialog);
            RecyclerView recyclerView_countryDialog = (RecyclerView)dialog.FindViewById(Resource.Id.recycler_countryDialog);
            
            TextView textViewTitle = (TextView)dialog.FindViewById(Resource.Id.textView_title);
            textViewTitle.Text = codePicker.GetDialogTitle();
            EditText editText_search = (EditText)dialog.FindViewById(Resource.Id.editText_search);
            editText_search.Hint = codePicker.GetSearchHintText();
            TextView textView_noResult = (TextView)dialog.FindViewById(Resource.Id.textView_noresult);
            textView_noResult.Text = codePicker.GetNoResultFoundText();
            CountryCodeAdapter cca = new CountryCodeAdapter(context, masterCountries, codePicker, editText_search, textView_noResult, dialog);
            recyclerView_countryDialog.SetLayoutManager(new LinearLayoutManager(context));
            recyclerView_countryDialog.SetAdapter(cca);
            
            dialog.Show();
        }
    }
}