using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models.Mcs;

public class GetCertRequest : ICloneable
{
    public string Scheme { get; set; } = "cgappgetcert";
    public string Operation { get; set; } = "getcert";
    public string Account { get; set; } = "exentric";
    public string Password { get; set; } = "70400845";
    public string HostialCode { get; set; } = "70400845";
    public string UrlSchemes { get; set; } = "exentricmcsapp";

    public object Clone()
    {
        var cloneObject = this.MemberwiseClone() as GetCertRequest;
        cloneObject.Password = string.Empty;

        return cloneObject;
    }
}
