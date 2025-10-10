using ShutdownController.Services.Abstraction;
using System.Timers;

namespace ShutdownController.Services;

public class EachSecondTick : IEachSecondTick
{
	private bool _disposedValue;
	private readonly System.Timers.Timer _timer;

	public event ElapsedEventHandler? Tick;

	public EachSecondTick()
	{
		_timer = new System.Timers.Timer
		{
			Interval = 1000,
			AutoReset = true
		};
		_timer.Elapsed += OnTimerElapsed; 
	}

	private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
	{
		Tick?.Invoke(this, e);
	}

	public void Start()
	{
		if (_timer.Enabled)
		{
			return;
		}
		_timer.Enabled = true;
		_timer.Start();
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				_timer.Stop();
				_timer.Dispose();
			}

			
			_disposedValue = true;
		}
	}


	~EachSecondTick()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
