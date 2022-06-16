using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;


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
            //MyLogger.GetInstance().DebugWithClassName("Execute Action: 'Shutdown' ....", this);
            if (_testingModeActiv)
            {
                MessageBox.Show("Shutdown!");
            }
            else
            {
                Process.Start("shutdown", "/s /f /t 0");
            }
            
        }

        public void Restart()
        { 
            //MyLogger.GetInstance().DebugWithClassName("Execute Action: 'Restart' ....", this);
            if (_testingModeActiv)
            {
                MessageBox.Show("Restart!");
            }
            else
            {
                Process.Start("shutdown", "/r /f /t 0");
            }
        }

        public void Sleep()
        {
            //MyLogger.GetInstance().DebugWithClassName("Execute Action: 'Sleep' ....", this);
            if (_testingModeActiv)
            {
                MessageBox.Show("Sleep!");
            }
            else
            {
                SetSuspendState(true, false, false); //With hibernate 
            }
           
        }

    }
}
