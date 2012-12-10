[[Xamarin.Mobile]] is a library that exposes a single set of APIs for accessing
common mobile device functionality across iOS, Android, and Windows platforms.
This increases the amount of code developers can share across mobile platforms,
making mobile app development easier and faster.

Developers often find that because mobile feature sets have evolved so quickly,
services that are common across mobile platforms are not exposed by the .NET
BCL. This limits the amount of code that could be shared across mobile
platforms. Developers often resort to creating their own platform abstractions,
or designing pluggable architectures, just to get access to the system address
book, the devices’s GPS, the compass, the accelerometer, the notification
service or the system calendar.

Xamarin.Mobile solves this problem, helping you maximize code sharing while
still delivering high-performance, beautiful, native user experiences.

Quick Samples
----------------

**Contacts:**

[code:csharp]
    var book = new AddressBook ();
    foreach (Contact contact in book.OrderBy (c => c.LastName)) {
        Console.WriteLine ("{0} {1}", contact.FirstName, contact.LastName);
    }
[code]

**Geolocation:**

[code:csharp]
    var geolocator = new Geolocator { DesiredAccuracy = 50 };
    geolocator.GetPositionAsync (timeout: 10000)
                .ContinueWith (t =>
                {
                    Console.WriteLine ("Position Status: {0}", t.Result.Timestamp);
                    Console.WriteLine ("Position Latitude: {0}", t.Result.Latitude);
                    Console.WriteLine ("Position Longitude: {0}", t.Result.Longitude);
                });
[code]

**Media:**

[code:csharp]
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
[code]
