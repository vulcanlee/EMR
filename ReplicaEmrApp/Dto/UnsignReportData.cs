using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Dto;

public class UnsignReportData
{
    public string id { get; set; }
    public string docId { get; set; }
    public int docVer { get; set; }
    public object unsnusId { get; set; }
    public string docsnId { get; set; }
    public string frmCode { get; set; }
    public string frmNm { get; set; }
    public string hosCode { get; set; }
    public string tenCode { get; set; }
    public string refNo { get; set; }
    public string patNo { get; set; }
    public string patNm { get; set; }
    public string patNid { get; set; }
    public string patFr { get; set; }
    public object userNm { get; set; }
    public object userNid { get; set; }
    public DateTime docTm { get; set; }
    public string caseNo { get; set; }
    public string depNo { get; set; }
    public string depNm { get; set; }
    public object unsnTp { get; set; }
    public string signedData { get; set; }
    public string digest { get; set; }


    public string patfullname
    {
        get
        {
            return $"{patNo} {patNm}";
        }
    }
    public string reportdateFormat
    {
        get
        {
            return $"{docTm.ToString("yyyy-MM-dd")}";
        }
    }
}


