using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReplicaEmrApp.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models
{
    public partial class SignProcessingViewModel : ObservableObject
    {
        [ObservableProperty]
        public IEventAggregator eventAggregator;
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

        public IRelayCommand StopViewModelCommand { get; set; }
        //[RelayCommand]
        //public void Stop()
        //{
        //    EventAggregator.GetEvent<StopSignEvent>().Publish(new StopSignPayload());
        //}
    }
}
