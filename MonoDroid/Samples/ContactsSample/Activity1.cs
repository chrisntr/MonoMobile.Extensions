using System.Text;
using Android.App;
using Android.Database;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Widget;
using Xamarin.Contacts;
using System.Linq;

namespace ContactsSample
{
	[Activity(Label = "ContactsSample", MainLauncher = true, Icon = "@drawable/icon")]
	public class Activity1 : Activity
	{
		int count = 1;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			var book = new AddressBook (this);
			
			StringBuilder builder = new StringBuilder();
			foreach (Contact contact in book.Where (c => c.Phones.Any() || c.Emails.Any()).OrderBy (c => c.LastName))
			{
				builder.AppendLine ("Display name: " + contact.DisplayName);
				builder.AppendLine ("Nickname: " + contact.Nickname);
				builder.AppendLine ("Prefix: " + contact.Prefix);
				builder.AppendLine ("First name: " + contact.FirstName);
				builder.AppendLine ("Middle name: " + contact.MiddleName);
				builder.AppendLine ("Last name: " + contact.LastName);
				builder.AppendLine ("Suffix:" + contact.Suffix);

				builder.AppendLine();
				builder.AppendLine ("Phones:");
				foreach (Phone p in contact.Phones)
					builder.AppendLine (p.Label + ": " + p.Number);

				builder.AppendLine();
				builder.AppendLine ("Emails:");
				foreach (Email e in contact.Emails)
					builder.AppendLine (e.Label + ": " + e.Address);

				builder.AppendLine();
				builder.AppendLine ("Notes:");
				foreach (string n in contact.Notes)
					builder.AppendLine (" - " + n);

				builder.AppendLine();
			}

			FindViewById<TextView> (Resource.Id.contacts).Text = builder.ToString();
		}
	}
}