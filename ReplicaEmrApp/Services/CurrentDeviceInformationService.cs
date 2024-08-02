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
        public CurrentDeviceInformation Current { get; set; } = new CurrentDeviceInformation();
        public IStorageJSONService<CurrentDeviceInformation> StorageJSONService { get; }

        public void Reset()
        {
            //TODO:尚未加入device機制
            //CurrentDeviceInformation.Account = string.Empty;
            //CurrentDeviceInformation.Password = string.Empty;
            Current.MauiVersion = string.Empty;
            Current.DeviceId = string.Empty;
            Current.DeviceName = string.Empty;
            Current.Model = string.Empty;
            Current.Manufacturer = string.Empty;
            Current.VersionString = string.Empty;
            Current.Platform = string.Empty;
            Current.Idiom = string.Empty;
            Current.DeviceType = string.Empty;
            Current.MauiVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Current.DeviceName = DeviceInfo.Current.Name;
            Current.Model = DeviceInfo.Current.Model;
            Current.Manufacturer = DeviceInfo.Current.Manufacturer;
            Current.VersionString = DeviceInfo.Current.VersionString;
            Current.Platform = DeviceInfo.Current.Platform.ToString();
            Current.Idiom = DeviceInfo.Current.Idiom.ToString();
            Current.DeviceType = DeviceInfo.Current.DeviceType.ToString();
        }

        public async Task SaveAsync()
        {
            await StorageJSONService.WriteToDataFileAsync(MagicValueHelper.DataPath,
                MagicValueHelper.CurrentDeviceInformationFilename, Current);
        }
        public async Task ReadAsync()
        {
            Current = await StorageJSONService.ReadFromFileAsync(MagicValueHelper.DataPath,
                               MagicValueHelper.CurrentDeviceInformationFilename);
        }
    }
}
