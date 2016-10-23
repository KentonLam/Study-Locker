using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyLockerProtocol
{
    public class ProgramSpec
    {


        public string FileName { get; set; }

        public override string ToString()
        {
            return FileName;
        }
    }
}
