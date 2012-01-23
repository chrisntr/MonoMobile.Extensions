using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MonoTouch.AddressBook;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Runtime.InteropServices;
using Xamarin.Media;

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

		public UIImage GetThumbnail()
		{
			if (!this.person.HasImage)
				return null;

			IntPtr data;
			lock (this.person)
				data = ABPersonCopyImageDataWithFormat (person.Handle, ABPersonImageFormat.Thumbnail);

			if (data == IntPtr.Zero)
				return null;

			return new UIImage (data);
		}

		public Task<MediaFile> SaveThumbnail (string path)
		{
			if (path == null)
				throw new ArgumentNullException ("path");

			return Task<MediaFile>.Factory.StartNew (s =>
			{
				string p = (string) s;

				using (UIImage img = GetThumbnail())
				using (NSDataStream stream = new NSDataStream(img.AsJPEG()))
				using (Stream writeStream = File.Create (p))
				{
					byte[] buffer = new byte[20480];
					int len;
					while ((len = stream.Read (buffer, 0, buffer.Length)) != 0)
						writeStream.Write (buffer, 0, len);

					writeStream.Flush();
				}

				return new MediaFile (p, () => File.OpenRead (p));
			}, path);
		}

		private readonly ABPerson person;

		[DllImport ("/System/Library/Frameworks/AddressBook.framework/AddressBook")]
		private static extern IntPtr ABPersonCopyImageDataWithFormat (IntPtr handle, ABPersonImageFormat format);
	}
}