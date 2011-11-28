using System.Collections.Generic;

namespace Xamarin.Contacts
{
	public class MetaContact
		: Contact
	{
		public IEnumerable<Account> Accounts
		{
			get;
			set;
		}
	}
}