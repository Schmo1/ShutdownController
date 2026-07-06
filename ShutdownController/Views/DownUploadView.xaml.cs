using Microsoft.Extensions.DependencyInjection;
using ShutdownController.ViewModels;
using System.Windows.Controls;

namespace ShutdownController.Views;

/// <summary>
/// Interaction logic for DownUploadView.xaml
/// </summary>
public partial class DownUploadView : UserControl
{
	public DownUploadView()
	{
		InitializeComponent();

		DownUploadViewModel viewModel = App.Services.GetRequiredService<DownUploadViewModel>();
		DataContext = viewModel;

		// Begin live sampling so the graph shows data without pressing Start.
		viewModel.Activate();
	}
}
