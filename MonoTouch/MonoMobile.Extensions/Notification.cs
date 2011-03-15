using System;
using MonoTouch.UIKit;
namespace MonoMobile.Extensions
{
	public class Notification
	{
		public Notification () 
		{
		}
	
		public void Alert (string message, Action<int> alertCallback)
		{
			Alert(message, alertCallback, "Alert", "OK");
		}

		public void Alert (string message, Action<int> alertCallback, string title)
		{
            Alert(message, alertCallback, title, "OK");
		}

		public void Alert (string message, Action<int> alertCallback, string title, string buttonName)
		{
			using(var alertView = new UIAlertView(title, message, null, null, null))
			{
				var labels = buttonName.Split(new Char[]{','});	
				foreach(var label in labels)
				{
					alertView.AddButton(label);
				}
				alertView.Show();
				alertView.Clicked += delegate(object sender, UIButtonEventArgs e) {
					alertCallback(e.ButtonIndex);
				};
			}
		}

		public void Confirm (string message, Action confirmCallback)
		{
			throw new NotImplementedException ();
		}

		public void Confirm (string message, Action confirmCallback, string title)
		{
			throw new NotImplementedException ();
		}

		public void Confirm (string message, Action confirmCallback, string title, string buttonLabels)
		{
			throw new NotImplementedException ();
		}

		public void Beep ()
		{
			throw new NotImplementedException ();
		}

		public void Beep (int times)
		{
			throw new NotImplementedException ();
		}

		public void Vibrate ()
		{
			throw new NotImplementedException ();
		}

		public void Vibrate (int milliseconds)
		{
			throw new NotImplementedException ();
		}
	}
}

