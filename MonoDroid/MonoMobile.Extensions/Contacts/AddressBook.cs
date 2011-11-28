using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Android.Content;
using Android.Content.Res;
using Android.Database;
using Android.Provider;

namespace Xamarin.Contacts
{
	/* TODODODODO
	 * SecurityException bubbling
	 */

	public class AddressBook
		: IQueryable<Contact>
	{
		public AddressBook (Context context)
		{
			if (context == null)
				throw new ArgumentNullException ("context");

			this.content = context.ContentResolver;
			this.resources = context.Resources;
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public IEnumerator<Contact> GetEnumerator()
		{
			return GetContacts().GetEnumerator();
		}

		/// <summary>
		/// Attempts to load a contact for the specified <paramref name="id"/>.
		/// </summary>
		/// <param name="id"></param>
		/// <returns>The <see cref="Contact"/> if found, <c>null</c> otherwise.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="id"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException"><paramref name="id"/> is empty.</exception>
		public Contact Load (string id)
		{
			if (id == null)
				throw new ArgumentNullException ("id");
			if (id.Trim() == String.Empty)
				throw new ArgumentException ("Invalid ID", "id");

			ICursor c = null;
			try
			{
				c = this.content.Query (ContactsContract.Contacts.ContentUri, null, ContactsContract.ContactsColumns.LookupKey + " = ?", new[] { id }, null);
				return (c.MoveToNext() ? GetContact (c) : null);
			}
			finally
			{
				if (c != null)
					c.Deactivate();
			}
		}

		//public Contact SaveNew (Contact contact)
		//{
		//    if (contact == null)
		//        throw new ArgumentNullException ("contact");
		//    if (contact.Id != null)
		//        throw new ArgumentException ("Contact is not new", "contact");

		//    throw new NotImplementedException();
		//}

		//public Contact SaveExisting (Contact contact)
		//{
		//    if (contact == null)
		//        throw new ArgumentNullException ("contact");
		//    if (String.IsNullOrWhiteSpace (contact.Id))
		//        throw new ArgumentException ("Contact is not existing");

		//    return Load (contact.Id);
		//}

		//public Contact Save (Contact contact)
		//{
		//    if (contact == null)
		//        throw new ArgumentNullException ("contact");

		//    return (String.IsNullOrWhiteSpace (contact.Id) ? SaveNew (contact) : SaveExisting (contact));
		//}

		//public void Delete (Contact contact)
		//{
		//    if (contact == null)
		//        throw new ArgumentNullException ("contact");
		//    if (!String.IsNullOrWhiteSpace (contact.Id))
		//        throw new ArgumentException ("Contact is not a persisted instance", "contact");

		//    this.content.Delete (ContactsContract.Data.ContentUri, ContactsContract.ContactsColumns.LookupKey + " = ?", new[] { contact.Id });
		//}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		Type IQueryable.ElementType
		{
			get { return typeof (Contact); }
		}

		Expression IQueryable.Expression
		{
			get { return GetContacts().AsQueryable().Expression; }
		}

		IQueryProvider IQueryable.Provider
		{
			get { return GetContacts().AsQueryable().Provider; }
		}

		private readonly ContentResolver content;
		private readonly Resources resources;

		private IEnumerable<Contact> GetContacts()
		{
			ICursor c = null;

			try
			{
				c = this.content.Query (ContactsContract.RawContacts.ContentUri, null, null, null, null);
				while (c.MoveToNext())
					yield return GetContact (c);
			}
			finally
			{
				if (c != null)
					c.Deactivate();
			}
		}

		private Contact GetContact (ICursor cursor)
		{
			//string id = cursor.GetString (cursor.GetColumnIndex (ContactsContract.ContactsColumns.LookupKey));
			string id = cursor.GetString (cursor.GetColumnIndex (ContactsContract.RawContactsColumns.ContactId));
			Contact contact = new Contact (id);
			contact.DisplayName = GetString (cursor, ContactsContract.ContactsColumns.DisplayName);
			FillContactExtras (id, contact);

			return contact;
		}

		private void FillContactExtras (string recordId, Contact contact)
		{
			ICursor c = null;

			List<Phone> phones = new List<Phone>();
			List<Email> emails = new List<Email>();
			List<string> notes = new List<string>();

			try
			{
				c = this.content.Query (ContactsContract.Data.ContentUri, null, ContactsContract.DataColumns.RawContactId + " = ?",
				                        new[] { recordId }, null);
				while (c.MoveToNext())
				{
					string dataType = c.GetString (c.GetColumnIndex (ContactsContract.DataColumns.Mimetype));
					switch (dataType)
					{
						case ContactsContract.CommonDataKinds.Nickname.ContentItemType:
							contact.Nickname = c.GetString (c.GetColumnIndex (ContactsContract.CommonDataKinds.Nickname.Name));
							break;

						case ContactsContract.CommonDataKinds.StructuredName.ContentItemType:
							contact.Prefix = GetString (c, ContactsContract.CommonDataKinds.StructuredName.Prefix);
							contact.FirstName = GetString (c, ContactsContract.CommonDataKinds.StructuredName.GivenName);
							contact.MiddleName = GetString (c, ContactsContract.CommonDataKinds.StructuredName.MiddleName);
							contact.LastName = GetString (c, ContactsContract.CommonDataKinds.StructuredName.FamilyName);
							contact.Suffix = GetString (c, ContactsContract.CommonDataKinds.StructuredName.Suffix);
							break;

						case ContactsContract.CommonDataKinds.Phone.ContentItemType:
							Phone p = new Phone();

							PhoneDataKind pkind = (PhoneDataKind) c.GetInt (c.GetColumnIndex (ContactsContract.CommonDataKinds.CommonColumns.Type));
							p.Type = GetPhoneType (pkind);
							if (p.Type == PhoneType.Custom)
								p.CustomLabel = GetString (c, ContactsContract.CommonDataKinds.CommonColumns.Label);

							p.Label = (p.Type != PhoneType.Custom)
							          	? ContactsContract.CommonDataKinds.Phone.GetTypeLabel (this.resources, pkind, p.CustomLabel)
							          	: p.CustomLabel;

							p.Number = GetString (c, ContactsContract.CommonDataKinds.Phone.Number);

							phones.Add (p);
							break;

						case ContactsContract.CommonDataKinds.Email.ContentItemType:
							Email e = new Email();

							EmailDataKind ekind = (EmailDataKind) c.GetInt (c.GetColumnIndex (ContactsContract.CommonDataKinds.CommonColumns.Type));
							e.Type = GetEmailType (ekind);
							if (e.Type == EmailType.Custom)
								e.CustomLabel = GetString (c, ContactsContract.CommonDataKinds.CommonColumns.Label);

							e.Label = (e.Type != EmailType.Custom)
							          	? ContactsContract.CommonDataKinds.Email.GetTypeLabel (this.resources, ekind, e.CustomLabel)
							          	: e.CustomLabel;

							e.Address = GetString (c, ContactsContract.DataColumns.Data1);

							emails.Add (e);
							break;

						case ContactsContract.CommonDataKinds.Note.ContentItemType:
							notes.Add (GetString (c, ContactsContract.CommonDataKinds.Note.NoteColumnId));
							break;
					}
				}

				contact.Phones = phones;
				contact.Emails = emails;
				contact.Notes = notes;
			}
			finally
			{
				if (c != null)
					c.Deactivate();
			}
		}

		private static EmailType GetEmailType (EmailDataKind emailKind)
		{
			switch (emailKind)
			{
				case 0:
					return EmailType.Custom;
				case EmailDataKind.Home:
					return EmailType.Home;
				case EmailDataKind.Mobile:
					return EmailType.Mobile;
				case EmailDataKind.Work:
					return EmailType.Work;
				default:
					return EmailType.Other;
			}
		}

		private static PhoneType GetPhoneType (PhoneDataKind phoneKind)
		{
			switch (phoneKind)
			{
				case 0:
					return PhoneType.Custom;
				case PhoneDataKind.Home:
					return PhoneType.Home;
				case PhoneDataKind.FaxHome:
					return PhoneType.HomeFax;
				case PhoneDataKind.Work:
					return PhoneType.Work;
				case PhoneDataKind.FaxWork:
					return PhoneType.WorkFax;

				default:
					return PhoneType.Other;
			}
		}

		private static string GetString (ICursor c, string colName)
		{
			return c.GetString (c.GetColumnIndex (colName));
		}
	}
}