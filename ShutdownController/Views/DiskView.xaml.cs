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

		DataContext = App.Services.GetRequiredService<DiskViewModel>();
	}
}
