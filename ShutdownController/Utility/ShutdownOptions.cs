using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ShutdownController.NotifyIcon;
using Hardcodet.Wpf.TaskbarNotification;

namespace ShutdownController.Utility
{
    public class ShutdownOptions
    {


        private static readonly ShutdownOptions instance = new ShutdownOptions(); //singleton design pattern. singl instance of this class.

        [DllImport("Powrprof.dll", SetLastError = true)]

        static extern uint SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

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





        public void Shutdown()
        {
            MyLogger.Instance().Info("Execute Action: 'Shutdown' ....");
            if (_testingModeActiv)
            {
                PushMessages.ShowBalloonTip("Debug", "Shutdown!", BalloonIcon.Info);
            }
            else
            {
                Process.Start("shutdown", "/s /f /t 0");
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
                Process.Start("shutdown", "/r /f /t 0");
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
