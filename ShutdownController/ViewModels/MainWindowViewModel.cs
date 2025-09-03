using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;

namespace ShutdownController.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
	private readonly ILogger _logger;

	public MainWindowViewModel(ILogger logger)
	{
		_logger = logger;
	}

}
