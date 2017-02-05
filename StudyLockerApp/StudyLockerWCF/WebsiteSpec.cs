using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StudyLockerProtocol
{
    [DataContract]
    public class WebsiteSpec
    {
        [DataMember]
        public string Website { get; set; }

        public WebsiteSpec()
        {
            this.Website = "";
        }

        public WebsiteSpec(string site)
        {
            this.Website = site;
        }

        public override string ToString()
        {
            return Website;
        }
    }
}
