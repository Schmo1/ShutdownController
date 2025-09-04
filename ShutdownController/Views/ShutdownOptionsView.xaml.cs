using Microsoft.Extensions.DependencyInjection;
using ShutdownController.ViewModels;
using System.Windows.Controls;

namespace ShutdownController.Views;

/// <summary>
/// Interaction logic for ShutdownOptionView.xaml
/// </summary>
public partial class ShutdownOptionsView : UserControl
{
    public ShutdownOptionsView()
    {
        InitializeComponent();
		DataContext = App.Services.GetRequiredService<ShutdownOptionsViewModel>();
	}
}

