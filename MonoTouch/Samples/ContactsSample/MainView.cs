using System;
using MonoTouch.UIKit;
using System.Drawing;
using Xamarin.Contacts;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Globalization;

namespace ContactsSample
{
	public class MainView : UIViewController
	{
		UITableView tableView;
		List<Contact> list;
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			this.Title = "Contacts";
			
			list = new List<Contact>();
            
			//
			// grab the contacts and put them into a list
			//
			var addressBook = new AddressBook();
			foreach (Contact contact in addressBook)
			{
				list.Add(contact);
			}
			
			//
			// create a tableview and use the list as the datasource
			//
			tableView = new UITableView()
            {
				Delegate = new TableViewDelegate(this),
                DataSource = new TableViewDataSource(list),
                AutoresizingMask =
                    UIViewAutoresizing.FlexibleHeight|
                    UIViewAutoresizing.FlexibleWidth,
            };
			
			//
			// size the tableview and add it to the parent view
			//
			tableView.SizeToFit();
			tableView.Frame = new RectangleF (
                0, 0, this.View.Frame.Width,
                this.View.Frame.Height);
            this.View.AddSubview(tableView);
		}
		
		private class TableViewDelegate : UITableViewDelegate
		{
			private MainView parent;

            public TableViewDelegate (MainView mainView)
            {
                parent = mainView;
            }
			
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				DetailContactView detailView = new DetailContactView(parent.list[indexPath.Row]);
				parent.NavigationController.PushViewController(detailView, true);
			}	
		}
		
		//
		// simple table view data source impl
		//
		private class TableViewDataSource : UITableViewDataSource
        {
            static NSString cellIdentifier =
                new NSString ("contactIdentifier");
            private List<Contact> list;

            public TableViewDataSource (List<Contact> list)
            {
                this.list = list;
            }

			public override int RowsInSection (
                UITableView tableview, int section)
            {
                return list.Count;
            }

            public override UITableViewCell GetCell (
                UITableView tableView, NSIndexPath indexPath)
            {
                UITableViewCell cell =
                    tableView.DequeueReusableCell (cellIdentifier);
                if (cell == null)
                {
                    cell = new UITableViewCell (
                        UITableViewCellStyle.Subtitle,
                        cellIdentifier);
                }
                cell.TextLabel.Text = list[indexPath.Row].DisplayName;
				Email firstEmail = list[indexPath.Row].Emails.FirstOrDefault();
				if(firstEmail != null)
				{
					cell.DetailTextLabel.Text = String.Format("{0}: {1}", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(firstEmail.Label), firstEmail.Address);
				}
				
                return cell;
            }
        }

	}
}

