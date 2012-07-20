using System;
using System.Threading.Tasks;

namespace Xamarin
{
	internal class Timeout
	{
		public Timeout (int timeout, Action timesup)
		{
			if (timeout < 0)
				throw new ArgumentOutOfRangeException ("timeout");
			if (timesup == null)
				throw new ArgumentNullException ("timesup");

			this.timeout = TimeSpan.FromMilliseconds (timeout);
			this.timesup = timesup;

			Task.Factory.StartNew (Runner, TaskCreationOptions.LongRunning);
		}

		public void Cancel()
		{
			this.canceled = true;
		}

		private readonly TimeSpan timeout;
		private readonly Action timesup;
		private volatile bool canceled;

		private void Runner()
		{
			DateTime start = DateTime.Now;
			while (!this.canceled)
			{
				if (DateTime.Now - start < this.timeout)
				{
					Task.Delay (1);
					continue;
				}

				this.timesup();
				return;
			}
		}
	}
}