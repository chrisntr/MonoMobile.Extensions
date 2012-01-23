using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Database;

namespace Xamarin
{
	internal class AsyncQuery<T>
		: AsyncQueryHandler
	{
		internal AsyncQuery (ContentResolver cr, Func<ICursor, T> selector)
			: base (cr)
		{
			if (selector == null)
				throw new ArgumentNullException ("selector");

			this.selector = selector;
		}

		internal AsyncQuery (ContentResolver cr, Func<ICursor, T> selector, Func<ICursor, bool> predicate)
			: this (cr, selector)
		{
			this.selector = selector;
			this.predicate = predicate;
		}

		public Task<T> Task
		{
			get { return this.tcs.Task; }
		}

		protected override void OnQueryComplete (int token, Java.Lang.Object cookie, ICursor cursor)
		{
			bool set = false;
			while (cursor.MoveToNext())
			{
				if (this.predicate == null || this.predicate (cursor))
				{
					set = true;
					this.tcs.SetResult (this.selector (cursor));
					break;
				}
			}

			if (!set)
				this.tcs.SetResult (default(T));

			base.OnQueryComplete (token, cookie, cursor);
		}

		private readonly TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
		private readonly Func<ICursor, T> selector;
		private readonly Func<ICursor, bool> predicate;
	}
}