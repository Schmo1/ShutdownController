﻿using System;
using System.Windows;
using ShutdownController.Utility;
using ShutdownController.ViewModels;
using ShutdownController.NotifyIcon;
using Hardcodet.Wpf.TaskbarNotification;
using ShutdownController.Views;
using System.Globalization;

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
                MainWindow = new MainWindow();
                MainWindow.Show();
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

        }


        public void OpenMainWindow()
        {
            MyLogger.Instance().Info("Open main window");

            if (MainWindow == null || MainWindow.IsVisible == false)
            {
                try
                {
                    MainWindow = new MainWindow();
                    MainWindow.Show();
                }
                catch (Exception ex)
                {
                    MyLogger.Instance().Error("Open main window. Exception " + ex.Message);
                }

            }

            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Activate();
        }
    }
}
