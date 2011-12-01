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
	/* TODO
	 * SecurityException bubbling
	 */

	public class AddressBook
		: IQueryable<Contact>
	{
		public AddressBook (Context context)
		{
			if (context == null)
				throw new ArgumentNullException ("context");

			this.content = context.ContentResolver;
			this.resources = context.Resources;
			this.rawContactsProvider = new ContactQueryProvider (true, context.ContentResolver, context.Resources);
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public IEnumerator<Contact> GetEnumerator()
		{
			return ContactHelper.GetContacts (true, this.content, this.resources).GetEnumerator();
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
				c = this.content.Query (ContactsContract.RawContacts.ContentUri, null, ContactsContract.RawContactsColumns.ContactId + " = ?", new[] { id }, null);
				return (c.MoveToNext() ? ContactHelper.GetContact (true, this.content, this.resources, c) : null);
			}
			finally
			{
				if (c != null)
					c.Deactivate();
			}
		}

		//public Contact SaveNew (Contact contact)
		//{
		//    if (contact == null)
		//        throw new ArgumentNullException ("contact");
		//    if (contact.Id != null)
		//        throw new ArgumentException ("Contact is not new", "contact");

		//    throw new NotImplementedException();
		//}

		//public Contact SaveExisting (Contact contact)
		//{
		//    if (contact == null)
		//        throw new ArgumentNullException ("contact");
		//    if (String.IsNullOrWhiteSpace (contact.Id))
		//        throw new ArgumentException ("Contact is not existing");

		//    throw new NotImplementedException();

		//    return Load (contact.Id);
		//}

		//public Contact Save (Contact contact)
		//{
		//    if (contact == null)
		//        throw new ArgumentNullException ("contact");

		//    return (String.IsNullOrWhiteSpace (contact.Id) ? SaveNew (contact) : SaveExisting (contact));
		//}

		//public void Delete (Contact contact)
		//{
		//    if (contact == null)
		//        throw new ArgumentNullException ("contact");
		//    if (!String.IsNullOrWhiteSpace (contact.Id))
		//        throw new ArgumentException ("Contact is not a persisted instance", "contact");

		//    // TODO: Does this cascade?
		//    this.content.Delete (ContactsContract.RawContacts.ContentUri, ContactsContract.RawContactsColumns.ContactId + " = ?", new[] { contact.Id });
		//}

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
			get { return this.rawContactsProvider; }
		}

		private readonly IQueryProvider rawContactsProvider;
		private readonly ContentResolver content;
		private readonly Resources resources;
	}
}