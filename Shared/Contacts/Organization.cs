namespace Xamarin.Contacts
{
	public enum OrganizationType
	{
		Work,
		Other
	}

	public class Organization
	{
		public OrganizationType Type
		{
			get;
			set;
		}

		public string Label
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string ContactTitle
		{
			get;
			set;
		}
	}
}