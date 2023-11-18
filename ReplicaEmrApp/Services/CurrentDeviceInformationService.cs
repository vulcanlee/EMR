using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using System.Reflection;

namespace ReplicaEmrApp.Services
{
    public class CurrentDeviceInformationService
    {
        public CurrentDeviceInformationService(IStorageJSONService<CurrentDeviceInformation> storageJSONService)
        {
            StorageJSONService = storageJSONService;
        }
        public CurrentDeviceInformation CurrentDeviceInformation { get; set; } = new CurrentDeviceInformation();
        public IStorageJSONService<CurrentDeviceInformation> StorageJSONService { get; }

        public void Reset()
        {
            //CurrentDeviceInformation.Account = string.Empty;
            //CurrentDeviceInformation.Password = string.Empty;
            CurrentDeviceInformation.MauiVersion = string.Empty;
            CurrentDeviceInformation.DeviceId = string.Empty;
            CurrentDeviceInformation.DeviceName = string.Empty;
            CurrentDeviceInformation.Model = string.Empty;
            CurrentDeviceInformation.Manufacturer = string.Empty;
            CurrentDeviceInformation.VersionString = string.Empty;
            CurrentDeviceInformation.Platform = string.Empty;
            CurrentDeviceInformation.Idiom = string.Empty;
            CurrentDeviceInformation.DeviceType = string.Empty;
            CurrentDeviceInformation.MauiVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            CurrentDeviceInformation.DeviceName = DeviceInfo.Current.Name;
            CurrentDeviceInformation.Model = DeviceInfo.Current.Model;
            CurrentDeviceInformation.Manufacturer = DeviceInfo.Current.Manufacturer;
            CurrentDeviceInformation.VersionString = DeviceInfo.Current.VersionString;
            CurrentDeviceInformation.Platform = DeviceInfo.Current.Platform.ToString();
            CurrentDeviceInformation.Idiom = DeviceInfo.Current.Idiom.ToString();
            CurrentDeviceInformation.DeviceType = DeviceInfo.Current.DeviceType.ToString();
        }

        public async Task SaveAsync()
        {
            await StorageJSONService.WriteToDataFileAsync(MagicValueHelper.DataPath,
                MagicValueHelper.CurrentDeviceInformationFilename, CurrentDeviceInformation);
        }
        public async Task ReadAsync()
        {
            CurrentDeviceInformation = await StorageJSONService.ReadFromFileAsync(MagicValueHelper.DataPath,
                               MagicValueHelper.CurrentDeviceInformationFilename);
        }
    }
}
