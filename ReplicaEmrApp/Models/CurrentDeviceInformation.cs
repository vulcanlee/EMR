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

        public void Clear()
        {
            Account = string.Empty;
            Password = string.Empty;
            MauiVersion = string.Empty;
            DeviceId = string.Empty;
            DeviceName = string.Empty;
            Model = string.Empty;
            Manufacturer = string.Empty;
            VersionString = string.Empty;
            Platform = string.Empty;
            Idiom = string.Empty;
            DeviceType = string.Empty;

            MauiVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            DeviceName = DeviceInfo.Current.Name;
            Model = DeviceInfo.Current.Model;
            Manufacturer = DeviceInfo.Current.Manufacturer;
            VersionString = DeviceInfo.Current.VersionString;
            Platform = DeviceInfo.Current.Platform.ToString();
            Idiom = DeviceInfo.Current.Idiom.ToString();
            DeviceType = DeviceInfo.Current.DeviceType.ToString();
        }
    }
}
