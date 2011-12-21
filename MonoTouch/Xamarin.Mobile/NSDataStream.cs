using System;
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

//		public override bool CanRead
//		{
//			get { return true; }
//		}
//
//		public override bool CanSeek
//		{
//			get { return true; }
//		}
//
//		public override bool CanWrite
//		{
//			get { return true; }
//		}
//
//		public override long Length
//		{
//			get
//			{
//				if (this.isDisposed)
//					throw new ObjectDisposedException (null);
//				
//				throw new NotImplementedException ();
//			}
//		}
//
//		public override long Position
//		{
//			get { return this.position; }
//			set
//			{
//				if (this.isDisposed)
//					throw new ObjectDisposedException (null);
//
//				this.position = value;
//			}
//		}
//		
//		public override void Flush ()
//		{
//			if (this.isDisposed)
//				throw new ObjectDisposedException (null);
//		}
//
//		public override int Read (byte[] buffer, int offset, int count)
//		{
//			if (buffer == null)
//				throw new ArgumentNullException ("buffer");
//			if (offset < 0)
//				throw new ArgumentOutOfRangeException ("offset");
//			if (count < 0)
//				throw new ArgumentOutOfRangeException ("count");
//			if (offset + count > buffer.Length)
//				throw new ArgumentException ("Offset and count are longer than buffer");
//			if (this.isDisposed)
//				throw new ObjectDisposedException (null);
//			
//		}
//
//		public override long Seek (long offset, SeekOrigin origin)
//		{
//			throw new NotImplementedException ();
//		}
//
//		public override void SetLength (long value)
//		{
//			throw new NotSupportedException();
//		}
//
//		public override void Write (byte[] buffer, int offset, int count)
//		{
//			throw new NotImplementedException ();
//		}
//		
//		protected override void Dispose (bool disposing)
//		{
//			if (this.isDisposed)
//				return;
//			
//			this.isDisposed = true;
//			
//			this.data.Dispose();
//			base.Dispose (disposing);
//		}
//		
//		private bool isDisposed;
//		
//		private int position;
		private NSData data;
	}
}

