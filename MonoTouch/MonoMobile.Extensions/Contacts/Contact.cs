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

		public IEnumerable<Address> Addresses
		{
			get;
			set;
		}

		public IEnumerable<InstantMessagingAccount> InstantMessagingAccounts
		{
			get;
			set;
		}
		
		public IEnumerable<Website> Websites
		{
			get;
			set;
		}

		public IEnumerable<Organization> Organizations
		{
			get;
			set;
		}

		public IEnumerable<Note> Notes
		{
			get;
			set;
		}

		public IEnumerable<Email> Emails
		{
			get;
			set;
		}

		public IEnumerable<Phone> Phones
		{
			get;
			set;
		}
		
		public UIImage PhotoThumbnail
		{
			get
			{
				LoadThumbnail();
				return this.thumbnail;
			}
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