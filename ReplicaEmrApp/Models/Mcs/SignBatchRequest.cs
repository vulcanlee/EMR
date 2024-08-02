using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models.Mcs;

public class SignBatchRequest : ICloneable
{
    public string Scheme { get; set; } = "cgappsign";
    public string Operation { get; set; } = "batchsign";
    public string Account { get; set; } = "exentric";
    public string Password { get; set; } = "70400845";
    public string HashFlag { get; set; } = "1";
    public string HostialCode { get; set; } = "70400845";
    public string Data { get; set; } = string.Empty;

    public object Clone()
    {
        var cloneObject = this.MemberwiseClone() as SignBatchRequest;
        cloneObject.Password = string.Empty;
        cloneObject.Data = string.Empty;

        return cloneObject;
    }
}
