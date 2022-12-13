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

        internal static void ShowClockInfo(Window window)
        {
            ShowCustomMessage(150, 0, window, "Clock", "Choose the time, when your action should triggered.", CustomNotificationArrowPosition.Bottom);
        }

        internal static void ShowDiskInfo(Window window)
        {
            ShowCustomMessage(575, 295, window, "", "Choose your observable disk's.", CustomNotificationArrowPosition.Right);
            ShowCustomMessage(575, 370, window, "", "Choose what you want to observe.", CustomNotificationArrowPosition.Right);
            ShowCustomMessage(140, 400, window, "Threshold", "Choose where the threshold should be.", CustomNotificationArrowPosition.Top);
            ShowCustomMessage(140, 190, window, "Seconds", "If the current value is under the threshold, the action will be started after x seconds.", CustomNotificationArrowPosition.Bottom);
        }

        internal static void ShowDownUploadInfo(Window window)
        {
            ShowCustomMessage(575, 295, window, "", "Choose your observable network interface.", CustomNotificationArrowPosition.Right);
            ShowCustomMessage(575, 370, window, "", "Choose what you want to observe.", CustomNotificationArrowPosition.Right);
            ShowCustomMessage(140, 400, window, "Threshold", "Choose where the threshold should be.", CustomNotificationArrowPosition.Top);
            ShowCustomMessage(140, 190, window, "Seconds", "If the current value is under the threshold, the action will be started after x seconds.", CustomNotificationArrowPosition.Bottom);
        }

        internal static void ShowSettingsInfo(Window window)
        {
            ShowCustomMessage(520, 400, window, "", "On closing programm is minimized in your system tray on your taskbar", CustomNotificationArrowPosition.Right);
        }

        internal static void ShowTimerInfo(Window window)
        {
            ShowCustomMessage(250, 0, window, "Timer", "After this time you action will be triggered", CustomNotificationArrowPosition.Bottom);
        }


        internal static void ShowInfoButton(Window window)
        {
            ShowCustomMessage(475, -28, window, "Info button", "Press me...", CustomNotificationArrowPosition.Bottom);
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
