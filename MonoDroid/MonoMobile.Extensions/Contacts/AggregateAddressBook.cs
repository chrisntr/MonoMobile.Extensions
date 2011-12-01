using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Android.Content;

namespace Xamarin.Contacts
{
	public class AggregateAddressBook
		: IOrderedQueryable<Contact>
	{
		public AggregateAddressBook (Context context)
		{
			if (context == null)
				throw new ArgumentNullException ("context");

			this.provider = new ContactQueryProvider (false, context.ContentResolver, context.Resources);
		}

		public IEnumerator<Contact> GetEnumerator()
		{
			throw new NotImplementedException();
			//return ContactHelper.GetContacts(false, this.)
		}

		private readonly IQueryProvider provider;

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
			get { return Expression.Constant (this); }
		}

		IQueryProvider IQueryable.Provider
		{
			get { return this.provider; }
		}
	}
}