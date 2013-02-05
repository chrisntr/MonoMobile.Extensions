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

shutil.copy (IOSDLL, "MonoTouch/Samples/Xamarin.Mobile.dll")
shutil.copy (ANDDLL, "MonoDroid/Samples/Xamarin.Mobile.dll")
shutil.copy (WPDLL, "WindowsPhone/Samples/Xamarin.Mobile.dll")

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
	"--icon", "icon_128x128.png",
	"--library", "ios:" + IOSDLL,
	"--library", "android:" + ANDDLL,
	"--library", "winphone-7.1:" + WPDLL,
	"--sample", "Xamarin.Mobile Android Samples. Android samples for the AddressBook, MediaPicker and Geolocator APIs.:MonoDroid/Samples/Xamarin.Mobile.Android.Samples.sln",
	"--sample", "Xamarin.Mobile iOS Samples. iOS samples for the AddressBook, MediaPicker and Geolocator APIs.:MonoTouch/Samples/Xamarin.Mobile.iOS.Samples.sln",
	"--sample", "Xamarin.Mobile Windows Phone 7.5 Samples. Windows Phone 7 samples for the AddressBook, MediaPicker and Geolocator APIs.:WindowsPhone/Samples/Xamarin.Mobile.WP.Samples.sln"
]
subprocess.call (xpkg_args)
