using System;
using Xamarin.Contacts;
using MonoTouch.UIKit;
using System.Text;
using System.Globalization;

namespace ContactsSample
{
	public class DetailContactView : UIViewController
	{
		Contact contact;
		UILabel outputLabel;
		public DetailContactView (Contact c)
		{
			this.contact = c;
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			outputLabel = new UILabel(this.View.Frame);
			outputLabel.Lines = 0;
			this.View.AddSubview(outputLabel);
			
			StringBuilder sb = new StringBuilder();
			
			//
			// contact name
			//
			sb.AppendLine(contact.DisplayName);
			
			foreach(var phone in contact.Phones)
			{
				sb.AppendLine(String.Format("{0}: {1}", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(phone.Label), phone.Number));
			}
			
			foreach(var email in contact.Emails)
			{
				sb.AppendLine(String.Format("{0}: {1}", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(email.Label), email.Address));
			}
			
			foreach(var organization in contact.Organizations)
			{
				sb.AppendLine(String.Format("{0} ({1}): {2}", organization.Name, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(organization.Label), organization.ContactTitle));
			}
			
			outputLabel.Text = sb.ToString();
		}
	}
}

