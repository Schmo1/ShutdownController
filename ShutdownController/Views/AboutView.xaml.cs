using System;
using System.Windows;
using System.Windows.Input;
using ShutdownController.ViewModels;

namespace ShutdownController.Views
{
    /// <summary>
    /// Interaktionslogik für AboutView.xaml
    /// </summary>
    public partial class AboutView : Window
    {
        public AboutView()
        {
            InitializeComponent();
            DataContext = new AboutViewModel(this);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }


    }
}
