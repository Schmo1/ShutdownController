

using CommunityToolkit.Mvvm.ComponentModel;
using ShutdownController.Services.Abstraction;
using ShutdownController.Theming;
using ShutdownController.Util;

namespace ShutdownController.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
	private readonly IThemeService _themeService;

	[ObservableProperty]
	private AppTheme _themeMode;

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

	public SettingsViewModel(IThemeService themeService)
	{
		_themeService = themeService;
		_themeMode = themeService.Current;
	}

	public void Init()
	{
		LoadSettings();
	}

	partial void OnThemeModeChanged(AppTheme value) => _themeService.Apply(value);

	partial void OnAutoStartActiveChanged(bool value)
	{
		App.Current.Properties[nameof(AutoStartActive)] = value;
	}

	partial void OnStartMinimizedChanged(bool value)
	{
		App.Current.Properties[nameof(StartMinimized)] = value;
	}

	partial void OnDisablePushMessagesChanged(bool value)
	{
		App.Current.Properties[nameof(DisablePushMessages)] = value;
	}

	partial void OnWaitingTimeBeforInvokeActionChanged(int value)
	{
		App.Current.Properties[nameof(WaitingTimeBeforInvokeAction)] = value;	
	}

	partial void OnOnClosingRunInBackgroundChanged(bool value)
	{
		App.Current.Properties[nameof(OnClosingRunInBackground)] = value;
	}


	private void LoadSettings()
	{
		AutoStartActive = PropertyParser.GetBoolProperty(nameof(AutoStartActive), false);
		StartMinimized = PropertyParser.GetBoolProperty(nameof(StartMinimized), false);	
		OnClosingRunInBackground = PropertyParser.GetBoolProperty(nameof(OnClosingRunInBackground), true);
		DisablePushMessages = PropertyParser.GetBoolProperty(nameof(DisablePushMessages), false);
		WaitingTimeBeforInvokeAction = PropertyParser.GetIntProperty(nameof(WaitingTimeBeforInvokeAction), 30);
	}
}
