using ShutdownController.Commands;
using ShutdownController.Core;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ShutdownController.ViewModels
{
    internal class MainViewModel :ObservableObject
    {

        //Views
        public TimerViewModel TimerVM { get; set; }
        public ClockViewModel ClockVM { get; set; }
        public DownUploadViewModel UpDownloadVM { get; set; }
        public DiskViewModel DiskVM { get; set; }


        //Menu Button commands
        public CommandHandler TimerViewCommand { get; set; }
        public CommandHandler ClockViewCommand { get; set; }
        public CommandHandler DownUploadViewCommand { get; set; }
        public CommandHandler DiskViewCommand { get; set; }
        public CommandHandler SettingsViewCommand { get; set; }
        public CommandHandler CloseCommand { get; set; }   




        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                SaveViewToSettings(_currentView);
                OnPropertyChanged();
            }
        }


        public bool IsShutdownSelected 
        { 
            get { return Properties.Settings.Default.IsShutdownSelected; }
            private set { Properties.Settings.Default.IsShutdownSelected = value; OnPropertyChanged(); }
        }

        public bool IsRestartSelected
        {
            get { return Properties.Settings.Default.IsRestartSelected; }
            private set { Properties.Settings.Default.IsRestartSelected = value; OnPropertyChanged(); }
        }

        public bool IsSleepSelected
        {
            get { return Properties.Settings.Default.IsSleepSelected; }
            private set { Properties.Settings.Default.IsSleepSelected = value; OnPropertyChanged(); }
        }




        public OptionButtonCommand ShutdownButtonCommand { get; private set; }
        public OptionButtonCommand RestartButtonCommand { get; private set; }
        public OptionButtonCommand SleepButtonCommand { get; private set; }





        public MainViewModel()
        {
            CreateViewModels();
            CurrentView = LoadViewSettings();


            ShutdownButtonCommand = new OptionButtonCommand(ShutdownIsPressed);
            RestartButtonCommand = new OptionButtonCommand(RestartIsPressed);
            SleepButtonCommand = new OptionButtonCommand(SleepIsPressed);


            TimerViewCommand = new CommandHandler(() => CurrentView = TimerVM, () => CurrentView != TimerVM);
            ClockViewCommand = new CommandHandler(() => CurrentView = ClockVM, () => CurrentView != ClockVM);
            DownUploadViewCommand = new CommandHandler(() => CurrentView = UpDownloadVM, () => CurrentView != UpDownloadVM);
            DiskViewCommand = new CommandHandler(() => CurrentView = DiskVM, () => CurrentView != DiskVM);

            CloseCommand = new CommandHandler(() => Application.Current.Shutdown(), () => true);

            SetSleepRestartShutdownDefault();
        }

        private void SetSleepRestartShutdownDefault()
        {
            if (!IsShutdownSelected &! IsRestartSelected &!IsSleepSelected)
            {
                IsShutdownSelected = true;
            }
        }

        private void CreateViewModels()
        {
            TimerVM = new TimerViewModel();
            ClockVM = new ClockViewModel();
            UpDownloadVM = new DownUploadViewModel();
            DiskVM = new DiskViewModel();

        }


        public override void OnPropertyChanged([CallerMemberName] string name = null)
        {
            Properties.Settings.Default.Save();
            base.OnPropertyChanged(name);
        }

        

        private void SaveViewToSettings(object view)
        {
            Properties.Settings.Default.LastView = view.GetType().Name;
            Properties.Settings.Default.Save();
        }

        private object LoadViewSettings()
        {
            switch (Properties.Settings.Default.LastView)
            {
                case "ClockViewModel":
                    return ClockVM;
                case "DiskViewModel":
                    return DiskVM;
                case "DownUploadViewModel":
                    return UpDownloadVM;

            }
            return TimerVM;
        }


        private void ShutdownIsPressed()
        {
            IsShutdownSelected = true;
            IsRestartSelected = false;
            IsSleepSelected = false;
        }

        private void RestartIsPressed()
        {
            IsShutdownSelected = false;
            IsRestartSelected = true;
            IsSleepSelected = false;
        }
        private void SleepIsPressed()
        {
            IsShutdownSelected = false;
            IsRestartSelected = false;
            IsSleepSelected = true;
        }





    }
}
