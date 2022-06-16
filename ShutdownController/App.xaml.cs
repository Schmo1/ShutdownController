using System;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace ShutdownController
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {

        public TaskbarIcon TaskbarIcon { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();


            base.OnStartup(e);


            TaskbarIcon = (TaskbarIcon)FindResource("WPFTaskbar");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            TaskbarIcon.Dispose();

            base.OnExit(e);
        }
    }
}
