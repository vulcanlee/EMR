using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReplicaEmrApp.Models
{
    public class CurrentDeviceInformation
    {
        public string Account { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string MauiVersion { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string VersionString { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string Idiom { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;

    }
}
