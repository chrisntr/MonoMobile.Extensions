using System;

namespace MonoMobile.Extensions
{
	internal class DelegatedObserver<T>
		: IObserver<T>, IDisposable
	{
		public DelegatedObserver (IObserver<T> observer, Action<IObserver<T>> dispose)
		{
			if (observer == null)
				throw new ArgumentNullException ("observer");
			if (dispose == null)
				throw new ArgumentNullException ("dispose");

			this.observer = observer;
			this.dispose = dispose;
		}

		public void OnCompleted()
		{
			this.observer.OnCompleted();
		}

		public void OnNext (T element)
		{
			this.observer.OnNext (element);
		}

		public void OnError (Exception error)
		{
			this.observer.OnError (error);
		}

		public void Dispose()
		{
			this.dispose (this);
		}

		private readonly IObserver<T> observer;
		private readonly Action<IObserver<T>> dispose;
	}
}