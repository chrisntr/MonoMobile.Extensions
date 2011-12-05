using System.Collections.Generic;
using MonoTouch.AddressBook;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Xamarin.Contacts
{
	public partial class Contact
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

		public IEnumerable<string> Notes
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

		private void LoadThumbnail()
		{
			if (this.thumbnailLoaded)
				return;

			this.thumbnailLoaded = false;

			if (!this.person.HasImage)
				return;

			using (NSData imageData = this.person.Image)
			{
				if (imageData != null)
					this.thumbnail = new UIImage (imageData);
			}
		}
	}
}