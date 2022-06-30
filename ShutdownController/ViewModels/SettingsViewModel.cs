using System;
using ShutdownController.Core;


namespace ShutdownController.ViewModels
{
    internal class SettingsViewModel : ObservableObject
    {
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


        public bool AutoStartActive
        {
            get { return App.AutoStartController.AutoStartActiv; }
            set {
                if (value)
                    App.AutoStartController.EnableAutoStart();
                else
                    App.AutoStartController.DisableAutoStart();
                 }
        }

    }
}
