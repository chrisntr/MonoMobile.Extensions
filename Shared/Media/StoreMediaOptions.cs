using System;

namespace Xamarin.Media
{
	public enum MediaFileStoreLocation
	{
		None,
		CameraRoll,
		Local
	}

	public class StoreMediaOptions
	{
		public MediaFileStoreLocation Location
		{
			get;
			set;
		}

		public string Directory
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}
		
		public bool AllowEditing
		{
			get;
			set;
		}
	}
	
	public enum CameraDevice
	{
		Rear,
		Front
	}
	
	public class StoreCameraMediaOptions
		: StoreMediaOptions
	{
		public CameraDevice DefaultCamera
		{
			get;
			set;
		}
	}

	public enum VideoQuality
	{
		Low = 0,
		High = 1,
	}

	public class StoreVideoOptions
		: StoreCameraMediaOptions
	{
		public StoreVideoOptions()
		{
			Quality = VideoQuality.High;
		}

		public TimeSpan MaximumLength
		{
			get;
			set;
		}

		public VideoQuality Quality
		{
			get;
			set;
		}
	}

}