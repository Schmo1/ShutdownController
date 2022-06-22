using System;
using System.Runtime.CompilerServices;
using ShutdownController.Core;
using ShutdownController.Utility;

namespace ShutdownController.ViewModels
{
    internal class ClockViewModel :ObservableObject
    {
        private bool _isClockActive;
        private string _clockHours;
        private string _clockMinutes;
        private string _clockSeconds;



        public bool ClockActive { get { return _isClockActive; } set { _isClockActive = value; base.OnPropertyChanged(); } }

        //Actual time
        public string ClockHours { get { return _clockHours; } set { _clockHours = value; base.OnPropertyChanged(); } }
        public string ClockMinutes { get { return _clockMinutes; } set { _clockMinutes = value; base.OnPropertyChanged(); } }
        public string ClockSeconds { get { return _clockSeconds; } set { _clockSeconds = value; base.OnPropertyChanged(); } }


        //User set time
        public int ClockSetHours { 
            get { return Properties.Settings.Default.ClockSetHours; } 
            set { Properties.Settings.Default.ClockSetHours= Math.Min(value, 23); OnPropertyChanged(); } 
        }

        public int ClockSetMinutes { 
            get { return Properties.Settings.Default.ClockSetMinutes; } 
            set { Properties.Settings.Default.ClockSetMinutes = Math.Min(value, 59); OnPropertyChanged(); } 
        }

        public int ClockSetSeconds { 
            get { return Properties.Settings.Default.ClockSetSeconds; } 
            set { Properties.Settings.Default.ClockSetSeconds = Math.Min(value, 59); OnPropertyChanged(); } 
        }



        //Commands
        public CommandHandler ClockStartCommand { get; set; }


        //Events
        public event EventHandler ClockRunsOutEvent;


        //Constructor
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

            if (TimeRunsOut() && ClockActive)
            {
                MyLogger.Instance().Info("Clock time run's out");
                ClockActive = false;
                ClockRunsOutEvent?.Invoke(this, EventArgs.Empty);
            }
            
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
            //OnPropertyChanged save Usersettings
            Properties.Settings.Default.Save();
            base.OnPropertyChanged(name);
        }

        private bool TimeRunsOut()
        {
            if (Clock.Instance.ActualTime.Hour == ClockSetHours && Clock.Instance.ActualTime.Minute == ClockSetMinutes && Clock.Instance.ActualTime.Second == ClockSetSeconds)
                return true; //Time runs out
            else
                return false;
        }

    }
}
