using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models
{
    public class GlobalObject
    {
        public string JSESSIONID  { get; set; }
        public string UserId  { get; set; }
        public string Version { get; set; }
        public string UserName  { get; set; }
        public List<ReportCodeNode> reportCodes { get; set; } = new();

        public void Copy(GlobalObject source,GlobalObject destination)
        {
            destination.JSESSIONID = source.JSESSIONID;
            destination.UserId = source.UserId;
            destination.Version = source.Version;
            destination.UserName = source.UserName;
            destination.reportCodes = source.reportCodes;
        }

        public void CleanUp()
        {
            JSESSIONID = null;
            UserId = null;
            UserName = null;
            reportCodes.Clear();
        }
    }
}
