using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net.Wifi;
using Android.OS;
using Android.Views;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Devices;
using ReplicaEmrApp.Services;

namespace ReplicaEmrApp;

public class DeviceService : IDeviceService
{
    private readonly IDeviceInfo deviceInfo;

    public DeviceService(IDeviceInfo deviceInfo)
    {
        this.deviceInfo = deviceInfo;
    }
    public string GetDeviceId()
    {
        string GetDeviceId()
        {
            return Android.Provider.Settings.Secure
                .GetString(Android.App.Application.Context.ContentResolver,
                Android.Provider.Settings.Secure.AndroidId);
        }

        return GetDeviceId();
    }

    public Task<string> GetDeviceIpAsync()
    {
        string GetIpAddress()
        {
            //TODO: 如何取得手機行動數據之 IP 或 WIFI IP
            WifiManager wifiManager = (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);
            var ipAddress = wifiManager.ConnectionInfo.IpAddress;
            return FormatIpAddress(ipAddress);
        }

        string FormatIpAddress(int ipAddress)
        {
            return string.Format("{0}.{1}.{2}.{3}",
                (ipAddress & 0xff),
                (ipAddress >> 8 & 0xff),
                (ipAddress >> 16 & 0xff),
                (ipAddress >> 24 & 0xff));
        }

        return Task.FromResult(GetIpAddress());
    }

    public Task<string> GetDevicePlatformAsync()
    {
        return Task.FromResult(deviceInfo.Platform.ToString());
    }

    public void KeepScreenOn()
    {
        var activity = (Activity)Android.App.Application.Context;
        activity.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
    }

    public void AllowScreenSleep()
    {
        var activity = (Activity)Android.App.Application.Context;
        activity.Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
    }
}
