
using StudyLockerProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StudyLockerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public StudyLockerProtocol.ProgramList programList { get; set; }
        public WebsiteList websiteList { get; set; }

        ServiceCommunication comm = new ServiceCommunication();

        public MainWindow()
        {
            InitializeComponent();

            bool exceptionCaught = true;
            while (exceptionCaught)
            {
                exceptionCaught = false;
                try
                {
                    this.programList = comm.PipeProxy.GetProgramList();
                    //this.websiteList = new List<WebsiteSpec>();
                    this.websiteList = comm.PipeProxy.GetWebsiteList();
                }
                catch (EndpointNotFoundException e)
                {
                    MessageBox.Show("Could not connect to service. Program will now crash.\n\n"+
                        e.ToString(), "Study Locker Init Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    exceptionCaught = true;
                }
            }

            

            //this.programList.Programs.Add(new StudyLockerProtocol.ProgramSpec() { FileName = "Notepad.exe" });
            this.dataGrid.CanUserAddRows = true;
            this.dataGrid.ItemsSource = programList.Programs;

            
            
            this.dataGrid1.ItemsSource = this.websiteList.Sites;
            this.dataGrid1.CanUserAddRows = true;
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Sending programs list: \n\n" + string.Join("|", programList.Programs));

            this.programList.Programs = this.programList.Programs.Where(a => !string.IsNullOrWhiteSpace(a.FileName)).ToList();

            this.programList = comm.PipeProxy.SetProgramList(this.programList);

            string message = "Now blocking programs";
            if (this.programList.Programs.Count == 0)
                message = "All programs unblocked";
            //await this.ShowMessageAsync(message, string.Join(", ", programList.Programs));

            this.dataGrid.ItemsSource = programList.Programs;



            this.dataGrid.Items.Refresh();
        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Sending website list: \n\n" + string.Join("|", this.websiteList.Sites));
            string message = "Now blocking websites";
            if (this.programList.Programs.Count == 0)
                message = "All websites unblocked";
            //await this.ShowMessageAsync(message, string.Join(", ", this.websiteList.Sites));

            this.comm.PipeProxy.SetWebsiteList(this.websiteList);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.websiteList.Sites.Add(new WebsiteSpec(""));
            this.dataGrid1.Items.Refresh();
            
        }
    }
}
