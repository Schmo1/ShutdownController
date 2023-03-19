using System;
using System.Windows;
using System.Windows.Threading;
using ShutdownController.Core;
using ShutdownController.Utility;
using ShutdownController.Resources.MainWindowStrings;

namespace ShutdownController.ViewModels
{
    public class CustomMessageBoxViewModel : ObservableObject
    {

        public static bool IsActive { get; private set; }   
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly Window _window;


        private int _timeLeft;
        public int TimeLeft
        {
            get { return _timeLeft; }
            set { _timeLeft = value; OnPropertyChanged(); }
        }

        public string ActionToPerforme 
        { get
            {
                if (Properties.Settings.Default.IsRestartSelected)
                    return MainWindowStrings.restart;
                else if(Properties.Settings.Default.IsShutdownSelected) 
                    return MainWindowStrings.shutdown;

                return MainWindowStrings.sleep;
            } 
        }

        public CommandHandler AbortAction { get; set; }

        public CustomMessageBoxViewModel(int timeLeft, Window window)
        {
            AbortAction = new CommandHandler(StopAction,() => true);
            IsActive = true;
            TimeLeft = timeLeft;  
            _window = window;   
            StartTimer();
        }

        public void StartTimer()
        {
            _timer.Interval = new TimeSpan(0,0,1);
            _timer.Tick += SubtractSecond;      
            _timer.Start();
        }

        public void StopAction() 
        {
            _timer?.Stop();
            _window.Close();
            IsActive = false;
        }

        private void SubtractSecond(object sender, EventArgs args)
        {
            TimeLeft -= 1;

            if(TimeLeft <= 0)
            {
                StopAction();
                ShutdownOptions.Instance.TriggerSelectedAction();
            }
        }

    }

}