using System;
using System.Net.NetworkInformation;
using System.Windows.Threading;

namespace ShutdownController.Utility
{
    public class DownUploadController
    {

        //Variables
        private DispatcherTimer _timer = new DispatcherTimer();
        private long _maxReceivedBytesNew;
        private long _maxSentBytesNew;
        private long _maxReceivedBytesOld;
        private long _maxSentBytesOld;
        private long _receivedBytesDelta;
        private long _sentBytesDelta;
        private double _receivedMBs;
        private double _sentMBs;



        //Properties
        public bool InternetConnectionExist { get { return NetworkInterface.GetIsNetworkAvailable(); } }
        public NetworkInterface[] InternetInterfaces { get { return GetInternetConnections(); } }
        public double ReceivedMBs { get { return _receivedMBs; } }
        public double SentMBs { get { return _sentMBs; } }




        //Events
        public event EventHandler NewDataEvent;



        public DownUploadController()
        {
            _timer.Interval = new TimeSpan(0,0,1); //1 second
            _timer.Tick += TimerTickEvent;
            _timer.Start();
            TimerTickEvent(null, EventArgs.Empty);//call ones on Start

            MyLogger.Instance().Debug("Starting read received and send data");
        }


        private void TimerTickEvent(object sender, EventArgs e)
        {

            if (!InternetConnectionExist)
            {
                NewDataEvent?.Invoke(this, EventArgs.Empty);
                return;
            }
            

            SetSentAndReceivedData();


            //if Firstscan set Old Data
            if (_maxReceivedBytesOld == 0 & _maxSentBytesOld == 0)
            {
                _maxReceivedBytesOld = _maxReceivedBytesNew;
                _maxSentBytesOld = _maxSentBytesNew;
                return;
            }



            //Calculate Data
            _receivedBytesDelta = _maxReceivedBytesNew  - _maxReceivedBytesOld;
            _sentBytesDelta     = _maxSentBytesNew      - _maxSentBytesOld;


            //Convert to MB/s
            _receivedMBs = BytesToMB(_receivedBytesDelta);   
            _sentMBs = BytesToMB(_sentBytesDelta); 


            //Set MaxOld
            _maxReceivedBytesOld = _maxReceivedBytesNew;
            _maxSentBytesOld = _maxSentBytesNew;
        
            NewDataEvent?.Invoke(this, EventArgs.Empty);


        }

        private double BytesToMB(long value)
        {
            double dValue = (double)value;
            return Math.Round(((dValue / 1024.0) / 1024.0), 1 ); // (maxReceived / 1024) /1024 = MB/s
        }

        private void SetSentAndReceivedData()
        {
            if (InternetInterfaces.Length == 0) return;

            //Get Sent and Received Bytes
            foreach (NetworkInterface ni in InternetInterfaces)
            {
                long bytesReceived = ni.GetIPStatistics().BytesReceived;
                long bytesSent = ni.GetIPStatistics().BytesSent;

                if (bytesReceived > _maxReceivedBytesNew) _maxReceivedBytesNew = bytesReceived;
                if (bytesSent > _maxSentBytesNew) _maxSentBytesNew = bytesSent;
            }

            //If maxReceived or maxSend was reseted from the pc, Reset the other variables
            if (_maxReceivedBytesNew == 0 || _maxSentBytesNew == 0 && (_maxReceivedBytesOld != 0 || _maxSentBytesOld != 0))
            {
                _maxReceivedBytesOld = 0;
                _maxSentBytesOld = 0;
            }


        }



        private NetworkInterface[] GetInternetConnections()
        {
            try
            {
                return NetworkInterface.GetAllNetworkInterfaces();
            }
            catch (Exception e)
            {

                MyLogger.Instance().Error("GetAllNetworkInterfaces. Exception " + e.Message);
                return null;
            }
        }




        ~DownUploadController()
        {
            _timer.Stop();
        }




    }
}
