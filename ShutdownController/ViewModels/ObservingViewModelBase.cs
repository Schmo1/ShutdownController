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
	private bool _isMonitoring;

	// Colours for the threshold marker line: green while the observed speed stays
	// below the limit, red once it is reached or exceeded.
	private static readonly Brush BelowThresholdBrush = CreateFrozenBrush(0x5f, 0xd3, 0x5f);
	private static readonly Brush AboveThresholdBrush = CreateFrozenBrush(0xff, 0x4d, 0x4d);

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
	private PointCollection _primaryArea = new();

	[ObservableProperty]
	private PointCollection _secondaryArea = new();

	[ObservableProperty]
	private string _yAxisMaxLabel = "1 MB/s";

	[ObservableProperty]
	private string _yAxisMidLabel = "0,5 MB/s";

	// Vertical position (in canvas pixels) of the horizontal threshold marker line.
	[ObservableProperty]
	private double _thresholdLineY = PlotHeight;

	// Brush the threshold marker line is drawn with (green below, red above the limit).
	[ObservableProperty]
	private Brush _thresholdBrush = BelowThresholdBrush;

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

	partial void OnObservingSpeedChanged(double value)
	{
		App.Current.Properties[$"{SettingsPrefix}ObservingSpeed"] = value;
		// Reposition and recolour the marker line for the new limit.
		RedrawGraph();
		UpdateThresholdIndicator();
	}

	partial void OnIsPrimarySelectedChanged(bool value)
	{
		App.Current.Properties[$"{SettingsPrefix}IsPrimarySelected"] = value;
		// The limit is checked against the selected series, so its colour may change.
		UpdateThresholdIndicator();
	}

	[RelayCommand]
	private void SelectPrimary() => IsPrimarySelected = true;

	[RelayCommand]
	private void SelectSecondary() => IsPrimarySelected = false;

	/// <summary>
	/// Starts the continuous sampling so the graph shows live data even while the
	/// auto-shutdown watch (Start button) is not armed. Called when the view is shown.
	/// </summary>
	public void Activate()
	{
		if (_isMonitoring)
		{
			return;
		}

		_isMonitoring = true;
		InitializeSource();
		RedrawGraph();
		UpdateThresholdIndicator();
		_tick.Start();
	}

	/// <summary>Re-primes the source and clears the graph after the drive/adapter changed.</summary>
	protected void RestartSource()
	{
		_primaryHistory.Clear();
		_secondaryHistory.Clear();
		_belowThresholdCounter = 0;

		if (_isMonitoring)
		{
			InitializeSource();
		}

		RedrawGraph();
		UpdateThresholdIndicator();
	}

	[RelayCommand]
	private void StartStop()
	{
		if (IsRunning)
		{
			StopObserving();
			return;
		}

		// The graph is already live; pressing Start only arms the auto-shutdown watch.
		_belowThresholdCounter = 0;
		Activate();
		IsRunning = true;
	}

	private void StopObserving()
	{
		// Keep sampling so the graph stays live; only disarm the shutdown watch.
		IsRunning = false;
	}

	private void OnTick(object? sender, System.Timers.ElapsedEventArgs e)
	{
		(double primary, double secondary) = ReadSpeed();

		// The timer fires on a thread-pool thread; marshal to the UI thread so the
		// bound polylines refresh reliably and freezables are created there.
		App.Current.Dispatcher.Invoke(() =>
		{
			Append(_primaryHistory, primary);
			Append(_secondaryHistory, secondary);

			RedrawGraph();
			UpdateThresholdIndicator();

			// Only the armed Start button drives the shutdown countdown.
			if (IsRunning)
			{
				EvaluateThreshold(IsPrimarySelected ? primary : secondary);
			}
		});
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

		// Keep the threshold within the visible range so its marker line is always shown.
		double yMax = Math.Max(1.0, Math.Ceiling(Math.Max(max, ObservingSpeed)));

		PrimaryPoints = BuildPoints(_primaryHistory, yMax);
		SecondaryPoints = BuildPoints(_secondaryHistory, yMax);
		PrimaryArea = BuildArea(_primaryHistory, yMax);
		SecondaryArea = BuildArea(_secondaryHistory, yMax);
		ThresholdLineY = MapY(ObservingSpeed, yMax);
		YAxisMaxLabel = $"{yMax:0.#} MB/s";
		YAxisMidLabel = $"{yMax / 2:0.#} MB/s";
	}

	private static double MapX(int index) => index * PlotWidth / (WindowSeconds - 1);

	private static double MapY(double value, double yMax) =>
		PlotHeight - Math.Min(value / yMax, 1.0) * PlotHeight;

	private static PointCollection BuildPoints(List<double> history, double yMax)
	{
		var points = new PointCollection();
		for (int i = 0; i < history.Count; i++)
		{
			points.Add(new System.Windows.Point(MapX(i), MapY(history[i], yMax)));
		}

		points.Freeze();
		return points;
	}

	// Closed polygon following the line and going back along the baseline, used
	// to fill the area under the curve so the current value stays clearly visible.
	private static PointCollection BuildArea(List<double> history, double yMax)
	{
		var points = new PointCollection();
		if (history.Count < 2)
		{
			return points;
		}

		points.Add(new System.Windows.Point(MapX(0), PlotHeight));
		for (int i = 0; i < history.Count; i++)
		{
			points.Add(new System.Windows.Point(MapX(i), MapY(history[i], yMax)));
		}
		points.Add(new System.Windows.Point(MapX(history.Count - 1), PlotHeight));

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

		// Already on the UI thread (called from the marshalled tick handler).
		CustomMessageBoxView? messageBox = _serviceProvider.GetService<CustomMessageBoxView>();
		messageBox?.Show();
	}

	private double CurrentObservedValue
	{
		get
		{
			List<double> history = IsPrimarySelected ? _primaryHistory : _secondaryHistory;
			return history.Count > 0 ? history[^1] : 0;
		}
	}

	private void UpdateThresholdIndicator()
	{
		// Below the limit → green (idle, counts towards shutdown); at/above → red.
		ThresholdBrush = CurrentObservedValue < ObservingSpeed ? BelowThresholdBrush : AboveThresholdBrush;
	}

	private static Brush CreateFrozenBrush(byte r, byte g, byte b)
	{
		var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
		brush.Freeze();
		return brush;
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
