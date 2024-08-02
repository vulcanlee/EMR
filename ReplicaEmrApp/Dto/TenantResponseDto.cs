using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Dto;

public class TenantResponseDto
{
    public string CreateBy { get; set; }
    public DateTime CreateTime { get; set; }
    public string UpdateBy { get; set; }
    public DateTime? UpdateTime { get; set; }
    public string Remark { get; set; }
    public string TenantId { get; set; }
    public string TenantCode { get; set; }
    public string TenantName { get; set; }
    public string Status { get; set; }
    public string DelFlag { get; set; }
}


