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
    private readonly ConfigService configService;
    private readonly CheckSessionService checkSessionService;
    #endregion

    #region Property Member
    #endregion

    #region Constructor
    public SplashPageViewModel(INavigationService navigationService, GlobalObject globalObject,
        ExceptionService exceptionService,
        IStorageJSONService<List<ExceptionRecord>> storageJSONService,
        IStorageJSONService<GlobalObject> storageJSONOfGlobalObjectService,
        CurrentDeviceInformationService currentDeviceInformationService,
        ConfigService configService,
        CheckSessionService checkSessionService
        )
    {
        this.navigationService = navigationService;
        this.globalObject = globalObject;
        this.exceptionService = exceptionService;
        this.storageJSONService = storageJSONService;
        this.storageJSONOfGlobalObjectService = storageJSONOfGlobalObjectService;
        this.currentDeviceInformationService = currentDeviceInformationService;
        this.configService = configService;
        this.checkSessionService = checkSessionService;
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
        try {
            globalObject.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            currentDeviceInformationService.Reset();

            GlobalObject gObject = await storageJSONOfGlobalObjectService
        .ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.GlobalObjectFilename);
            if (gObject == null || string.IsNullOrEmpty(gObject.Token))
            {
                await navigationService.NavigateAsync(MagicValueHelper.LoginPage);
            }
            else
            {
                globalObject.Copy(gObject, globalObject);
                currentDeviceInformationService.Current
                    .Account = gObject.UserName;

                //隨機摳一隻api確認token沒有失效後才做簽章動作
                (var configApiResult, var specifyLog) = await configService.GetAsync();
                if (await checkSessionService.ReloadDataAsync(configApiResult, specifyLog, false)) return;

                //TODO: 移到login、homepage
                //List<ExceptionRecord> datas = await storageJSONService
                //    .ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.ExceptionRecordFilename);
                //if (datas != null && datas.Count > 0)
                //{
                //    if (await exceptionService.UploadAsync(datas) == false)
                //    {
                //        await navigationService.NavigateAsync(MagicValueHelper.LoginPage);
                //        return;
                //    }
                //}

                await navigationService.NavigateAsync(MagicValueHelper.HomePage);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            await navigationService.NavigateAsync(MagicValueHelper.LoginPage);
            return;
        }
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
