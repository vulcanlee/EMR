using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Dto;

public class ReportData
{
    public string uid { get; set; }
    public string reportId { get; set; }
    public string doctorid { get; set; }
    public string reportdate { get; set; }
    public string patname { get; set; }
    public string tableSource { get; set; }
    public string signType { get; set; }
    public string patno { get; set; }
    public string referno { get; set; }
    public string version { get; set; }
    public string doctorname { get; set; }

    public string patfullname
    {
        get
        {
            return $"{patno} {patname}";
        }
    }
    public string reportdateFormat
    {
        get
        {
            return $"{reportdate.Substring(0, 4)}-{reportdate.Substring(5, 2)}-{reportdate.Substring(8, 2)}";
        }
    }
}

public class ReturnMessage
{
    public string reportName { get; set; }
    public string reportCode { get; set; }
    public int reportCount { get; set; }
    public List<ReportData> reportDatas { get; set; } = new();
}

public class ReportDetailResponseDto
{
    public string returnCode { get; set; }
    public List<ReturnMessage> returnMessage { get; set; } = new();
}

