using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StudyLockerProtocol
{
    [DataContract]
    public class WebsiteList
    {
        [DataMember]
        public List<WebsiteSpec> Sites { get; set; }

        public WebsiteList()
        {
            this.Sites = new List<StudyLockerProtocol.WebsiteSpec>();
        }

        public WebsiteList(List<WebsiteSpec> list)
        {
            this.Sites = list;
        }
    }
}
