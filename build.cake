#addin "Cake.Xamarin"

var TARGET = Argument ("t", Argument ("target", Argument ("Target", "Default")));

Task ("libs").Does (() => 
{
	if (IsRunningOnWindows ()) {
		
		CreateDirectory ("./output/");
		CreateDirectory ("./output/wp8/");
		CreateDirectory ("./output/winrt/");
		
		NuGetRestore ("./Xamarin.Mobile.sln");

		DotNetBuild ("./WindowsPhone8/Xamarin.Mobile/Xamarin.Mobile.csproj", c => c.Configuration = "Release");
		CopyFiles ("./WindowsPhone8/Xamarin.Mobile/bin/Release/*.dll", "./output/wp8");

		DotNetBuild ("./WindowsRT/Xamarin.Mobile/Xamarin.Mobile.csproj", c => c.Configuration = "Release");
		CopyFiles ("./WindowsRT/Xamarin.Mobile/bin/Release/*.dll", "./output/winrt");

	} else {

		CreateDirectory ("./output/");
		CreateDirectory ("./output/android/");
		CreateDirectory ("./output/ios-unified/");
		CreateDirectory ("./output/ios/");

		NuGetRestore ("./Xamarin.Mobile.Mac.sln");

		DotNetBuild ("./MonoDroid/Xamarin.Mobile/Xamarin.Mobile.Android.csproj");
		CopyFiles ("./MonoDroid/Xamarin.Mobile/bin/Release/*.dll", "./output/android");

		iOSBuild ("./MonoTouch/Xamarin.Mobile/Xamarin.Mobile.iOS.csproj", new MDToolSettings { Configuration = "Release|iPhone" });
		CopyFiles ("./MonoTouch/Xamarin.Mobile/bin/unified/iPhone/Release/*.dll", "./output/ios-unified");

		iOSBuild ("./MonoTouch/Xamarin.Mobile/Xamarin.Mobile.iOS-Classic.csproj", new MDToolSettings { Configuration = "Release|iPhone" });
		CopyFiles ("./MonoTouch/Xamarin.Mobile/bin/classic/iPhone/Release/*.dll", "./output/ios");
	}
});
Task ("Default").IsDependentOn ("libs").Does (() => { });

Task ("samples").IsDependentOn ("libs").Does (() => 
{
	if (IsRunningOnWindows ()) {
		DotNetBuild ("./WindowsPhone8/Samples/ContactsSample/ContactsSample.csproj", c => c.Configuration = "Release");
		DotNetBuild ("./WindowsPhone8/Samples/GeolocationSample/GeolocationSample.csproj", c => c.Configuration = "Release");
		DotNetBuild ("./WindowsPhone8/Samples/MediaPickerSample/MediaPickerSample.csproj", c => c.Configuration = "Release");

		DotNetBuild ("./WindowsRT/Samples/ContactsSample/ContactsSample.csproj", c => c.Configuration = "Release");
		DotNetBuild ("./WindowsRT/Samples/GeolocationSample/GeolocationSample.csproj", c => c.Configuration = "Release");
		DotNetBuild ("./WindowsRT/Samples/MediaPickerSample/MediaPickerSample.csproj", c => c.Configuration = "Release");
	} else {		
		DotNetBuild ("./MonoDroid/Samples/ContactsSample/Contacts Sample.csproj", c => c.Configuration = "Release");
		DotNetBuild ("./MonoDroid/Samples/GeolocationSample/Geolocation Sample.csproj", c => c.Configuration = "Release");
		DotNetBuild ("./MonoDroid/Samples/MediaPickerSample/MediaPicker Sample.csproj", c => c.Configuration = "Release");

		iOSBuild ("./MonoTouch/Samples/ContactsSample/Contacts Sample.csproj", new MDToolSettings { Configuration = "Release|iPhone" });
		iOSBuild ("./MonoTouch/Samples/GeolocationSample/Geolocation Sample.csproj", new MDToolSettings { Configuration = "Release|iPhone" });
		iOSBuild ("./MonoTouch/Samples/MediaPickerSample/MediaPicker Sample.csproj", new MDToolSettings { Configuration = "Release|iPhone" });

		iOSBuild ("./MonoTouch/Samples/ContactsSample-Classic/Contacts Sample-Classic.csproj", new MDToolSettings { Configuration = "Release|iPhone" });
		iOSBuild ("./MonoTouch/Samples/GeolocationSample-Classic/Geolocation Sample-Classic.csproj", new MDToolSettings { Configuration = "Release|iPhone" });
		iOSBuild ("./MonoTouch/Samples/MediaPickerSample-Classic/MediaPicker Sample-Classic.csproj", new MDToolSettings { Configuration = "Release|iPhone" });
	}
});

Task ("component").IsDependentOn ("samples").Does (() =>
{
	if (!FileExists ("./tools/xamarin-component.exe")) {
		DownloadFile ("https://components.xamarin.com/submit/xpkg", "./tools/xpkg.zip");
		Unzip ("./tools/xpkg.zip", "./tools/");		
	}

	StartProcess (MakeAbsolute (new FilePath ("./tools/xamarin-component.exe")), new ProcessSettings {
		Arguments = "package ./"
	});

	DeleteFiles ("./output/*.xam");
	MoveFiles ("./*.xam", "./output/");
});

Task ("clean").Does (() => 
{
	if (DirectoryExists ("./output/"))
		DeleteDirectory ("./output/", true);

	if (DirectoryExists ("./tools/"))
		DeleteDirectory ("./tools/", true);

	CleanDirectories ("./**/bin");
	CleanDirectories ("./**/obj");
});

RunTarget (TARGET);