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
		internal static IEnumerable<Contact> GetContacts (bool rawContacts, ContentResolver content, Resources resources)
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

		internal static Contact GetContact (bool rawContact, ContentResolver content, Resources resources, ICursor cursor)
		{
			string id = (rawContact)
							? cursor.GetString (cursor.GetColumnIndex (ContactsContract.RawContactsColumns.ContactId))
							: cursor.GetString (cursor.GetColumnIndex (ContactsContract.ContactsColumns.LookupKey));

			Contact contact = new Contact (id, !rawContact, content);
			contact.DisplayName = GetString (cursor, ContactsContract.ContactsColumns.DisplayName);

			FillContactExtras (rawContact, content, resources, id, contact);

			return contact;
		}

		internal static void FillContactExtras (bool rawContact, ContentResolver content, Resources resources, string recordId, Contact contact)
		{
			ICursor c = null;

			List<Phone> phones = new List<Phone>();
			List<Email> emails = new List<Email>();
			List<string> notes = new List<string>();
			List<Organization> organizations = new List<Organization>();

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
							p.Number = GetString (c, ContactsContract.CommonDataKinds.Phone.Number);

							PhoneDataKind pkind = (PhoneDataKind)c.GetInt (c.GetColumnIndex (ContactsContract.CommonDataKinds.CommonColumns.Type));
							p.Type = pkind.ToPhoneType();
							p.Label = ContactsContract.CommonDataKinds.Phone.GetTypeLabel (resources, pkind, GetString (c, ContactsContract.CommonDataKinds.CommonColumns.Label));

							phones.Add (p);
							break;

						case ContactsContract.CommonDataKinds.Email.ContentItemType:
							Email e = new Email();
							e.Address = GetString (c, ContactsContract.DataColumns.Data1);

							EmailDataKind ekind = (EmailDataKind)c.GetInt (c.GetColumnIndex (ContactsContract.CommonDataKinds.CommonColumns.Type));
							e.Type = ekind.ToEmailType();
							e.Label = ContactsContract.CommonDataKinds.Email.GetTypeLabel (resources, ekind, GetString (c, ContactsContract.CommonDataKinds.CommonColumns.Label));

							emails.Add (e);
							break;

						case ContactsContract.CommonDataKinds.Note.ContentItemType:
							notes.Add (GetString (c, ContactsContract.CommonDataKinds.Note.NoteColumnId));
							break;

						case ContactsContract.CommonDataKinds.Organization.ContentItemType:
							Organization o = new Organization();
							o.Name = c.GetString (ContactsContract.CommonDataKinds.Organization.Company);
							o.ContactTitle = c.GetString (ContactsContract.CommonDataKinds.Organization.Title);
							
							OrganizationDataKind d = (OrganizationDataKind)c.GetInt (c.GetColumnIndex (ContactsContract.CommonDataKinds.CommonColumns.Type));
							o.Type = d.ToOrganizationType();
							o.Label = ContactsContract.CommonDataKinds.Organization.GetTypeLabel (resources, d, GetString (c, ContactsContract.CommonDataKinds.CommonColumns.Label));

							organizations.Add (o);

							break;
					}
				}

				contact.Phones = phones;
				contact.Emails = emails;
				contact.Notes = notes;
				contact.Organizations = organizations;
			}
			finally
			{
				if (c != null)
					c.Close();
			}
		}

		internal static string GetString (this ICursor c, string colName)
		{
			return c.GetString (c.GetColumnIndex (colName));
		}

		internal static EmailType ToEmailType (this EmailDataKind emailKind)
		{
			switch (emailKind)
			{
				case EmailDataKind.Home:
					return EmailType.Home;
				case EmailDataKind.Work:
					return EmailType.Work;
				default:
					return EmailType.Other;
			}
		}

		internal static PhoneType ToPhoneType (this PhoneDataKind phoneKind)
		{
			switch (phoneKind)
			{
				case PhoneDataKind.Home:
					return PhoneType.Home;
				case PhoneDataKind.Mobile:
					return PhoneType.Mobile;
				case PhoneDataKind.FaxHome:
					return PhoneType.HomeFax;
				case PhoneDataKind.Work:
					return PhoneType.Work;
				case PhoneDataKind.FaxWork:
					return PhoneType.WorkFax;
				case PhoneDataKind.Pager:
				case PhoneDataKind.WorkPager:
					return PhoneType.Pager;
				default:
					return PhoneType.Other;
			}
		}

		internal static OrganizationType ToOrganizationType (this OrganizationDataKind organizationKind)
		{
			switch (organizationKind)
			{
				case OrganizationDataKind.Work:
					return OrganizationType.Work;

				default:
					return OrganizationType.Other;
			}
		}
	}
}