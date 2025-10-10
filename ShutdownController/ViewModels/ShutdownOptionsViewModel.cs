using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ShutdownController.Messages;
using ShutdownController.Services;
using System.Windows;

namespace ShutdownController.ViewModels;

public partial class ShutdownOptionsViewModel : ObservableObject, IRecipient<InvokeActionMessage>
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
		WeakReferenceMessenger.Default.Register(this);	
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

	public void Receive(InvokeActionMessage message)
	{
		try
		{
#if !DEBUG
			if (IsSleepButtonSelected)
			{
				ShutdownOptionInvoker.Sleep();
			}
			else if (IsRestartButtonSelected)
			{
				ShutdownOptionInvoker.Restart();
			}
			else if (IsShutdownButtonSelected)
			{
				ShutdownOptionInvoker.Shutdown();
			}
			else
				//default action
				ShutdownOptionInvoker.Sleep();
#else
			MessageBox.Show("Debug mode - action skipped");
#endif
		}
		catch (Exception)
		{

			//todo implement logging
		}
	}
}
