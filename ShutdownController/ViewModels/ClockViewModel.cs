using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShutdownController.ViewModels;

public partial class ClockViewModel : ObservableObject
{
	[ObservableProperty]
	private int _clockHours;

	[ObservableProperty]
	private int _clockMinutes;

	[ObservableProperty]
	private int _clockSeconds;


	[ObservableProperty]
	private int _clockSetHours;

	[ObservableProperty]
	private int _clockSetMinutes;

	[ObservableProperty]
	private int _clockSetSeconds;


	[RelayCommand]
	public Task OnClockStartCommand()
	{
		return Task.CompletedTask;
	}
}
