Xamarin Mobile API Preview 0.5.1

At this time, the Windows Phone version of the library requires the
Visual Studio Async CTP (http://www.microsoft.com/en-us/download/details.aspx?id=9983).
As this CTP installs to a user-specific directory, you'll likely need to
correct references to this library in the samples to use them.

Changelog

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
