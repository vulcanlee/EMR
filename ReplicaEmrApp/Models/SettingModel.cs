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
        string endPoint;
        [ObservableProperty]
        string intervalSeconds;
        [ObservableProperty]
        string hid ;
        [ObservableProperty]
        string name;
        [ObservableProperty]
        string reportListName;
        [ObservableProperty]
        bool engineerMode;
    }
}
