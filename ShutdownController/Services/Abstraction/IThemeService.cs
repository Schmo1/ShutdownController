using ShutdownController.Theming;

namespace ShutdownController.Services.Abstraction;

/// <summary>Applies and persists the selected application theme (light / dark / system).</summary>
public interface IThemeService
{
	/// <summary>The currently selected theme mode.</summary>
	AppTheme Current { get; }

	/// <summary>Loads the persisted theme and applies it. Call once at startup.</summary>
	void Initialize();

	/// <summary>Applies the given theme, persists the choice and reacts to system changes.</summary>
	void Apply(AppTheme theme);
}
