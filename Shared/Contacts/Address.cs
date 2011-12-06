namespace Xamarin.Contacts
{
	public enum AddressType
	{
		Work,
		Home,
		Other
	}

	public class Address
	{
		internal Address()
		{
		}

		public AddressType Type
		{
			get;
			internal set;
		}

		public string Label
		{
			get;
			internal set;
		}

		public string StreetAddress
		{
			get;
			internal set;
		}

		public string City
		{
			get;
			internal set;
		}

		public string Region
		{
			get;
			internal set;
		}

		public string Country
		{
			get;
			internal set;
		}

		public string PostalCode
		{
			get;
			internal set;
		}
	}
}