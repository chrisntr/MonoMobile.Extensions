using System;
using System.Linq;
using MonoTouch.AddressBook;
using MonoTouch.Foundation;
using System.Globalization;

namespace Xamarin.Contacts
{
	internal static class ContactHelper
	{
		internal static Contact GetContact (ABPerson person)
		{
			Contact contact = new Contact (person)
			{
				DisplayName = person.ToString(),
				Prefix = person.Prefix,
				FirstName = person.FirstName,
				MiddleName = person.MiddleName,
				LastName = person.LastName,
				Suffix = person.Suffix,
				Nickname = person.Nickname
			};
			
			contact.Notes = (person.Note != null) ? new [] { new Note { Contents = person.Note } } : new Note[0];

			contact.Emails = person.GetEmails().Select (e => new Email
			{
				Address = e.Value,
				Type = GetEmailType (e.Label),
				Label = (e.Label != null) ? GetLabel (e.Label) : GetLabel (ABLabel.Other)
			}).ToArray();
			
			contact.Phones = person.GetPhones().Select (p => new Phone
			{
				Number = p.Value,
				Type = GetPhoneType (p.Label),
				Label = GetLabel (p.Label)
			}).ToArray();
			
			Organization[] orgs;
			if (person.Organization != null)
			{
				orgs = new Organization[1];
				orgs[0] = new Organization
				{
					Name = person.Organization,
					ContactTitle = person.JobTitle,
					Type = OrganizationType.Work,
					Label = GetLabel (ABLabel.Work)
				};
			}
			else
				orgs = new Organization[0];
			
			contact.Organizations = orgs;
			
			return contact;
		}

		internal static string GetLabel (NSString label)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase (ABAddressBook.LocalizedLabel (label));
		}

		internal static EmailType GetEmailType (string label)
		{
			if (label == ABLabel.Home)
				return EmailType.Home;
			if (label == ABLabel.Work)
				return EmailType.Work;

			return EmailType.Other;
		}
		
		internal static PhoneType GetPhoneType (string label)
		{
			if (label == ABLabel.Home)
				return PhoneType.Home;
			if (label == ABLabel.Work)
				return PhoneType.Work;
			if (label == ABPersonPhoneLabel.Mobile || label == ABPersonPhoneLabel.iPhone)
				return PhoneType.Mobile;
			if (label == ABPersonPhoneLabel.Pager)
				return PhoneType.Pager;
			if (label == ABPersonPhoneLabel.HomeFax)
				return PhoneType.HomeFax;
			if (label == ABPersonPhoneLabel.WorkFax)
				return PhoneType.WorkFax;
			
			return PhoneType.Other;
		}
	}
}

