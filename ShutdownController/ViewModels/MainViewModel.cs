using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using ShutdownController.Commands;
using ShutdownController.Core;
using ShutdownController.Enums;
using ShutdownController.Utility;
using ShutdownController.Views.ToastNotification;

namespace ShutdownController.ViewModels
{
    internal class MainViewModel : ObservableObject, IViewModel
    {

        public ViewNameEnum ViewName => ViewNameEnum.MainView;


        //Views
        public TimerViewModel TimerVM { get; set; }
        public ClockViewModel ClockVM { get; set; }
        public DownUploadViewModel UpDownloadVM { get; set; }
        public DiskViewModel DiskVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }
        public List<IViewModel> ListOfViewModels { get; private set; } = new List<IViewModel>();



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


        public static event EventHandler RaiseInfoMessages;


        //Commands

        public OptionButtonCommand ShutdownButtonCommand { get; private set; }
        public OptionButtonCommand RestartButtonCommand { get; private set; }
        public OptionButtonCommand SleepButtonCommand { get; private set; }
        public CommandHandler InfoButtonCommand { get; private set; }



        public MainViewModel()
        {
            CreateViewModels();
            CurrentView = ListOfViewModels.Find(x => x.ViewName == Properties.Settings.Default.LastView);

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
            //InfoButtonCommand = new CommandHandler(() => RaiseInfoMessages?.Invoke(this, EventArgs.Empty), () => true); 
            InfoButtonCommand = new CommandHandler(() => ShowInfoMessages(), () => true);

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

            ListOfViewModels.Clear();

            ListOfViewModels.Add(TimerVM);
            ListOfViewModels.Add(ClockVM);
            ListOfViewModels.Add(UpDownloadVM);
            ListOfViewModels.Add(DiskVM);
            ListOfViewModels.Add(SettingsVM);


        }

        private void ShowInfoMessages()
        {
            CustomNotifierCaller.ShowTabInfo(Application.Current.MainWindow);

            switch (((IViewModel)CurrentView).ViewName)
            {
                case ViewNameEnum.TimerView:
                    CustomNotifierCaller.ShowTimerInfo();
                    break;
                case ViewNameEnum.ClockView:
                    CustomNotifierCaller.ShowClockInfo();
                    break;
                case ViewNameEnum.DownUploadView:
                    CustomNotifierCaller.ShowDownUploadInfo();
                    break;
                case ViewNameEnum.DiskView:
                    CustomNotifierCaller.ShowDiskInfo();
                    break;
                case ViewNameEnum.SettingsView:
                    CustomNotifierCaller.ShowSettingsInfo();
                    break;
            }
        }


        public override void OnPropertyChanged([CallerMemberName] string name = null)
        {
            Properties.Settings.Default.Save();
            base.OnPropertyChanged(name);
        }




        private static void SaveViewToSettings(object view)     
        {
            try
            {
                Properties.Settings.Default.LastView = ((IViewModel)view).ViewName;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MyLogger.Instance().Error("Convert view error: " + ex.Message);
            }
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
                System.Windows.Application.Current.MainWindow.Close();
            }
            else if (ClockVM.ClockActive)
            {
                MyLogger.Instance().Info("HideWindow because Clock is running");
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
