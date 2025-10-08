

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
	private bool _savePerformance;

	[ObservableProperty]
	private bool _onClosingRunInBackground;

}
