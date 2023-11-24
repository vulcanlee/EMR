using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models
{
    public partial class StopSignViewModel:ObservableObject
    {
       public IRelayCommand CancelCommand { get; set; }
       public IRelayCommand StopCommand { get; set; }
    }
}
