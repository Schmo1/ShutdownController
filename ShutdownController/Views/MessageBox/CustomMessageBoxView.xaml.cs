using Microsoft.Extensions.DependencyInjection;
using ShutdownController.ViewModels;

using System.Windows;
using System.Windows.Input;

namespace ShutdownController.Views.MessageBox;

public partial class CustomMessageBoxView : Window
{

    public CustomMessageBoxView()
    {
        InitializeComponent();

		DataContext = App.Services.GetRequiredService<CustomMessageBoxViewModel>();
	}

    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }
}
