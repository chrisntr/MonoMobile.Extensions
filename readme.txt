Xamarin Mobile API Preview 0.3

At this time, the Windows Phone version of the library requires the
Visual Studio Async CTP (http://msdn.microsoft.com/en-us/vstudio/gg316360).
As this CTP installs to a user-specific directory, you'll likely need to
correct references to this library in the samples to use them.

Changelog

Release 0.3

Features:
 - MediaPicker class, providing asynchronous methods to invoke
   the system UI for taking and picking photos and video.
 - Windows Phone version of all existing APIs

Enhancements:
 - Improved AddressBook iteration performance on Andriod by 2x
 - Many queries now translate to native queries on Android,
   improving performance on many simple queries.
 - Removed Contact.PhotoThumbnail
 - Added Contact.GetThumbnail()
 - Added Task<MediaFile> Contact.SaveThumbnail(string)
 - Added bool AddressBook.LoadSupported

Fixes:
 - Fixed an issue where iterating the AddressBook without a query
   would always return aggregate contacts, regardless of PreferContactAggregation

Release 2

Features:
 - iOS and Android AddressBook

Release 1

Features:
 - iOS and Android Geolocator