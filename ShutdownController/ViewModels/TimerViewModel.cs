using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ShutdownController.Services.Abstraction;
using ShutdownController.Util;
using ShutdownController.Views.MessageBox;
using System.Windows;

namespace ShutdownController.ViewModels;

public partial class TimerViewModel : ObservableObject
{
	private readonly IEachSecondTick _timerTick;
	private readonly IServiceProvider _serviceProvider;

	private int _remainingSeconds;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TimerDisplay))]
	private int _timerSetHours;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TimerDisplay))]
	private int _timerSetMinutes;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TimerDisplay))]
	private int _timerSetSeconds;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TimerDisplay))]
	private bool _isRunning;

	public string TimerDisplay
	{
		get
		{
			TimeSpan time = IsRunning
				? TimeSpan.FromSeconds(_remainingSeconds)
				: new TimeSpan(TimerSetHours, TimerSetMinutes, TimerSetSeconds);

			return $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
		}
	}

	public TimerViewModel(IEachSecondTick timerTick, IServiceProvider serviceProvider)
	{
		_timerTick = timerTick;
		_serviceProvider = serviceProvider;
		_timerTick.Tick += OnTimerTick;

		TimerSetHours = PropertyParser.GetIntProperty(nameof(TimerSetHours));
		TimerSetMinutes = PropertyParser.GetIntProperty(nameof(TimerSetMinutes));
		TimerSetSeconds = PropertyParser.GetIntProperty(nameof(TimerSetSeconds));
	}

	partial void OnTimerSetHoursChanged(int value) => App.Current.Properties[nameof(TimerSetHours)] = value;

	partial void OnTimerSetMinutesChanged(int value) => App.Current.Properties[nameof(TimerSetMinutes)] = value;

	partial void OnTimerSetSecondsChanged(int value) => App.Current.Properties[nameof(TimerSetSeconds)] = value;

	[RelayCommand]
	public Task OnStartStop()
	{
		if (IsRunning)
		{
			StopTimer();
			return Task.CompletedTask;
		}

		_remainingSeconds = (TimerSetHours * 3600) + (TimerSetMinutes * 60) + TimerSetSeconds;

		if (_remainingSeconds <= 0)
		{
			return Task.CompletedTask;
		}

		IsRunning = true;
		_timerTick.Start();
		return Task.CompletedTask;
	}

	private void StopTimer()
	{
		_timerTick.Stop();
		IsRunning = false;
	}

	private void OnTimerTick(object? sender, System.Timers.ElapsedEventArgs e)
	{
		if (!IsRunning)
		{
			return;
		}

		_remainingSeconds--;
		OnPropertyChanged(nameof(TimerDisplay));

		if (_remainingSeconds > 0)
		{
			return;
		}

		StopTimer();
		TriggerAction();
	}

	private void TriggerAction()
	{
		App.Current.Dispatcher.Invoke(() =>
		{
			CustomMessageBoxView? messageBox = _serviceProvider.GetService<CustomMessageBoxView>();
			messageBox?.Show();
		});
	}
}
