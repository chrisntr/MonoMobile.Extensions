namespace Xamarin.Contacts
{
	public enum PhoneType
	{
		Custom,
		Home,
		HomeFax,
		Work,
		WorkFax,
		Pager,
		Mobile,
		Other,
	}

	public class Phone
	{
		public PhoneType Type
		{
			get;
			internal set;
		}

		public string Label
		{
			get;
			internal set;
		}

		public string CustomLabel
		{
			get;
			internal set;
		}

		public string Number
		{
			get;
			internal set;
		}
	}
}