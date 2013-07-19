//
//  Copyright 2011-2013, Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.Provider;
using Xamarin.Media;

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

		public Bitmap GetThumbnail()
		{
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
						return BitmapFactory.DecodeByteArray (tdata, 0, tdata.Length);
				}
			}
			finally
			{
				if (c != null)
					c.Close();
			}

			return null;
		}

		public Task<MediaFile> SaveThumbnailAsync (string path)
		{
			if (path == null)
				throw new ArgumentNullException ("path");

			string lookupColumn = (IsAggregate)
									? ContactsContract.ContactsColumns.LookupKey
									: ContactsContract.RawContactsColumns.ContactId;

			AsyncQuery<byte[]> query = new AsyncQuery<byte[]> (this.content, c => c.GetBlob (c.GetColumnIndex (ContactsContract.CommonDataKinds.Photo.PhotoColumnId)));
			query.StartQuery (0, null, ContactsContract.Data.ContentUri, new[] { ContactsContract.CommonDataKinds.Photo.PhotoColumnId, ContactsContract.DataColumns.Mimetype },
					lookupColumn + "=? AND " + ContactsContract.DataColumns.Mimetype + "=?", new[] { Id, ContactsContract.CommonDataKinds.Photo.ContentItemType }, null);

			return query.Task.ContinueWith (t =>
			{
				if (t.Result == null)
					return null;

				File.WriteAllBytes (path, t.Result);
				return new MediaFile (path, deletePathOnDispose: false);
			}, TaskScheduler.Default);
		}

		private readonly ContentResolver content;
	}
}