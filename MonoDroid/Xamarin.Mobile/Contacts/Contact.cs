using System.Collections.Generic;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.Provider;

namespace Xamarin.Contacts
{
	public class Contact
	{
		public Contact()
		{
		}

		internal Contact (string id, bool isAggregate, ContentResolver content)
		{
			this.content = content;
			IsAggregate = isAggregate;
			Id = id;
		}

		public string Id
		{
			get;
			private set;
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

		internal List<Relationship> relationships = new List<Relationship>();
		public IEnumerable<Relationship> Relationships
		{
			get { return this.relationships; }
			set { this.relationships = new List<Relationship> (value); }
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

		public Bitmap GetPhotoThumbnail()
		{
			LoadThumbnail();

			return this.thumbnail;
		}

		private readonly ContentResolver content;

		private bool thubnailLoaded;
		private Bitmap thumbnail;

		private void LoadThumbnail()
		{
			if (this.thubnailLoaded || this.content == null)
				return;

			this.thubnailLoaded = true;

			string lookupColumn = (IsAggregate)
			                      	? ContactsContract.ContactsColumns.LookupKey
			                      	: ContactsContract.RawContactsColumns.ContactId;

			ICursor c = null;
			try
			{
				c = this.content.Query (ContactsContract.Data.ContentUri, new[] { ContactsContract.CommonDataKinds.Photo.PhotoColumnId, ContactsContract.DataColumns.Mimetype },
					lookupColumn + "=? AND " + ContactsContract.DataColumns.Mimetype + "=?", new[] { Id, ContactsContract.CommonDataKinds.Photo.ContentItemType }, null);

				while (c.MoveToNext())
				{
					byte[] tdata = c.GetBlob (c.GetColumnIndex (ContactsContract.CommonDataKinds.Photo.PhotoColumnId));
					if (tdata != null)
					{
						this.thumbnail = BitmapFactory.DecodeByteArray (tdata, 0, tdata.Length);
						break;
					}
				}
			}
			finally
			{
				if (c != null)
					c.Close();
			}
		}
	}
}