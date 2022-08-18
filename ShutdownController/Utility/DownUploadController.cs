using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows.Threading;

namespace ShutdownController.Utility
{
    public class DownUploadController
    {

        //Variables
        private DispatcherTimer _timer = new DispatcherTimer();
        private int _timerTicks;
        private double _receivedMBs;
        private double _sentMBs;
        private List<PerformanceCounter> _nicReceivedCounters = new List<PerformanceCounter>();
        private List<PerformanceCounter> _nicSentCounters = new List<PerformanceCounter>();
        private readonly string MachineName = Environment.MachineName;


        //Properties
        public bool InternetConnectionExist { get { return NetworkInterface.GetIsNetworkAvailable(); } }
        
        public double ReceivedMBs { get { return _receivedMBs; } }
        public double SentMBs { get { return _sentMBs; } }

        public string[] NetworkInterfaces { get; private set; }




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
            
            //Check if internet connection exists
            if (!InternetConnectionExist)
            {
                NewDataEvent?.Invoke(this, EventArgs.Empty); //Trigger Event for upper class
                return;
            }

            //Update all 10s the Interfaces
            if(_timerTicks % 10 == 0)
            {
                UpdateNetworkInterfaces();
            }


            //Convert to MB/s
            _receivedMBs = BytesToMB(GetBytes(_nicReceivedCounters));   
            _sentMBs = BytesToMB(GetBytes(_nicSentCounters)); 

        
            NewDataEvent?.Invoke(this, EventArgs.Empty);

            _timerTicks++;
        }

        private double BytesToMB(float value)
        {
            value = value / 1024 / 1024;
            return Math.Round(value, 1 ); // (maxReceived / 1024) /1024 = MB/s
        }


        private void UpdateNetworkInterfaces()
        {
            if (NetworkInterfaces == GetNetworkInterfaces())
                return;

            NetworkInterfaces = GetNetworkInterfaces();

            foreach (string networkInterface in NetworkInterfaces)
            {
                _nicReceivedCounters.Add(new PerformanceCounter("Network Interface", "Bytes Received/sec", networkInterface, MachineName));
                _nicSentCounters.Add(new PerformanceCounter("Network Interface", "Bytes Sent/sec", networkInterface, MachineName));
            }

        }

        private float GetBytes(List<PerformanceCounter> _nicCounters)
        {
            float bytes = 0;
            foreach (PerformanceCounter pfCounter in _nicCounters)
            {
                float tempReceived = pfCounter.NextValue();
                if (tempReceived > bytes)
                    bytes = tempReceived;
            }

            return bytes;
        }

        private string[] GetNetworkInterfaces()
        {
            PerformanceCounterCategory performanceCounterCategory = new PerformanceCounterCategory("Network Interface");
            return performanceCounterCategory.GetInstanceNames();
        }



        ~DownUploadController()
        {
            _timer.Stop();
        }


    }
}
