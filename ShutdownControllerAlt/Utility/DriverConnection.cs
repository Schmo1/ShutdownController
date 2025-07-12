using System;
using System.Management;

namespace ShutdownController.Utility
{

    public class DriverConnection:IDisposable
    {
        ManagementEventWatcher watcher;

        public event EventHandler DriveRemovedOrConnectedEvent;



        public DriverConnection()
        {
            watcher = new ManagementEventWatcher();
            
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2 OR EventType = 3");
            watcher.EventArrived += new EventArrivedEventHandler(RiseEvent);
            watcher.Query = query;
            watcher.Start();

        }

        private void RiseEvent(object sender, EventArgs e)
        {
            DriveRemovedOrConnectedEvent(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            watcher.Stop();
        }
    }
}
