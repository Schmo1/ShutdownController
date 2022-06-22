using System;
using System.Runtime.CompilerServices;
using ShutdownController.Core;


namespace ShutdownController.ViewModels
{
    internal class ClockViewModel :ObservableObject
    {
        private bool _isClockActive;
        private string _clockHours;
        private string _clockMinutes;
        private string _clockSeconds;



        public bool ClockActive { get { return _isClockActive; } set { _isClockActive = value; base.OnPropertyChanged(); } }
        public string ClockHours { get { return _clockHours; } set { _clockHours = value; base.OnPropertyChanged(); } }
        public string ClockMinutes { get { return _clockMinutes; } set { _clockMinutes = value; base.OnPropertyChanged(); } }
        public string ClockSeconds { get { return _clockSeconds; } set { _clockSeconds = value; base.OnPropertyChanged(); } }


        public int ClockSetHours { 
            get { return Properties.Settings.Default.ClockSetHours; } 
            set 
            {   
                if (value > 23)
                    Properties.Settings.Default.ClockSetHours = 23; 
                else
                    Properties.Settings.Default.ClockSetHours = value;

                OnPropertyChanged(); 
            } 
        }
        public int ClockSetMinutes { 
            get { return Properties.Settings.Default.ClockSetMinutes; } 
            set {
                if (value > 59)
                    Properties.Settings.Default.ClockSetMinutes = 59;
                else
                    Properties.Settings.Default.ClockSetMinutes = value;

                OnPropertyChanged(); 
            } 
        }
        public int ClockSetSeconds { 
            get { return Properties.Settings.Default.ClockSetSeconds; } 
            set {
                if (value > 59)
                    Properties.Settings.Default.ClockSetSeconds = 59;
                else
                    Properties.Settings.Default.ClockSetSeconds = value;

                OnPropertyChanged(); 
            } 
        }


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

        public override void OnPropertyChanged([CallerMemberName] string name = null)
        {
            Properties.Settings.Default.Save();
            base.OnPropertyChanged(name);
        }


    }
}
