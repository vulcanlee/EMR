using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Dto;

public class CheckUploadRequestDto
{
    public string CertData { get; set; }
    public int CertFrom { get; set; }
    public string BasicId { get; set; }
}


