using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using StudyLockerWCF;
using System.ServiceModel;

namespace StudyLockerService
{
    public partial class StudyLockerService : ServiceBase, StudyLockerWCF.IStudyLockerWCF
    {
        ManagementEventWatcher processStartWatch;

        ServiceHost wcfHost;

        public StudyLockerService()
        {
            InitializeComponent();
            this.eventLog1 = new System.Diagnostics.EventLog();
            if (!EventLog.SourceExists("StudyLockerServiceBeta2"))
            {
                EventLog.CreateEventSource("StudyLockerServiceBeta2", "Study Locker");
            }
            eventLog1.Source = "StudyLockerServiceBeta2";
            eventLog1.Log = "Study Locker";

            processStartWatch = new ManagementEventWatcher(
                new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            processStartWatch.EventArrived += ProcessStartWatch_EventArrived;
            processStartWatch.Start();
            
        }

        private void ProcessStartWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            eventLog1.WriteEntry("Process started: " + e.NewEvent.Properties["ProcessName"].Value);
        }

        protected override void OnStart(string[] args)
        {
            if (this.wcfHost != null)
                this.wcfHost.Close();

            Uri pipeUri = new Uri("net.pipe://localhost");
            wcfHost = new ServiceHost(typeof(StudyLockerService), pipeUri);
            wcfHost.AddServiceEndpoint(typeof(StudyLockerWCF.IStudyLockerWCF), new NetNamedPipeBinding(), "");
            wcfHost.Open();


            eventLog1.WriteEntry("Started");
        }

        protected override void OnStop()
        {
            if (this.wcfHost != null)
                this.wcfHost.Close();
            eventLog1.WriteEntry("Stopped");
        }

        public string GetData(int value)
        {
            return "asadf: " + value.ToString();
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            throw new NotImplementedException();
        }

        public decimal DecimalTest(decimal value)
        {
            eventLog1.WriteEntry("Received: " + value.ToString());
            return value + new decimal(128);
        }
    }
    class Watcher
    {
        // http://stackoverflow.com/questions/967646/monitor-when-an-exe-is-launched
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
