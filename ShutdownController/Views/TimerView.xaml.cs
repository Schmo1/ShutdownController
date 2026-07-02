using Microsoft.Extensions.DependencyInjection;
using ShutdownController.ViewModels;
using System.Windows.Controls;

namespace ShutdownController.Views;

/// <summary>
/// Interaction logic for TimerView.xaml
/// </summary>
public partial class TimerView : UserControl
{
	public TimerView()
	{
		InitializeComponent();

		DataContext = App.Services.GetRequiredService<TimerViewModel>();
	}
}
