using System;
using ShutdownController.Core;
using ShutdownController.Utility;
using ShutdownController.NotifyIcon;
using LiveCharts;
using Hardcodet.Wpf.TaskbarNotification;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace ShutdownController.ViewModels
{
    public class DownUploadViewModel : ObservableObject
    {

        private int _scaleMax;
        private int _ySteps;
        private ChartValues<double> downloadValues;
        private ChartValues<double> uploadValues;
        private ChartValues<double> observedDownloadValues = new ChartValues<double>();
        private ChartValues<double> observedUploadValues = new ChartValues<double>();
        private const int _maxValuesInChart = 30;
        private const int _maxSecondsToObserve = 60;
        private bool _isObserveActive;
        private bool _isValueUnderObservingSpeed;
        private bool _internetConnectionExist;
        private string[] _networkInterfaces;



        public Func<double, string> FormatterYAxis { get; set; }
        public Func<double, string> FormatterXAxis { get; set; }


        public bool InternetConnectionExist 
        {
            get { return _internetConnectionExist; }
            set 
            { 
                _internetConnectionExist = value;
                if (_internetConnectionExist)
                    MyLogger.Instance().Info("Internet Connection Exist");
                else
                    MyLogger.Instance().Info("No Internet Connection Exist");

                base.OnPropertyChanged(); 
            }
        }


        //Chart values
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


        public ChartValues<double> ObservedDownloadValues
        {
            get { return observedDownloadValues; }
            set { observedDownloadValues = value;}
        }
        public ChartValues<double> ObservedUploadValues
        {
            get { return observedUploadValues; }
            set { observedUploadValues = value;}
        }
        public bool IsValueUnderObservingSpeed
        {
            get { return _isValueUnderObservingSpeed; }
            set { _isValueUnderObservingSpeed = value; base.OnPropertyChanged(); }
        }

 




        //Usersettings
        public int Seconds
        {
            get { return Properties.Settings.Default.ObservingSeconds; }
            set 
            { 
                if (value < 5)
                    value = 5;
                Properties.Settings.Default.ObservingSeconds = Math.Min(value, _maxSecondsToObserve); 
                
                OnPropertyChanged(); 
            }
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
            set { Properties.Settings.Default.DownloadObservingActive = value; OnPropertyChanged(); ClearObservationValues(); }
        }

        public bool UploadObservingPressed
        {
            get { return Properties.Settings.Default.UploadObservingActive; }
            set { Properties.Settings.Default.UploadObservingActive = value; OnPropertyChanged(); ClearObservationValues();   }
        }

        public bool ObserveActive
        {
            get { return _isObserveActive; }
            set
            {
                _isObserveActive = value;

                if (_isObserveActive) 
                { 
                    PushMessages.ShowBalloonTip("Down/Upload", "Download/Upload observing is active", BalloonIcon.Info);
                    ValueUnderObservingSpeed(); 
                }
                else
                {
                    ClearObservationValues();
                    IsValueUnderObservingSpeed = false;
                }


                base.OnPropertyChanged();

            }
        }

        public string[] NetworkInterfaces 
        {
            get { return _networkInterfaces; }
            set { _networkInterfaces = value; base.OnPropertyChanged(); }
        }

        public string SelectedNetworkInterface 
        { 
            get { return Properties.Settings.Default.NetworkInterface; } 
            set 
            { 
                Properties.Settings.Default.NetworkInterface = value;
                downUploadController.SelectedNetworkInterface = value;
                ClearObservationValues();
                ClearScalaValues();
                OnPropertyChanged(); 
            } 
        }


        private DownUploadController downUploadController = null;


        //Commands
        public CommandHandler ObserveCommand { get; set; }
        public CommandHandler DownloadObservingCommand { get; set; }
        public CommandHandler UploadObservingCommand { get; set; }





        //Construktor
        public DownUploadViewModel()
        {

            SetDefaultValues();

            DownloadValues = new ChartValues<double> {};
            UploadValues = new ChartValues<double> {};

            downUploadController = new DownUploadController();
            downUploadController.NewDownloadUploadData += DownUploadController_NewDataEvent;
            downUploadController.NetworkinterfaceHasChanged += NetworkInterfacesHasChanged;

            NetworkInterfacesHasChanged(this, EventArgs.Empty);

            FormatterYAxis = value => value + " MB/s";
            FormatterXAxis = value => value + " s";

            //Create commands
            ObserveCommand = new CommandHandler(() => ObserveActive = !_isObserveActive, () => true);

            DownloadObservingCommand = new CommandHandler(() => DownloadObservingnPressed(), () => !DownloadObservingPressed);

            UploadObservingCommand = new CommandHandler(() => UploadObservingnPressed(), () => !UploadObservingPressed);


            if(!DownloadObservingPressed &!UploadObservingPressed)
                DownloadObservingPressed = true; //if nothin is pressed select download
            
            

        }

        private void NetworkInterfacesHasChanged(object sender, EventArgs e)
        {
            NetworkInterfaces = downUploadController.NetworkInterfaces;

            if (NetworkInterfaces.Length == 0)
            {
                SelectedNetworkInterface = null;
                return;
            }

            if (string.IsNullOrEmpty(SelectedNetworkInterface))
            {
                SelectedNetworkInterface = NetworkInterfaces[0];
            }
            else
            {
                bool interfaceExist = false; //Check if selected interface already exist
                foreach (string networkInterface in NetworkInterfaces)
                {
                    if(networkInterface == SelectedNetworkInterface)
                    {
                        interfaceExist = true;
                    }
                }

                if(!interfaceExist)
                    SelectedNetworkInterface = NetworkInterfaces[0];

            }

            downUploadController.SelectedNetworkInterface = SelectedNetworkInterface;
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
                NoInternetConnectionExist();
                return;
            }

            InternetConnectionExist = true;

            AddValuesToChart();

            if (!ObserveActive)
                return;


            AddValuesToObservation();

            ValueUnderObservingSpeed();

            if (ValueAchieved())
            {
                ObserveActive = false;
                ClearObservationValues();
                MyLogger.Instance().Info("Down Upload is running out");
                ShutdownOptions.Instance.TriggerSelectedAction();
            }
           
        }

        private bool ValueAchieved()
        {

            if (DownloadObservingPressed)
            {
                if(ObservedDownloadValues.Count < Seconds) //not enough values to calculate
                    return false;

                int valuesUnderSpeed = 0;
                for (int i = ObservedDownloadValues.Count - Seconds; i < ObservedDownloadValues.Count; i++) 
                {
                    if (ObservedDownloadValues[i] < ObservingSpeed) 
                        valuesUnderSpeed++;
                }

                if(valuesUnderSpeed >= Seconds)
                    return true;

            }
            else if (UploadObservingPressed)
            {
                foreach (double value in ObservedUploadValues)
                {
                    if (ObservedUploadValues.Count < Seconds) //not enough values to calculate
                        return false;

                    int valuesUnderSpeed = 0;
                    for (int i = ObservedUploadValues.Count - Seconds; i < ObservedUploadValues.Count; i++)
                    {
                        if (ObservedUploadValues[i] < ObservingSpeed)
                            valuesUnderSpeed++;
                    }

                    if (valuesUnderSpeed >= Seconds)
                        return true;
                }
            }
            return false;
        }

        private void ValueUnderObservingSpeed() //Check if last value was under set speed
        {
            

            if (DownloadObservingPressed)
            {
                if (DownloadValues.Count == 0)
                    return;

                IsValueUnderObservingSpeed = DownloadValues[DownloadValues.Count - 1] < ObservingSpeed;
            }
            else if (UploadObservingPressed)
            {
                if (UploadValues.Count == 0)
                    return;

                IsValueUnderObservingSpeed = UploadValues[UploadValues.Count - 1] < ObservingSpeed;
            }
        }

        private void AddValuesToObservation()
        {
            if (DownloadObservingPressed)
            {
                if (ObservedDownloadValues.Count > _maxSecondsToObserve)
                    ObservedDownloadValues.RemoveAt(0);

                ObservedDownloadValues.Add(downUploadController.ReceivedMBs);

            }else if (UploadObservingPressed)
            {
                if (ObservedUploadValues.Count > _maxSecondsToObserve)
                    ObservedUploadValues.RemoveAt(0);

                ObservedUploadValues.Add(downUploadController.SentMBs);
            }
            else
            {
                throw new Exception( "Download or Upload is not selected ");
            }
        }

        private void ClearObservationValues()
        {
            ObservedDownloadValues.Clear();
            ObservedUploadValues.Clear();
        }

        private void ClearScalaValues()
        {
            DownloadValues.Clear();
            UploadValues.Clear();
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


        private void NoInternetConnectionExist()
        {
            InternetConnectionExist = false;
            IsValueUnderObservingSpeed = false;
            ClearScalaValues();
            ClearObservationValues();
        }

        private void AddValuesToChart()
        {
            if (DownloadValues.Count >= _maxValuesInChart)
                DownloadValues.RemoveAt(0);

            if (UploadValues.Count >= _maxValuesInChart)
                UploadValues.RemoveAt(0);

            DownloadValues.Add(downUploadController.ReceivedMBs);
            UploadValues.Add(downUploadController.SentMBs);

            UpdateScala();
        }

        private void SetDefaultValues()
        {
            ScalaMax = 10;
            YSteps = 2;

            if (Seconds == 0)
                Seconds = 10;

            if (ObservingSpeed == 0)
                ObservingSpeed = 1.5;
        }

    }
}
