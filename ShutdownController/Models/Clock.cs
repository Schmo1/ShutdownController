using System;
using System.Windows.Threading;

namespace ShutdownController.Models
{


    public class Clock
    {


        private static Clock instance = new Clock(); //singleton design pattern. singl instance of this class.

        private DateTime _actualTime;

        private DispatcherTimer _timer = new DispatcherTimer();


        public DateTime ActualTime { get { return _actualTime; } }


        public event EventHandler ClockTick;

        //exits int he programm, then send them the reference to the original.
        public static Clock Instance { get { return instance; } }


        private Clock()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += Update_Timer;
            _timer.Start();

            Update_Timer(null, EventArgs.Empty);
        }



        private void Update_Timer(object sender, EventArgs e)
        {
            _actualTime = DateTime.Now;
            ClockTick?.Invoke(this, EventArgs.Empty); //Event    
        }









    }
}
