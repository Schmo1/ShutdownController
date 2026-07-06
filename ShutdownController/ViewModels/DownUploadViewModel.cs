using CommunityToolkit.Mvvm.ComponentModel;
using ShutdownController.Services.Abstraction;
using System.Net.NetworkInformation;

namespace ShutdownController.ViewModels;

public partial class DownUploadViewModel : ObservingViewModelBase
{
	private const double BytesPerMegaByte = 1024 * 1024;

	private long _previousReceived;
	private long _previousSent;

	[ObservableProperty]
	private List<NetworkInterface> _adapters;

	[ObservableProperty]
	private NetworkInterface? _selectedAdapter;

	public DownUploadViewModel(IEachSecondTick tick, IServiceProvider serviceProvider)
		: base(tick, serviceProvider)
	{
		// Order by the amount of traffic seen so far so the real, active adapter
		// (e.g. Wi-Fi) is preselected instead of an idle virtual one.
		_adapters = NetworkInterface.GetAllNetworkInterfaces()
			.Where(adapter => adapter.OperationalStatus == OperationalStatus.Up
							  && adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback)
			.OrderByDescending(TotalBytes)
			.ToList();

		_selectedAdapter = _adapters.FirstOrDefault();
	}

	private static long TotalBytes(NetworkInterface adapter)
	{
		try
		{
			IPInterfaceStatistics statistics = adapter.GetIPStatistics();
			return statistics.BytesReceived + statistics.BytesSent;
		}
		catch (NetworkInformationException)
		{
			return 0;
		}
	}

	protected override string SettingsPrefix => "DownUpload";

	partial void OnSelectedAdapterChanged(NetworkInterface? value) => RestartSource();

	protected override void InitializeSource()
	{
		(_previousReceived, _previousSent) = ReadRawBytes();
	}

	protected override (double primary, double secondary) ReadSpeed()
	{
		(long received, long sent) = ReadRawBytes();

		double downloadPerSecond = Math.Max(0, received - _previousReceived) / BytesPerMegaByte;
		double uploadPerSecond = Math.Max(0, sent - _previousSent) / BytesPerMegaByte;

		_previousReceived = received;
		_previousSent = sent;

		return (downloadPerSecond, uploadPerSecond);
	}

	private (long received, long sent) ReadRawBytes()
	{
		if (SelectedAdapter is null)
		{
			return (0, 0);
		}

		try
		{
			IPInterfaceStatistics statistics = SelectedAdapter.GetIPStatistics();
			return (statistics.BytesReceived, statistics.BytesSent);
		}
		catch (NetworkInformationException)
		{
			return (0, 0);
		}
	}
}
