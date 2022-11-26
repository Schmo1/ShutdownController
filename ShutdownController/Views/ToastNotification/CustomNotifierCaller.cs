using System;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace ShutdownController.Views.ToastNotification
{
    public static class CustomNotifierCaller
    {
        private const int TapStartPosition = 52;
        private const int TapPositionX = -225;
        private const int TapPositionY = 72;

        public static void ShowTimerInfo(Window window)
        {

            Notifier notifier = new Notifier(cfg =>
            {
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(5), MaximumNotificationCount.FromCount(15));
                cfg.PositionProvider = new WindowPositionProvider(parentWindow: window, corner: Corner.TopLeft, offsetX: TapPositionX, offsetY: TapStartPosition);
            });


            notifier.ShowCustomMessage("Timer", "Select timerspan.");

        }

        public static void ShowClockInfo(Window window)
        {

            Notifier notifier = new Notifier(cfg =>
            {
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(5), MaximumNotificationCount.FromCount(15));
                cfg.PositionProvider = new WindowPositionProvider(parentWindow: window, corner: Corner.TopLeft, offsetX: TapPositionX, offsetY: TapStartPosition + TapPositionY);
            });


            notifier.ShowCustomMessage("Clock", "Choose your exact clock time.");

        }

        public static void ShowDownUploadInfo(Window window)
        {

            Notifier notifier = new Notifier(cfg =>
            {
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(5), MaximumNotificationCount.FromCount(15));
                cfg.PositionProvider = new WindowPositionProvider(parentWindow: window, corner: Corner.TopLeft, offsetX: TapPositionX, offsetY: TapStartPosition + (TapPositionY *2));
            });


            notifier.ShowCustomMessage("Down- Upload observing", "Choose your action to behave on your down- upload.");

        }

        public static void ShowDiskInfo(Window window)
        {

            Notifier notifier = new Notifier(cfg =>
            {
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(5), MaximumNotificationCount.FromCount(15));
                cfg.PositionProvider = new WindowPositionProvider(parentWindow: window, corner: Corner.TopLeft, offsetX: TapPositionX, offsetY: TapStartPosition + (TapPositionY * 3));
            });


            notifier.ShowCustomMessage("Disk observing", "Choose your action to behave on your disk read or write.");

        }




    }
}
