using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShutdownController.ViewModels;

public partial class ShutdownOptionsViewModel : ObservableObject
{

	[ObservableProperty]
	private bool _isShutdownButtonSelected;

	[ObservableProperty]
	private bool _isRestartButtonSelected;

	[ObservableProperty]
	private bool _isSleepButtonSelected;


	public ShutdownOptionsViewModel()
	{
		LoadSettings();
	}


	[RelayCommand]
	public Task OnShutdownButton()
	{
		
		IsShutdownButtonSelected = true;
		IsSleepButtonSelected = false;
		IsRestartButtonSelected = false;
		SaveSettings();
		return Task.CompletedTask;
	}

	[RelayCommand]
	public Task OnRestartButton()
	{
		
		IsShutdownButtonSelected = false;
		IsSleepButtonSelected = false;
		IsRestartButtonSelected = true;
		SaveSettings();
		return Task.CompletedTask;
	}

	[RelayCommand]
	public Task OnSleepButton()
	{		
		IsShutdownButtonSelected = false;
		IsSleepButtonSelected = true;
		IsRestartButtonSelected = false;
		SaveSettings();
		return Task.CompletedTask;
	}

	private void LoadSettings()
	{
		string? loadedValue = App.Current.Properties[nameof(IsShutdownButtonSelected)]?.ToString();
		if (!string.IsNullOrEmpty(loadedValue))
		{
			IsShutdownButtonSelected = bool.Parse(loadedValue);
		}

		loadedValue = App.Current.Properties[nameof(IsSleepButtonSelected)]?.ToString();
		if (!string.IsNullOrEmpty(loadedValue))
		{
			IsSleepButtonSelected = bool.Parse(loadedValue);
		}

		loadedValue = App.Current.Properties[nameof(IsRestartButtonSelected)]?.ToString();
		if (!string.IsNullOrEmpty(loadedValue))
		{
			IsRestartButtonSelected = bool.Parse(loadedValue);
		}
	}

	private void SaveSettings()
	{
		App.Current.Properties[nameof(IsShutdownButtonSelected)] = IsShutdownButtonSelected;
		App.Current.Properties[nameof(IsRestartButtonSelected)] = IsRestartButtonSelected;
		App.Current.Properties[nameof(IsSleepButtonSelected)] = IsSleepButtonSelected;
	}

}
