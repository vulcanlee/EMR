using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Dto;

public class SignatureAddRequestDto : ICloneable
{
    public string DocId { get; set; }
    public string DocsnId { get; set; }
    public string SignatureValue { get; set; }
    public string CertificateData { get; set; }
    public int CertFrom { get; set; }
    public string BasicId { get; set; }
    public string CardSn { get; set; }
    public int OperatorType { get; set; }
    public int ReadType { get; set; }

    public object Clone()
    {
        var cloneObject = this.MemberwiseClone() as SignatureAddRequestDto;
        cloneObject.SignatureValue = string.Empty;
        cloneObject.CertificateData = string.Empty;

        return cloneObject;
    }
}


