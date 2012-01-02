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
		Medium = 1,
		High = 2,
	}

	public class StoreVideoOptions
		: StoreCameraMediaOptions
	{
		public StoreVideoOptions()
		{
			Quality = VideoQuality.High;
			DesiredLength = TimeSpan.FromMinutes (10);
		}

		public TimeSpan DesiredLength
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