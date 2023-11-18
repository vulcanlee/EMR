using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models
{
    public partial class RefreshReportStatusViewModel : ObservableObject
    {
        [ObservableProperty]
        string title = string.Empty;
        [ObservableProperty]
        string subTitle1 = string.Empty;
        [ObservableProperty]
        string subTitle2 = string.Empty;
        [ObservableProperty]
        string message = string.Empty;
        [ObservableProperty]
        double progress = 0;
    }
}
