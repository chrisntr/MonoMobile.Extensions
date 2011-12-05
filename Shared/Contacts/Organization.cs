namespace Xamarin.Contacts
{
	public enum OrganizationType
	{
		Work,
		Other
	}

	public class Organization
	{
		internal Organization()
		{
		}

		public OrganizationType Type
		{
			get;
			internal set;
		}

		public string Label
		{
			get;
			internal set;
		}

		public string Name
		{
			get;
			internal set;
		}

		public string ContactTitle
		{
			get;
			internal set;
		}
	}
}