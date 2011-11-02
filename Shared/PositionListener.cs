using System;
using System.Collections.Generic;

namespace MonoMobile.Extensions
{
	public class PositionListener
		: IObservable<Position>, IDisposable
	{
		internal PositionListener (Action<PositionListener> close)
		{
			this.close = close;
		}

		public IDisposable Subscribe (IObserver<Position> observer)
		{
			if (observer == null)
				throw new ArgumentNullException ("observer");

			lock (this.observers)
			{
				this.observers.Add (observer, true);
				return new DelegatedObserver<Position> (observer, Remove);
			}
		}

		public void Dispose()
		{
			this.close (this);
		}

		internal void OnNext (Position position)
		{
			lock (this.observers)
			{
				foreach (IObserver<Position> o in this.observers.Keys)
					o.OnNext (position);
			}
		}

		internal void OnError (Exception error)
		{
			lock (this.observers)
			{
				foreach (IObserver<Position> o in this.observers.Keys)
					o.OnError (error);
			}
		}

		internal void OnCompleted()
		{
			lock (this.observers)
			{
				foreach (IObserver<Position> o in this.observers.Keys)
					o.OnCompleted();
			}
		}

		private readonly Action<PositionListener> close;
		// WP7 still doesn't have HashSet
		private readonly Dictionary<IObserver<Position>, bool> observers = new Dictionary<IObserver<Position>, bool>();

		private void Remove (IObserver<Position> observer)
		{
			lock (this.observers)
				this.observers.Remove (observer);
		}
	}
}