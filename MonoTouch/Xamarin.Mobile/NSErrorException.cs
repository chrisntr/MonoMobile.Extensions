using System;
using MonoTouch.Foundation;

namespace Xamarin
{
	public class NSErrorException
		: Exception
	{
		internal NSErrorException (NSError error)
			: base (error.LocalizedDescription)
		{
			Error = error;
		}

		public NSError Error
		{
			get;
			private set;
		}
	}
}