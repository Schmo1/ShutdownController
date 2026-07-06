using CommunityToolkit.Mvvm.ComponentModel;
using ShutdownController.Services.Abstraction;
using System.Diagnostics;
using System.IO;

namespace ShutdownController.ViewModels;

public partial class DiskViewModel : ObservingViewModelBase
{
	private const double BytesPerMegaByte = 1024 * 1024;

	private PerformanceCounter? _readCounter;
	private PerformanceCounter? _writeCounter;

	[ObservableProperty]
	private List<DiskDriveItem> _drives;

	[ObservableProperty]
	private DiskDriveItem? _selectedDrive;

	public DiskViewModel(IEachSecondTick tick, IServiceProvider serviceProvider)
		: base(tick, serviceProvider)
	{
		_drives = DriveInfo.GetDrives()
			.Where(drive => drive.IsReady)
			.Select(drive => new DiskDriveItem(drive))
			.ToList();

		_selectedDrive = _drives.FirstOrDefault();
	}

	protected override string SettingsPrefix => "Disk";

	partial void OnSelectedDriveChanged(DiskDriveItem? value) => RestartSource();

	protected override void InitializeSource()
	{
		_readCounter?.Dispose();
		_writeCounter?.Dispose();
		_readCounter = null;
		_writeCounter = null;

		if (SelectedDrive is null)
		{
			return;
		}

		try
		{
			_readCounter = new PerformanceCounter("LogicalDisk", "Disk Read Bytes/sec", SelectedDrive.Instance);
			_writeCounter = new PerformanceCounter("LogicalDisk", "Disk Write Bytes/sec", SelectedDrive.Instance);

			// Prime the counters; the first sample is always zero.
			_readCounter.NextValue();
			_writeCounter.NextValue();
		}
		catch (Exception)
		{
			_readCounter = null;
			_writeCounter = null;
		}
	}

	protected override (double primary, double secondary) ReadSpeed()
	{
		double read = SampleCounter(_readCounter);
		double write = SampleCounter(_writeCounter);
		return (read, write);
	}

	private static double SampleCounter(PerformanceCounter? counter)
	{
		if (counter is null)
		{
			return 0;
		}

		try
		{
			return counter.NextValue() / BytesPerMegaByte;
		}
		catch (Exception)
		{
			return 0;
		}
	}
}

public sealed class DiskDriveItem
{
	public DiskDriveItem(DriveInfo drive)
	{
		Display = $"{drive.Name} {drive.DriveType}";
		Instance = drive.Name.TrimEnd(Path.DirectorySeparatorChar);
	}

	public string Display { get; }

	public string Instance { get; }
}
