using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Contacts;

namespace ContactsSample
{
	[Activity (Label = "ContactActivity")]			
	public class ContactActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			var displayName = bundle.GetString("displayName");
			//var mobilePhone = bundle.GetString("mobilePhone");
			
			SetContentView (Resource.Layout.contact_view);
			
			var fullNameTextView = FindViewById<TextView> (Resource.Id.full_name);
			//var mobilePhoneTextView = FindViewById<TextView> (Resource.Id.mobile_phone);
			
			fullNameTextView.Text = displayName;
			//mobilePhoneTextView.Text = mobilePhone;
		}
	}
}

