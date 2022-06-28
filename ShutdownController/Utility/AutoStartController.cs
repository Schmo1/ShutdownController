using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Reflection;
using ShutdownController.Utility;

namespace ShutdownController.Utility
{


    public class AutoStartController
    {

        // The path to the key where Windows looks for startup applications
        private readonly RegistryKey startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private readonly string _appName;
        private string _args;

        public bool AutoStartActiv { get => IsAutoStartActiv(); }
        public string Arguments { get { return _args; } set { _args = value; EnableAutoStart(); } }



        public AutoStartController()
        {
            _appName = Application.ResourceAssembly.GetName().Name;
        }


        public AutoStartController(string args) :this() //call the base constructor
        {
            _args = args;
        }
  

        public AutoStartController(string args, string appName)
        {
            _appName = appName;
            _args = args;
        }



        private bool IsAutoStartActiv()
        {
            object objValue = startupKey.GetValue(_appName);

            // Check to see the current state (running at startup or not)
            if (objValue == null)
            {
                // The value doesn't exist, the application is not set to run at startup
                return false;
            }
            else
            {
                // The value exists, the application is set to run at startup  
                try
                {
                    EnableAutoStart();
                    return true;
                }
                catch (Exception ex)
                {
                    MyLogger.Instance().Error("Error Registry GetValue ==> Exception: " + ex.Message);
                    return false;
                }
            }
        }

        public object GetValue()
        {
            if (startupKey.GetValue(_appName) != null)
                return startupKey.GetValue(_appName);
            else
                return "";
        }


        public void EnableAutoStart()
        {
            try
            {
                //Get Full Path
                string dirPath = GetRegPath();
                string getPath = GetValue().ToString();

                if (!getPath.Equals(dirPath) && _appName != null)
                {
                    //Update path
                    MyLogger.Instance().Debug($"EnableAutoStart, appName: {_appName},path: {dirPath}");
                    // Add the value in the registry so that the application runs at startup
                    startupKey?.SetValue(_appName, dirPath);
                }
            }
            catch (Exception ex)
            {
                MyLogger.Instance().Error("Error create Registry entry ==> Exception: " + ex.Message);
            }
        }

        public void DisableAutoStart()
        {
            MyLogger.Instance().Debug("DisableAutoStart");
            try
            {
                // Remove the value from the registry so that the application doesn't start
                startupKey?.DeleteValue(_appName, false);
            }
            catch (Exception ex)
            {
                MyLogger.Instance().Error("Error Delete Registry entry ==> Exception: " + ex.Message);
            }
        }

        public void ChangeAutoStartChecked()
        {
            if (AutoStartActiv)     
                DisableAutoStart();   // Remove the value from the registry so that the application doesn't start        
            else  
                EnableAutoStart();
            
        }


        private string GetRegPath()
        {
            //Get Full Path
            string dirInfo = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
            //add quotation marks and argument
            dirInfo = $"\"{dirInfo}\"" + Arguments;

            return dirInfo;
        }
    }
}
