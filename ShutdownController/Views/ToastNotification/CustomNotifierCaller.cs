using ShutdownController.Resources.ClockStrings;
using ShutdownController.Resources.DiskStrings;
using ShutdownController.Resources.DownUploadStrings;
using ShutdownController.Resources.MainWindowStrings;
using ShutdownController.Resources.TimerStrings;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Lifetime.Clear;
using ToastNotifications.Position;
using Window = System.Windows.Window;

namespace ShutdownController.Views.ToastNotification
{
    public static class CustomNotifierCaller
    {
        public static readonly TimeSpan timeSpan = TimeSpan.FromSeconds(Convert.ToDouble(Properties.ConstTemplates.SecondsForToastMessage));

     

        private static List<Notifier> _notifications;

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
            ShowCustomMessage(575, 370, window, string.Empty, DownUploadStrings.chooseDownOrUp, CustomNotificationArrowPosition.Right);
            ShowCustomMessage(140, 400, window, DownUploadStrings.thresholdSpeed, DownUploadStrings.thresholdInfo, CustomNotificationArrowPosition.Top);
            ShowCustomMessage(140, 190, window, DownUploadStrings.seconds, DownUploadStrings.secondsExplanation, CustomNotificationArrowPosition.Bottom);
        }

        internal static void ShowDiskInfo(Window window)
        {
            ShowCustomMessage(575, 295, window, string.Empty, DiskStrings.chooseDisk, CustomNotificationArrowPosition.Right);
            ShowCustomMessage(575, 370, window, string.Empty, DiskStrings.chooseReadWrite, CustomNotificationArrowPosition.Right);
            ShowCustomMessage(140, 400, window, DiskStrings.thresholdSpeed, DiskStrings.chooseThreshold, CustomNotificationArrowPosition.Top);
            ShowCustomMessage(140, 190, window, DiskStrings.seconds, DiskStrings.secondsExplanation, CustomNotificationArrowPosition.Bottom);
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

            if(_notifications == null) { _notifications = new List<Notifier>(); }
            if(_notifications.Count > 10) { ClearAllMessages(null, EventArgs.Empty); }

            

            _notifications.Add(
                new Notifier(cfg =>
                    {
                    cfg.PositionProvider = new WindowPositionProvider(
                        parentWindow: window,
                        corner: Corner.TopLeft,
                        offsetX: xPos,
                        offsetY: yPos);

                    cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(timeSpan, MaximumNotificationCount.FromCount(1));
                    cfg.Dispatcher = Application.Current.Dispatcher;

                    }));

                    _notifications[_notifications.Count -1].ShowCustomMessage(titel,message,arrowPosition);

        }

        internal static void ClearAllMessages(object source, EventArgs args)
        {
            ShowInfoMessage.StopTimerMessageActive();
            _notifications?.ForEach((notify) => { notify.ClearMessages(new ClearAll()); });
            _notifications?.Clear();
        }



    }
}
