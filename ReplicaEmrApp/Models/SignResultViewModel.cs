using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReplicaEmrApp.Events;
using ReplicaEmrApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models
{
    public partial class SignResultViewModel : ObservableObject
    {
        [ObservableProperty]
        public IEventAggregator eventAggregator;
        [ObservableProperty]
        string title = "簽章完成";
        [ObservableProperty]
        int total = 0;
        [ObservableProperty]
        int successCount = 0;
        [ObservableProperty]
        int failCount = 0;
        [ObservableProperty]
        string message = string.Empty;

        [ObservableProperty]
        Color buttonColor;
        [ObservableProperty]
        string buttonText = string.Empty;

        public IRelayCommand StopAutoSignButtonAsyncCommand { get; set; }
    }
}
