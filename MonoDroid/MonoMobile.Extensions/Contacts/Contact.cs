using System;
using System.Collections.Generic;
using System.IO;
using Android.Content;
using Android.Content.Res;
using Android.Database;
using Android.Graphics;
using Android.Net;
using Android.Provider;
using Java.IO;
using Uri = Android.Net.Uri;

namespace Xamarin.Contacts
{
	public class Contact
	{
		public Contact()
		{
		}

		internal Contact (string id, ContentResolver content)
		{
			this.content = content;
			Id = id;
		}

		public string Id
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

		public Bitmap PhotoThumbnail
		{
			get
			{
				LoadThumbnail();

				return this.thumbnail;
			}
		}

		public Bitmap Photo
		{
			get
			{
				LoadPhoto();

				return this.photo;
			}
		}

		private readonly ContentResolver content;
		internal bool isAggregate;

		private bool photoLoaded;
		private Bitmap photo;

		private bool thubnailLoaded;
		private Bitmap thumbnail;

		private void LoadPhoto()
		{
			if (this.photoLoaded)
				return;

			this.photoLoaded = true;

			throw new NotImplementedException();
		}

		private void LoadThumbnail()
		{
			if (this.thubnailLoaded)
				return;

			this.thubnailLoaded = true;

			string lookupColumn = (this.isAggregate)
			                      	? ContactsContract.ContactsColumns.LookupKey
			                      	: ContactsContract.RawContactsColumns.ContactId;

			ICursor c = null;
			try
			{
				c = this.content.Query (ContactsContract.Data.ContentUri, new[] { ContactsContract.CommonDataKinds.Photo.PhotoColumnId, ContactsContract.DataColumns.Mimetype },
					lookupColumn + "=? AND " + ContactsContract.DataColumns.Mimetype + "=?", new[] { Id, ContactsContract.CommonDataKinds.Photo.ContentItemType }, null);

				while (c.MoveToNext())
				{
					byte[] tdata = c.GetBlob (c.GetColumnIndex (ContactsContract.DataColumns.Data15));
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