Xamarin Mobile API Preview 0.6.1

SDK Requirements:

 - Xamarin.Mobile for Android requires a minimum Android version of 2.3 (API Level 9).
 - Xamarin.Mobile for iOS requires a minimum iOS version of 5.0.
 - Xamarin.Mobile for Windows Phone requires a minimum SDK version of 7.1.

Known Issues:

 - Windows Phone 7.1 version of the library requires the
 Visual Studio Async CTP (http://www.microsoft.com/en-us/download/details.aspx?id=9983).
 As this CTP installs to a user-specific directory, you'll likely need to
 correct references to this library in the samples to use them.
 - On Android, Tasks returned from MediaPicker may fail to fire if the
 holding Activity is killed.


Changelog

Release 0.6.2

Fixes:
 - Fixed presenting UIViewControllers in the continuation of a MediaPicker call.

Release 0.6.1

Fixes:
 - Throw a proper exception from inside MediaPicker on Android when writing fails.
 - Fixed MediaPicker actions launched from a UIAlertView.
 - Fixed an ArgumentNullException iterating through certain contacts on iOS.

Release 0.6

Features:
 - Windows 8 support for Xamarin.Media and Xamarin.Geolocation.
 - Windows Phone 8 support.

Fixes:
 - Fixed positioning issues with MediaPicker.Pick* on retina iPads and iOS6.
 - Fixed MediaPicker presenting under certain view controller hierarchies.
 - Fixed incorrect Position.Timestamp on Android.
 - Fixed MediaPicker.Take* for Android devices with front facing cameras only.

Release 0.5.1

Enhancements:
 - Geolocator.GetPositionAsync() and StartListening() now optionally support
   including the heading or not. BREAKING CHANGE: The default is false where
   previously it was automatically included.

Fixes:
 - Xamarin.Geolocation has been updated to support iOS6 and MonoTouch 6.
 - Xamarin.Contacts.AddressBook now provides a Task<bool> RequestPermission()
   method to support iOS6's new privacy settings on ABAddressBook. This method
   checks manifest privileges on Android and Windows Phone.
 - Improved the likelihood of receiving a result from Geolocator.GetPositionAsync()
   on Windows Phone.

Release 0.5

Fixes:
 - Fixed issues surrounding Geolocator.IsGeolocationAvailable &
   IsGeolocationEnabled on all platforms.
 - Fixed an error on Windows Phone where Geolocator.StopListening
   was called before StartListening.
 - Fixed occasional NaNs for Speed on Windows Phone.

Release 0.4

Features:
 - Includes a build against Mono for Android 4.2

Fixes:
 - Fixed memory leaks in Geolocator
 - Fixed an issue with MediaPicker picking on iPads
 - Fixed an issue with MediaPicker on iOS devices with no camera
 - Fixed an issue with cancelling MediaPicker on iOS devices
 - Fixed an issue with rotation in MediaPicker on Android

Release 0.3

Features:
 - MediaPicker class, providing asynchronous methods to invoke
   the system UI for taking and picking photos and video.
 - Windows Phone version of all existing APIs

Enhancements:
 - Improved AddressBook iteration performance on Android by 2x
 - Many queries now translate to native queries on Android,
   improving performance on many simple queries.
 - Removed Contact.PhotoThumbnail
 - Added Contact.GetThumbnail()
 - Added Task<MediaFile> Contact.SaveThumbnailAsync(string)
 - Added bool AddressBook.LoadSupported

Fixes:
 - Fixed an issue where iterating the AddressBook without a query
   would always return aggregate contacts, regardless of PreferContactAggregation
 - Fixed an AddressBook crash with the latest version of MonoTouch
 - Fixed an occassional exception from Geolocator.GetPositionAsync timeouts

Release 0.2

Features:
 - iOS and Android AddressBook

Release 0.1

Features:
 - iOS and Android Geolocator
