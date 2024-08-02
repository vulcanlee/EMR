using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models.Mcs;

public class UserDeviceRequest
{
    public string Scheme { get; set; } = "cgappsign";
    public string Operation { get; set; } = "getuserdevice";
    public string Account { get; set; } = "exentric";
    public string HostialCode { get; set; } = "70400845";
    public string UrlSchemes { get; set; } = "exentricmcsapp";
}
