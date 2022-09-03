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
        private readonly string _machineName = Environment.MachineName;
        private bool _timerTickIsInUse;


        //Properties
        public bool InternetConnectionExist { get { return NetworkInterface.GetIsNetworkAvailable(); } }

        public double ReceivedMBs { get { return _receivedMBs; } }
        public double SentMBs { get { return _sentMBs; } }

        public string[] NetworkInterfaces { get; private set; }
        public string SelectedNetworkInterface { get; set; }

        public bool NoInterfaceSelected { get; private set; }




        //Events
        public event EventHandler NewDownloadUploadData;
        public event EventHandler NetworkinterfaceHasChanged;



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
            if (_timerTickIsInUse)
                return;

            _timerTickIsInUse = true;

            //Check if internet connection exists
            if (!InternetConnectionExist)
            {
                NewDownloadUploadData?.Invoke(this, EventArgs.Empty); //Trigger Event for upper class
                return;
            }

            //Update all 10s the Interfaces
            if(_timerTicks % 30 == 0)
            {
                UpdateNetworkInterfaces();
            }


            //Convert to MB/s
            _receivedMBs = BytesToMB(GetBytes(_nicReceivedCounters));   
            _sentMBs = BytesToMB(GetBytes(_nicSentCounters)); 

        
            NewDownloadUploadData?.Invoke(this, EventArgs.Empty);

            _timerTicks++;
            _timerTickIsInUse = false;

        }

        private double BytesToMB(float value)
        {
            try
            {
                value = value / 1024 / 1024;
                return Math.Round(value, 1 ); // (maxReceived / 1024) /1024 = MB/s

            }
            catch (Exception e)
            {
                MyLogger.Instance().Error("Could not round Bytes to MB. Exception: " + e.Message);
                throw;
            }
        }


        private void UpdateNetworkInterfaces()
        {
            string[] networkInterfaces = GetNetworkInterfaces(); //Added to save performance
            if (NetworkInterfaces == networkInterfaces)
                return;

            NetworkInterfaces = networkInterfaces;

            _nicReceivedCounters.Clear();
            _nicSentCounters.Clear();

            foreach (string networkInterface in NetworkInterfaces)
            {
                _nicReceivedCounters.Add(new PerformanceCounter("Network Interface", "Bytes Received/sec", networkInterface, _machineName));
                _nicSentCounters.Add(new PerformanceCounter("Network Interface", "Bytes Sent/sec", networkInterface, _machineName));
            }

            NetworkinterfaceHasChanged?.Invoke(this, EventArgs.Empty);

        }

        private float GetBytes(List<PerformanceCounter> _nicCounters)
        {
            try
            {
                foreach (PerformanceCounter pfCounter in _nicCounters)
                {
                    if (SelectedNetworkInterface == pfCounter.InstanceName)
                    {
                        NoInterfaceSelected = false;
                        return pfCounter.NextValue();
                    }
                }

            }
            catch (Exception e)
            {

                throw;
            }
            NoInterfaceSelected = true;
            return 0;
            
        }

        private string[] GetNetworkInterfaces()
        {
            PerformanceCounterCategory performanceCounterCategory = new PerformanceCounterCategory("Network Interface", _machineName);


            //List<string> result = new List<string>();
            //foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            //{

            //    result.Add(item.Description);
            //}

            //return result.ToArray();
  



            return performanceCounterCategory.GetInstanceNames();

        }



        ~DownUploadController()
        {
            _timer.Stop();
        }


    }
}
