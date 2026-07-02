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

		DataContext = App.Services.GetRequiredService<DownUploadViewModel>();
	}
}
