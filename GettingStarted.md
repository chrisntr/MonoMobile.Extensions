# Getting Started

Xamarin.Mobile is a library that exposes a single set of APIs for accessing common mobile device functionality across iOS, Android, and Windows platforms. This increases the amount of code developers can share across mobile platforms, making mobile app development easier and faster.

Using the Addon
================

Contacts API:

	var book = new AddressBook ();
	foreach (Contact contact in book.OrderBy (c => c.LastName)) {
		Console.WriteLine ("{0} {1}", contact.FirstName, contact.LastName);
	}

Geolocation API:

	var geolocator = new Geolocator { DesiredAccuracy = 50 };
	geolocator.GetPositionAsync (timeout: 10000)
		.ContinueWith (t =>
		{
			Console.WriteLine ("Position Status: {0}", t.Result.Timestamp);
			Console.WriteLine ("Position Latitude: {0}", t.Result.Latitude);
			Console.WriteLine ("Position Longitude: {0}", t.Result.Longitude);
		});

MediaPicker API:

	var picker = new MediaPicker ();
	picker.TakePhotoAsync (new StoreCameraMediaOptions
	{
		Name = "test.jpg",
		Directory = "MediaPickerSample"
	})
	.ContinueWith (t =>
	{
		if (t.IsCanceled)
		{
			Console.WriteLine ("User canceled");
			return;
		}

		Console.WriteLine (t.Result.Path);
	});