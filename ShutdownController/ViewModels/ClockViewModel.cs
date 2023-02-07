using System;
using System.Runtime.CompilerServices;
using Hardcodet.Wpf.TaskbarNotification;
using ShutdownController.Core;
using ShutdownController.Enums;
using ShutdownController.NotifyIcon;
using ShutdownController.Utility;

namespace ShutdownController.ViewModels
{
    public class ClockViewModel :ObservableObject, IViewModel
    {
        private bool _isClockActive;
        private string _clockHours;
        private string _clockMinutes;
        private string _clockSeconds;
        private readonly AccurateTimerTick _timerTick = new AccurateTimerTick(new TimeSpan(0, 0, 1));



        public bool ClockActive 
        { 
            get { return _isClockActive; } 
            set { _isClockActive = value; 
                if (_isClockActive)
                {
                    MyLogger.Instance().Info("Clock is set active : " + ClockHours + " : " + ClockMinutes + " : " + ClockSeconds);
                }
                else
                {
                    MyLogger.Instance().Info("Clock is inactive");
                }
                base.OnPropertyChanged(); } 
        }

        //Actual time
        public string ClockHours { get { return _clockHours; } set { _clockHours = value; base.OnPropertyChanged(); } }
        public string ClockMinutes { get { return _clockMinutes; } set { _clockMinutes = value; base.OnPropertyChanged(); } }
        public string ClockSeconds { get { return _clockSeconds; } set { _clockSeconds = value; base.OnPropertyChanged(); } }


        //User set time
        public int ClockSetHours { 
            get { return Properties.Settings.Default.ClockSetHours; } 
            set { Properties.Settings.Default.ClockSetHours = Math.Min(value, 23); OnPropertyChanged(); } 
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

        public ViewNameEnum ViewName => ViewNameEnum.ClockView;




        //Constructor
        public ClockViewModel()
        {

            ClockStartCommand = new CommandHandler(() => ClockActive = !_isClockActive, () => true);

            UpdateTimeSpan(null, EventArgs.Empty);
            _timerTick.Tick += new EventHandler(UpdateTimeSpan);
            _timerTick.Start();
            
        }



        private void UpdateTimeSpan(object myObject, EventArgs myEventArgs)
        {
            ClockHours = FillEmptyStringWithZero(DateTime.Now.Hour.ToString());
            ClockMinutes = FillEmptyStringWithZero(DateTime.Now.Minute.ToString());
            ClockSeconds = FillEmptyStringWithZero(DateTime.Now.Second.ToString());

            if(!ClockActive)
                return;


            if(IsTimeEquelSetTime())
            {
                MyLogger.Instance().Info("Clock time run's out");
                ClockActive = false;
                ShutdownOptions.Instance.TriggerSelectedAction();
            }
            else if (OneMinuteLeft())
            {
                PushMessages.ShowBalloonTip("Clock", "One minute left", BalloonIcon.Info);
            }
            
        }

        private string FillEmptyStringWithZero(string time)
        {
            if (string.IsNullOrEmpty(time))  //if Null return 00
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

        private bool IsTimeEquelSetTime()
        {
            return  DateTime.Now.Hour == ClockSetHours &&
                    DateTime.Now.Minute == ClockSetMinutes &&
                    DateTime.Now.Second == ClockSetSeconds;
        }

        private bool OneMinuteLeft()
        {
            int reducedMin = ClockSetMinutes - 1; //
            return  DateTime.Now.Hour == ClockSetHours && 
                    DateTime.Now.Minute == reducedMin && 
                    DateTime.Now.Second == ClockSetSeconds;
               
        }

        ~ClockViewModel()
        {
            _timerTick.Stop();
        }

    }
}
