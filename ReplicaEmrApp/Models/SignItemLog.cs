using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models
{
    public class SignItemLog
    {
        public DateTime CreateAt { get; set; }
        public DateTime FinishAt { get; set; }
        public string Title { get; set; }
        public double TotalSeconds { get; set; }
        public string TotalSecondsString { get; set; }

        public void Begin(string Title)
        {
            this.Title = Title;
            CreateAt = DateTime.Now;
        }

        public void End()
        {
            FinishAt = DateTime.Now;
            TotalSeconds = (FinishAt - CreateAt).TotalSeconds;
            TotalSecondsString = TotalSeconds.ToString("0.00");
        }
    }
}
