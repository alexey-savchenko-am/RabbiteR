namespace Rabbiter.Core
{
	using Rabbiter.Core.Abstractions;
	using System;
	using System.Collections.Concurrent;
	using System.Threading;
	using System.Threading.Tasks;

	public class QueueManager<T>
		: IQueueBasedDispatcher<T>
	{
		public int DispatcherCount
		{
			get => _dispatcherCount;
		}

		public bool IsWorking
		{
			get => _running == 1;
		}

		private volatile ConcurrentQueue<QueuedObject> _queue =
			new ConcurrentQueue<QueuedObject>();

		private volatile int _running = 0;

		private volatile int _dispatcherCount = 0;
		private readonly object _lockObject = new object();

		private readonly Func<T, Task> _handler;

		public QueueManager(Func<T, Task> handler)
        {
            _handler = handler;
        }

        public void Enqueue(T item)
		{
			var runningTmp = _running;

			Interlocked.Increment(ref _dispatcherCount);

			_queue.Enqueue(new QueuedObject(item));

			if (Interlocked.CompareExchange(ref _running, 1, runningTmp) == 0)
			{
				Task.Run(StartProcessor);
			}
		}

		private void StartProcessor()
		{
			SpinWait wait = new SpinWait();

			while (true)
			{
				if (!_queue.TryDequeue(out var queuedObj))
				{
					var runningTmp = _running;

	
					if (Interlocked.CompareExchange(ref _running, 0, runningTmp) == 1)
						return;
						
					continue;
				}

				try
				{
					_handler.Invoke(queuedObj.Item);
				}
				finally
				{
					Interlocked.Decrement(ref _dispatcherCount);
				}

				wait.SpinOnce();
			}
		}

		class QueuedObject
		{
			public QueuedObject(T item)
			{
				Item = item;
			}

			internal T Item { get; }

		}
	}
}
