using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ShutdownController.Core;
using ShutdownController.Utility;
using ShutdownController.Enums;
using LiveCharts;

namespace ShutdownController.ViewModels
{
    public class DownUploadViewModel : ObservableObject, IViewModel
    {

        #region Variables

        private int _scaleMax;
        private int _ySteps;
        private ChartValues<double> downloadValues = new ChartValues<double>() ;
        private ChartValues<double> uploadValues = new ChartValues<double> ();
        private ChartValues<double> observedDownloadValues = new ChartValues<double>();
        private ChartValues<double> observedUploadValues = new ChartValues<double>();
        private const int _maxValuesInChart = 30;
        private const int _maxSecondsToObserve = 240;
        private bool _isObserveActive;
        private bool _isValueUnderObservingSpeed;
        private bool _internetConnectionExist;
        private string[] _networkInterfaces;

        #endregion


        public ViewNameEnum ViewName => ViewNameEnum.DownUploadView;


        private ScalaCalculator scalaCalculator;

        public ChartValues<double> ObservedDownloadValues
        {
            get { return observedDownloadValues; }
            set { observedDownloadValues = value; }
        }
        public ChartValues<double> ObservedUploadValues
        {
            get { return observedUploadValues; }
            set { observedUploadValues = value; }
        }


        public bool InternetConnectionExist 
        {
            get { return _internetConnectionExist; }
            set 
            {
                if (_internetConnectionExist != value)
                {
                    if (value)
                        MyLogger.Instance().Info("Internet Connection Exist");
                    else
                        MyLogger.Instance().Info("No Internet Connection Exist");
                }

                _internetConnectionExist = value;
                base.OnPropertyChanged(); 
            }
        }


        #region Chartvalues


        public Func<double, string> FormatterYAxis { get; set; }
        public Func<double, string> FormatterXAxis { get; set; }


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

        public bool SavePerformance
        {
            get { return Properties.Settings.Default.SavePerformance; }
        }


        public bool IsValueUnderObservingSpeed
        {
            get { return _isValueUnderObservingSpeed; }
            set { _isValueUnderObservingSpeed = value; base.OnPropertyChanged(); }
        }
        #endregion


        #region Usersettings
        public int Seconds
        {
            get { return Properties.Settings.Default.ObservingSecondsDownUp; }
            set 
            { 
                if (value < 5)
                    value = 5;
                Properties.Settings.Default.ObservingSecondsDownUp = Math.Min(value, _maxSecondsToObserve); 
                
                OnPropertyChanged(); 
            }
        }

        public double ThresholdSpeed
        {
            get { return Properties.Settings.Default.ThresholdSpeedDownUp; }
            set 
            { 
                Properties.Settings.Default.ThresholdSpeedDownUp = Math.Round(Math.Min(value, 150), 2); 
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
                    MyLogger.Instance().Info("Observe Down/Upload is active. Seconds: " + Seconds.ToString() + " Observing Speed: " + ThresholdSpeed.ToString());
                    ValueUnderObservingSpeed(); 
                }
                else
                {
                    MyLogger.Instance().Info("Observe Down/Upload is inactive");
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
        #endregion

        private DownUploadController downUploadController = null;


        #region Commands
        public CommandHandler ObserveCommand { get; set; }
        public CommandHandler DownloadObservingCommand { get; set; }
        public CommandHandler UploadObservingCommand { get; set; }



        #endregion




        //Construktor
        public DownUploadViewModel()
        {

            SetDefaultValues();


            scalaCalculator = new ScalaCalculator(new List<ChartValues<double>>() { DownloadValues, UploadValues });

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



        }

        private void NetworkInterfacesHasChanged(object sender, EventArgs e)
        {
            NetworkInterfaces = downUploadController.NetworkInterfaces;

            if (NetworkInterfaces == null)
                return;


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
                    if (ObservedDownloadValues[i] < ThresholdSpeed) 
                        valuesUnderSpeed++;
                }

                if(valuesUnderSpeed >= Seconds)
                    return true;

            }
            else if (UploadObservingPressed)
            {
                
                if (ObservedUploadValues.Count < Seconds) //not enough values to calculate
                    return false;

                int valuesUnderSpeed = 0;
                for (int i = ObservedUploadValues.Count - Seconds; i < ObservedUploadValues.Count; i++)
                {
                    if (ObservedUploadValues[i] < ThresholdSpeed)
                        valuesUnderSpeed++;
                }

                if (valuesUnderSpeed >= Seconds)
                    return true;
                
            }
            return false;
        }

        private void ValueUnderObservingSpeed() //Check if last value was under set speed
        {
            

            if (DownloadObservingPressed)
            {
                if (DownloadValues.Count == 0)
                    return;

                IsValueUnderObservingSpeed = DownloadValues[DownloadValues.Count - 1] < ThresholdSpeed;
            }
            else if (UploadObservingPressed)
            {
                if (UploadValues.Count == 0)
                    return;

                IsValueUnderObservingSpeed = UploadValues[UploadValues.Count - 1] < ThresholdSpeed;
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
                MyLogger.Instance().Error("Download or Upload is not selected ");
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
            int step;
            ScalaMax = scalaCalculator.GetChartUpperLine(out step);
            YSteps = step;

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

            if (ThresholdSpeed == 0)
                ThresholdSpeed = 1.5;

            if (!DownloadObservingPressed & !UploadObservingPressed)
                DownloadObservingPressed = true; //if nothin is pressed select download
        }

    }
}
