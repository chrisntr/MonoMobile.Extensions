using System;
using MonoTouch.UIKit;
using MonoTouch.AudioToolbox;
namespace MonoMobile.Extensions
{
	public class Notification
	{
		public Notification () 
		{
		}
	
		public void Alert (string message, Action alertCallback)
		{
			Alert(message, alertCallback, "Alert", "OK");
		}

		public void Alert (string message, Action alertCallback, string title)
		{
            Alert(message, alertCallback, title, "OK");
		}

		public void Alert (string message, Action alertCallback, string title, string buttonName)
		{
			using(var alertView = new UIAlertView(title, message, null, buttonName, null))
			{
				alertView.Show();
				alertView.Dismissed += delegate(object sender, UIButtonEventArgs e) {
					alertCallback();
				};
			}
		}

		public void Confirm (string message, Action<int> confirmCallback)
		{
			
			Confirm(message, confirmCallback, "Alert", "OK");
		}

		public void Confirm (string message, Action<int> confirmCallback, string title)
		{
			Confirm(message, confirmCallback, title, "OK");
		}

		public void Confirm (string message, Action<int> confirmCallback, string title, string buttonLabels)
		{
			using(var alertView = new UIAlertView(title, message, null, null, null))
			{
				var labels = buttonLabels.Split(new Char[]{','});	
				foreach(var label in labels)
				{
					alertView.AddButton(label);
				}
				alertView.Show();
				alertView.Clicked += delegate(object sender, UIButtonEventArgs e) {
					confirmCallback(e.ButtonIndex);
				};
			}
		}

		public void Beep ()
		{
			var beep = SystemSound.FromFile("beep.wav");
			beep.PlaySystemSound();
		}

		public void Beep (int times)
		{
			Beep();
		}

		public void Vibrate ()
		{
			SystemSound.Vibrate.PlaySystemSound();
		}

		public void Vibrate (int milliseconds)
		{
			Vibrate();
		}
	}
}

