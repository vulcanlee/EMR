using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using ReplicaEmrApp.Services;

namespace ReplicaEmrApp.ViewModels;

public partial class LoginPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly LoginService loginService;
    private readonly GlobalObject globalObject;
    private readonly ReportCodeService reportCodeService;
    private readonly IStorageJSONService<GlobalObject> storageJSONService;
    #endregion

    #region Property Member
    [ObservableProperty]
    string account = string.Empty;

    [ObservableProperty]
    string password = string.Empty;

    [ObservableProperty]
    string pinCode = string.Empty;

    [ObservableProperty]
    string errorMessage = string.Empty;

    [ObservableProperty]
    bool isError = false;

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    string version = string.Empty;
    #endregion

    #region Constructor
    public LoginPageViewModel(INavigationService navigationService,
        LoginService loginService, GlobalObject globalObject,
        ReportCodeService reportCodeService, IStorageJSONService<GlobalObject> storageJSONService)
    {
        this.navigationService = navigationService;
        this.loginService = loginService;
        this.globalObject = globalObject;
        this.reportCodeService = reportCodeService;
        this.storageJSONService = storageJSONService;
#if DEBUG
        Account = "admin";
        Password = "cirtnexe0845";
        PinCode = "pincode";
#endif
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    async Task DoSettingAsync()
    {
        await navigationService.NavigateAsync(MagicValueHelper.SettingPage);
    }
    [RelayCommand]
    async Task DoLoginAsync()
    {
        IsBusy = true;
        IsError = false;
        ErrorMessage = string.Empty;

        try
        {
            bool loginStatus = await loginService.LoginAsync(Account, Password);
            if (loginStatus)
            {
                await reportCodeService.GetAsync();
                await storageJSONService
                    .WriteToDataFileAsync(MagicValueHelper.DataPath, MagicValueHelper.GlobalObjectFilename, globalObject);

                await navigationService.NavigateAsync(MagicValueHelper.HomePage);

            }
            else
            {
                IsError = true;
                ErrorMessage = "請檢查帳號密碼正確性";
            }

        }
        catch (Exception ex)
        {
            IsError = true;
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        Version = globalObject.Version;
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
