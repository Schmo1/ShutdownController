using Microsoft.Extensions.DependencyInjection;
using ShutdownController.ViewModels;
using System.Windows.Controls;

namespace ShutdownController.Views;

/// <summary>
/// Interaction logic for DiskView.xaml
/// </summary>
public partial class DiskView : UserControl
{
	public DiskView()
	{
		InitializeComponent();

		DiskViewModel viewModel = App.Services.GetRequiredService<DiskViewModel>();
		DataContext = viewModel;

		// Begin live sampling so the graph shows data without pressing Start.
		viewModel.Activate();
	}
}
