using StudyLockerWCF;
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

            List < string> test = new List<string>();
            while (true)
            {
                Console.Write(": ");
                test.Add(Console.ReadLine());
                if (test.Last() == "end")
                {
                    Console.WriteLine(pipeProxy.SetProgramList(new ProgramList() { Programs = test }));
                    Console.WriteLine(string.Join("|", test));
                    test.Clear();
                }
            }
        }
    }
}
