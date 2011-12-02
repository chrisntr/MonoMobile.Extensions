using System;
using System.Linq;
using System.Linq.Expressions;
using MonoTouch.AddressBook;
using System.Collections.Generic;

namespace Xamarin.Contacts
{
	public class AggregateAddressBook
		: IQueryable<Contact>
	{
		public AggregateAddressBook ()
		{
			this.addressBook = new ABAddressBook();
			this.provider = new ContactQueryProvider (this.addressBook);
		}

		public IEnumerator<Contact> GetEnumerator ()
		{
			return this.addressBook.GetPeople().Select (ContactHelper.GetContact).GetEnumerator();
		}
		
		public Contact Load (string id)
		{
			if (String.IsNullOrWhiteSpace (id))
				throw new ArgumentNullException ("id");
			
			int rowId;
			if (!Int32.TryParse (id, out rowId))
				throw new ArgumentException ("Not a valid contact ID", "id");
			
			ABPerson person = this.addressBook.GetPerson (rowId);
			if (person == null)
				return null;
			
			return ContactHelper.GetContact (person);
		}
		
		private readonly ABAddressBook addressBook;
		private readonly ContactQueryProvider provider;

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}
		
		Type IQueryable.ElementType
		{
			get { return typeof(Contact); }
		}

		Expression IQueryable.Expression
		{
			get { return Expression.Constant (this); }
		}

		IQueryProvider IQueryable.Provider
		{
			get { return this.provider; }
		}
	}
}