using System.Collections.Generic;

namespace Xamarin.Contacts
{
	public partial class Contact
	{
		public Contact()
		{
		}

		internal Contact (string id)
		{
			Id = id;
		}

		public string Id
		{
			get;
			private set;
		}

		public string DisplayName
		{
			get;
			internal set;
		}

		public string Prefix
		{
			get;
			internal set;
		}

		public string FirstName
		{
			get;
			internal set;
		}

		public string MiddleName
		{
			get;
			internal set;
		}

		public string LastName
		{
			get;
			internal set;
		}

		public string Nickname
		{
			get;
			internal set;
		}

		public string Suffix
		{
			get;
			internal set;
		}

		public IEnumerable<string> Notes
		{
			get;
			internal set;
		}

		public IEnumerable<Email> Emails
		{
			get;
			internal set;
		}

		public IEnumerable<Phone> Phones
		{
			get;
			internal set;
		}
	}
}