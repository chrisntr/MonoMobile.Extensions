namespace Xamarin.Contacts
{
	public enum PhoneType
	{
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
			set;
		}

		public string Label
		{
			get;
			set;
		}

		public string Number
		{
			get;
			set;
		}
	}
}