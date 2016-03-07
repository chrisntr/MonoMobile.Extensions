//
//  Copyright 2011-2013, Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

using System;
using System.IO;

namespace Xamarin.Media
{
	public sealed class MediaFile
		: IDisposable
	{
		internal MediaFile (string path, Func<Stream> streamGetter, Action<bool> dispose = null)
		{
			this.dispose = dispose;
			this.streamGetter = streamGetter;
			this.path = path;
		}

		public string Path
		{
			get
			{
				if (this.isDisposed)
					throw new ObjectDisposedException (null);

				return this.path;
			}
		}

		public Stream GetStream()
		{
			if (this.isDisposed)
				throw new ObjectDisposedException (null);

			return this.streamGetter();
		}

		public void Dispose()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		private bool isDisposed;
		private readonly Action<bool> dispose;
		private readonly Func<Stream> streamGetter;
		private readonly string path;
		
		private void Dispose (bool disposing)
		{
			if (this.isDisposed)
				return;

			this.isDisposed = true;
			if (this.dispose != null)
				this.dispose (disposing);
		}

		~MediaFile()
		{
			Dispose (false);
		}
	}
}