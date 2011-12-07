using System;
using Xamarin.Contacts;
using MonoTouch.UIKit;
using System.Text;
using System.Globalization;
using System.Drawing;

namespace ContactsSample
{
	public class DetailContactView : UIViewController
	{
		Contact contact;
		UIImageView contactImage;
		UILabel nameLabel;
		UILabel phoneLabel;
		UILabel emailLabel;
		public DetailContactView (Contact c)
		{
			this.contact = c;
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			View.BackgroundColor = UIColor.White;
			
			contactImage = new UIImageView(new RectangleF(210, 5, 60, 60));
			this.View.AddSubview(contactImage);
			contactImage.Image = contact.PhotoThumbnail;
			
			nameLabel = new UILabel(new RectangleF(5, 5, 200, 30));
			this.View.AddSubview(nameLabel);
			nameLabel.Text = contact.DisplayName;
			
			phoneLabel = new UILabel(new RectangleF(5, 40, 200, 30));
			this.View.AddSubview(phoneLabel);
			
			String phoneString = String.Empty; 
			foreach(var phone in contact.Phones)
			{
				phoneString = String.Format("{0}: {1}", phone.Label, phone.Number);
				break; //just take the first phone number in this example
			}
			
			phoneLabel.Text = phoneString;
			
			emailLabel = new UILabel(new RectangleF(5, 80, 200, 30));
			this.View.AddSubview(emailLabel);
			
			String emailString = String.Empty; 
			foreach(var email in contact.Emails)
			{
				emailString = String.Format("{0}: {1}", email.Label, email.Address);
				break; //just take the first email in this example
			}
			
			emailLabel.Text = emailString;
			
		}
	}
}

