using System;
using ShutdownController.Core;
using ShutdownController.Utility;
using LiveCharts;

namespace ShutdownController.ViewModels
{
    public class DownUploadViewModel : ObservableObject
    {

        private int _scaleMax;
        private int _ySteps;
        private ChartValues<double> downloadValues;
        private ChartValues<double> uploadValues;
        private const int _maxValues = 30;


        public int ScalaMax
        {
            get { return _scaleMax; }
            set { _scaleMax = value; OnPropertyChanged(); }
        }
        
        public int YSteps
        {
            get { return _ySteps; }
            set { _ySteps = value; OnPropertyChanged(); }
        }

        public ChartValues<double> DownloadValues
        {
            get { return downloadValues; }
            set { downloadValues = value; OnPropertyChanged(); }
        }
        public ChartValues<double> UploadValues
        {
            get { return uploadValues; }
            set { uploadValues = value; OnPropertyChanged(); }
        }


        private DownUploadController downUploadController = null;



        public DownUploadViewModel()
        {
            ScalaMax = 10;
            YSteps = 2;
            DownloadValues = new ChartValues<double> {};
            UploadValues = new ChartValues<double> {};

            downUploadController = new DownUploadController();
            downUploadController.NewDataEvent += DownUploadController_NewDataEvent;


        }

        private void DownUploadController_NewDataEvent(object sender, System.EventArgs e)
        {
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
    }
}
