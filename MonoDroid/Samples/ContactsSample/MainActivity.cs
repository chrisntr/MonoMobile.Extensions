using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.Database;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Widget;
using Xamarin.Contacts;
using System.Linq;
using System.Collections.Generic;

namespace ContactsSample
{
	[Activity(Label = "ContactsSample", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : ListActivity
	{
		List<String> contacts = new List<String>();

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			
			//
			// get the address book, which contains a list of contacts
			//
			var book = new AddressBook (this);
			
			//
			// loop through the contacts and put them into a List<String>
			//
			// Note that the contacts are ordered by last name - contacts can be selected and sorted using linq!
			// A more performant solution would create a custom adapter to lazily pull the contacts
			//
			foreach (Contact contact in book.Where (c => c.Phones.Any() || c.Emails.Any()).OrderBy (c => c.LastName))
			{
				contacts.Add(contact.DisplayName);
			}
			
			ListAdapter = new ArrayAdapter<string> (this, Resource.Layout.list_item, contacts.ToArray());

		    ListView.TextFilterEnabled = true;
		
			ListView.ItemClick += delegate (object sender, ItemEventArgs args) {
		        //
				// When clicked, start a new activity to display more contact details
				//
				String displayName = ((TextView)args.View).Text;
				//String mobilePhone = ((TextView)args.View).Text;
				
				Intent showContactDetails = new Intent(this, typeof(ContactActivity));
				showContactDetails.PutExtra("displayName", displayName);
				//showContactDetails.PutExtra("mobilePhone", mobilePhone);
				StartActivity(showContactDetails);
				
				//
				// alternatively, show a toast
				//
				//Toast.MakeText (Application, ((TextView)args.View).Text, ToastLength.Short).Show ();
		    };
		}
	}
}