using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models
{
    public partial class UnSignItem : ObservableObject
    {
        public string ReportCode { get; set; }
        public string ReportName { get; set; }
        [ObservableProperty]
        int totalReport = 0; 
    }
}
