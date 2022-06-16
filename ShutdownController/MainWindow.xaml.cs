using System.Windows;
using System.Windows.Input;

namespace ShutdownController
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool IsMaximized = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount!=2)
                return;

            //Optional
            if (IsMaximized)
            {
                WindowState = WindowState.Normal;
                Width = 800;
                Height = 550;
                IsMaximized = false;
            }
            else
            {
                WindowState = WindowState.Maximized;
                IsMaximized = true;
            }
        }
    }
}
