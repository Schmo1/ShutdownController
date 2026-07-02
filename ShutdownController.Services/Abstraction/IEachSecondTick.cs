using System.Timers;

namespace ShutdownController.Services.Abstraction;

public interface IEachSecondTick: IDisposable
{
	event ElapsedEventHandler? Tick;

	void Start();

	void Stop();
}