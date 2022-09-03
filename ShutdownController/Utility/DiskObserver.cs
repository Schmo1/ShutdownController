﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Linq;

namespace ShutdownController.Utility
{
    internal class DiskObserver
    {

        private string _machineName = Environment.MachineName;
        private DispatcherTimer _timer = new DispatcherTimer();
        private DriverConnection _connection = new DriverConnection();
        private string _selectedDisk;

        private List<string> _disks = new List<string>();
        private PerformanceCounter _nicReadCounter = new PerformanceCounter();
        private PerformanceCounter _nicWriteCounter = new PerformanceCounter();



        private string[] PerformanceCounterDiskNames { get; set; }


        public double ReadValue { get; private set; }
        public double WriteValue { get; private set; }

        public string SelectedDisk { get { return _selectedDisk; } set { _selectedDisk = value; UpdatePerformanceCounter(); } }
        public List<string> Disks { get => _disks; }


        public event EventHandler NewReadWriteData;
        public event EventHandler DisksAreUpdated;


        public DiskObserver()
        {
            _connection.DriveRemovedOrConnectedEvent += DriveHasChangedEvent;
            _timer.Interval = new TimeSpan(0, 0, 1); //1 second
            _timer.Tick += TimerTickEvent;
            _timer.Start();
            TimerTickEvent(null, EventArgs.Empty);//call ones on Start
            UpdatePhyisicaDisks();


        }



        private void TimerTickEvent(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedDisk))
                return;


            ReadValue = BytesToMB(_nicReadCounter.NextValue());
            WriteValue = BytesToMB(_nicWriteCounter.NextValue());


            NewReadWriteData(this, EventArgs.Empty);
        }

        private void DriveHasChangedEvent(object sender, EventArgs e)
        {
            UpdatePhyisicaDisks();
            DisksAreUpdated(null, EventArgs.Empty);
        }


        private void UpdatePhyisicaDisks()
        {

            try
            {
                PerformanceCounterCategory performanceCounterCategory = new PerformanceCounterCategory("PhysicalDisk");

                _disks.Clear();

                PerformanceCounterDiskNames = performanceCounterCategory.GetInstanceNames();

                _disks = CreateFullDiskName(PerformanceCounterDiskNames);

    
            }

            catch (Exception e)
            {

                MyLogger.Instance().Error("Error Get Physical Disk. Exception: " + e.Message);
                throw;
            }

           
            UpdatePerformanceCounter();

        }

        private List<string> CreateFullDiskName(string[] counterCategoryInstanceNames)
        {
            if (counterCategoryInstanceNames == null || counterCategoryInstanceNames.Length == 0)
                return null;

            List<string> finalDiskNamestoReturn = new List<string>();

            try
            {

                DriveInfo[] allDrives = DriveInfo.GetDrives();


                foreach (string counterCategoryInstanceName in counterCategoryInstanceNames)
                {
                    foreach (DriveInfo driveInfo in allDrives)
                    {
                        if(counterCategoryInstanceName.Contains(driveInfo.Name.Substring(0,2))) //delete Backshlashes
                            finalDiskNamestoReturn.Add(driveInfo.Name + " " + driveInfo.DriveType);
                    }
                }
            }
            catch (Exception e)
            {
                MyLogger.Instance().Error("Exception on create Full Disk " + e.Message);
            }

            finalDiskNamestoReturn.Sort(); //Sort list

            return finalDiskNamestoReturn;

        }

        private void UpdatePerformanceCounter()
        {

            if (_disks.Count == 0 || PerformanceCounterDiskNames == null || PerformanceCounterDiskNames.Length == 0)
                return;

            if (string.IsNullOrEmpty(SelectedDisk))
                return;

            try
            {
                foreach (string disk in _disks)
                {
                    if (SelectedDisk == disk)
                    {
                        _nicReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", GetPerformanceCounterDiskName(disk), _machineName);
                        _nicWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", GetPerformanceCounterDiskName(disk), _machineName);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MyLogger.Instance().Error("Error on Update Counters. Exception: " + e.Message);
                throw;
            }

        }

        private string GetPerformanceCounterDiskName(string disk)
        {
            foreach (string performanceCounterDiskName in PerformanceCounterDiskNames)
            {
                if (performanceCounterDiskName.Contains(disk.Substring(0, 2)))
                        return performanceCounterDiskName;
            }

            return null;
        }


        private double BytesToMB(float value)
        {
            try
            {
                value = value / 1024 / 1024;
                return Math.Round(value, 1); // (maxReceived / 1024) /1024 = MB/s

            }
            catch (Exception e)
            {
                MyLogger.Instance().Error("Could not round Bytes to MB. Exception: " + e.Message);
                throw;
            }
        }


        ~DiskObserver()
        {
            _timer.Stop();
        }
    }
}