using System;
using Contacts;
using ContactsUI;

namespace SeekiosApp.iOS
{
	public class ContactPickerDelegate : CNContactPickerDelegate
	{
        #region ===== Constructor =================================================================

        public ContactPickerDelegate() { }

		public ContactPickerDelegate(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Public Overrides Methodes ===================================================

        public override void ContactPickerDidCancel(CNContactPickerViewController picker)
		{
			// Raise the selection canceled event
			RaiseSelectionCanceled();
		}

		public override void DidSelectContact(CNContactPickerViewController picker, CNContact contact)
		{
			// Raise the contact selected event
			RaiseContactSelected(contact);
		}

		public override void DidSelectContactProperty(CNContactPickerViewController picker, CNContactProperty contactProperty)
		{
			// Raise the contact property selected event
			RaiseContactPropertySelected(contactProperty);
		}

		public override void DidSelectContacts(CNContactPickerViewController picker, CNContact[] contacts)
		{
			foreach (var contact in contacts)
            {
                Console.WriteLine("Selected: {0}", contact);
            }

            // Raise the contact selected event
            RaiseContactsSelected(picker,contacts);
		}

        #endregion

        #region ===== Event =======================================================================

        public delegate void SelectionCanceledDelegate();
		public event SelectionCanceledDelegate SelectionCanceled;

		internal void RaiseSelectionCanceled()
		{
            SelectionCanceled?.Invoke();
        }

		public delegate void ContactsSelectedDelegate(CNContactPickerViewController picker, CNContact[] contacts);
		public event ContactsSelectedDelegate ContactsSelected;

		internal void RaiseContactsSelected(CNContactPickerViewController picker, CNContact[] contacts)
		{
            ContactsSelected?.Invoke(picker,contacts);
        }

		public delegate void ContactSelectedDelegate(CNContact contact);
		public event ContactSelectedDelegate ContactSelected;

		internal void RaiseContactSelected(CNContact contact)
		{
			ContactSelected?.Invoke(contact);
		}

		public delegate void ContactPropertySelectedDelegate(CNContactProperty property);
		public event ContactPropertySelectedDelegate ContactPropertySelected;

		internal void RaiseContactPropertySelected(CNContactProperty property)
		{
            ContactPropertySelected?.Invoke(property);
        }

		#endregion
	}
}
