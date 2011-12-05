using System;
using System.Linq.Expressions;
using System.Reflection;
using Android.Provider;

namespace Xamarin.Contacts
{
	internal static class AndroidContactExtensions
	{
		internal static EmailType ToEmailType (this EmailDataKind emailKind)
		{
			switch (emailKind)
			{
				case 0:
					return EmailType.Custom;
				case EmailDataKind.Home:
					return EmailType.Home;
				case EmailDataKind.Work:
					return EmailType.Work;
				default:
					return EmailType.Other;
			}
		}

		internal static PhoneType ToPhoneType (this PhoneDataKind phoneKind)
		{
			switch (phoneKind)
			{
				case 0:
					return PhoneType.Custom;
				case PhoneDataKind.Home:
					return PhoneType.Home;
				case PhoneDataKind.Mobile:
					return PhoneType.Mobile;
				case PhoneDataKind.FaxHome:
					return PhoneType.HomeFax;
				case PhoneDataKind.Work:
					return PhoneType.Work;
				case PhoneDataKind.FaxWork:
					return PhoneType.WorkFax;
				case PhoneDataKind.Pager:
				case PhoneDataKind.WorkPager:
					return PhoneType.Pager;
				default:
					return PhoneType.Other;
			}
		} 
	}
}