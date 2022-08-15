using System;
using ShutdownController.Core;
using ShutdownController.Utility;
using ShutdownController.NotifyIcon;
using LiveCharts;
using Hardcodet.Wpf.TaskbarNotification;
using System.Runtime.CompilerServices;

namespace ShutdownController.ViewModels
{
    public class DownUploadViewModel : ObservableObject
    {

        private int _scaleMax;
        private int _ySteps;
        private ChartValues<double> downloadValues;
        private ChartValues<double> uploadValues;
        private const int _maxValues = 30;
        private bool _isObserveActive;
        private bool _internetConnectionExist;



        public Func<double, string> FormatterYAxis { get; set; }
        public Func<double, string> FormatterXAxis { get; set; }


        public bool InternetConnectionExist 
        {
            get { return _internetConnectionExist; }
            set { _internetConnectionExist = value; base.OnPropertyChanged(); }
        }

        public int ScalaMax
        {
            get { return _scaleMax; }
            set { _scaleMax = value; base.OnPropertyChanged(); }
        }
        
        public int YSteps
        {
            get { return _ySteps; }
            set { _ySteps = value; base.OnPropertyChanged(); }
        }

        public ChartValues<double> DownloadValues
        {
            get { return downloadValues; }
            set { downloadValues = value; base.OnPropertyChanged(); }
        }
        public ChartValues<double> UploadValues
        {
            get { return uploadValues; }
            set { uploadValues = value; base.OnPropertyChanged(); }
        }




        //Usersettings
        public int Seconds
        {
            get { return Properties.Settings.Default.ObservingSeconds; }
            set { Properties.Settings.Default.ObservingSeconds = Math.Min(value, 60); OnPropertyChanged(); }
        }

        public double ObservingSpeed
        {
            get { return Properties.Settings.Default.ObservingSpeed; }
            set 
            { 
                Properties.Settings.Default.ObservingSpeed = Math.Round(Math.Min(value, 150), 2); 
                OnPropertyChanged(); 
            }
        }

        public bool DownloadObservingPressed
        {
            get { return Properties.Settings.Default.DownloadObservingActive; }
            set { Properties.Settings.Default.DownloadObservingActive = value; OnPropertyChanged(); }
        }

        public bool UploadObservingPressed
        {
            get { return Properties.Settings.Default.UploadObservingActive; }
            set { Properties.Settings.Default.UploadObservingActive = value; OnPropertyChanged();
            }
        }

        public bool ObserveActive
        {
            get { return _isObserveActive; }
            set
            {
                _isObserveActive = value;
                if (_isObserveActive)
                    PushMessages.ShowBalloonTip("Down/Upload", "Download/Upload observing is active", BalloonIcon.Info);
                base.OnPropertyChanged();
            }
        }



        private DownUploadController downUploadController = null;


        //Commands
        public CommandHandler ObserveCommand { get; set; }
        public CommandHandler DownloadObservingCommand { get; set; }
        public CommandHandler UploadObservingCommand { get; set; }


        public DownUploadViewModel()
        {
            ScalaMax = 10;
            YSteps = 2;
            DownloadValues = new ChartValues<double> {};
            UploadValues = new ChartValues<double> {};

            downUploadController = new DownUploadController();
            downUploadController.NewDataEvent += DownUploadController_NewDataEvent;

            FormatterYAxis = value => value + " MB/s";
            FormatterXAxis = value => value + " s";

            //Create commands
            ObserveCommand = new CommandHandler(() => ObserveActive = !_isObserveActive, () => true);

            DownloadObservingCommand = new CommandHandler(() => DownloadObservingnPressed(), () => !DownloadObservingPressed);

            UploadObservingCommand = new CommandHandler(() => UploadObservingnPressed(), () => !UploadObservingPressed);


            if(!DownloadObservingPressed &!UploadObservingPressed)
                DownloadObservingPressed = true; //if nothin is pressed select download
            
            

        }



        private void DownloadObservingnPressed()
        {
            DownloadObservingPressed = true;
            UploadObservingPressed = false;
        }


        private void UploadObservingnPressed()
        {
            DownloadObservingPressed = false;
            UploadObservingPressed = true;
        }

        private void DownUploadController_NewDataEvent(object sender, EventArgs e)
        {
            if (!downUploadController.InternetConnectionExist)
            {
                InternetConnectionExist = false;
                DownloadValues.Clear();
                UploadValues.Clear();
                return;
            }

            InternetConnectionExist = true;


            if (DownloadValues.Count > _maxValues)
                DownloadValues.RemoveAt(0);

            if (UploadValues.Count > _maxValues)
                UploadValues.RemoveAt(0);

            DownloadValues.Add(downUploadController.ReceivedMBs);
            UploadValues.Add(downUploadController.SentMBs);

            UpdateScala(); 

        }

        private void UpdateScala()
        {


            int scaleMax = DetermineMaxValueInChart();


            if (scaleMax > 100)
            {
                ScalaMax = 150;
                YSteps = 20;
            }
            else if (scaleMax > 80)
            {
                ScalaMax = 100;
                YSteps = 20;
            }
            else if(scaleMax > 50)
            {
                ScalaMax = 80;
                YSteps = 20;
            }
            else if(scaleMax > 30)
            {
                ScalaMax = 50;
                YSteps = 10;
            }
            else if (scaleMax > 20)
            {
                ScalaMax = 30;
                YSteps = 5;
            }
            else if (scaleMax > 15)
            {
                ScalaMax = 20;
                YSteps = 5;
            }
            else if (scaleMax > 10)
            {
                ScalaMax = 15;
                YSteps = 3;
            }
            else if (scaleMax > 7)
            {
                ScalaMax = 10;
                YSteps = 2;
            }
            else if (scaleMax > 4)
            {
                ScalaMax = 7;
                YSteps = 2;
            }
            else if (scaleMax > 2)
            {
                ScalaMax = 4;
                YSteps = 1;
            }
            else
            {
                ScalaMax = 2;
                YSteps = 1;
            }



        }


        private int DetermineMaxValueInChart()
        {
            double downloadMax = 0;
            double uploadMax = 0;


            foreach (double value in DownloadValues)
            {
                if (value > downloadMax)
                    downloadMax = value;
            }

            foreach (double value in UploadValues)
            {
                if (value > uploadMax)
                    uploadMax = value;
            }



            if (downloadMax > uploadMax)
                return Convert.ToInt32(Math.Ceiling(downloadMax));
            else
                return Convert.ToInt32(Math.Ceiling(uploadMax));

            
        }

        public override void OnPropertyChanged([CallerMemberName] string name = null)
        {
            //OnPropertyChanged save Usersettings
            Properties.Settings.Default.Save();
            base.OnPropertyChanged(name);
        }
    }
}
