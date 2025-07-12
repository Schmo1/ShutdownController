using System;
using System.Windows;
using System.Timers;
using ShutdownController.Enums;
using ShutdownController.Core;
using System.Runtime.CompilerServices;

namespace ShutdownController.Views.ToastNotification
{
    internal static class ShowInfoMessage
    {
        public static bool IsMessageActive { get; set; }
        private static Timer _timer;

        public static void ShowMessage(object viewModel)
        {
            CustomNotifierCaller.ClearAllMessages(null, EventArgs.Empty);

            if(IsMessageActive)
            {
                StopTimerMessageActive();
                return;
            }

            StartTimer();

            CustomNotifierCaller.ShowTabInfo(Application.Current.MainWindow);

            switch (((IViewModel)viewModel).ViewName)
            {
                case ViewNameEnum.TimerView:
                    CustomNotifierCaller.ShowTimerInfo(Application.Current.MainWindow);
                    break;
                case ViewNameEnum.ClockView:
                    CustomNotifierCaller.ShowClockInfo(Application.Current.MainWindow);
                    break;
                case ViewNameEnum.DownUploadView:
                    CustomNotifierCaller.ShowDownUploadInfo(Application.Current.MainWindow);
                    break;
                case ViewNameEnum.DiskView:
                    CustomNotifierCaller.ShowDiskInfo(Application.Current.MainWindow);
                    break;
                case ViewNameEnum.SettingsView:
                    CustomNotifierCaller.ShowSettingsInfo(Application.Current.MainWindow);
                    break;
            }
        }

        private static void StartTimer()
        {
            if (_timer == null) 
            {   //Time in Milliseconds
                _timer = new Timer(Convert.ToDouble(Properties.ConstTemplates.SecondsForToastMessage) * 1000); 
            } 

            IsMessageActive = true;
            _timer.Elapsed += TimeElapsed;
            _timer.Start();
        }

        private static void TimeElapsed(object sender, EventArgs e)
        {
            StopTimerMessageActive();
            _timer.Elapsed += TimeElapsed;
        }

        public static void StopTimerMessageActive()
        {
            IsMessageActive = false;
            _timer?.Stop();
        }

    }
}
