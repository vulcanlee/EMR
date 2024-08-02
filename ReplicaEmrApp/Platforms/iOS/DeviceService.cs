using ReplicaEmrApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace ReplicaEmrApp.Platforms.iOS
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceInfo deviceInfo;

        public DeviceService(IDeviceInfo deviceInfo)
        {
            this.deviceInfo = deviceInfo;
        }

        public string GetDeviceId()
        {
            //await Task.Yield();
            return UIDevice.CurrentDevice.IdentifierForVendor.AsString();
        }
        //TODO:改成同步
        public Task<string> GetDeviceIpAsync()
        {
            string GetIpAddress()
            {
                string ipAddress = string.Empty;
                var networkInterfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();

                foreach (var netInterface in networkInterfaces)
                {
                    if (netInterface.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                    {
                        var properties = netInterface.GetIPProperties();
                        foreach (var unicastAddress in properties.UnicastAddresses)
                        {
                            if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                ipAddress = unicastAddress.Address.ToString();
                            }
                        }
                    }
                }
                return ipAddress;
            }

            return Task.FromResult(GetIpAddress());
        }

        public Task<string> GetDevicePlatformAsync() 
        {
            return Task.FromResult(deviceInfo.Platform.ToString());
        }

        public void KeepScreenOn()
        {
            UIApplication.SharedApplication.IdleTimerDisabled = true;
        }

        public void AllowScreenSleep()
        {
            UIApplication.SharedApplication.IdleTimerDisabled = false;
        }
    }
}
