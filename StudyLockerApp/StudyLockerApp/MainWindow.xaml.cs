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
    public partial class MainWindow : Window
    {
        public StudyLockerProtocol.ProgramList programList { get; set; }

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
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Join("|", programList.Programs));

            this.programList.Programs = this.programList.Programs.Where(a => !string.IsNullOrWhiteSpace(a.FileName)).ToList();

            this.programList = comm.PipeProxy.SetProgramList(this.programList);
            this.dataGrid.ItemsSource = programList.Programs;

            this.dataGrid.Items.Refresh();
        }
    }
}
