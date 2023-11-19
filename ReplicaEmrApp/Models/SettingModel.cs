using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models
{
    public partial class SettingModel : ObservableObject
    {
        [ObservableProperty]
         string endingPoint  = "http://office.exentric.com.tw:8080/webemr";
        [ObservableProperty]
         int intervalSeconds  = 600;
        [ObservableProperty]
        string hid  = "70400845";
        [ObservableProperty]
        string name  = "耀瑄科技";
        [ObservableProperty]
        string reportListName  = "未簽章報告";
    }
}
