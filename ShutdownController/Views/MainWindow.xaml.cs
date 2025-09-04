using Microsoft.Extensions.DependencyInjection;
using ShutdownController.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace ShutdownController.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<MainWindowViewModel>();
    }

	private void Border_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Left)
		{
			DragMove();
		}
	}
}
