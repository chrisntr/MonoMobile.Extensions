Xamarin.Mobile is an API for accessing common platform features, such as
reading the user's address book and using the camera, across iOS,
Android, and Windows Phone.

The goal of Xamarin.Mobile is to decrease the amount of
platform-specific code needed to perform common tasks in multiplatform
apps, making development simpler and faster.

Xamarin.Mobile is currently a preview release and is subject to API
changes. Current known issues:

 - Windows Phone 7.1 version of the library requires the
 Visual Studio Async CTP (http://www.microsoft.com/en-us/download/details.aspx?id=9983).
 As this CTP installs to a user-specific directory, you'll likely need to
 correct references to this library in the samples to use them.

## Examples

To access the address book (requires `READ_CONTACTS` permissions
on Android):

```csharp
using Xamarin.Contacts;
// ...

var book = new AddressBook ();
//         new AddressBook (this); on Android
book.RequestPermission().ContinueWith (t => {
 if (!t.Result) {
		Console.WriteLine ("Permission denied by user or manifest");
		return;
	}

	foreach (Contact contact in book.OrderBy (c => c.LastName)) {
		Console.WriteLine ("{0} {1}", contact.FirstName, contact.LastName);
	}
}, TaskScheduler.FromCurrentSynchronizationContext());
```

To get the user's location (requires `ACCESS_COARSE_LOCATION` and
`ACCESS_FINE_LOCATION` permissions on Android):

```csharp
using Xamarin.Geolocation;
// ...

var locator = new Geolocator { DesiredAccuracy = 50 };
//            new Geolocator (this) { ... }; on Android
locator.GetPositionAsync (timeout: 10000).ContinueWith (t => {
	Console.WriteLine ("Position Status: {0}", t.Result.Timestamp);
	Console.WriteLine ("Position Latitude: {0}", t.Result.Latitude);
	Console.WriteLine ("Position Longitude: {0}", t.Result.Longitude);
}, TaskScheduler.FromCurrentSynchronizationContext());
```

To take a photo:

```csharp
using Xamarin.Media;
// ...

var picker = new MediaPicker ();
if (!picker.IsCameraAvailable)
	Console.WriteLine ("No camera!");
else {
	picker.TakePhotoAsync (new StoreCameraMediaOptions {
		Name = "test.jpg",
		Directory = "MediaPickerSample"
	}).ContinueWith (t => {
		if (t.IsCanceled) {
			Console.WriteLine ("User canceled");
			return;
		}
		Console.WriteLine (t.Result.Path);
	}, TaskScheduler.FromCurrentSynchronizationContext());
}
```

On Android (requires `WRITE_EXTERNAL_STORAGE` permissions):

```csharp
using Xamarin.Media;
// ...

protected override void OnCreate (Bundle bundle)
{
	var picker = new MediaPicker (this);
	if (!picker.IsCameraAvailable)
		Console.WriteLine ("No camera!");
	else {
		var intent = picker.GetTakePhotoUI (new StoreCameraMediaOptions {
			Name = "test.jpg",
			Directory = "MediaPickerSample"
		});
		StartActivityForResult (intent, 1);
	}
}

protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
{
	// User canceled
	if (resultCode == Result.Canceled)
		return;

	data.GetMediaFileExtraAsync (this).ContinueWith (t => {
		Console.WriteLine (t.Result.Path);
	}, TaskScheduler.FromCurrentSynchronizationContext());
}
```
