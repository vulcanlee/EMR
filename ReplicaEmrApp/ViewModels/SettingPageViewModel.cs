using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using ReplicaEmrApp.Services;

namespace ReplicaEmrApp.ViewModels;

public partial class SettingPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly IStorageJSONService<SettingModel> storageJSONService;
    private readonly IPageDialogService pageDialogService;
    private readonly ParameterService parameterService;
    #endregion

    #region Property Member
    [ObservableProperty]
    SettingModel settingModel = new SettingModel();

    [ObservableProperty]
    string verifyCode = string.Empty;

    [ObservableProperty]
    bool showVerifyArea = true;

    [ObservableProperty]
    bool showSettingArea = false;

    public Action TurnOffSoftKeyboard;

    #endregion

    #region Constructor
    public SettingPageViewModel(INavigationService navigationService,
        IStorageJSONService<SettingModel> storageJSONService,
        IPageDialogService pageDialogService,
        ParameterService parameterService)
    {
        this.navigationService = navigationService;
        this.storageJSONService = storageJSONService;
        this.pageDialogService = pageDialogService;
        this.parameterService = parameterService;

#if DEBUG
        VerifyCode = "70400845";
#endif
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    void ShowExceptionList()
    {
        navigationService.NavigateAsync("ExceptionListPage");
    }

    [RelayCommand]
    void Tap()
    {
        TurnOffSoftKeyboard?.Invoke();
    }

    [RelayCommand]
    public async Task SaveSettingCommand()
    {
        await storageJSONService.WriteToDataFileAsync(
            MagicValueHelper.DataPath, MagicValueHelper.SettingModelFilename, SettingModel);
        await pageDialogService.DisplayAlertAsync("通知", "設定已經儲存完成", "確定");
        await navigationService.GoBackAsync();
    }

    [RelayCommand]
    public async Task Verify()
    {
        if (VerifyCode == MagicValueHelper.VerifyCode)
        {
            ShowVerifyArea = false;
            ShowSettingArea = true;
        }
        else
        {
            await pageDialogService.DisplayAlertAsync("警告", "驗證碼錯誤", "確定");
        }
    }
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public async void OnNavigatedTo(INavigationParameters parameters)
    {
        //SettingModel = await storageJSONService.ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.SettingModelFilename);
        SettingModel = await parameterService.GetSettingModelAsync();
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
