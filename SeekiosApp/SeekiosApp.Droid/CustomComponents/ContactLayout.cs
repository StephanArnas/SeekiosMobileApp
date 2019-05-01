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
using Android.Util;
using SeekiosApp.Model.APP;
using XamSvg;
using Android.Graphics;
using SeekiosApp.Model.DTO;
using SeekiosApp.Droid.View;
using SeekiosApp.Droid.Helper;
using SeekiosApp.Droid.View.FragmentView;

namespace SeekiosApp.Droid.CustomComponents
{
    public class ContactLayout : LinearLayout
    {
        #region ===== Constructeur(s) =============================================================

        public ContactLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { ContactInit(); }
        public ContactLayout(Context context) : this(context, null) { ContactInit(); }
        public ContactLayout(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }
        public ContactLayout(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { ContactInit(); }

        #endregion

        #region ===== Méthodes Privées ============================================================

        private void ContactInit()
        {
            var emptyTextView = FindViewById(12);
            var emptyTextView2 = FindViewById(13);
            if (emptyTextView == null && emptyTextView2 == null)
            {
                var contactTextView = new TextView(Context);
                contactTextView.Id = 12;
                var layoutParamTextView = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                layoutParamTextView.Gravity = GravityFlags.CenterVertical;
                layoutParamTextView.LeftMargin = AccessResources.Instance.SizeOf20Dip();
                contactTextView.Text = Resources.GetString(Resource.String.alert_contactEmpty);
                contactTextView.SetTextColor(Color.ParseColor(Resources.GetString(Resource.Color.textColorHint)));
                contactTextView.SetTextSize(ComplexUnitType.Dip, 16);
                contactTextView.LayoutParameters = layoutParamTextView;
                SetBackgroundColor(Color.Transparent);

                contactTextView.Click += (o, e) =>
                {
                    EmptyClick?.Invoke(null, null);
                };

                AddView(contactTextView);
            }
        }

        #endregion

        #region ===== Public Methodes =============================================================

        /// <summary>
        /// Ajoute un contact à l'élément parent (LinearLayout)
        /// </summary>
        /// <param name="contact">contact à ajouter</param>
        public void AddChild(Contact contact)
        {
            if (contact == null) return;
            var emptyTextView = FindViewById(12);
            if (emptyTextView != null) RemoveView(emptyTextView);

            // layout for displaying the name and the phone number or email
            var numberEmailLayout = new LinearLayout(Context);
            var layoutParammainLayout = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
            numberEmailLayout.Clickable = true;
            numberEmailLayout.Orientation = Orientation.Vertical;
            numberEmailLayout.SetBackgroundColor(Color.Transparent);
            numberEmailLayout.LayoutParameters = layoutParammainLayout;

            // layout that contains all the element
            var rootLayout = new LinearLayout(Context);
            var layoutParamLayout = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
            rootLayout.Clickable = true;
            rootLayout.Orientation = Orientation.Horizontal;
            rootLayout.SetBackgroundColor(Color.Transparent);
            rootLayout.SetPadding(AccessResources.Instance.SizeOf5Dip()
                , AccessResources.Instance.SizeOf5Dip()
                , AccessResources.Instance.SizeOf5Dip()
                , AccessResources.Instance.SizeOf5Dip());
            rootLayout.LayoutParameters = layoutParamLayout;

            // display the name
            var contactTextView = new TextView(Context);
            contactTextView.Id = 13;
            var layoutParamTextView = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            layoutParamTextView.Gravity = GravityFlags.CenterVertical;
            contactTextView.Text = contact.DisplayName;
            contactTextView.SetTextColor(Color.ParseColor(Resources.GetString(Resource.Color.textColorContent)));
            contactTextView.SetTextSize(ComplexUnitType.Dip, 16);
            contactTextView.LayoutParameters = layoutParamTextView;
            contactTextView.Clickable = false;

            // delete button
            var buttonDeleteContact = new SvgImageView(Context);
            var layoutParamButton = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            layoutParamButton.Gravity = GravityFlags.Top;
            layoutParamButton.Height = AccessResources.Instance.SizeOf20Dip();
            layoutParamButton.Width = AccessResources.Instance.SizeOf20Dip();
            layoutParamButton.SetMargins(AccessResources.Instance.SizeOf10Dip(), AccessResources.Instance.SizeOf5Dip(), AccessResources.Instance.SizeOf10Dip(), 0);
            buttonDeleteContact.SetSvg(Context, Resource.Drawable.RoundedDelete);
            buttonDeleteContact.LayoutParameters = layoutParamButton;
            buttonDeleteContact.Click += ((o, e) =>
            {
                App.Locator.Alert.LsRecipients.RemoveAll(el => el.DisplayName.TrimEnd(' ') == contact.DisplayName);
                App.Locator.AlertSOS.LsRecipients.RemoveAll(el => el.DisplayName.TrimEnd(' ') == contact.DisplayName);
                RemoveView(rootLayout);
                ContactInit();
            });

            // display the phone number or the email
            var phoneEmailTextView = new TextView(Context);
            var layoutParamTxtView = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            layoutParamTxtView.Gravity = GravityFlags.CenterVertical;
            phoneEmailTextView.SetPadding(0, AccessResources.Instance.SizeOf5Dip(), 0, 0);
            phoneEmailTextView.Text = contact.Email;
            phoneEmailTextView.SetTextColor(Color.ParseColor(Resources.GetString(Resource.Color.textColorContent)));
            phoneEmailTextView.SetTextSize(ComplexUnitType.Dip, 14);
            phoneEmailTextView.LayoutParameters = layoutParamTxtView;
            phoneEmailTextView.Clickable = false;

            numberEmailLayout.AddView(contactTextView);
            numberEmailLayout.AddView(phoneEmailTextView);

            rootLayout.AddView(numberEmailLayout);
            rootLayout.AddView(buttonDeleteContact);

            AddView(rootLayout);
        }

        #endregion

        #region ===== Public Methodes =============================================================

        public event EventHandler EmptyClick;

        #endregion
    }
}