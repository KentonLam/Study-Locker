using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ProcessWatcher
{
    class Program
    {


        static void Main(string[] args)
        {
            var a = new Watcher();
            a.WaitForProcess();
            Console.ReadLine();
        }
        }
    class Watcher { 
        ManagementEventWatcher startWatch;
        ManagementEventWatcher stopWatch;
        public void WaitForProcess()
        {
            ManagementEventWatcher startWatch = new ManagementEventWatcher(
              new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            startWatch.EventArrived
                                += new EventArrivedEventHandler(startWatch_EventArrived);
            startWatch.Start();

            ManagementEventWatcher stopWatch = new ManagementEventWatcher(
              new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            stopWatch.EventArrived
                                += new EventArrivedEventHandler(stopWatch_EventArrived);
            stopWatch.Start();
        }

         void stopWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            //stopWatch.Stop();
            Console.WriteLine("Process stopped: {0}"
                              , e.NewEvent.Properties["ProcessName"].Value);
        }

        void startWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            //startWatch.Stop();
            Console.WriteLine("Process started: {0}"
                              , e.NewEvent.Properties["ProcessName"].Value);
        }
    }
}

