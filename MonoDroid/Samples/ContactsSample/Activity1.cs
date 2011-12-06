using System;
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
				builder.AppendLine (contact.DisplayName);

				if (contact.Organizations.Any())
				{
					builder.AppendLine();
					builder.AppendLine ("Organizations:");
					foreach (Organization o in contact.Organizations)
					{
						builder.Append (o.Label + ": " + o.Name);
						if (o.ContactTitle != null)
							builder.Append (", " + o.ContactTitle);

						builder.AppendLine();
					}
				}

				if (contact.Phones.Any())
				{
					builder.AppendLine();
					builder.AppendLine ("Phones:");
					foreach (Phone p in contact.Phones)
						builder.AppendLine (p.Label + ": " + p.Number);
				}

				if (contact.Emails.Any())
				{
					builder.AppendLine();
					builder.AppendLine ("Emails:");
					foreach (Email e in contact.Emails)
						builder.AppendLine (e.Label + ": " + e.Address);
				}

				if (contact.Addresses.Any())
				{
					builder.AppendLine();
					builder.AppendLine ("Addresses:");
					foreach (Address a in contact.Addresses)
					{
						builder.AppendLine();
						builder.AppendLine (a.Label + ":");
						builder.AppendLine (a.StreetAddress);
						builder.AppendLine (String.Format ("{0}, {1} {2}", a.City, a.Region, a.PostalCode));
						builder.AppendLine (a.Country);
					}
				}

				if (contact.InstantMessagingAccounts.Any())
				{
					builder.AppendLine();
					builder.AppendLine ("Instant Messaging Accounts:");
					foreach (InstantMessagingAccount imAccount in contact.InstantMessagingAccounts)
						builder.AppendLine (String.Format ("{0}: {1}", imAccount.ServiceLabel, imAccount.Account));
				}

				if (contact.Notes.Any())
				{
					builder.AppendLine();
					builder.AppendLine ("Notes:");
					foreach (string n in contact.Notes)
						builder.AppendLine (" - " + n);
				}

				builder.AppendLine();
			}

			FindViewById<TextView> (Resource.Id.contacts).Text = builder.ToString();
		}
	}
}