using System.IO;
using MonoTouch.Foundation;

namespace Xamarin
{
	internal unsafe class NSDataStream
		: UnmanagedMemoryStream
	{
		public NSDataStream (NSData data)
			: base ((byte*)data.Bytes, data.Length)
		{
			this.data = data;
		}

		private readonly NSData data;

		protected override void Dispose (bool disposing)
		{
			if (disposing)
				this.data.Dispose();

			base.Dispose (disposing);
		}
	}
}