﻿//
//  Copyright 2011-2013, Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

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
