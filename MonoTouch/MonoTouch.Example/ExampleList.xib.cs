
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoMobile.Extensions;
using System.Threading;

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
		
		CancellationTokenSource cancelSource;

		UILabel longitudeText, latitudeText, accuracyText;
		UIButton alertButton, confirmButton, beepButton, vibrateButton, cameraButton,
			currentLocationButton, cancelLocationButton, listenPositionButton;
		Geolocation locator;
		
		UIAlertView currentLocationAlert;

		bool isListening;
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
//			var notification = new Notification();
//			alertButton = UIButton.FromType(UIButtonType.RoundedRect);
//			alertButton.Frame = new System.Drawing.RectangleF(40f, 20f, 200f, 40f);
//			alertButton.SetTitle("Alert button", UIControlState.Normal);
//			alertButton.TouchUpInside += (s, e) => {
//				notification.Alert("My Message", () => {
//					Console.WriteLine ("Dismissed");
//				}, "Title", "OK");
//			};
//			this.View.AddSubview(alertButton);
//			
//			confirmButton = UIButton.FromType(UIButtonType.RoundedRect);
//			confirmButton.Frame = new System.Drawing.RectangleF(40f, 60f, 200f, 40f);
//			confirmButton.SetTitle("Confirm button", UIControlState.Normal);
//			confirmButton.TouchUpInside += (s, e) => {
//				notification.Confirm("My Message", (i) => {
//					Console.WriteLine ("Button {0} pressed", i);	
//				}, "Alert!", "One, Two, Cancelled");
//			};
//			this.View.AddSubview(confirmButton);
//			
//			beepButton = UIButton.FromType(UIButtonType.RoundedRect);
//			beepButton.Frame = new System.Drawing.RectangleF(40f, 100f, 200f, 40f);
//			beepButton.SetTitle("Beep!", UIControlState.Normal);
//			beepButton.TouchUpInside += (s, e) => {
//				// Beep overload just calls beep anyway due to iPhone limitation.
//				// Make sure there's a beep.wav set as content in the root of the app.
//				notification.Beep();
//			};
//			this.View.AddSubview(beepButton);
//			
//			
//			vibrateButton = UIButton.FromType(UIButtonType.RoundedRect);
//			vibrateButton.Frame = new System.Drawing.RectangleF(40f, 140f, 200f, 40f);
//			vibrateButton.SetTitle("Vibrate!", UIControlState.Normal);
//			vibrateButton.TouchUpInside += (s, e) => {
//				// Vibrate overload just calls vibrate anyway due to iPhone limitation.
//				notification.Vibrate();
//			};
//			this.View.AddSubview(vibrateButton);
//			
//			var camera = new Camera(this);
//			cameraButton = UIButton.FromType(UIButtonType.RoundedRect);
//			cameraButton.Frame = new System.Drawing.RectangleF(40f, 180f, 200f, 40f);
//			cameraButton.SetTitle("Get Picture!", UIControlState.Normal);
//			cameraButton.TouchUpInside += (s, e) => {
//				// Vibrate overload just calls vibrate anyway due to iPhone limitation.
//				camera.GetPicture(new Camera.CameraOptions() { SourceType = UIImagePickerControllerSourceType.SavedPhotosAlbum }, 
//					(p) => { Console.WriteLine ("Got picture as {0}", p);},
//					() => { Console.WriteLine ("Cancelled"); }
//				);
//			};
//			this.View.AddSubview(cameraButton);

			locator = new Geolocation();
			locator.DesiredAccuracy = 50;

			currentLocationButton = UIButton.FromType (UIButtonType.RoundedRect);
			currentLocationButton.Frame = new System.Drawing.RectangleF (40f, 220f, 200f, 40f);
			currentLocationButton.Enabled = locator.IsGeolocationAvailable;
			currentLocationButton.SetTitle ("Get current Location!", UIControlState.Normal);
			currentLocationButton.TouchUpInside += (s, e) =>
			{
				this.cancelSource = new CancellationTokenSource();
				locator.GetCurrentPosition (30000, this.cancelSource.Token)
					.ContinueWith (t =>
					{
						InvokeOnMainThread(() =>
						{
							this.cancelSource = null;

							if (t.IsCanceled)
							{
								currentLocationAlert = new UIAlertView ("Location", "Future canceled", new UIAlertViewDelegate(), "OK");
								currentLocationAlert.Show();
							}
							else if (t.IsFaulted)
							{
								currentLocationAlert = new UIAlertView ("Location", "Error: " + t.Exception.InnerExceptions.First ().Message, new UIAlertViewDelegate(), "OK");
								currentLocationAlert.Show();
							}
							else
							{
								currentLocationAlert = new UIAlertView ("Location",
									"Altitude: " + t.Result.Altitude + Environment.NewLine
									+ "Accuracy: " + t.Result.Accuracy + Environment.NewLine
									+ "Latitude: " + t.Result.Latitude + Environment.NewLine
									+ "Longitude: " + t.Result.Longitude + Environment.NewLine
									+ "Heading: " + t.Result.Heading, new UIAlertViewDelegate(), "OK");
								currentLocationAlert.Show();
							}
						});
					});
			};
			this.View.AddSubview (currentLocationButton);
			
			cancelLocationButton = UIButton.FromType (UIButtonType.RoundedRect);
			cancelLocationButton.Frame = new System.Drawing.RectangleF (40f, 260f, 200f, 40f);
			cancelLocationButton.Enabled = locator.IsGeolocationAvailable;
			cancelLocationButton.SetTitle ("Cancel location", UIControlState.Normal);
			cancelLocationButton.TouchUpInside += (s,e) =>
			{
				var c = this.cancelSource;
				if (c != null)
					c.Cancel();
			};
			this.View.AddSubview (cancelLocationButton);
			
			latitudeText = new UILabel();
			latitudeText.Font = UIFont.FromName ("Courier New", 10);
			latitudeText.Frame = new System.Drawing.RectangleF (40f, 340f, 200f, 20f);
			this.View.AddSubview (latitudeText);
			
			longitudeText = new UILabel();
			longitudeText.Font = UIFont.FromName ("Courier New", 10);
			longitudeText.Frame = new System.Drawing.RectangleF (40f, 360f, 200f, 20f);
			this.View.AddSubview (longitudeText);
			
			accuracyText = new UILabel();
			accuracyText.Font = UIFont.FromName ("Courier New", 10);
			accuracyText.Frame = new System.Drawing.RectangleF (40f, 380f, 200f, 20f);
			this.View.AddSubview (accuracyText);
			
			listenPositionButton = UIButton.FromType (UIButtonType.RoundedRect);
			listenPositionButton.Frame = new System.Drawing.RectangleF (40f, 300f, 200f, 40f);
			listenPositionButton.Enabled = locator.IsGeolocationAvailable;
			listenPositionButton.SetTitle ("Start listening", UIControlState.Normal);
			listenPositionButton.TouchUpInside += (s, e) =>
			{
				ToggleListener();
			};
			
			this.View.AddSubview (listenPositionButton);
		}

		private void ToggleListener ()
		{
			if (!this.isListening)
			{
				listenPositionButton.SetTitle ("Stop listening", UIControlState.Normal);
				this.locator.PositionChanged += OnPositionChanged;
				this.locator.StartListening (500, 1);
				this.isListening = true;
			}
			else
			{
				listenPositionButton.SetTitle ("Start listening", UIControlState.Normal);
				this.locator.PositionChanged -= OnPositionChanged;
				this.locator.StopListening ();
				this.isListening = false;
			}
		}

		void OnPositionChanged (object sender, PositionEventArgs e)
		{
			UpdateLocation (e.Position);
		}

		private void UpdateLocation (Position p)
		{
			InvokeOnMainThread (() =>
			{
				latitudeText.Text = String.Format ("{0,-15}{1,-6:N3}", "Latitude:", p.Latitude);
				longitudeText.Text = String.Format ("{0,-15}{1,-6:N3}", "Longitude:", p.Longitude);
				accuracyText.Text = String.Format ("{0,-15}{1,-6}", "Accuracy:", p.Accuracy);
			});
		}

		public void Update()
		{
			currentLocationButton.Enabled = locator.IsGeolocationAvailable;
			listenPositionButton.Enabled = locator.IsGeolocationAvailable;
			cancelLocationButton.Enabled = locator.IsGeolocationAvailable;
		}

		private class LocationObserver
			: IObserver<Position>
		{
			private readonly Action<Position> onNext;
			private readonly Action onCompleted;
			private readonly Action<Exception> onError;

			public LocationObserver (Action<Position> onNext, Action onCompleted, Action<Exception> onError)
			{
				this.onNext = onNext;
				this.onCompleted = onCompleted;
				this.onError = onError;
			}

			public void OnCompleted()
			{
				this.onCompleted();
			}

			public void OnError (Exception error)
			{
				this.onError(error);
			}

			public void OnNext (Position value)
			{
				this.onNext (value);
			}
		}
	}
}

