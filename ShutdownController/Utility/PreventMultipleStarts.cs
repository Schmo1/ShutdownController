using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Windows;

namespace ShutdownController.Utility
{
    internal class PreventMultipleStarts
    {

        private static Mutex mutex;
        private static Thread thReciver;
        private static bool _firstInstance;

        private const string GUITextSend = "OpenGUI";


        public event EventHandler OnOpenRequest;


        public PreventMultipleStarts()
        {

            mutex = new Mutex(false, Application.ResourceAssembly.GetName().Name + "BySchmo", out _firstInstance);

            thReciver = new Thread(ReciveRequestForGUI);
            

        }



        private void ReciveRequestForGUI()
        {
            MyLogger.Instance().Debug("Start listening for GUI Request");
            //if instance of the programm has started, this instance will open the GUI and the other instance closes itself.
            using (var mmf = MemoryMappedFile.CreateOrOpen("ShowGuiMapName", 1024))
            using (var view = mmf.CreateViewStream())
            {
                BinaryReader reader = new BinaryReader(view);
                EventWaitHandle signal = new EventWaitHandle(false, EventResetMode.AutoReset, "ListenForOpenGUI");


                while (true)
                {
                    signal.WaitOne();
                    mutex.WaitOne();
                    reader.BaseStream.Position = 0;
                    if (reader.ReadString() == GUITextSend)
                    {
                        MyLogger.Instance().Debug("Other application was found ==> Open GUI");
                        OnOpenRequest?.Invoke(this, EventArgs.Empty); //Event
                    }
                    mutex.ReleaseMutex();
                }
            }
        }

        public bool IsFirstInstance()
        {
            try
            {
                if (!_firstInstance)
                {
                    //Send request to 
                    SendGUIRequest();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MyLogger.Instance().Error("AppController IsFirstInstance. Exception: " + ex.Message);
            }

            return true;
        }

        public void SendGUIRequest()
        {
            MyLogger.Instance().Debug("Send GUI request");
            try
            {
                using (MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen("ShowGuiMapName", 1024))
                using (var view = mmf.CreateViewStream())
                {
                    BinaryWriter writer = new BinaryWriter(view);
                    EventWaitHandle signal = new EventWaitHandle(false, EventResetMode.AutoReset, "ListenForOpenGUI");


                    mutex.WaitOne();
                    writer.BaseStream.Position = 0;
                    writer.Write(GUITextSend);
                    signal.Set();
                    mutex.ReleaseMutex();

                    Thread.Sleep(1000);

                }
            }
            catch (Exception ex)
            {
                MyLogger.Instance().Error("Could not Send Gui request. Exception: " + ex.Message);
                
            }

        }


        public void StartListening()
        {
            thReciver.Start();
        }

        public void StopListening()
        {
            thReciver.Abort();
        }

        ~PreventMultipleStarts()
        {
            thReciver.Abort();
        }
    }
}
