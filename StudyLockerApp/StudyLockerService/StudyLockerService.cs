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
using StudyLockerProtocol;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace StudyLockerService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class StudyLockerService : ServiceBase, StudyLockerProtocol.IStudyLockerWCF
    {
        ManagementEventWatcher processStartWatch;

        ServiceHost wcfHost;

        ProgramList activeList = new ProgramList();

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
            
            string name = e.NewEvent.Properties["ProcessName"].Value.ToString();
            //eventLog1.WriteEntry("Process started: " + name);
            foreach (ProgramSpec blocked in this.activeList.Programs)
            {
                if (blocked.FileName.ToLowerInvariant() == name.ToLowerInvariant())
                {
                    Process proc = Process.GetProcessById(Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value));
                    proc.Kill();
                    eventLog1.WriteEntry("Killed: " + name);
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            if (this.wcfHost != null)
                this.wcfHost.Close();

            Uri pipeUri = new Uri("net.pipe://localhost");
            wcfHost = new ServiceHost(typeof(StudyLockerService), pipeUri);
            wcfHost.AddServiceEndpoint(typeof(StudyLockerProtocol.IStudyLockerWCF), new NetNamedPipeBinding(), "StudyLocker");
            
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            wcfHost.Description.Behaviors.Add(smb);

            wcfHost.Open();

            eventLog1.WriteEntry("Started");
        }

        protected override void OnStop()
        {
            if (this.wcfHost != null)
                this.wcfHost.Close();
            eventLog1.WriteEntry("Stopped");
        }

        ProgramList IStudyLockerWCF.SetProgramList(ProgramList list)
        {
            eventLog1.WriteEntry(string.Join(",", list.Programs), EventLogEntryType.Warning);
            if (list != null)
                this.activeList = list;
            return list;
        }

        ProgramList IStudyLockerWCF.GetProgramList()
        {
            eventLog1.WriteEntry("Sending list: " + string.Join(",", this.activeList.Programs));
            return this.activeList;
        }
    }
}
