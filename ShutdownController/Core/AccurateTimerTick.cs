using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ShutdownController.Core
{
    internal class AccurateTimerTick
    {
        private static List<AccurateTimerTick> accurateTimerTicks = new List<AccurateTimerTick>();

        private Thread _thTimer;
        private TimeSpan _interval;

        public bool IsActive { get; private set; }
        public event EventHandler Tick;


        public AccurateTimerTick(TimeSpan interval) 
        {
            _interval = interval;
            accurateTimerTicks.Add(this); 
        }
            

        public void Start() 
        {
            
            _thTimer = new Thread(Loop);
            _thTimer.Name = GetType().FullName;
            IsActive = true;
            _thTimer.Start();
             
        }



        public void Stop() 
        {
            // Signal the shutdown event
            IsActive = false;

             // Wait for the thread to exit
            //_thTimer.Join();
        }

        private void Loop()
        {
            var interval = _interval;
            var nextTick = DateTime.Now + interval;
            while (IsActive)
            {

                while (DateTime.Now < nextTick && IsActive)
                {
                    var sleepTime = nextTick - DateTime.Now;
                    if(!(sleepTime < new TimeSpan(0,0,0)))
                        Thread.Sleep(sleepTime);
                }
                nextTick += interval; // Notice we're adding onto when the last tick  
                                      // was supposed to be, not when it is now.
                                      // Insert tick() code here
                if (IsActive)
                    Tick(this, EventArgs.Empty);
            }

        }

        public static void DisposeAll() 
        { 
            accurateTimerTicks.ForEach((timer) => timer.Dispose()); 
            accurateTimerTicks.Clear();
        }

        public void Dispose([CallerMemberName] string nameOfCaller = null) 
        { 
            IsActive = false; 

        } 
        

        ~AccurateTimerTick() { Dispose(); }

    }
}
