using System;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Window = System.Windows.Window;

namespace ShutdownController.Views.ToastNotification
{
    public static class CustomNotifierCaller
    {
        private const int TapStartPosition = 52;
        private const int TapPositionX = -225;
        private const int TapPositionY = 72;

        public static void ShowTabInfo(Window window)
        {

            ShowCustomMessage(TapPositionX, TapStartPosition, window, "Timer", "Select timerspan.");
            ShowCustomMessage(TapPositionX, TapStartPosition + TapPositionY, window, "Clock", "Choose your exact clock time.");
            ShowCustomMessage(TapPositionX, TapStartPosition + (TapPositionY * 2), window, "Down- Upload observing", "Choose your action to behave on your down- upload.");
            ShowCustomMessage(TapPositionX, TapStartPosition + (TapPositionY * 3), window, "Disk observing", "Choose your action to behave on your disk read or write.");

        }

        internal static void ShowClockInfo()
        {
            throw new NotImplementedException();
        }

        internal static void ShowDiskInfo()
        {
            throw new NotImplementedException();
        }

        internal static void ShowDownUploadInfo()
        {
            throw new NotImplementedException();
        }

        internal static void ShowSettingsInfo()
        {
            throw new NotImplementedException();
        }

        internal static void ShowTimerInfo()
        {
            throw new NotImplementedException();
        }

        private static void ShowCustomMessage(double xPos, double yPos, Window window, string titel, string message)
        {

            Notifier notifier = new Notifier(cfg =>
            {
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(5), MaximumNotificationCount.FromCount(15));
                cfg.PositionProvider = new WindowPositionProvider(parentWindow: window, corner: Corner.TopLeft, offsetX: xPos, offsetY: yPos);
            });


            notifier.ShowCustomMessage(titel, message);
        }



    }
}
