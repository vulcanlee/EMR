using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models
{
    public class ConfigModel
    {
        public string createBy { get; set; }
        public string createTime { get; set; }
        public string updateBy { get; set; }
        public string updateTime { get; set; }
        public string remark { get; set; }
        public string configId { get; set; }
        public string configName { get; set; }
        public string configKey { get; set; }
        public string configValue { get; set; }
        public string configType { get; set; }
    }
}
