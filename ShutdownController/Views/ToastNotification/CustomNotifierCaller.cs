using System;
using Window = System.Windows.Window;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ShutdownController.Resources.ClockStrings;
using ShutdownController.Resources.DiskStrings;
using ShutdownController.Resources.DownUploadStrings;
using ShutdownController.Resources.TimerStrings;
using ShutdownController.Resources.MainWindowStrings;

namespace ShutdownController.Views.ToastNotification
{
    public static class CustomNotifierCaller
    {

        public static void ShowTabInfo(Window window)
        {
            const int TapStartPosition = 52;
            const int TapPositionX = -215;
            const int TapPositionY = 72;


            ShowCustomMessage(TapPositionX, TapStartPosition, window, TimerStrings.timer, TimerStrings.selectTimeSpan);
            ShowCustomMessage(TapPositionX, TapStartPosition + TapPositionY, window, ClockStrings.clock, ClockStrings.ToastTapNotification);
            ShowCustomMessage(TapPositionX, TapStartPosition + (TapPositionY * 2), window, DownUploadStrings.downUploadObserving, DownUploadStrings.tabToastNotification);
            ShowCustomMessage(TapPositionX, TapStartPosition + (TapPositionY * 3), window, DiskStrings.diskObserving, DiskStrings.tabToastNotification);

        }

        internal static void ShowClockInfo(Window window)
        {        
            ShowCustomMessage(150, 0, window, ClockStrings.clock, ClockStrings.ToastNotification, CustomNotificationArrowPosition.Bottom);
        }
        internal static void ShowDownUploadInfo(Window window)
        {
            ShowCustomMessage(575, 295, window, DownUploadStrings.networkInterface, DownUploadStrings.networkinterfaceInfo, CustomNotificationArrowPosition.Right);
            ShowCustomMessage(575, 370, window, string.Empty, "Choose what you want to observe.", CustomNotificationArrowPosition.Right);
            ShowCustomMessage(140, 400, window, DownUploadStrings.thresholdSpeed, "Choose where the threshold should be.", CustomNotificationArrowPosition.Top);
            ShowCustomMessage(140, 190, window, DownUploadStrings.seconds, "If the current value is under the threshold, the action will be started after x seconds.", CustomNotificationArrowPosition.Bottom);
        }

        internal static void ShowDiskInfo(Window window)
        {
            ShowCustomMessage(575, 295, window, string.Empty, "Choose your observable disk's.", CustomNotificationArrowPosition.Right);
            ShowCustomMessage(575, 370, window, string.Empty, "Choose what you want to observe.", CustomNotificationArrowPosition.Right);
            ShowCustomMessage(140, 400, window, DiskStrings.thresholdSpeed, "Choose where the threshold should be.", CustomNotificationArrowPosition.Top);
            ShowCustomMessage(140, 190, window, DiskStrings.seconds, "If the current value is under the threshold, the action will be started after x seconds.", CustomNotificationArrowPosition.Bottom);
        }


        internal static void ShowSettingsInfo(Window window)
        {
            ShowCustomMessage(520, 400, window, string.Empty, "On closing programm is minimized in your system tray on your taskbar", CustomNotificationArrowPosition.Right);
        }

        internal static void ShowTimerInfo(Window window)
        {
            ShowCustomMessage(250, 0, window, TimerStrings.timer, TimerStrings.timerInfo, CustomNotificationArrowPosition.Bottom);
        }


        internal static void ShowInfoButton(Window window)
        {
            ShowCustomMessage(475, -28, window, MainWindowStrings.infoButton, MainWindowStrings.pressMe, CustomNotificationArrowPosition.Bottom);
        }

        private static void ShowCustomMessage(double xPos, double yPos, Window window, string titel, string message, CustomNotificationArrowPosition arrowPosition = CustomNotificationArrowPosition.Left)
        {

            Notifier notifier = new Notifier(cfg =>
            {
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(5), MaximumNotificationCount.FromCount(15));
                cfg.PositionProvider = new WindowPositionProvider(parentWindow: window, corner: Corner.TopLeft, offsetX: xPos, offsetY: yPos);
            });


            notifier.ShowCustomMessage(titel, message, arrowPosition);
        }



    }
}
