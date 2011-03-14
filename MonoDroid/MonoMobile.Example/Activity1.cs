using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace MonoMobile.Example
{
	[Activity(Label = "MonoMobile Android Example", MainLauncher = true)]
	public class Activity1 : Activity
	{
		int count = 1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			
			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.MyButton);
			
			button.Click += delegate {
				var device = new MonoMobile.Extensions.Device(this);
				
				Android.Util.Log.Info("MonoMobile.Extensions", "Device Name: {0}", device.Name);
				Android.Util.Log.Info("MonoMobile.Extensions", "Device Platform: {0}", device.Platform);
				Android.Util.Log.Info("MonoMobile.Extensions", "Device UUID: {0}", device.UUID);
				Android.Util.Log.Info("MonoMobile.Extensions", "Device Version: {0}", device.Version);
				Android.Util.Log.Info("MonoMobile.Extensions", "MonoMobile Version: {0}", device.MonoMobileVersion);
			};
		}
	}
}


