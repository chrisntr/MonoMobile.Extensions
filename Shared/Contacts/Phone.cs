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
		internal Phone()
		{
		}

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

		public string Number
		{
			get;
			internal set;
		}
	}
}