using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using ReplicaEmrApp.Services;
using ReplicaEmrApp.Views;
using ShareResource.Models;
using System.Diagnostics;
using System.Reflection;

namespace ReplicaEmrApp.ViewModels;

public partial class SplashPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly GlobalObject globalObject;
    private readonly ExceptionService exceptionService;
    private readonly IStorageJSONService<List<ExceptionRecord>> storageJSONService;
    private readonly IStorageJSONService<GlobalObject> storageJSONOfGlobalObjectService;
    private readonly CurrentDeviceInformationService currentDeviceInformationService;
    #endregion

    #region Property Member
    #endregion

    #region Constructor
    public SplashPageViewModel(INavigationService navigationService, GlobalObject globalObject,
        ExceptionService exceptionService,
        IStorageJSONService<List<ExceptionRecord>> storageJSONService,
        IStorageJSONService<GlobalObject> storageJSONOfGlobalObjectService,
        CurrentDeviceInformationService currentDeviceInformationService)
    {
        this.navigationService = navigationService;
        this.globalObject = globalObject;
        this.exceptionService = exceptionService;
        this.storageJSONService = storageJSONService;
        this.storageJSONOfGlobalObjectService = storageJSONOfGlobalObjectService;
        this.currentDeviceInformationService = currentDeviceInformationService;
    }
    #endregion

    #region Method Member
    #region Command Method
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public async void OnNavigatedTo(INavigationParameters parameters)
    {
        globalObject.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        currentDeviceInformationService.Reset();
        try
        {
            List<ExceptionRecord> datas = await storageJSONService
                .ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.ExceptionRecordFilename);
            if (datas != null && datas.Count > 0)
            {
                await exceptionService.UploadAsync(datas);
            }

        }
        catch (Exception ex)
        {

        }

        GlobalObject gObject = await storageJSONOfGlobalObjectService
            .ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.GlobalObjectFilename);
        if (gObject == null || string.IsNullOrEmpty(gObject.JSESSIONID))
        {
            await navigationService.NavigateAsync(MagicValueHelper.LoginPage);
        }
        else
        {
            globalObject.Copy(gObject, globalObject);
            currentDeviceInformationService.Current
                .Account = gObject.UserId;
            await navigationService.NavigateAsync(MagicValueHelper.HomePage);
        }

    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
