using System;
using System.Windows;
using System.Diagnostics;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using ShutdownController.NotifyIcon;
using ShutdownController.Views.MessageBox;
using ShutdownController.ViewModels;
using Hardcodet.Wpf.TaskbarNotification;

namespace ShutdownController.Utility
{
    public class ShutdownOptions
    {


        private static readonly ShutdownOptions instance = new ShutdownOptions(); //singleton design pattern. singl instance of this class.
        private const int _secondsForDialog = 10;

        [DllImport("Powrprof.dll", SetLastError = true)]

        static extern uint SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        CustomMessageBox _customMessageBox;

        private bool _testingModeActiv;


        //single design pattern - private constructor
        private ShutdownOptions()
        {
#if DEBUG
            _testingModeActiv = true;
#else
             _testingModeActiv = false;
#endif
        }


        //exits int he programm, then send them the reference to the original.
        public static ShutdownOptions Instance { get { return instance; } } 

        public void ShowDialog()
        {
            //Invoke dialog in main thread
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new Action(() => 
                    {
                        _customMessageBox = new CustomMessageBox();
                        _customMessageBox.DataContext = new CustomMessageBoxViewModel(_secondsForDialog, _customMessageBox);
                        _customMessageBox.ShowDialog();
                    }));
            
        }


        public void TriggerSelectedAction()
        {


            if (Properties.Settings.Default.IsRestartSelected)
                Instance.Restart();
            else if (Properties.Settings.Default.IsShutdownSelected)
                Instance.Shutdown();
            else
                Instance.Sleep();
        }


        public void Shutdown()
        {
            MyLogger.Instance().Info("Execute Action: 'Shutdown' ....");
            if (_testingModeActiv)
            {
                PushMessages.ShowBalloonTip("Debug", "Shutdown!", BalloonIcon.Info);
            }
            else
            {
                try
                {
                    Process.Start("shutdown", "/s /f /t 0");
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error on performing action 'Shutdown'. Exception " + ex.Message, "Error", MessageBoxButton.OK);
                    MyLogger.Instance().Error("Error on Performing action Shutdown ==> Exception: " + ex.Message);
                }
            }
            
        }

        public void Restart()
        { 
            MyLogger.Instance().Info("Execute Action: 'Restart' ....");
            if (_testingModeActiv)
            {
                PushMessages.ShowBalloonTip("Debug", "Restart!", BalloonIcon.Info);
            }
            else
            {
                try
                {
                    Process.Start("shutdown", "/r /f /t 0");
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error on performing action 'Restart'. Exception " + ex.Message, "Error", MessageBoxButton.OK);
                    MyLogger.Instance().Error("Error on Performing action Restart ==> Exception: " + ex.Message);
                }
            }
        }

        public void Sleep()
        {
            MyLogger.Instance().Info("Execute Action: 'Sleep' ....");
            if (_testingModeActiv)
            {
                PushMessages.ShowBalloonTip("Debug", "Sleep!", BalloonIcon.Info);
            }
            else
            {
                SetSuspendState(true, false, false); //With hibernate 
            }
           
        }

    }
}
