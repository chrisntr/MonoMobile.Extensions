
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoMobile.Extensions;

namespace MonoTouch.Example
{
	public partial class ExampleList : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public ExampleList (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public ExampleList (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public ExampleList () : base("ExampleList", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		#endregion
		
		UIButton alertButton, confirmButton, beepButton, vibrateButton, cameraButton;
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			var notification = new Notification();
			alertButton = UIButton.FromType(UIButtonType.RoundedRect);
			alertButton.Frame = new System.Drawing.RectangleF(40f, 20f, 200f, 40f);
			alertButton.SetTitle("Alert button", UIControlState.Normal);
			alertButton.TouchUpInside += (s, e) => {
				notification.Alert("My Message", () => {
					Console.WriteLine ("Dismissed");
				}, "Title", "OK");
			};
			this.View.AddSubview(alertButton);
			
			confirmButton = UIButton.FromType(UIButtonType.RoundedRect);
			confirmButton.Frame = new System.Drawing.RectangleF(40f, 60f, 200f, 40f);
			confirmButton.SetTitle("Confirm button", UIControlState.Normal);
			confirmButton.TouchUpInside += (s, e) => {
				notification.Confirm("My Message", (i) => {
					Console.WriteLine ("Button {0} pressed", i);	
				}, "Alert!", "One, Two, Cancelled");
			};
			this.View.AddSubview(confirmButton);
			
			beepButton = UIButton.FromType(UIButtonType.RoundedRect);
			beepButton.Frame = new System.Drawing.RectangleF(40f, 100f, 200f, 40f);
			beepButton.SetTitle("Beep!", UIControlState.Normal);
			beepButton.TouchUpInside += (s, e) => {
				// Beep overload just calls beep anyway due to iPhone limitation.
				// Make sure there's a beep.wav set as content in the root of the app.
				notification.Beep();
			};
			this.View.AddSubview(beepButton);
			
			
			vibrateButton = UIButton.FromType(UIButtonType.RoundedRect);
			vibrateButton.Frame = new System.Drawing.RectangleF(40f, 140f, 200f, 40f);
			vibrateButton.SetTitle("Vibrate!", UIControlState.Normal);
			vibrateButton.TouchUpInside += (s, e) => {
				// Vibrate overload just calls vibrate anyway due to iPhone limitation.
				notification.Vibrate();
			};
			this.View.AddSubview(vibrateButton);
			
			var camera = new Camera(this);
			cameraButton = UIButton.FromType(UIButtonType.RoundedRect);
			cameraButton.Frame = new System.Drawing.RectangleF(40f, 180f, 200f, 40f);
			cameraButton.SetTitle("GetPicture!", UIControlState.Normal);
			cameraButton.TouchUpInside += (s, e) => {
				// Vibrate overload just calls vibrate anyway due to iPhone limitation.
				camera.GetPicture(new Camera.CameraOptions() { SourceType = UIImagePickerControllerSourceType.SavedPhotosAlbum }, null, null);
			};
			this.View.AddSubview(cameraButton);
		}
	}
}

