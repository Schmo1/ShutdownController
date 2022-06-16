using ShutdownController.Core;

namespace ShutdownController.ViewModels
{
    internal class TimerViewModel : ObservableObject
    {

        private bool _isTimerActive;

        public bool TimerActive { get { return _isTimerActive; } set { _isTimerActive = value; OnPropertyChanged(); } }


        public CommandHandler TimerStartCommand { get; set; }



        public TimerViewModel()
        {
            TimerStartCommand = new CommandHandler(() => TimerActive = !_isTimerActive, () => true);

        }

    }
}
