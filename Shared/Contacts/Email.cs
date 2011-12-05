using System;
using System.Net.Mail;

namespace Xamarin.Contacts
{
	public enum EmailType
	{
		Custom,
		Home,
		Mobile,
		Work,
		Other
	}

	public class Email
	{
		internal Email()
		{
		}

		public EmailType Type
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

		public string Address
		{
			get;
			internal set;
		}

		//public string Address
		//{
		//    get
		//    {
		//        if (this.address == null)
		//            return null;

		//        return this.address.ToString();
		//    }

		//    set
		//    {
		//        if (String.IsNullOrWhiteSpace (value))
		//        {
		//            this.address = null;
		//            return;
		//        }

		//        this.address = new MailAddress (value);
		//    }
		//}

		//public override string ToString()
		//{
		//    return Address;
		//}

		//private MailAddress address;

		//public static bool TryParse (string address, out Email email)
		//{
		//    email = new Email();
		//    try
		//    {
		//        email.Address = address;
		//        return true;
		//    }
		//    catch (Exception)
		//    {
		//        email = null;
		//        return false;
		//    }
		//}
	}
}