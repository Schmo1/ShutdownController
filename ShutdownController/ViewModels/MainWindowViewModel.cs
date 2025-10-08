using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace ShutdownController.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
	private readonly ILogger _logger;
	private readonly SettingsViewModel _settingsViewModel;
	private readonly DiskViewModel _diskViewModel;
	private readonly DownUploadViewModel _downUploadViewModel;
	private readonly ClockViewModel _clockViewModel;
	private readonly TimerViewModel _timerViewModel;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(DiskViewCommand))]
	[NotifyCanExecuteChangedFor(nameof(ClockViewCommand))]
	[NotifyCanExecuteChangedFor(nameof(DownUploadViewCommand))]
	[NotifyCanExecuteChangedFor(nameof(SettingsViewCommand))]
	[NotifyCanExecuteChangedFor(nameof(TimerViewCommand))]
	private object _currentView;



	public bool TestingModeActiv { get; }

	public MainWindowViewModel(
		ILogger logger, 
		SettingsViewModel settingsViewModel,
		DiskViewModel diskViewModel, 
		DownUploadViewModel downUploadViewModel, 
		ClockViewModel clockViewModel,
		TimerViewModel timerViewModel)
	{
		_logger = logger;
		_settingsViewModel = settingsViewModel;
		_diskViewModel = diskViewModel;
		_downUploadViewModel = downUploadViewModel;
		_clockViewModel = clockViewModel;
		_timerViewModel = timerViewModel;
#if DEBUG
		TestingModeActiv = true;
#endif
		_currentView = _timerViewModel;
	}


	private bool IsTimerViewInActive() { return CurrentView != _timerViewModel; }

	[RelayCommand(CanExecute = nameof(IsTimerViewInActive))]
	
	public Task OnTimerView()
	{
		CurrentView = _timerViewModel;
		return Task.CompletedTask;
	}

	private bool IsClockViewActive() { return CurrentView != _clockViewModel; }

	[RelayCommand(CanExecute = nameof(IsClockViewActive))]
	public Task OnClockView()
	{
		CurrentView = _clockViewModel;
		return Task.CompletedTask;
	}


	private bool IsDownUploadViewInActive() { return CurrentView != _downUploadViewModel; }

	[RelayCommand(CanExecute = nameof(IsDownUploadViewInActive))]
	public Task OnDownUploadView()
	{
		CurrentView = _downUploadViewModel;
		return Task.CompletedTask;
	}

	private bool IsDiskViewInActive() { return CurrentView != _diskViewModel; }


	[RelayCommand(CanExecute = nameof(IsDiskViewInActive))]
	public Task OnDiskView()
	{
		CurrentView = _diskViewModel;
		return Task.CompletedTask;
	}

	private bool IsSettingsViewInActive() { return CurrentView != _settingsViewModel; }

	[RelayCommand(CanExecute = nameof(IsSettingsViewInActive))]
	public Task OnSettingsView()
	{
		CurrentView = _settingsViewModel;
		return Task.CompletedTask;
	}

	

	[RelayCommand]
	public Task OnCloseApp()
	{
		App.Current.Shutdown();
		return Task.CompletedTask;
	}


	[RelayCommand]
	public Task OnLoaded()
	{
		return Task.CompletedTask;
	}

	[RelayCommand]
	public Task OnClosing()
	{
		_logger.Information("Application is closing...");
		return Task.CompletedTask;
	}

}
