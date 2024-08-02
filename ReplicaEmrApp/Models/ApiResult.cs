using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models
{
    public class ApiResultModel<T>
    {
        public string msg { get; set; }
        public string code { get; set; }
        public string token { get; set; }
        public List<T> data { get; set; }
        public List<T> rows { get; set; }

        public bool IsSuccess()
        {
            return code == "200";
        }
    }
}
