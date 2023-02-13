using System;
using System.Runtime.CompilerServices;
using System.Windows;
using ShutdownController.Resources.TimerStrings;
using ShutdownController.Core;
using ShutdownController.Utility;
using MessageBox = System.Windows.MessageBox;
using ShutdownController.Enums;
using Hardcodet.Wpf.TaskbarNotification;
using ShutdownController.NotifyIcon;
using ShutdownController.Views.MessageBox;

namespace ShutdownController.ViewModels
{
    public class TimerViewModel : ObservableObject, IViewModel
    {
        #region Variables
        private bool _isTimerStarted;
        private bool _isTimerPaused;
        private int _timerHours;
        private int _timerMinutes;
        private int _timerSeconds;


        private readonly AccurateTimerTick _timerTick = new AccurateTimerTick(new TimeSpan(0,0,1));

        #endregion

        #region Properties
        public ViewNameEnum ViewName => ViewNameEnum.TimerView;


        public bool TimerPaused
        {
            get { return _isTimerPaused; }
            set 
            { 
                _isTimerPaused = value;
                if (_isTimerPaused) { MyLogger.Instance().Info("Timer paused"); }
                OnPropertyChanged(); 
            }
        }

        public bool TimerStarted
        {
            get { return _isTimerStarted; }
            private set 
            {
                _isTimerStarted = value; 
                if(_isTimerStarted) { MyLogger.Instance().Info("Timer started"); }
                OnPropertyChanged();
            }
        }


        //Sliders
        public int TimerSetSliderHours
        {
            get { return Properties.Settings.Default.TimerSetHours; }
            set
            {
                TimerSetHours = Properties.Settings.Default.TimerSetHours = Math.Min(value, 23);
                OnPropertyChanged();
            }
        }
        public int TimerSetSliderMinutes
        {
            get { return Properties.Settings.Default.TimerSetMinutes; }
            set
            {
                TimerSetMinutes = Properties.Settings.Default.TimerSetMinutes = Math.Min(value, 59);
                OnPropertyChanged();
            }
        }
        public int TimerSetSliderSeconds
        {
            get { return Properties.Settings.Default.TimerSetSeconds; }
            set
            {
                TimerSetSeconds = Properties.Settings.Default.TimerSetSeconds = Math.Min(value, 59);
                OnPropertyChanged();
            }
        }


        //Display
        public int TimerSetHours
        {
            get { return _timerHours; }
            set
            {
                if (!TimerStarted)
                {
                    Properties.Settings.Default.TimerSetHours = value;
                    OnPropertyChanged("TimerSetSliderHours");
                }
                _timerHours = value;
                base.OnPropertyChanged();
            }
        }
        public int TimerSetMinutes
        {
            get { return _timerMinutes; }
            set
            {
                if (!TimerStarted)
                {
                    Properties.Settings.Default.TimerSetMinutes = value;
                    OnPropertyChanged("TimerSetSliderMinutes");
                }
                _timerMinutes = value;
                base.OnPropertyChanged();
            }
        }
        public int TimerSetSeconds
        {
            get { return _timerSeconds; }
            set
            {
                if (!TimerStarted)
                {
                    Properties.Settings.Default.TimerSetSeconds = value;
                    OnPropertyChanged("TimerSetSliderSeconds");
                }
                _timerSeconds = value;
                base.OnPropertyChanged();
            }
        }

        #endregion


        //Commands

        public CommandHandler TimerStartCommand { get; set; }
        public CommandHandler TimerStopCommand { get; set; }

        //Events
        public event EventHandler TimerExpiredEvent;

        //Constructor

        public TimerViewModel()
        {
            TimerStartCommand = new CommandHandler(() => TimerStartPause(), () => true);
            TimerStopCommand = new CommandHandler(() => TimerStop(), () => true);


            _timerTick.Tick += SupstractSecond;

            LoadTimerSettings();
        }


        //Methods

        private void SupstractSecond(object sender, EventArgs e)
        {
            //Check if Timer is zero
            if (TimerSetSeconds <= 0 && TimerSetMinutes <= 0 && TimerSetHours <= 0)
            {
                TimerExpired();
                return;
            }else if (TimerSetSeconds == 0 && TimerSetMinutes == 1 && TimerSetHours == 0) //Check if one minute left
            {
                PushMessages.ShowBalloonTip(TimerStrings.timer, TimerStrings.oneMinuteLeft, BalloonIcon.Info);
            }

            

            if (TimerSetSeconds == 0)
            {
                TimerSetSeconds = 59;

                if (TimerSetMinutes == 0)
                {
                    TimerSetHours -= 1;
                    TimerSetMinutes = 59;
                }
                else
                    TimerSetMinutes -= 1;
            }
            else
                TimerSetSeconds -= 1;
            
        }

        public void TimerExpired()
        {
            MyLogger.Instance().Info("Timer expired!");
            TimerStop();
            TimerExpiredEvent?.Invoke(this, EventArgs.Empty);
            LoadTimerSettings();
            ShutdownOptions.Instance.TriggerSelectedAction();
        }

        private void TimerStartPause()
        {
            if (!_isTimerStarted || _isTimerPaused)
            {
                if (!TimeIsZeroAndExecuteAnyway())
                    return;

                TimerStarted = true;
                TimerPaused = false;
                _timerTick.Start();
            }
            else
            {
                _timerTick.Stop();
                TimerPaused = true;
            }

        }

        private void TimerStop()
        {
            TimerStarted = false;
            TimerPaused = false;
            _timerTick.Stop();
            LoadTimerSettings();
            MyLogger.Instance().Info("Timer stopped");
        }
        private bool TimeIsZeroAndExecuteAnyway()
        {
            if (TimerSetHours == 0 && TimerSetMinutes == 0 && TimerSetSeconds == 0)
            {
                MessageBoxResult mBr = MessageBox.Show("Time is Zero. Do you want to execute the action anyway?", "Timer", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(MessageBoxResult.Yes == mBr)
                    return true;
                return false;
            }
            return true; //Time is not zero
        }

        private void LoadTimerSettings()
        {
            MyLogger.Instance().Debug("Load timer settings");
            TimerSetHours = Properties.Settings.Default.TimerSetHours;
            TimerSetMinutes = Properties.Settings.Default.TimerSetMinutes;
            TimerSetSeconds = Properties.Settings.Default.TimerSetSeconds;
        }

        public override void OnPropertyChanged([CallerMemberName] string name = null)
        {
            Properties.Settings.Default.Save();
            base.OnPropertyChanged(name);
        }

        ~TimerViewModel()
        {
            _timerTick.Stop();
        }
    }
}
