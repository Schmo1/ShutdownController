using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using ShutdownController.Core;
using ShutdownController.Utility;
using MessageBox = System.Windows.MessageBox;

namespace ShutdownController.ViewModels
{
    public class TimerViewModel : ObservableObject
    {

        private bool _isTimerStarted;
        private bool _isTimerPaused;
        private int _timerHours;
        private int _timerMinutes;
        private int _timerSeconds;

        private readonly Timer _timer = new Timer();



        //Properties
        public bool TimerPaused
        {
            get { return _isTimerPaused; }
            set { _isTimerPaused = value; OnPropertyChanged(); }
        }

        public bool TimerStarted
        {
            get { return _isTimerStarted; }
            private set {_isTimerStarted = value; OnPropertyChanged(); }
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

            _timer.Interval = 1000;
            _timer.Tick += SupstractSecond;

            ResetDisplayTimer();
        }


        //Methods

        private void SupstractSecond(object sender, EventArgs e)
        {
            //Check if Timer is zero
            if (TimerSetSeconds <= 0 && TimerSetMinutes <= 0 && TimerSetHours <= 0)
            {
                TimerExpired();
                return;
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
                _timer.Start();
            }
            else
            {
                _timer.Stop();
                TimerPaused = true;
            }

        }

        private void TimerStop()
        {
            TimerStarted = false;
            TimerPaused = false;
            _timer.Stop();
            ResetDisplayTimer();
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

        private void ResetDisplayTimer()
        {
            TimerSetHours = Properties.Settings.Default.TimerSetHours;
            TimerSetMinutes = Properties.Settings.Default.TimerSetMinutes;
            TimerSetSeconds = Properties.Settings.Default.TimerSetSeconds;
        }


        public override void OnPropertyChanged([CallerMemberName] string name = null)
        {
            Properties.Settings.Default.Save();
            base.OnPropertyChanged(name);
        }

    }
}
