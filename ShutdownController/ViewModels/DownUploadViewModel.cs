using LiveCharts;
using ShutdownController.Core;

namespace ShutdownController.ViewModels
{
    public class DownUploadViewModel : ObservableObject
    {

        private int scaleMax;
        private ChartValues<double> downloadValues;
        private ChartValues<double> uploadValues;


        public int ScalaMax
        {
            get { return scaleMax; }
            set { scaleMax = value; OnPropertyChanged(); }
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






        public DownUploadViewModel()
        {
            ScalaMax = 100;
            DownloadValues = new ChartValues<double> { 23,23,2,3,1,10,50,40};
            UploadValues = new ChartValues<double> { 20, 10, 20, 30, 30, 10, 20, 20, 20, 20, 30, 30, 50, 20, 32, 30, 50, 80, 80, 70, 80, 54, 54, 30, 57, 40, 40, 80, 93, 95, 85 };
        }

    }
}
