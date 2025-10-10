using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ShutdownController.Services.Abstraction;
using ShutdownController.Util;
using ShutdownController.Views.MessageBox;


namespace ShutdownController.ViewModels;

public partial class ClockViewModel : ObservableObject
{
	private readonly IEachSecondTick _timerTick;
	private readonly IServiceProvider _serviceProvider;
	[ObservableProperty]
	private string _currentTime;


	[ObservableProperty]
	private int _clockSetHours;

	[ObservableProperty]
	private int _clockSetMinutes;

	[ObservableProperty]
	private int _clockSetSeconds;

	[ObservableProperty]
	private bool _clockActive;


	public ClockViewModel(IEachSecondTick timerTick, IServiceProvider serviceProvider)
	{
		_timerTick = timerTick;
		_serviceProvider = serviceProvider;
		_timerTick.Tick += OnTimerTick;
		CurrentTime = string.Empty;

	}

	private void OnTimerTick(object? sender, System.Timers.ElapsedEventArgs e)
	{
		var currentTime = DateTimeOffset.Now;
		CurrentTime = currentTime.ToString("HH:mm:ss");
		CompareClock(currentTime);
	}

	internal void Init()
	{
		_timerTick.Start();

		ClockSetHours = PropertyParser.GetIntProperty(nameof(ClockSetHours));
		ClockSetMinutes = PropertyParser.GetIntProperty(nameof(ClockSetMinutes));
		ClockSetSeconds = PropertyParser.GetIntProperty(nameof(ClockSetSeconds));
	}

	partial void OnClockSetHoursChanged(int value)
	{
		App.Current.Properties[nameof(ClockSetHours)] = value;
	}

	partial void OnClockSetMinutesChanged(int value)
	{
		App.Current.Properties[nameof(ClockSetMinutes)] = value;
	}

	partial void OnClockSetSecondsChanged(int value)
	{
		App.Current.Properties[nameof(ClockSetSeconds)] = value;
	}

	private void CompareClock(DateTimeOffset currentTime)
	{
		if(ClockSetSeconds != currentTime.Second)
		{
			return;
		}

		if (ClockSetMinutes != currentTime.Minute)
		{
			return;
		}

		if (ClockSetHours != currentTime.Hour)
		{
			return;
		}

		CustomMessageBoxView? messagebox = _serviceProvider.GetService<CustomMessageBoxView>();
		
		if(messagebox is not null)
		{
			messagebox.Show();
		}
	}


	[RelayCommand]
	public Task OnClockStart()
	{
		ClockActive = !ClockActive;
		return Task.CompletedTask;
	}
}
