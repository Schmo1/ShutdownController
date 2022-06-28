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

        public static AutoStartController AutoStartController { get; set; }

        public App()
        {
            MyLogger.Instance().Info("App is starting...");
            AutoStartController AutoStartController = new AutoStartController(" " + ShutdownController.Properties.ConstTemplates.ArgWithoutUI);
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            MainWindow = new MainWindow();

            base.OnStartup(e);

            CreateTaskBarIcon();

            if(IsWithUserInterface())
                MainWindow.Show();

        }

        protected override void OnExit(ExitEventArgs e)
        {
            TaskbarIcon.Dispose();
            MyLogger.Instance().Info("App is closing...");
            base.OnExit(e);
        }

        private void CreateTaskBarIcon()
        {
            try
            {
                TaskbarIcon = (TaskbarIcon)FindResource("WPFTaskbar");
            }
            catch (Exception ex)
            {
                MyLogger.Instance().Error("Error on Find TaskbarIcon. Exception: " + ex.Message);
            }

        }

        private bool IsWithUserInterface()
        {
            //Check if its on Startup
            string[] arguments = Environment.GetCommandLineArgs();

            foreach (string arg in arguments)
            {
                if (arg == ShutdownController.Properties.ConstTemplates.ArgWithoutUI)
                {
                    MyLogger.Instance().Debug($"Open without UserInterface Found argument: {arg}");
                    return false;
                }
            }
            MyLogger.Instance().Debug("Open with User Interface");
            return true;
        }
    }
}
