using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using ShutdownController.Services.Abstraction;
using ShutdownController.Util;
using ShutdownController.Views.MessageBox;
using System.Windows.Media;

namespace ShutdownController.ViewModels;

/// <summary>
/// Shared logic for the Down-/Upload and Disk observing views: it polls a speed
/// source once per second, draws the two rolling line graphs and triggers the
/// selected shutdown action once the observed speed stays below the threshold
/// for the configured amount of seconds.
/// </summary>
public abstract partial class ObservingViewModelBase : ObservableObject
{
	// Coordinate system of the graph canvas defined in the views.
	protected const double PlotWidth = 430;
	protected const double PlotHeight = 140;
	private const int WindowSeconds = 30;

	private readonly IEachSecondTick _tick;
	private readonly IServiceProvider _serviceProvider;
	private readonly List<double> _primaryHistory = new();
	private readonly List<double> _secondaryHistory = new();
	private int _belowThresholdCounter;

	[ObservableProperty]
	private bool _isRunning;

	[ObservableProperty]
	private int _seconds;

	[ObservableProperty]
	private double _observingSpeed;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsSecondarySelected))]
	private bool _isPrimarySelected = true;

	public bool IsSecondarySelected => !IsPrimarySelected;

	[ObservableProperty]
	private PointCollection _primaryPoints = new();

	[ObservableProperty]
	private PointCollection _secondaryPoints = new();

	[ObservableProperty]
	private string _yAxisMaxLabel = "1 MB/s";

	[ObservableProperty]
	private string _yAxisMidLabel = "0,5 MB/s";

	protected ObservingViewModelBase(IEachSecondTick tick, IServiceProvider serviceProvider)
	{
		_tick = tick;
		_serviceProvider = serviceProvider;
		_tick.Tick += OnTick;

		Seconds = PropertyParser.GetIntProperty($"{SettingsPrefix}Seconds", 10);
		ObservingSpeed = GetDoubleProperty($"{SettingsPrefix}ObservingSpeed", 1.5);
		IsPrimarySelected = PropertyParser.GetBoolProperty($"{SettingsPrefix}IsPrimarySelected", true);
	}

	/// <summary>Prefix used to store this view's settings in the application properties.</summary>
	protected abstract string SettingsPrefix { get; }

	/// <summary>Reads the current primary and secondary speed in MB/s.</summary>
	protected abstract (double primary, double secondary) ReadSpeed();

	/// <summary>Called right before observing starts so the source can capture a baseline.</summary>
	protected abstract void InitializeSource();

	partial void OnSecondsChanged(int value) => App.Current.Properties[$"{SettingsPrefix}Seconds"] = value;

	partial void OnObservingSpeedChanged(double value) =>
		App.Current.Properties[$"{SettingsPrefix}ObservingSpeed"] = value;

	partial void OnIsPrimarySelectedChanged(bool value) =>
		App.Current.Properties[$"{SettingsPrefix}IsPrimarySelected"] = value;

	[RelayCommand]
	private void SelectPrimary() => IsPrimarySelected = true;

	[RelayCommand]
	private void SelectSecondary() => IsPrimarySelected = false;

	[RelayCommand]
	private void StartStop()
	{
		if (IsRunning)
		{
			StopObserving();
			return;
		}

		_primaryHistory.Clear();
		_secondaryHistory.Clear();
		_belowThresholdCounter = 0;
		InitializeSource();

		IsRunning = true;
		_tick.Start();
	}

	private void StopObserving()
	{
		_tick.Stop();
		IsRunning = false;
	}

	private void OnTick(object? sender, System.Timers.ElapsedEventArgs e)
	{
		if (!IsRunning)
		{
			return;
		}

		(double primary, double secondary) = ReadSpeed();

		Append(_primaryHistory, primary);
		Append(_secondaryHistory, secondary);

		RedrawGraph();
		EvaluateThreshold(IsPrimarySelected ? primary : secondary);
	}

	private static void Append(List<double> history, double value)
	{
		history.Add(value);
		if (history.Count > WindowSeconds)
		{
			history.RemoveAt(0);
		}
	}

	private void RedrawGraph()
	{
		double max = 0;
		foreach (double value in _primaryHistory)
		{
			max = Math.Max(max, value);
		}
		foreach (double value in _secondaryHistory)
		{
			max = Math.Max(max, value);
		}

		double yMax = Math.Max(1.0, Math.Ceiling(max));

		PrimaryPoints = BuildPoints(_primaryHistory, yMax);
		SecondaryPoints = BuildPoints(_secondaryHistory, yMax);
		YAxisMaxLabel = $"{yMax:0.#} MB/s";
		YAxisMidLabel = $"{yMax / 2:0.#} MB/s";
	}

	private static PointCollection BuildPoints(List<double> history, double yMax)
	{
		var points = new PointCollection();
		for (int i = 0; i < history.Count; i++)
		{
			double x = i * PlotWidth / (WindowSeconds - 1);
			double y = PlotHeight - Math.Min(history[i] / yMax, 1.0) * PlotHeight;
			points.Add(new System.Windows.Point(x, y));
		}

		points.Freeze();
		return points;
	}

	private void EvaluateThreshold(double observedValue)
	{
		if (observedValue < ObservingSpeed)
		{
			_belowThresholdCounter++;
		}
		else
		{
			_belowThresholdCounter = 0;
		}

		if (_belowThresholdCounter < Seconds)
		{
			return;
		}

		StopObserving();
		App.Current.Dispatcher.Invoke(() =>
		{
			CustomMessageBoxView? messageBox = _serviceProvider.GetService<CustomMessageBoxView>();
			messageBox?.Show();
		});
	}

	private static double GetDoubleProperty(string key, double defaultValue)
	{
		object? value = App.Current.Properties[key];
		if (double.TryParse(value?.ToString(), out double parsed))
		{
			return parsed;
		}

		App.Current.Properties[key] = defaultValue;
		return defaultValue;
	}
}
