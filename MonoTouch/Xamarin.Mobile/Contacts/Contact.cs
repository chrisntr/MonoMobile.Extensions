using System.Collections.Generic;
using MonoTouch.AddressBook;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Runtime.InteropServices;

namespace Xamarin.Contacts
{
	public class Contact
	{
		public Contact()
		{
		}
		
		internal Contact (ABPerson person)
		{
			Id = person.Id.ToString();
			this.person = person;
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

		internal List<InstantMessagingAccount> imAccounts = new List<InstantMessagingAccount>();
		public IEnumerable<InstantMessagingAccount> InstantMessagingAccounts
		{
			get { return this.imAccounts; }
			set { this.imAccounts = new List<InstantMessagingAccount> (value); }
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

		public UIImage GetPhotoThumbnail()
		{
			LoadThumbnail();
			return this.thumbnail;
		}

		private readonly ABPerson person;

		private bool thumbnailLoaded;
		private UIImage thumbnail;

		[DllImport ("/System/Library/Frameworks/AddressBook.framework/AddressBook")]
		private static extern IntPtr ABPersonCopyImageDataWithFormat (IntPtr handle, ABPersonImageFormat format);

		private void LoadThumbnail()
		{
			if (this.thumbnailLoaded || this.person == null)
				return;

			this.thumbnailLoaded = true;

			if (!this.person.HasImage)
				return;

			NSData imageData = new NSData (ABPersonCopyImageDataWithFormat (person.Handle, ABPersonImageFormat.Thumbnail));
			if (imageData != null)
				this.thumbnail = new UIImage (imageData);
		}
	}
}