
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin.Geolocation;
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
		UIButton currentLocationButton, cancelLocationButton, listenPositionButton;
		Geolocator locator;
		
		UIAlertView currentLocationAlert;

		bool isListening;
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			locator = new Geolocator();
			locator.DesiredAccuracy = 50;

			currentLocationButton = UIButton.FromType (UIButtonType.RoundedRect);
			currentLocationButton.Frame = new System.Drawing.RectangleF (40f, 220f, 200f, 40f);
			currentLocationButton.Enabled = locator.IsGeolocationAvailable;
			currentLocationButton.SetTitle ("Get current Location!", UIControlState.Normal);
			currentLocationButton.TouchUpInside += (s, e) =>
			{
				this.cancelSource = new CancellationTokenSource();
				locator.GetPositionAsync (5000, this.cancelSource.Token)
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
			latitudeText.Font = UIFont.FromName ("Courier New", 12);
			latitudeText.Frame = new System.Drawing.RectangleF (40f, 340f, 200f, 20f);
			this.View.AddSubview (latitudeText);
			
			longitudeText = new UILabel();
			longitudeText.Font = UIFont.FromName ("Courier New", 12);
			longitudeText.Frame = new System.Drawing.RectangleF (40f, 360f, 200f, 20f);
			this.View.AddSubview (longitudeText);
			
			accuracyText = new UILabel();
			accuracyText.Font = UIFont.FromName ("Courier New", 12);
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
			InvokeOnMainThread (() =>
			{
				latitudeText.Text = String.Format ("{0,-15}{1,-6:N3}", "Latitude:", e.Position.Latitude);
				longitudeText.Text = String.Format ("{0,-15}{1,-6:N3}", "Longitude:", e.Position.Longitude);
				accuracyText.Text = String.Format ("{0,-15}{1,-6}", "Accuracy:", e.Position.Accuracy);
			});
		}

		public void Update()
		{
			currentLocationButton.Enabled = locator.IsGeolocationAvailable;
			listenPositionButton.Enabled = locator.IsGeolocationAvailable;
			cancelLocationButton.Enabled = locator.IsGeolocationAvailable;
		}
	}
}