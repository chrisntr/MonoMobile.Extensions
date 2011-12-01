using System;
using MonoTouch.UIKit;
using System.Drawing;
using Xamarin.Contacts;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace ContactsSample
{
	public class MainView : UIViewController
	{
		UITableView tableView;
		List<string> list;
		
		public MainView ()
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			list = new List<string>();
            
			//
			// grab the contacts and put them into a list
			//
			var addressBook = new AddressBook();
			foreach (Contact contact in addressBook)
			{
				list.Add(contact.DisplayName);
			}
			
			//
			// create a tableview and use the list as the datasource
			//
			tableView = new UITableView()
            {
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
		
		//
		// simple table view data source impl
		//
		private class TableViewDataSource : UITableViewDataSource
        {
            static NSString cellIdentifier =
                new NSString ("contactIdentifier");
            private List<string> list;

            public TableViewDataSource (List<string> list)
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
                        UITableViewCellStyle.Default,
                        cellIdentifier);
                }
                cell.TextLabel.Text = list[indexPath.Row];
                return cell;
            }
        }

	}
}

