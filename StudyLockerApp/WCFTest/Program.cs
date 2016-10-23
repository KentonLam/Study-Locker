using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<StudyLockerWCF.IStudyLockerWCF> pipeFactory = new ChannelFactory<StudyLockerWCF.IStudyLockerWCF>(
                new NetNamedPipeBinding(),
                new EndpointAddress("net.pipe://localhost"));

            StudyLockerWCF.IStudyLockerWCF pipeProxy = pipeFactory.CreateChannel();

            while (true)
            {
                Console.Write("\n: ");
                Console.WriteLine(pipeProxy.DecimalTest(new Decimal(Convert.ToInt32(Console.ReadLine()))));

            }
        }
    }
}
