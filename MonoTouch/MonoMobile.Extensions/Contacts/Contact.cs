using System.Collections.Generic;
using MonoTouch.AddressBook;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Xamarin.Contacts
{
	public class Contact
	{
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
			internal set;
		}

		public string Prefix
		{
			get;
			internal set;
		}

		public string FirstName
		{
			get;
			internal set;
		}

		public string MiddleName
		{
			get;
			internal set;
		}

		public string LastName
		{
			get;
			internal set;
		}

		public string Nickname
		{
			get;
			internal set;
		}

		public string Suffix
		{
			get;
			internal set;
		}

		public IEnumerable<Address> Addresses
		{
			get;
			internal set;
		}

		public IEnumerable<InstantMessagingAccount> InstantMessagingAccounts
		{
			get;
			internal set;
		}
		
		public IEnumerable<Website> Websites
		{
			get;
			internal set;
		}

		public IEnumerable<Organization> Organizations
		{
			get;
			internal set;
		}

		public IEnumerable<Note> Notes
		{
			get;
			internal set;
		}

		public IEnumerable<Email> Emails
		{
			get;
			internal set;
		}

		public IEnumerable<Phone> Phones
		{
			get;
			internal set;
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
			if (this.thumbnailLoaded)
				return;

			this.thumbnailLoaded = false;

			if (!this.person.HasImage)
				return;

			NSData imageData = new NSData (ABPersonCopyImageDataWithFormat (person.Handle, ABPersonImageFormat.Thumbnail));
			if (imageData != null)
				this.thumbnail = new UIImage (imageData);
		}
	}
}