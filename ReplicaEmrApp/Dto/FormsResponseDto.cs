using ReplicaEmrApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Dto
{
    public class FormsResponseDto
    {
        public string msg { get; set; }
        public string code { get; set; }
        public List<ReportCodeNode> data { get; set; }
    }
}
