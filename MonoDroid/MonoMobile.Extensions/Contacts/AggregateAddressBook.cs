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
	public class AggregateAddressBook
		: IQueryable<Contact>
	{
		public AggregateAddressBook (Context context)
		{
			if (context == null)
				throw new ArgumentNullException ("context");

			this.provider = new ContactQueryProvider (false, context.ContentResolver, context.Resources);
			this.content = context.ContentResolver;
			this.resources = context.Resources;
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
				return (c.MoveToNext() ? ContactHelper.GetContact (true, this.content, this.resources, c) : null);
			}
			finally
			{
				if (c != null)
					c.Deactivate();
			}
		}

		public IEnumerator<Contact> GetEnumerator()
		{
			throw new NotImplementedException();
			//return ContactHelper.GetContacts(false, this.)
		}

		private readonly IQueryProvider provider;
		private readonly ContentResolver content;
		private readonly Resources resources;

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