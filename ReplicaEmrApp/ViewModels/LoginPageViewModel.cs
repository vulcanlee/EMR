using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Enums;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using ReplicaEmrApp.Models.Mcs;
using ReplicaEmrApp.Services;
using ShareResource.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ReplicaEmrApp.ViewModels;

public partial class LoginPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly LoginService loginService;
    private readonly GlobalObject globalObject;
    private readonly IStorageJSONService<GlobalObject> storageJSONService;
    private readonly IStorageJSONService<UserInfo> userInfoService;
    private readonly IStorageJSONService<List<ExceptionRecord>> exceptionStorageJSONService;
    private readonly ExceptionService exceptionService;
    private readonly ParameterService parameterService;
    private readonly TenantService tenantService;
    private readonly CurrentDeviceInformationService currentDeviceInformationService;
    private readonly ConfigService configService;
    private readonly CheckSessionService checkSessionService;
    private readonly CheckUploadService checkUploadService;
    private readonly IDeviceService deviceService;
    private readonly FailLogService failLogService;
    #endregion

    #region Property Member
    [ObservableProperty]
    string account = string.Empty;

    [ObservableProperty]
    string password = string.Empty;

    [ObservableProperty]
    bool showTenantPicker = true;

    [ObservableProperty]
    ObservableCollection<TenantResponseDto> tenantCodeList = new ObservableCollection<TenantResponseDto>();
    [ObservableProperty]
    TenantResponseDto tenantCode;

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

    [ObservableProperty]
    bool rememberAccount = false;

    public Action TurnOffSoftKeyboard;
    public GetCertRequest getCertRequest = new GetCertRequest();
    #endregion

    #region Constructor
    public LoginPageViewModel(INavigationService navigationService,
        LoginService loginService, GlobalObject globalObject,
        IStorageJSONService<GlobalObject> storageJSONService,
        IStorageJSONService<UserInfo> userInfoService,
        IStorageJSONService<List<ExceptionRecord>> ExceptionStorageJSONService,
        ExceptionService exceptionService,
        ParameterService parameterService,
        TenantService tenantService,
        CurrentDeviceInformationService currentDeviceInformationService,
        ConfigService configService,
        CheckSessionService checkSessionService,
        CheckUploadService checkUploadService,
        IDeviceService deviceService,
        FailLogService failLogService
        )
    {
        this.navigationService = navigationService;
        this.loginService = loginService;
        this.globalObject = globalObject;
        this.storageJSONService = storageJSONService;
        this.userInfoService = userInfoService;
        exceptionStorageJSONService = ExceptionStorageJSONService;
        this.exceptionService = exceptionService;
        this.parameterService = parameterService;
        this.tenantService = tenantService;
        this.currentDeviceInformationService = currentDeviceInformationService;
        this.configService = configService;
        this.checkSessionService = checkSessionService;
        this.checkUploadService = checkUploadService;
        this.deviceService = deviceService;
        this.failLogService = failLogService;
#if DEBUG
        //account = "admin";
        //password = "nimdarme0845";
        account = "exentric";
        password = "70400845";
#endif

    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    void Tap()
    {
        TurnOffSoftKeyboard?.Invoke();
    }
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
            if (TenantCode == null) {
                IsError = true;
                ErrorMessage = "院區代碼為必填";
                return;
            }

            ApiResultModel<string> loginResult = await loginService.LoginAsync(Account, Password, TenantCode?.TenantCode);
            if (loginResult.IsSuccess())
            {
                globalObject.Token = loginResult.token;
                globalObject.UserName = Account;
                globalObject.PWord = Password;
                globalObject.TenantCode = TenantCode?.TenantCode;
                currentDeviceInformationService.Current
                    .Account = globalObject.UserName;
                (var configApiResult, var specifyLog) = await configService.GetAsync();
                if (await checkSessionService.ReloadDataAsync(configApiResult, specifyLog)) return;
                globalObject.config = configApiResult.data;
               
                if (RememberAccount)
                {
                    UserInfo userInfo = new UserInfo()
                    {
                        Account = Account,
                        PWord = Password
                    };
                    await userInfoService
                        .WriteToDataFileAsync(MagicValueHelper.DataPath, MagicValueHelper.UserInfoFilename, userInfo);
                }
                else
                {
                    UserInfo userInfo = new UserInfo();

                    await userInfoService
                        .WriteToDataFileAsync(MagicValueHelper.DataPath, MagicValueHelper.UserInfoFilename, userInfo);

                }

                List<ExceptionRecord> datas = await exceptionStorageJSONService
                .ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.ExceptionRecordFilename);
                if (datas != null && datas.Count > 0)
                {
                    await exceptionService.UploadAsync(datas);
                }

                if (await parameterService.GetEngineerModeAsync())
                {
                    await storageJSONService
                   .WriteToDataFileAsync(MagicValueHelper.DataPath, MagicValueHelper.GlobalObjectFilename, globalObject);
                    await navigationService.NavigateAsync(MagicValueHelper.HomePage);
                    return;
                }

                //TODO: hospitalCode
                getCertRequest = new GetCertRequest()
                {
                    HostialCode = globalObject.TenantCode,
                    Account = Account,
                    Password = Password,
                    Scheme = MagicValueHelper.GetCertScheme,
                    Operation = MagicValueHelper.GetCertOperation
                };
#if ANDROID
                WeakReferenceMessenger.Default.Send(getCertRequest);
#elif IOS
                //TODO: hospitalCode
                bool supportsUri = await Launcher.Default.CanOpenAsync("cgappsign://");

                if (supportsUri)
                {
                    AppDelegate.requestCode = ActivityRequestCodeEnum.GetCertRequestCode;
                    await Launcher.Default.OpenAsync("cgappsign://ridetype/?urlSchemes=exentricemrapp&" +
                        $"operation=getcert&account={Account}&password={Password}&hospital_code={globalObject.TenantCode}");
                }
                else 
                {
                    var customResult = new GetCertResponse
                    {
                        Code = "-1",
                        Message = "請確認是否安裝行動憑證App"
                    };

                    WeakReferenceMessenger.Default.Send(customResult);
                }
#endif

            }
            else
            {
                IsError = true;
                ErrorMessage = loginResult.msg;
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
        WeakReferenceMessenger.Default.Unregister<GetCertResponse>(this);
    }

    public async void OnNavigatedTo(INavigationParameters parameters)
    {
        IsError = false;
        ErrorMessage = string.Empty;
        TenantCodeList = new ObservableCollection<TenantResponseDto>();
        TenantCode = new TenantResponseDto();
        try {
            WeakReferenceMessenger.Default.Register<GetCertResponse>(this, async (sender, message) =>
            {
                if (message.Code == "0")
                {
                    globalObject.CertificateData = message.Message;
                    globalObject.IdentityNo = message.Idno;

                    //檢驗憑證並上傳
                    var checkUploadRequest = new CheckUploadRequestDto()
                    {
                        CertData = message.Message,
                        CertFrom = 4,
                        BasicId = message.Idno
                    };
                    (ApiResultModel<object> checkUploadApiResult, var specifyLog) = await checkUploadService.PostAsync(checkUploadRequest);
                    if (await checkSessionService.ReloadDataAsync(checkUploadApiResult, specifyLog)) return;

                    await storageJSONService
                            .WriteToDataFileAsync(MagicValueHelper.DataPath, MagicValueHelper.GlobalObjectFilename, globalObject);

                    await navigationService.NavigateAsync(MagicValueHelper.HomePage);
                }
                else
                {
                    OperlogDto operlog = new OperlogDto();
                    var operParam = getCertRequest.Clone();
                    operlog.PrepareBaseData(parameterService, deviceService);
                    operlog.PrepareExceptionData(new OperlogDto()
                    {
                        businessType = BusinessTypeEnum.其他.GetHashCode(),
                        method = "mcs:getcert",
                        requestMethod = "URL Scheme",
                        operUrl = "",
                        operParam = JsonConvert.SerializeObject(operParam),
                        operName = globalObject.UserName,
                        jsonResult = JsonConvert.SerializeObject(message),
                        errorMsg = message.Message,
                    });
                    await failLogService.AddPostToFileAsync(operlog);

                    IsError = true;
                    ErrorMessage = message.Code + message.Message;
                }
            });

            Version = globalObject.Version;

            UserInfo userInfo = await userInfoService
                .ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.UserInfoFilename);

            if (userInfo != null && !string.IsNullOrEmpty(userInfo.Account))
            {
                Account = userInfo.Account;
                Password = userInfo.PWord;
                RememberAccount = true;
            }

            (ApiResultModel<TenantResponseDto> tenantResult, var specifyLog) = await this.tenantService.GetAsync();
            if (await checkSessionService.ReloadDataAsync(tenantResult, specifyLog))
            {
                IsError = true;
                ErrorMessage = specifyLog.errorMsg;
                return;
            }
            if (tenantResult.data.Count == 0)
            {
                IsError = true;
                ErrorMessage = "查無院區代碼";
            }
            else if (tenantResult.data.Count > 1)
            {
                TenantCodeList = new ObservableCollection<TenantResponseDto>(tenantResult.data);
                TenantCode = tenantResult.data.FirstOrDefault();
                ShowTenantPicker = true;
            }
            else
            {
                TenantCode = tenantResult.data.FirstOrDefault();
                ShowTenantPicker = false;
            }
        }
        catch (Exception ex)
        {
            IsError = true;
            ErrorMessage = ex.Message;
            return;
        }
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
