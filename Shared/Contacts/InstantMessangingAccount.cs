namespace Xamarin.Contacts
{
	//public enum InstantMessagingType
	//{
	//    Work,
	//    Home,
	//    Other
	//}

	public enum InstantMessagingService
	{
		Aim,
		Msn,
		Yahoo,
		Icq,
		Jabber,
		Other
	}

	public class InstantMessagingAccount
	{
		public InstantMessagingService Service
		{
			get;
			set;
		}

		public string ServiceLabel
		{
			get;
			set;
		}

		//public InstantMessagingType Type
		//{
		//    get;
		//    internal set;
		//}

		//public string Label
		//{
		//    get;
		//    internal set;
		//}

		public string Account
		{
			get;
			set;
		}
	}
}