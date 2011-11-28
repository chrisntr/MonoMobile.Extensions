using System;
using System.Linq;
using System.Linq.Expressions;
using MonoTouch.AddressBook;
using System.Collections.Generic;
using System.Collections;

namespace Xamarin.Contacts
{
	public class AddressBook
		: IQueryable<Contact>
	{
		public AddressBook()
		{			
		}
		
		public bool IsReadOnly
		{
			get { return true; }
		}
		
		public IEnumerator<Contact> GetEnumerator()
		{
			return GetContacts().GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		public Contact Load (string id)
		{
			throw new NotImplementedException();
		}
		
		Type IQueryable.ElementType
		{
			get { return typeof(Contact); }
		}
		
		Expression IQueryable.Expression
		{
			get { return GetContacts().AsQueryable().Expression; }
		}
		
		IQueryProvider IQueryable.Provider
		{
			get { return GetContacts().AsQueryable().Provider; }
		}
		
		private readonly ABAddressBook addressBook = new ABAddressBook();
		
		private IEnumerable<Contact> GetContacts()
		{
			foreach (ABPerson person in this.addressBook.GetPeople())
				yield return GetContact (person);
		}
		
		private Contact GetContact (ABPerson person)
		{
			Contact contact = new Contact (person.Id.ToString())
			{
				Prefix = person.Prefix,
				FirstName = person.FirstName,
				MiddleName = person.MiddleName,
				LastName = person.LastName,
				Suffix = person.Suffix,
				Nickname = person.Nickname
			};
			
			contact.Notes = (person.Note != null) ? new [] { person.Note } : new string[0];

			// TODO: Fill Label
			contact.Emails = person.GetEmails().Select (e => new Email { Address = e.Value, Type = EmailType.Other }).ToArray();
			contact.Phones = person.GetPhones().Select (p => new Phone { Number = p.Value, Type = GetPhoneType (p) }).ToArray();
			
			return contact;
		}
		
		private PhoneType GetPhoneType (ABMultiValueEntry<string> phone)
		{
			if (phone.Label == ABLabel.Home)
				return PhoneType.Home;
			if (phone.Label == ABLabel.Work)
				return PhoneType.Work;
			if (phone.Label == ABPersonPhoneLabel.Mobile || phone.Label == ABPersonPhoneLabel.iPhone)
				return PhoneType.Mobile;
			if (phone.Label == ABPersonPhoneLabel.Pager)
				return PhoneType.Pager;
			if (phone.Label == ABPersonPhoneLabel.HomeFax)
				return PhoneType.HomeFax;
			if (phone.Label == ABPersonPhoneLabel.WorkFax)
				return PhoneType.WorkFax;
			
			return PhoneType.Other;
		}
	}
}