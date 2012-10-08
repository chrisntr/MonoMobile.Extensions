using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;

namespace Xamarin.Contacts
{
	public sealed class AddressBook
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

		public Task<bool> RequestPermission()
		{
			return Task<bool>.Factory.StartNew (() =>
			{
				try
				{
					var contacts = new Microsoft.Phone.UserData.Contacts();
					contacts.Accounts.ToArray(); // Will trigger exception if manifest doesn't specify ID_CAP_CONTACTS
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			}, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
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
