namespace ReplicaEmrApp.Services;

public interface IDeviceService
{
    string GetDeviceId();
    Task<string> GetDeviceIpAsync();
    Task<string> GetDevicePlatformAsync();
    void KeepScreenOn();
    void AllowScreenSleep();
}
