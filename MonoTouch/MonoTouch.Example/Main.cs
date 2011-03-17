
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
		UIViewController exampleList;
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
			
			exampleList = new ExampleList();
			
			window.AddSubview(exampleList.View);
			
			window.MakeKeyAndVisible ();
			
			return true;
		}

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}

