using CommunityToolkit.Mvvm.ComponentModel;
using ReplicaEmrApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models
{
    public class AppSettings
    {
        public string EmrService { get; set; }
        public string TenantCode { get; set; }
        public string AppHospName { get; set; }
        public string UnsingListName { get; set; }
    }
}
