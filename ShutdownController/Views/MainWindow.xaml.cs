using Microsoft.Extensions.DependencyInjection;
using ShutdownController.ViewModels;
using System.Windows;

namespace ShutdownController.Views
{
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
    }
}
