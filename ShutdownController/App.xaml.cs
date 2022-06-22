using System;
using System.Windows;
using ShutdownController.Utility;
using Hardcodet.Wpf.TaskbarNotification;

namespace ShutdownController
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {

        public TaskbarIcon TaskbarIcon { get; set; }

        public App()
        {
            MyLogger.Instance().Info("App is starting...");
        }

        protected override void OnStartup(StartupEventArgs e)
        {




            MainWindow = new MainWindow();
            MainWindow.Show();


            base.OnStartup(e);

            try
            {
                TaskbarIcon = (TaskbarIcon)FindResource("WPFTaskbar");
            }
            catch (Exception ex)
            {

                MyLogger.Instance().Error("Error on Find TaskbarIcon. Exception: " + ex.Message);
            }


            
        }

        protected override void OnExit(ExitEventArgs e)
        {
            TaskbarIcon.Dispose();
            MyLogger.Instance().Info("App is closing...");
            base.OnExit(e);
        }
    }
}
