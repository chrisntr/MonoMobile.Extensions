using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Xamarin.Media;

namespace Xamarin.Contacts
{
	public class Contact
	{
		private readonly Microsoft.Phone.UserData.Contact contact;

		public Contact()
		{
		}

		internal Contact (Microsoft.Phone.UserData.Contact contact)
		{
			this.contact = contact;
		}

		public string Id
		{
			get { return null; }
		}

		public bool IsAggregate
		{
			get;
			private set;
		}

		public string DisplayName
		{
			get;
			set;
		}

		public string Prefix
		{
			get;
			set;
		}

		public string FirstName
		{
			get;
			set;
		}

		public string MiddleName
		{
			get;
			set;
		}

		public string LastName
		{
			get;
			set;
		}

		public string Nickname
		{
			get;
			set;
		}

		public string Suffix
		{
			get;
			set;
		}

		internal List<Address> addresses = new List<Address>();
		public IEnumerable<Address> Addresses
		{
			get { return this.addresses; }
			set { this.addresses = new List<Address> (value); }
		}

		internal List<InstantMessagingAccount> instantMessagingAccounts = new List<InstantMessagingAccount>();
		public IEnumerable<InstantMessagingAccount> InstantMessagingAccounts
		{
			get { return this.instantMessagingAccounts; }
			set { this.instantMessagingAccounts = new List<InstantMessagingAccount> (value); }
		}

		internal List<Website> websites = new List<Website>();
		public IEnumerable<Website> Websites
		{
			get { return this.websites; }
			set { this.websites = new List<Website> (value); }
		}

		internal List<Organization> organizations = new List<Organization>();
		public IEnumerable<Organization> Organizations
		{
			get { return this.organizations; }
			set { this.organizations = new List<Organization> (value); }
		}

		internal List<Note> notes = new List<Note>();
		public IEnumerable<Note> Notes
		{
			get { return this.notes; }
			set { this.notes = new List<Note> (value); }
		}

		internal List<Email> emails = new List<Email>();
		public IEnumerable<Email> Emails
		{
			get { return this.emails; }
			set { this.emails = new List<Email> (value); }
		}

		internal List<Phone> phones = new List<Phone>();
		public IEnumerable<Phone> Phones
		{
			get { return this.phones; }
			set { this.phones = new List<Phone> (value); }
		}

		internal List<Relationship> relationships = new List<Relationship>();
		public IEnumerable<Relationship> Relationships
		{
			get { return this.relationships; }
			set { this.relationships = new List<Relationship> (value); }
		}

		public Task<MediaFile> SaveThumbnail (string path)
		{
			throw new NotImplementedException();
		}

		public BitmapImage GetThumbnail()
		{
			Stream s = this.contact.GetPicture();
			if (s == null)
				return null;

			var image = new BitmapImage();
			image.SetSource (s);
			return image;
		}
	}
}