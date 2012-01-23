using System;

namespace Xamarin
{
	internal static class DateTimeExtensions
	{
		public static long ToAndroidTimestamp (this DateTime self)
		{
			return (long)self.ToUniversalTime().Subtract (Epoch).TotalMilliseconds;
		}

		private static readonly DateTime Epoch = new DateTime (1970, 1, 1);
	}
}