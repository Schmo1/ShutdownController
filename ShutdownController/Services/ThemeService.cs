using Microsoft.Win32;
using ShutdownController.Services.Abstraction;
using ShutdownController.Theming;
using System.Windows;

namespace ShutdownController.Services;

/// <summary>
/// Swaps the merged theme <see cref="ResourceDictionary"/> at runtime. In
/// <see cref="AppTheme.System"/> mode it follows the Windows light/dark setting
/// and reacts to live changes of it.
/// </summary>
public sealed class ThemeService : IThemeService
{
	private const string PropertyKey = "AppTheme";
	private static readonly Uri LightUri = new("Themes/LightTheme.xaml", UriKind.Relative);
	private static readonly Uri DarkUri = new("Themes/DarkTheme.xaml", UriKind.Relative);

	private bool _subscribed;

	public AppTheme Current { get; private set; } = AppTheme.System;

	public void Initialize()
	{
		Current = ReadStoredTheme();
		EnsureSystemSubscription();
		ApplyInternal(Current);
	}

	public void Apply(AppTheme theme)
	{
		Current = theme;
		App.Current.Properties[PropertyKey] = theme.ToString();
		EnsureSystemSubscription();
		ApplyInternal(theme);
	}

	private void EnsureSystemSubscription()
	{
		if (_subscribed)
		{
			return;
		}

		SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
		_subscribed = true;
	}

	private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
	{
		// Only the "System" mode mirrors the OS; the General category covers theme changes.
		if (Current == AppTheme.System && e.Category == UserPreferenceCategory.General)
		{
			App.Current.Dispatcher.Invoke(() => ApplyInternal(AppTheme.System));
		}
	}

	private static void ApplyInternal(AppTheme theme)
	{
		bool dark = theme == AppTheme.Dark
			|| (theme == AppTheme.System && IsSystemInDarkMode());

		var themeDictionary = new ResourceDictionary { Source = dark ? DarkUri : LightUri };
		var merged = App.Current.Resources.MergedDictionaries;

		// Drop any theme dictionary that is already merged, then add the new one.
		for (int i = merged.Count - 1; i >= 0; i--)
		{
			string source = merged[i].Source?.OriginalString ?? string.Empty;
			if (source.Contains("LightTheme.xaml") || source.Contains("DarkTheme.xaml"))
			{
				merged.RemoveAt(i);
			}
		}

		merged.Insert(0, themeDictionary);
	}

	private static AppTheme ReadStoredTheme()
	{
		object? value = App.Current.Properties[PropertyKey];
		return Enum.TryParse(value?.ToString(), out AppTheme theme) ? theme : AppTheme.System;
	}

	private static bool IsSystemInDarkMode()
	{
		using RegistryKey? key = Registry.CurrentUser.OpenSubKey(
			@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");

		// AppsUseLightTheme == 0  ->  dark mode is active.
		return key?.GetValue("AppsUseLightTheme") is int value && value == 0;
	}
}
