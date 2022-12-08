using System;
using ShutdownController.Core;
using ShutdownController.Enums;

namespace ShutdownController.ViewModels
{
    internal class SettingsViewModel : ObservableObject, IViewModel
    {


        public ViewNameEnum ViewName => ViewNameEnum.SettingsView;


        public bool OnClosingRunInBackground { 
            get { return Properties.Settings.Default.OnClosingRunInBackground; }
            set { Properties.Settings.Default.OnClosingRunInBackground = value; Properties.Settings.Default.Save(); }
        }

        public bool DisablePushMessages
        {
            get { return Properties.Settings.Default.DisablePushMessages; }
            set { Properties.Settings.Default.DisablePushMessages = value; Properties.Settings.Default.Save(); }
        }

        public bool StartMinimized
        {
            get { return Properties.Settings.Default.StartMinimized; }
            set { Properties.Settings.Default.StartMinimized = value; Properties.Settings.Default.Save(); }
        }

        public bool SavePerformance
        {
            get { return Properties.Settings.Default.SavePerformance; }
            set { Properties.Settings.Default.SavePerformance = value; Properties.Settings.Default.Save(); }
        }


        public bool AutoStartActive
        {
            get { 
                bool isActive = App.AutoStartController.AutoStartActiv;
                if (isActive)
                    App.AutoStartController.EnableAutoStart(); //Update Path if RegKey exists

                return isActive; 
                }

            set {
                if (value)
                    App.AutoStartController.EnableAutoStart();
                else
                    App.AutoStartController.DisableAutoStart();
                 }
        }

    }
}
