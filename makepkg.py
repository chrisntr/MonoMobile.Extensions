#!/usr/bin/env python

import subprocess
import sys
import shutil

XPKG = sys.argv[1]
OUTPUT = sys.argv[2]

#
# BUILD
#
def mdbuild (solution_path):
	xbuild_args = [
		"/Applications/MonoDevelop.app/Contents/MacOS/mdtool",
		"build",
		"--configuration:Release",
		solution_path,
	]
	err = subprocess.call (xbuild_args)
	if err:
		raise Exception ("MDTOOL RETURNED %s" % err)


# mdbuild ("MonoDroid/Xamarin.Mobile Android.sln")
# mdbuild ("MonoTouch/Xamarin.Mobile iOS.sln")

# Assemble docs
err = subprocess.call (["make", "-C", "docs", "publish"])
if err:
	raise Exception ("MAKE RETURNED %s" % err)

# Move refs
IOSDLL="MonoTouch/Xamarin.Mobile/bin/iPhoneSimulator/Release/Xamarin.Mobile.dll"
ANDDLL="MonoDroid/Xamarin.Mobile/bin/Release/Xamarin.Mobile.dll"
WPDLL ="WindowsPhone/Xamarin.Mobile/bin/Release/Xamarin.Mobile.dll"
WP8DLL ="WindowsPhone8/Xamarin.Mobile/bin/Release/Xamarin.Mobile.dll"
W8DLL = "WindowsRT/Xamarin.Mobile/bin/Release/Xamarin.Mobile.dll"

shutil.copy (IOSDLL, "MonoTouch/Samples/Xamarin.Mobile.dll")
shutil.copy (ANDDLL, "MonoDroid/Samples/Xamarin.Mobile.dll")
shutil.copy (WPDLL, "WindowsPhone/Samples/Xamarin.Mobile.dll")
shutil.copy (WP8DLL, "WindowsPhone8/Samples/Xamarin.Mobile.dll")
shutil.copy (W8DLL, "WindowsRT/Samples/Xamarin.Mobile.dll")

#
# PACKAGE
#
xpkg_args = [
	"mono",
	XPKG,
	"create",
	OUTPUT,
	"--name", "Xamarin.Mobile",
	"--details", "Details.md",
	"--license", "LICENSE.md",
	"--getting-started", "GettingStarted.md",
	"--publisher", "Xamarin",
	"--publisher-url", "http://xamarin.com",
	"--docs", "http://xamarin.com/mobileapi",
	"--monodoc", "docs",
	"--summary", "Xamarin.Mobile is a library that exposes a single set of APIs for accessing common mobile device functionality across iOS, Android and Windows platforms.",
	"--icon", "component/icon_128x128.png",
	"--icon", "component/icon_512x512.png",
	"--library", "ios:" + IOSDLL,
	"--library", "android:" + ANDDLL,
	"--library", "winphone-7.1:" + WPDLL,
	"--library", "winphone-8:" + WP8DLL,
	"--library", "winrt:" + W8DLL,
	"--sample", "Xamarin.Mobile Android Samples. Android samples for the AddressBook, MediaPicker and Geolocator APIs.:MonoDroid/Samples/Xamarin.Mobile.Android.Samples.sln",
	"--sample", "Xamarin.Mobile iOS Samples. iOS samples for the AddressBook, MediaPicker and Geolocator APIs.:MonoTouch/Samples/Xamarin.Mobile.iOS.Samples.sln",
	"--sample", "Xamarin.Mobile Windows Phone 7.5 Samples. Windows Phone 7 samples for the AddressBook, MediaPicker and Geolocator APIs.:WindowsPhone/Samples/Xamarin.Mobile.WP.Samples.sln",
	"--sample", "Xamarin.Mobile Windows Phone 8 Samples. Windows Phone 8 Samples for the AddressBook, MediaPicker and Geolocator APIs.:WindowsPhone8/Samples/Xamarin.Mobile.WP8.Samples.sln",
	"--sample", "Xamarin.Mobile Windows 8 Samples. Windows 8 Samples for the MediaPicker and Geolocator APIs.:WindowsRT/Samples/Xamarin.Mobile.WinRT.Samples.sln"
]
subprocess.call (xpkg_args)
