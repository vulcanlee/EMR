using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models.Mcs;

public class UserDeviceResponse
{
    public string Code { get; set; }
    public string BasicId { get; set; }
    public string Message { get; set; }
}
public class UserDeviceModel
{
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string IdNumber { get; set; }
    public string HospitalCode { get; set; }
    public int? UserStatus { get; set; }
    public string CertSerialNumber { get; set; }
    public DateTime? CertNotBefore { get; set; }
    public DateTime? CertNotAfter { get; set; }
    public int? CertStatus { get; set; }
    public int? ReqStatus { get; set; }
    public DateTime CertLastusedTime { get; set; }
    public string DeviceUuid { get; set; }
    public int DeviceStatus { get; set; }
    public int ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
}
