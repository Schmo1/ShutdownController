using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace ShutdownController.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
	private readonly ILogger _logger;
	private readonly SettingsViewModel _settingsViewModel;
	[ObservableProperty]
	private object _currentView;



	public bool TestingModeActiv { get; }

	public MainWindowViewModel(ILogger logger, SettingsViewModel settingsViewModel)
	{
		_logger = logger;
		_settingsViewModel = settingsViewModel;
#if DEBUG
		TestingModeActiv = true;
#endif
	}

	[RelayCommand]
	public Task OnTimerView()
	{
		return Task.CompletedTask;
	}

	[RelayCommand]
	public Task OnClockView()
	{
		return Task.CompletedTask;
	}

	[RelayCommand]
	public Task OnDownUploadView()
	{
		return Task.CompletedTask;
	}


	[RelayCommand]
	public Task OnDiskView()
	{
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
