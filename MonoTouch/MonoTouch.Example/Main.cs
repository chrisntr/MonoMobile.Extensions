
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoMobile.Extensions;

namespace MonoTouch.Example
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}

	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIButton alertButton, confirmButton, beepButton, vibrateButton;
		
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// If you have defined a view, add it here:
			// window.AddSubview (navigationController.View);
			
			var device = new Device();
			Console.WriteLine ("Device Name: {0}", device.Name);
			Console.WriteLine ("Device Platform: {0}", device.Platform);
			Console.WriteLine ("Device UUID: {0}", device.UUID);
			Console.WriteLine ("Device Version: {0}", device.Version);
			Console.WriteLine ("MonoMobile Version: {0}", device.MonoMobileVersion);
			
			
			var notification = new Notification();
			alertButton = UIButton.FromType(UIButtonType.RoundedRect);
			alertButton.Frame = new System.Drawing.RectangleF(40f, 20f, 200f, 40f);
			alertButton.SetTitle("Alert button", UIControlState.Normal);
			alertButton.TouchUpInside += (s, e) => {
				notification.Alert("My Message", () => {
					Console.WriteLine ("Dismissed");
				}, "Title", "OK");
			};
			window.AddSubview(alertButton);
			
			confirmButton = UIButton.FromType(UIButtonType.RoundedRect);
			confirmButton.Frame = new System.Drawing.RectangleF(40f, 60f, 200f, 40f);
			confirmButton.SetTitle("Confirm button", UIControlState.Normal);
			confirmButton.TouchUpInside += (s, e) => {
				notification.Confirm("My Message", (i) => {
					Console.WriteLine ("Button {0} pressed", i);	
				}, "Alert!", "One, Two, Cancelled");
			};
			window.AddSubview(confirmButton);
			
			beepButton = UIButton.FromType(UIButtonType.RoundedRect);
			beepButton.Frame = new System.Drawing.RectangleF(40f, 100f, 200f, 40f);
			beepButton.SetTitle("Beep!", UIControlState.Normal);
			beepButton.TouchUpInside += (s, e) => {
				// Beep overload just calls beep anyway due to iPhone limitation.
				// Make sure there's a beep.wav set as content in the root of the app.
				notification.Beep();
			};
			window.AddSubview(beepButton);
			
			
			vibrateButton = UIButton.FromType(UIButtonType.RoundedRect);
			vibrateButton.Frame = new System.Drawing.RectangleF(40f, 140f, 200f, 40f);
			vibrateButton.SetTitle("Vibrate!", UIControlState.Normal);
			vibrateButton.TouchUpInside += (s, e) => {
				// Vibrate overload just calls vibrate anyway due to iPhone limitation.
				notification.Vibrate();
			};
			window.AddSubview(vibrateButton);
			
			window.MakeKeyAndVisible ();
			
			return true;
		}

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}

