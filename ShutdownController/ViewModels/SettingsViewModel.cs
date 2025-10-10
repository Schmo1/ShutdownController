

using CommunityToolkit.Mvvm.ComponentModel;

namespace ShutdownController.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
	[ObservableProperty]
	private bool _autoStartActive;

	[ObservableProperty]
	private bool _startMinimized;

	[ObservableProperty]
	private bool _disablePushMessages;


	[ObservableProperty]
	private bool _onClosingRunInBackground;

	[ObservableProperty]
	private int _waitingTimeBeforInvokeAction;




	private void LoadSettings()
	{
		string? loadedValue = App.Current.Properties[nameof(AutoStartActive)]?.ToString();
		if (!string.IsNullOrEmpty(loadedValue) && bool.TryParse(loadedValue, out bool autoValue))
		{			
			AutoStartActive = autoValue;			
		}

		loadedValue = App.Current.Properties[nameof(StartMinimized)]?.ToString();
		if (!string.IsNullOrEmpty(loadedValue) && bool.TryParse(loadedValue, out bool value))
		{
			StartMinimized = value;
		}

		loadedValue = App.Current.Properties[nameof(DisablePushMessages)]?.ToString();
		if (!string.IsNullOrEmpty(loadedValue) && bool.TryParse(loadedValue, out bool disableValue))
		{
			DisablePushMessages = disableValue;
		}

		loadedValue = App.Current.Properties[nameof(OnClosingRunInBackground)]?.ToString();
		if (!string.IsNullOrEmpty(loadedValue) && bool.TryParse(loadedValue, out bool onClosing))
		{
			DisablePushMessages = onClosing;
		}

		loadedValue = App.Current.Properties[nameof(WaitingTimeBeforInvokeAction)]?.ToString();
		if (!string.IsNullOrEmpty(loadedValue) && int.TryParse(loadedValue, out int waitingTime))
		{
			WaitingTimeBeforInvokeAction = waitingTime;
		}
		else
		{
			WaitingTimeBeforInvokeAction = 30;
		}
	}



	private void SaveSettings()
	{
		App.Current.Properties[nameof(AutoStartActive)] = AutoStartActive;
		App.Current.Properties[nameof(StartMinimized)] = StartMinimized;
		App.Current.Properties[nameof(DisablePushMessages)] = DisablePushMessages;
		App.Current.Properties[nameof(WaitingTimeBeforInvokeAction)] = WaitingTimeBeforInvokeAction;
		App.Current.Properties[nameof(OnClosingRunInBackground)] = OnClosingRunInBackground;
	}


}
