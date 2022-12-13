using System;
using System.Windows;
using ShutdownController.Utility;
using ShutdownController.Views;
using ShutdownController.ViewModels;
using ShutdownController.NotifyIcon;
using ShutdownController.Views.ToastNotification;
using Hardcodet.Wpf.TaskbarNotification;
using System.Globalization;
using WinAutoStart;

namespace ShutdownController
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {

        public TaskbarIcon TaskbarIcon { get; set; }

        public static AutoStartController AutoStartController { get; set; }
        private static PreventMultipleStarts multipleStarts;


        //ViewModels
        public static TimerViewModel STimerViewModel { get; set; }
        public static ClockViewModel SClockViewModel { get; set; }
        public static DownUploadViewModel SDownUploadViewModel { get; set; }
        public static DiskViewModel SDiskViewModel { get; set; }




        private delegate void OpenMainWindowDel();

        public App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledEceptionLogger);

            MyLogger.Instance().Info("App is starting...");

            Console.WriteLine(CultureInfo.CurrentUICulture.Name);
            AutoStartController = new AutoStartController(" " + ShutdownController.Properties.ConstTemplates.ArgWithoutUI);
            multipleStarts = new PreventMultipleStarts();
            multipleStarts.OnOpenRequest += OpenGUIRequest; //Timer is over event

        }



        protected override void OnStartup(StartupEventArgs e)
        {
            Views.SplashScreen splash = new Views.SplashScreen();

            
            base.OnStartup(e);
            if (!IsFirstInstance())
                return;

            CreateTaskBarIcon();


            if (IsWithUserInterface())
            {
                splash.Show();
                OpenMainWindow();
            }
            else
                PushMessages.ShowBalloonTip(null, "App started!", BalloonIcon.Info);


            splash.Close();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            TaskbarIcon?.Dispose();
            multipleStarts?.StopListening();
            MyLogger.Instance().Info("App is closing...");
            base.OnExit(e);
        }

        private void CreateTaskBarIcon()
        {
            try
            {
                TaskbarIcon = (TaskbarIcon)FindResource(ShutdownController.Properties.ConstTemplates.NameOfTaskbar);
            }
            catch (Exception ex)
            {
                MyLogger.Instance().Error("Error on Find TaskbarIcon. Exception: " + ex.Message);
            }

        }

        private bool IsWithUserInterface()
        {
            //Check if its on Startup
            foreach (string arg in Environment.GetCommandLineArgs())
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

        private bool IsFirstInstance()
        {
            if (!multipleStarts.IsFirstInstance())
            {
                MyLogger.Instance().Debug("App is not first instance ==> shutdown the application");
                Current.Shutdown();
                return false;
            }else
                multipleStarts.StartListening();

            return true;

        }

        private void OpenGUIRequest(object source, EventArgs args)
        {
            Dispatcher.BeginInvoke(new OpenMainWindowDel(OpenMainWindow));
        }

        static void UnhandledEceptionLogger(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;
            MyLogger.Instance().Error("Unhandled Exception: " + ex.Message); 
            MessageBox.Show("Unhandled Exception: " + ex.Message, "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);

        }


        public static void OpenMainWindow()
        {
            MyLogger.Instance().Info("Open main window");

            if (Current.MainWindow == null || Current.MainWindow.IsVisible == false || Current.MainWindow.Title != "") //If its main window, then the Tile is empty
            {
                try
                {
                    Current.MainWindow = new MainWindow();
                    Current.MainWindow.Show();
                    Current.MainWindow.Closing += OnMainWindowClosing;
                    
                }
                catch (Exception ex)
                {
                    MyLogger.Instance().Error("Open main window. Exception " + ex.Message);
                }

            }

            Current.MainWindow.WindowState = WindowState.Normal;
            Current.MainWindow.Activate();

            if(!ShutdownController.Properties.Settings.Default.NotFirstTimeUI)
                CustomNotifierCaller.ShowInfoButton(Current.MainWindow);
        }

        private static void OnMainWindowClosing(object source, EventArgs args)
        {

            MyLogger.Instance().Debug("Closing main window");

            Current.MainWindow.Closing -= OnMainWindowClosing;

            if (ShutdownController.Properties.Settings.Default.OnClosingRunInBackground)
                return;


            //Show user Message, if some timer is still running in the backgroudn

            if(STimerViewModel.TimerStarted)
                PushMessages.ShowBalloonTip("Timer", "Timer is still running in the background", BalloonIcon.Info, true);

            else if (SClockViewModel.ClockActive)
                PushMessages.ShowBalloonTip("Clock", "Clock is still running in the background", BalloonIcon.Info, true);

            else if (SDownUploadViewModel.ObserveActive)
                PushMessages.ShowBalloonTip("DownUpload", "Down- Upload observing is still running in the background", BalloonIcon.Info, true);

            else if (SDiskViewModel.ObserveActive)
                PushMessages.ShowBalloonTip("Disk", "Disk observing is still running in the background", BalloonIcon.Info, true);


            if (!ShutdownController.Properties.Settings.Default.NotFirstTimeUI)
            {
                ShutdownController.Properties.Settings.Default.NotFirstTimeUI = true;
                ShutdownController.Properties.Settings.Default.Save();
            }


        }




    }

}
