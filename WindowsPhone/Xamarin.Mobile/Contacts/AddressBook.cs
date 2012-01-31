using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Xamarin.Contacts
{
	public class AddressBook
		: IQueryable<Contact>
	{
		public AddressBook()
		{
			this.provider = new ContactQueryProvider();
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public bool SingleContactsSupported
		{
			get { return false; }
		}

		public bool AggregateContactsSupported
		{
			get { return true; }
		}

		public bool PreferContactAggregation
		{
			get;
			set;
		}

		public bool LoadSupported
		{
			get { return false; }
		}

		public Contact Load (string id)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<Contact> GetEnumerator()
		{
			return this.provider.GetContacts().GetEnumerator();
		}

		private readonly ContactQueryProvider provider;

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		Expression IQueryable.Expression
		{
			get { return Expression.Constant (this); }
		}

		Type IQueryable.ElementType
		{
			get { return typeof (Contact); }
		}

		IQueryProvider IQueryable.Provider
		{
			get { return this.provider; }
		}
	}
}
