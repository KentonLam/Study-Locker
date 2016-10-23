using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StudyLockerApp
{
    public class ServiceCommunication
    {
        public StudyLockerProtocol.IStudyLockerWCF PipeProxy { get; }

        public ServiceCommunication()
        {
            ChannelFactory<StudyLockerProtocol.IStudyLockerWCF> pipeFactory = new ChannelFactory<StudyLockerProtocol.IStudyLockerWCF>(
                new NetNamedPipeBinding(),
                new EndpointAddress("net.pipe://localhost/StudyLocker"));

            PipeProxy = pipeFactory.CreateChannel();
        }

        public StudyLockerProtocol.ProgramList SetProgramList(StudyLockerProtocol.ProgramList prog)
        {
            return PipeProxy.SetProgramList(prog);
        }

    }
}
