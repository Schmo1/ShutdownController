using System;
using System.Windows.Input;
using ShutdownController.Core;
using ShutdownController.Models;


namespace ShutdownController.ViewModels
{
    internal class ClockViewModel :ObservableObject
    {
        private bool _isClockActive;
        private string _clockHours;
        private string _clockMinutes;
        private string _clockSeconds;


        public bool ClockActive { get { return _isClockActive; } set { _isClockActive = value; OnPropertyChanged(); } }
        public string ClockHours { get { return _clockHours; } set { _clockHours = value; OnPropertyChanged(); } }
        public string ClockMinutes { get { return _clockMinutes; } set { _clockMinutes = value; OnPropertyChanged(); } }
        public string ClockSeconds { get { return _clockSeconds; } set { _clockSeconds = value; OnPropertyChanged(); } }


        public CommandHandler ClockStartCommand { get; set; }

        public ClockViewModel()
        {

            ClockStartCommand = new CommandHandler(() => ClockActive = !_isClockActive, () => true);

            UpdateTimeSpan(null, EventArgs.Empty);
            Clock.Instance.ClockTick += new EventHandler(UpdateTimeSpan);
        }


        private void UpdateTimeSpan(Object myObject, EventArgs myEventArgs)
        {
            ClockHours = SetNumberTo2Char(Clock.Instance.ActualTime.Hour.ToString());
            ClockMinutes = SetNumberTo2Char(Clock.Instance.ActualTime.Minute.ToString());
            ClockSeconds = SetNumberTo2Char(Clock.Instance.ActualTime.Second.ToString());
        }

        private string SetNumberTo2Char(string time)
        {
            if (time == null)  //if Null return 00
                return "00";

            if (time.Length < 2) //if length is under 2, add some zero
                return "0" + time;

            return time;
        }


    }
}
