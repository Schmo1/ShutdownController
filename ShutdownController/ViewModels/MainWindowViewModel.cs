using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace ShutdownController.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
	private readonly ILogger _logger;

	[ObservableProperty]
	private object _currentView;

	[ObservableProperty]
	private ShutdownOptionsViewModel _shutdownOptionsViewModel;


	public bool TestingModeActiv { get; }

	public MainWindowViewModel(ILogger logger, ShutdownOptionsViewModel optionsViewModel)
	{
		_logger = logger;
#if DEBUG
		TestingModeActiv = true;
#endif
		ShutdownOptionsViewModel = optionsViewModel;
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


	[RelayCommand]
	public Task OnSettingsView()
	{
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
