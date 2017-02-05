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
using System.IO;

namespace StudyLockerService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class StudyLockerService : ServiceBase, StudyLockerProtocol.IStudyLockerWCF
    {
        ManagementEventWatcher processStartWatch;

        ServiceHost wcfHost;

        ProgramList activeList = new ProgramList();
        WebsiteList activeWebsites = new WebsiteList();

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

        public WebsiteList SetWebsiteList(WebsiteList list)
        {
            eventLog1.WriteEntry("Received websites: " + string.Join(",", list.Sites));
            using (StreamReader hostsFile = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\Drivers\etc\hosts"))
            {
                using (StreamWriter newHostsFile = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\Drivers\etc\hosts_new"))
                {
                    bool studyLocker = false;
                    while (true)
                    {
                        string line = hostsFile.ReadLine();

                        if (line == null)
                        {
                            break;
                        }
                        else
                        {
                            if (line == "# STUDY LOCKER START")
                                studyLocker = true;
                            else if (line == "# STUDY LOCKER END")
                                studyLocker = false;
                            else
                            {
                                if (!studyLocker)
                                    newHostsFile.WriteLine(line);
                            }
                        }
                    }
                    newHostsFile.WriteLine("# STUDY LOCKER START");
                    foreach (WebsiteSpec site in list.Sites)
                    {
                        newHostsFile.WriteLine("0.0.0.0 " + site.Website);
                    }
                    newHostsFile.WriteLine("# STUDY LOCKER END");
                }
            }
            System.IO.File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\Drivers\etc\hosts_new", Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\Drivers\etc\hosts", true);
            this.activeWebsites = list;
            return list;

        }

        public WebsiteList GetWebsiteList()
        {
            List<WebsiteSpec> websiteList = new List<WebsiteSpec>();
            using (StreamReader hostsFile = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\Drivers\etc\hosts"))
            {
                bool studyLocker = false;
                while (true)
                {
                    string line = hostsFile.ReadLine();

                    if (line == null)
                    {
                        break;
                    }
                    else
                    {
                        if (line == "# STUDY LOCKER START")
                            studyLocker = true;
                        else if (line == "# STUDY LOCKER END")
                            studyLocker = false;
                        else if (studyLocker)
                        {
                            string[] lineSplit = line.Split(new string[] { " " }, 3, StringSplitOptions.RemoveEmptyEntries);
                            if (!lineSplit[0].StartsWith("#"))
                            {
                                websiteList.Add(new WebsiteSpec(lineSplit[1]));
                            }
                        }
                    }
                }
                
            }
            this.activeWebsites = new WebsiteList(websiteList);
            eventLog1.WriteEntry("Sending website list: " + string.Join(",", websiteList));
            return new WebsiteList(websiteList);
        }
    }
}
