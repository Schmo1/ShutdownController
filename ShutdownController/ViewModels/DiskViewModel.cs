using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using LiveCharts;
using ShutdownController.Core;
using ShutdownController.Utility;

namespace ShutdownController.ViewModels
{
    public class DiskViewModel:ObservableObject
    {

        #region Variables
        private bool _isObserveActive; 
        private const int _maxValuesInChart = 30;
        private const int _maxSecondsToObserve = 60;
        private int _scaleMax;
        private int _ySteps;
        private bool _isValueUnderObservingSpeed;
        private ChartValues<double> _readValues = new ChartValues<double>();
        private ChartValues<double> _writeValues =  new ChartValues<double>();
        private ChartValues<double> observedReadValues = new ChartValues<double>();
        private ChartValues<double> _observedWriteValues = new ChartValues<double>();
        private string[] _connectedDisks;
        #endregion


        private DiskObserver diskObserver;

        public ChartValues<double> ObservedReadValues
        {
            get { return observedReadValues; }
            set { observedReadValues = value; }
        }
        public ChartValues<double> ObservedWriteValues
        {
            get { return _observedWriteValues; }
            set { _observedWriteValues = value; }
        }

        #region ChartValues
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

        public ChartValues<double> ReadValues
        {
            get { return _readValues; }
            set { _readValues = value; base.OnPropertyChanged(); }
        }
        public ChartValues<double> WriteValues
        {
            get { return _writeValues; }
            set { _writeValues = value; base.OnPropertyChanged(); }
        }


        public bool IsValueUnderObservingSpeed
        {
            get { return _isValueUnderObservingSpeed; }
            set { _isValueUnderObservingSpeed = value; base.OnPropertyChanged(); }
        }
        #endregion


        #region Usersettings
        public bool ObserveActive 
        { 
            get { return _isObserveActive; } 
            set 
            {
                _isObserveActive = value;
                if (_isObserveActive)
                    MyLogger.Instance().Info("Observe Disk is active. Seconds: " + Seconds.ToString() + " Observing Speed: " + ObservingSpeed.ToString());
                base.OnPropertyChanged(); 
            } 
        }
        public bool ReadObservingActive 
        { 
            get { return Properties.Settings.Default.ReadObservingActive; } 
            set { Properties.Settings.Default.ReadObservingActive = value; OnPropertyChanged(); ClearObservationValues(); } 
        }

        public bool WriteObservingActive 
        { 
            get { return Properties.Settings.Default.WriteObservingActive; } 
            set { Properties.Settings.Default.WriteObservingActive = value; OnPropertyChanged(); ClearObservationValues(); } 
        }

        public int Seconds
        {
            get { return Properties.Settings.Default.ObservingSecondsDisk; }
            set
            {
                if (value < 5)
                    value = 5;
                Properties.Settings.Default.ObservingSecondsDisk = Math.Min(value, _maxSecondsToObserve);

                OnPropertyChanged();
            }
        }

        public double ObservingSpeed
        {
            get { return Properties.Settings.Default.ObservingSpeedDisk; }
            set
            {
                Properties.Settings.Default.ObservingSpeedDisk = Math.Round(Math.Min(value, 150), 2);
                OnPropertyChanged();
            }
        }
        public string SelectedDisk
        { 
            get { return Properties.Settings.Default.SelectedDisk; } 
            set 
            { 
                Properties.Settings.Default.SelectedDisk = value; 
                diskObserver.SelectedDisk = value; 
                OnPropertyChanged(); 
                ClearScalaValues(); 
            } 
        }
        #endregion


        public string[] ConnectedDisks
        {
            get { return _connectedDisks; }
            set { _connectedDisks = value; base.OnPropertyChanged(); }
        }



        #region Commands
        public CommandHandler ObserveCommand { get; set; }
        public CommandHandler ReadObservingCommand { get; set; }
        public CommandHandler WriteObservingCommand { get; set; }
        #endregion



        public DiskViewModel()
        {
            SetDefaultValues();

            //Create commands
            ObserveCommand = new CommandHandler(() => ObserveActive = !_isObserveActive, () => true);

            ReadObservingCommand = new CommandHandler(() => ReadObservingnPressed(), () => !ReadObservingActive);

            WriteObservingCommand = new CommandHandler(() => WriteObservingnPressed(), () => !WriteObservingActive);


            FormatterYAxis = value => value + " MB/s";
            FormatterXAxis = value => value + " s";

            diskObserver = new DiskObserver();
            diskObserver.NewReadWriteData += WriteNewDataToChart;
            diskObserver.DisksAreUpdated += DiskHasChanged;

            DiskHasChanged(null, EventArgs.Empty);



        }

        private void DiskHasChanged(object sender, EventArgs e)
        {
            if (diskObserver.Disks.Count == 0)
                return;

            ConnectedDisks = diskObserver.Disks.ToArray();

            if (string.IsNullOrEmpty(SelectedDisk))
            {
                SelectedDisk = ConnectedDisks[0];
                return;
            }

            foreach (string disk in ConnectedDisks)
            {
                if (SelectedDisk == disk)
                {
                    diskObserver.SelectedDisk = SelectedDisk;
                    return;
                }
            }

            SelectedDisk = ConnectedDisks[0]; // If Selected Disk did not exist, select disk 1

        }

        private void WriteNewDataToChart(object sender, EventArgs e)
        {
            AddValuesToChart();
            
        }
        private void AddValuesToChart()
        {
            if (ReadValues.Count >= _maxValuesInChart)
                ReadValues.RemoveAt(0);

            if (WriteValues.Count >= _maxValuesInChart)
                WriteValues.RemoveAt(0);

            ReadValues.Add(diskObserver.ReadValue);
            WriteValues.Add(diskObserver.WriteValue);

            
        }

        private void SetDefaultValues()
        {
            ScalaMax = 10;
            YSteps = 2;

            if (Seconds == 0)
                Seconds = 10;

            if (ObservingSpeed == 0)
                ObservingSpeed = 1.5;

            if (!ReadObservingActive & !WriteObservingActive)
                ReadObservingActive = true; //if nothin is pressed select download
        }

        private void ReadObservingnPressed()
        {
            ReadObservingActive = true;
            WriteObservingActive = false;
        }

        private void WriteObservingnPressed()
        {
            ReadObservingActive = false;
            WriteObservingActive = true;
        }

        private void ClearObservationValues()
        {
            ObservedReadValues.Clear();
            ObservedWriteValues.Clear();
        }

        private void ClearScalaValues()
        {
            ReadValues.Clear();
            WriteValues.Clear();
        }

        public override void OnPropertyChanged([CallerMemberName] string name = null)
        {
            //OnPropertyChanged save Usersettings
            Properties.Settings.Default.Save();
            base.OnPropertyChanged(name);
        }

    }
}
