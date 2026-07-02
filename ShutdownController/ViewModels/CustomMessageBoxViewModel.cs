using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ShutdownController.Messages;
using ShutdownController.Views.MessageBox;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace ShutdownController.ViewModels;

public partial class CustomMessageBoxViewModel : ObservableObject
{

    public static bool IsActive { get; private set; }   
    private readonly DispatcherTimer _timer = new DispatcherTimer();
 
	[ObservableProperty]
	private int _waitingTimeBeforInvokeAction;


    public string ActionToPerforme { get; }


	public CustomMessageBoxViewModel(ShutdownOptionsViewModel shutdownOptionsView)
    {
        IsActive = true;  
        StartTimer();

        if (shutdownOptionsView.IsSleepButtonSelected)
        {
            ActionToPerforme = "Sleep";
		}else if(shutdownOptionsView.IsRestartButtonSelected)
        {
            ActionToPerforme = "Restart";
        }
        else
        {
            ActionToPerforme = "Shutdown";
        }

		var loadedValue = App.Current.Properties[nameof(WaitingTimeBeforInvokeAction)]?.ToString();
		if (!string.IsNullOrEmpty(loadedValue))
		{
            if (!int.TryParse(loadedValue, out int parsedValue))
            {
                WaitingTimeBeforInvokeAction = 30;
                return;
            }
            WaitingTimeBeforInvokeAction = parsedValue;
		}

	}

    public void StartTimer()
    {
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += SubtractSecond;      
        _timer.Start();
    }

    public void StopAction()
    {
        _timer?.Stop();
        IsActive = false;

		// Only close the message box windows, never the main window.
		foreach (Window window in Application.Current.Windows.OfType<CustomMessageBoxView>().ToList())
		{
			window.Close();
		}
	}

    [RelayCommand]
    public Task OnAbortAction()
    {
        StopAction();
        return Task.CompletedTask;
	}

	private void SubtractSecond(object? sender, EventArgs args)
    {
		WaitingTimeBeforInvokeAction --;

        if(WaitingTimeBeforInvokeAction <= 0)
        {
            StopAction();
            WeakReferenceMessenger.Default.Send(new InvokeActionMessage());
		}
    }
}