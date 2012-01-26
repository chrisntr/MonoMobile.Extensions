using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Xamarin.Contacts;

namespace ContactsSample
{
	public class MainPageViewModel
	{
		public IEnumerable<ContactViewModel> Contacts
		{
			get { return addressBook.Select (c => new ContactViewModel (c)); }
		}

		public class ContactViewModel
		{
			private readonly Contact contact;

			public ContactViewModel (Contact contact)
			{
				this.contact = contact;
			}

			public BitmapImage Photo
			{
				get { return this.contact.GetThumbnail(); }
			}

			public string DisplayName
			{
				get { return this.contact.DisplayName; }
			}

			public string PhoneNumber
			{
				get
				{
					Phone p = this.contact.Phones.FirstOrDefault();
					if (p == null)
						return null;

					return p.Number;
				}
			}

			public string EmailAddress
			{
				get
				{
					Email e = this.contact.Emails.FirstOrDefault();
					if (e == null)
						return null;

					return e.Address;
				}
			}
		}

		private readonly AddressBook addressBook = new AddressBook();
	}
}
