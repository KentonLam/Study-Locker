using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyLockerProtocol
{
    public class ProgramList
    {
        public List<ProgramSpec> Programs { get; set; }

        public ProgramList()
        {
            this.Programs = new List<ProgramSpec>();
        }
    }
}
