using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Database;
using Android.Net;
using Android.Provider;

namespace Xamarin.Contacts
{
	internal static class ContactHelper
	{
		public static IEnumerable<Contact> GetContacts (bool rawContacts, ContentResolver content, Resources resources)
		{
			ICursor c = null;

			Uri curi = (rawContacts)
						? ContactsContract.RawContacts.ContentUri
						: ContactsContract.Contacts.ContentUri;

			try
			{
				c = content.Query (curi, null, null, null, null);
				while (c.MoveToNext())
					yield return GetContact (rawContacts, content, resources, c);
			}
			finally
			{
				if (c != null)
					c.Close();
			}
		}

		public static Contact GetContact (bool rawContact, ContentResolver content, Resources resources, ICursor cursor)
		{
			string id = (rawContact)
							? cursor.GetString (cursor.GetColumnIndex (ContactsContract.RawContactsColumns.ContactId))
							: cursor.GetString (cursor.GetColumnIndex (ContactsContract.ContactsColumns.LookupKey));

			Contact contact = new Contact (id, !rawContact, content);
			contact.DisplayName = GetString (cursor, ContactsContract.ContactsColumns.DisplayName);

			FillContactExtras (rawContact, content, resources, id, contact);

			return contact;
		}

		public static void FillContactExtras (bool rawContact, ContentResolver content, Resources resources, string recordId, Contact contact)
		{
			ICursor c = null;

			List<Phone> phones = new List<Phone>();
			List<Email> emails = new List<Email>();
			List<string> notes = new List<string>();

			string column = (rawContact)
			                	? ContactsContract.RawContactsColumns.ContactId
			                	: ContactsContract.ContactsColumns.LookupKey;

			try
			{
				c = content.Query (ContactsContract.Data.ContentUri, null, column + " = ?", new[] { recordId }, null);
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
							p.Type = pkind.ToPhoneType();
							if (p.Type == PhoneType.Custom)
								p.CustomLabel = GetString (c, ContactsContract.CommonDataKinds.CommonColumns.Label);

							p.Label = (p.Type != PhoneType.Custom)
							          	? ContactsContract.CommonDataKinds.Phone.GetTypeLabel (resources, pkind, p.CustomLabel)
							          	: p.CustomLabel;

							p.Number = GetString (c, ContactsContract.CommonDataKinds.Phone.Number);

							phones.Add (p);
							break;

						case ContactsContract.CommonDataKinds.Email.ContentItemType:
							Email e = new Email();

							EmailDataKind ekind = (EmailDataKind) c.GetInt (c.GetColumnIndex (ContactsContract.CommonDataKinds.CommonColumns.Type));
							e.Type = ekind.ToEmailType();
							if (e.Type == EmailType.Custom)
								e.CustomLabel = GetString (c, ContactsContract.CommonDataKinds.CommonColumns.Label);

							e.Label = (e.Type != EmailType.Custom)
							          	? ContactsContract.CommonDataKinds.Email.GetTypeLabel (resources, ekind, e.CustomLabel)
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
					c.Close();
			}
		}

		public static string GetString (this ICursor c, string colName)
		{
			return c.GetString (c.GetColumnIndex (colName));
		}
	}
}