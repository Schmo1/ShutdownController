using System;
using System.Runtime.CompilerServices;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using ShutdownController.Commands;
using ShutdownController.Core;
using ShutdownController.NotifyIcon;
using ShutdownController.Utility;

namespace ShutdownController.ViewModels
{
    internal class MainViewModel :ObservableObject
    {

        //Views
        public TimerViewModel TimerVM { get; set; }
        public ClockViewModel ClockVM { get; set; }
        public DownUploadViewModel UpDownloadVM { get; set; }
        public DiskViewModel DiskVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }


        //Menu Button commands
        public CommandHandler TimerViewCommand { get; set; }
        public CommandHandler ClockViewCommand { get; set; }
        public CommandHandler DownUploadViewCommand { get; set; }
        public CommandHandler DiskViewCommand { get; set; }
        public CommandHandler SettingsViewCommand { get; set; }
        public CommandHandler SettingsCommand { get; set; }
        public CommandHandler CloseCommand { get; set; }




        private object _currentView;

        public bool TestingModeActiv { get; }
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



        //Commands

        public OptionButtonCommand ShutdownButtonCommand { get; private set; }
        public OptionButtonCommand RestartButtonCommand { get; private set; }
        public OptionButtonCommand SleepButtonCommand { get; private set; }
        public CommandHandler InfoButtonCommand { get; private set; }





        public MainViewModel()
        {
            CreateViewModels();
            CurrentView = GetSavedView();

            CreateCommands();

            SetSleepRestartShutdownDefault(); //Sets to default, if nothing is selected

#if DEBUG
            TestingModeActiv = true;
#else
            TestingModeActiv = false;
#endif

        }

        private void SetSleepRestartShutdownDefault()
        {
            if (!IsShutdownSelected &! IsRestartSelected &!IsSleepSelected)
            {
                IsShutdownSelected = true;
            }
        }

        private void CreateCommands()
        {
            //Shutdown Option Buttons
            ShutdownButtonCommand = new OptionButtonCommand(ShutdownIsPressed);
            RestartButtonCommand = new OptionButtonCommand(RestartIsPressed);
            SleepButtonCommand = new OptionButtonCommand(SleepIsPressed);
            InfoButtonCommand = new CommandHandler(() => InfoButtonIsPressed(), () => true);


            //Create View CommandHandler
            TimerViewCommand = new CommandHandler(() => CurrentView = TimerVM, () => CurrentView != TimerVM);
            ClockViewCommand = new CommandHandler(() => CurrentView = ClockVM, () => CurrentView != ClockVM);
            DownUploadViewCommand = new CommandHandler(() => CurrentView = UpDownloadVM, () => CurrentView != UpDownloadVM);
            DiskViewCommand = new CommandHandler(() => CurrentView = DiskVM, () => CurrentView != DiskVM);
            SettingsViewCommand = new CommandHandler(() => CurrentView = SettingsVM, () => CurrentView != SettingsVM);

            CloseCommand = new CommandHandler(() => CloseWindowCommand() , () => true);
        }

        private void CreateViewModels()
        {

            if (App.STimerViewModel == null)
                App.STimerViewModel = new TimerViewModel();

            if (App.SClockViewModel == null)
                App.SClockViewModel = new ClockViewModel();

            if (App.SDownUploadViewModel == null)
                App.SDownUploadViewModel = new DownUploadViewModel();

            if (App.SDiskViewModel == null)
                App.SDiskViewModel = new DiskViewModel();


            TimerVM = App.STimerViewModel;
            ClockVM = App.SClockViewModel;
            UpDownloadVM = App.SDownUploadViewModel;
            DiskVM = App.SDiskViewModel;

            //Settings
            SettingsVM = new SettingsViewModel();

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

        private object GetSavedView()
        {
            switch (Properties.Settings.Default.LastView)
            {
                case "ClockViewModel":
                    return ClockVM;
                case "DiskViewModel":
                    return DiskVM;
                case "DownUploadViewModel":
                    return UpDownloadVM;
                case "SettingsViewModel":
                    return SettingsVM;

            }
            return TimerVM;
        }

        private void InfoButtonIsPressed()
        {

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

        private void CloseWindowCommand()
        {
            if (TimerVM.TimerStarted &! TimerVM.TimerPaused)
            {
                MyLogger.Instance().Info("HideWindow because Timer is running");
                PushMessages.ShowBalloonTip("Timer", "is still active in the background", BalloonIcon.Info);
                Application.Current.MainWindow.Close();
            }
            else if (ClockVM.ClockActive)
            {
                MyLogger.Instance().Info("HideWindow because Clock is running");
                PushMessages.ShowBalloonTip("Clock", "observing is still active in the background", BalloonIcon.Info);
                Application.Current.MainWindow.Close();
            }
            else if (Properties.Settings.Default.OnClosingRunInBackground)
            {
                MyLogger.Instance().Info("HideWindow pressed on MainWindow"); 
                Application.Current.MainWindow.Close();
            }
            else
            {
                MyLogger.Instance().Info("Close Application pressed on MainWindow");
                Application.Current.Shutdown();
            }
        }


    }
}
