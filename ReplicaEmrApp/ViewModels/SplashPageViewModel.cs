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
    [ObservableProperty]
    bool showRetryButton = false;
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
    [RelayCommand]
    public async Task Retry()
    {
        ShowRetryButton = false;
        await LaunchAsync();
    }
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public async void OnNavigatedTo(INavigationParameters parameters)
    {
        await LaunchAsync();
    }

    #endregion

    #region Other Method
    private async Task LaunchAsync()
    {
        try
        {
            await SnackbarHelper.Show("正在初始化，請稍後 ...");
            await Task.Delay(1000);
            //throw new Exception("Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception Test Exception ");
            string version = $"{AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})";
            globalObject.Version = version;
            currentDeviceInformationService.Reset();

            GlobalObject gObject = await storageJSONOfGlobalObjectService
        .ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.GlobalObjectFilename);
            if (gObject == null || string.IsNullOrEmpty(gObject.Token))
            {
                //await SnackbarHelper.Show("切換到 身分驗證 頁面...");

                await SnackbarHelper.Dismiss();
                await navigationService.NavigateAsync(MagicValueHelper.LoginPage);
            }
            else
            {
                globalObject.Copy(gObject, globalObject);
                currentDeviceInformationService.Current
                    .Account = gObject.UserName;

                //隨機摳一隻api確認token沒有失效後才做簽章動作
                (var configApiResult, var specifyLog) = await configService.GetAsync();
                if (await checkSessionService.ReloadDataAsync(configApiResult, specifyLog, false))
                {
                    await SnackbarHelper.Show("存取權杖逾期，需要重新進行身分驗證");
                    return;
                };

                //await SnackbarHelper.Show("切換到 首頁 頁面...");
                await SnackbarHelper.Dismiss();
                await navigationService.NavigateAsync(MagicValueHelper.HomePage);
            }
        }
        catch (Exception ex)
        {
            await SnackbarHelper.Show($"系統啟動發生問題，請排除問題之後，點選[重新啟動]按鈕 : {ex.Message}");
            ShowRetryButton = true;
            return;
        }
    }
    #endregion
    #endregion
}
