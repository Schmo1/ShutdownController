using System;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace ShutdownController.NotifyIcon
{
    public class PushMessages
    {

        private static TaskbarIcon taskbarIcon;


        private static TaskbarIcon Instance()
        {
            //Creates a new Instance if it's null
            if (taskbarIcon == null) 
                taskbarIcon = (TaskbarIcon)Application.Current.FindResource(Properties.ConstTemplates.NameOfTaskbar);
                
            return taskbarIcon;
        }



        public static void ShowBalloonTip(string titel, string message, BalloonIcon balloonIcon, bool forceMessage = false)
        {
            if(!Properties.Settings.Default.DisablePushMessages || forceMessage)
                Instance().ShowBalloonTip(titel, message, balloonIcon);
            
        }



    }
}
