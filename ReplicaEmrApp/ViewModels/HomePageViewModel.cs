using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using Prism.Services;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Enums;
using ReplicaEmrApp.Events;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using ReplicaEmrApp.Models.Mcs;
using ReplicaEmrApp.Services;
using ShareResource.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ReplicaEmrApp.ViewModels;

public partial class HomePageViewModel : ObservableObject, INavigatedAware, IApplicationLifecycleAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly GlobalObject globalObject;
    private readonly UnsignService unsignService;
    private readonly IEventAggregator eventAggregator;
    private readonly IPageDialogService dialogService;
    private readonly IStorageJSONService<GlobalObject> storageJSONService;
    private readonly CurrentDeviceInformationService currentDeviceInformationService;
    private readonly ParameterService parameterService;
    private readonly ConfigService configService;
    private readonly CheckSessionService checkSessionService;
    private readonly SignatureAddService signatureAddService;
    private readonly IDeviceService deviceService;
    private readonly FailLogService failLogService;
    private readonly CheckUploadService checkUploadService;

    //debug用
    private readonly IStorageJSONService<SettingModel> settingStorageJSONService;

    List<UnsignReportData> data { get; set; }
    UnsignReportData currentSignItem { get; set; }
    int currentIndex { get; set; }
    bool autoSignMode = false;
    SignRequest signRequest = new SignRequest();
    #endregion

    #region Property Member
    [ObservableProperty]
    string title = string.Empty;

    [ObservableProperty]
    ObservableCollection<UnSignItem> unSignItems = new();

    [ObservableProperty]
    UnSignItem selectedItem = new();

    [ObservableProperty]
    bool showNavigationPage = true;
    [ObservableProperty]
    bool isRefreshing = true;
    [ObservableProperty]
    bool refreshingView = false;
    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    RefreshReportStatusViewModel refreshReportStatusViewModel = new();

    [ObservableProperty]
    bool showSignProcessingView = false;
    [ObservableProperty]
    SignProcessingViewModel signProcessingViewModel = new();
    bool isPauseSign = false;
    bool isCancelSign = false;
    bool isStopSign = false;

    [ObservableProperty]
    StopSignViewModel stopSignViewModel = new StopSignViewModel();
    [ObservableProperty]
    bool showStopSignView = false;

    [ObservableProperty]
    SignResultViewModel signResultViewModel = new SignResultViewModel();
    [ObservableProperty]
    bool showSignResultView = false;

    bool isSigningAndSaving = false;

    [ObservableProperty]
    string testValue = string.Empty;

    #endregion

    #region Constructor
    public HomePageViewModel(INavigationService navigationService, GlobalObject globalObject,
        IEventAggregator eventAggregator, UnsignService unsignService,
        IPageDialogService dialogService, IStorageJSONService<GlobalObject> storageJSONService,
        CurrentDeviceInformationService currentDeviceInformationService
        , IStorageJSONService<SettingModel> settingStorageJSONService,
        ParameterService parameterService, ConfigService configService,
        CheckSessionService checkSessionService, SignatureAddService signatureAddService,
        IDeviceService deviceService, FailLogService failLogService,
        CheckUploadService checkUploadService)
    {
        this.navigationService = navigationService;
        this.globalObject = globalObject;
        this.unsignService = unsignService;
        this.eventAggregator = eventAggregator;
        this.dialogService = dialogService;
        this.storageJSONService = storageJSONService;
        this.currentDeviceInformationService = currentDeviceInformationService;
        this.settingStorageJSONService = settingStorageJSONService;
        this.parameterService = parameterService;
        this.configService = configService;
        this.checkSessionService = checkSessionService;
        this.signatureAddService = signatureAddService;
        this.deviceService = deviceService;
        this.failLogService = failLogService;
        this.checkUploadService = checkUploadService;
        SignProcessingViewModel.StopViewModelCommand = StopSignProcessingCommand;
        StopSignViewModel.CancelCommand = CancelButtonCommand;
        StopSignViewModel.StopCommand = StopButtonCommand;
        SignResultViewModel.StopAutoSignButtonAsyncCommand = StopAutoSignButtonCommand;

    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    public async Task RefreshAsync()
    {
        //IsRefreshing = true;
        await ReloadAsync();
        ShowNavigationPage = true;
    }
    [RelayCommand]
    public async Task TapItem(UnSignItem unSignItem)
    {
        NavigationParameters parameters = new();

        List<UnsignReportData> reports = data.Where(x => x.frmCode == unSignItem.ReportCode).ToList();
        parameters.Add("UnsignReport", reports);
        await navigationService.NavigateAsync(MagicValueHelper.ReportDetailPage, parameters);
    }

    [RelayCommand]
    public async Task ViewOperlog()
    {
        await navigationService.NavigateAsync(MagicValueHelper.FaillLogPage);
    }

    [RelayCommand]
    public async Task DoLogoutAsync()
    {
        bool result = await dialogService.DisplayAlertAsync("確定要登出嗎?", "登出", "確定", "取消");

        if (result)
        {
            globalObject.CleanUp();
            await storageJSONService
                .WriteToDataFileAsync(MagicValueHelper.DataPath, MagicValueHelper.GlobalObjectFilename,
                globalObject);
            currentDeviceInformationService.Current.Account = string.Empty;
            WeakReferenceMessenger.Default.Unregister<SignResponse>(this);
            await navigationService.NavigateAsync(MagicValueHelper.LoginPage);
        }
    }

    [RelayCommand]
    public void DoException()
    {
        throw new Exception("Test Exception");
    }

    [RelayCommand]
    public async Task SignAllAsync()
    {
        if (data.Count < 1)
        {
            await dialogService.DisplayAlertAsync("提示", "無簽章資料", "確定");
            return;
        }
        MainThread.BeginInvokeOnMainThread(() =>
        {
            DeviceDisplay.Current.KeepScreenOn = true;
            autoSignMode = true;
        });

        isPauseSign = false;
        isCancelSign = false;
        isStopSign = false;
        ShowSignResultView = false;
        currentIndex = 0;

        //檢驗憑證並上傳、確認token是否失效
        if (string.IsNullOrEmpty(globalObject.CertificateData) || string.IsNullOrEmpty(globalObject.IdentityNo))
        {
            bool isEngineerMode = await parameterService.GetEngineerModeAsync();
            string message = isEngineerMode ? "請重新登入並關閉工程模式" : "請重新登入";
            await dialogService.DisplayAlertAsync("查無憑證", message, "確定");
            return;
        }
        var checkUploadRequest = new CheckUploadRequestDto()
        {
            CertData = globalObject.CertificateData,
            CertFrom = 4,
            BasicId = globalObject.IdentityNo
        };
        (ApiResultModel<object> checkUploadApiResult, var specifyLog) = await checkUploadService.PostAsync(checkUploadRequest);
        if (await checkSessionService.ReloadDataAsync(checkUploadApiResult, specifyLog))
        {
            DeviceDisplay.Current.KeepScreenOn = false;
            autoSignMode = false;
            return;
        }

        ShowNavigationPage = false;
        ShowSignProcessingView = true;
        SignProcessingViewModel.Message = $"{currentIndex}/{data.Count}";
        SignProcessingViewModel.Progress = 0;
        SignResultViewModel.Title = "簽章完成";
        SignResultViewModel.Total = data.Count;
        SignResultViewModel.SuccessCount = 0;
        SignResultViewModel.FailCount = 0;
        SignResultViewModel.ButtonColor = MagicValueHelper.Danger;
        SignResultViewModel.ButtonText = "關閉自動簽章";

        await Signature();

    }
    [RelayCommand]
    public void StopSignProcessing()
    {
        ShowStopSignView = true;
        ShowSignProcessingView = false;
        isPauseSign = true;
    }

    [RelayCommand]
    public async Task CancelButton()
    {
        ShowStopSignView = false;
        ShowSignProcessingView = true;
        isPauseSign = false;
        isStopSign = false;
        await Signature();
    }

    [RelayCommand]
    public void StopButton()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            DeviceDisplay.Current.KeepScreenOn = false;
            autoSignMode = false;
        });
        isStopSign = true;
        ShowStopSignView = false;
        ShowSignResultView = true;
        //TODO:關閉自動簽章
        SignResultViewModel.Title = "簽章中斷";
        SignResultViewModel.Total = SignResultViewModel.SuccessCount + SignResultViewModel.FailCount;
        SignResultViewModel.Message = "已關閉自動簽章模式";
        SignResultViewModel.ButtonColor = MagicValueHelper.Primary;
        SignResultViewModel.ButtonText = "確認";
    }

    [RelayCommand]
    public async Task StopAutoSignButtonAsync()
    {
        MainThread.BeginInvokeOnMainThread(() => { DeviceDisplay.Current.KeepScreenOn = false; autoSignMode = false; });
        ShowSignResultView = false; 
        await ReloadAsync();
        ShowNavigationPage = true;
    }

    [RelayCommand]
    public void Empty()
    {
    }
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
        WeakReferenceMessenger.Default.Unregister<SignResponse>(this);
    }


    public async void OnNavigatedTo(INavigationParameters parameters)
    {
        WeakReferenceMessenger.Default.Register<SignResponse>(this, async (sender, message) =>
        {
            try
            {
                Debug.WriteLine("------------ getMcsResponse:" + DateTime.Now);
                //await Task.Delay(3000);
                if (isPauseSign || isStopSign)
                {
                    isSigningAndSaving = false;
                    return;
                }
                if (message.Code == "0")
                {
                    Debug.WriteLine($"{this.GetHashCode()}");
                    if (currentSignItem == null) Debug.WriteLine("currentSignItem is null");
                    if (message == null) Debug.WriteLine("message is null");
                    if (globalObject == null) Debug.WriteLine("globalObject is null");
                    if (signatureAddService == null) Debug.WriteLine("signatureAddService is null");
                    Debug.WriteLine("------------ beforeSave:" + DateTime.Now);
                    ApiResultModel<SignatureAddRequestDto> addResult = new ApiResultModel<SignatureAddRequestDto>();

                    (addResult, var specifyLog) = await signatureAddService.PostAsync(new SignatureAddRequestDto()
                    {
                        DocId = currentSignItem.docId,
                        DocsnId = currentSignItem.docsnId,
                        SignatureValue = message.Message,
                        CertificateData = globalObject.CertificateData,
                        CertFrom = 4,
                        BasicId = message.Idno,
                        OperatorType = 3,
                        ReadType = 3
                    });
                    //await Task.Delay(3000);
                    Debug.WriteLine("------------ afterSave:" + DateTime.Now);
                    await checkSessionService.ReloadDataAsync(addResult, specifyLog, false);
                    Debug.WriteLine("------------ afterReload:" + DateTime.Now);
                    if (addResult.code == MagicValueHelper.SuccessStatus)
                    {
                        SignResultViewModel.SuccessCount++;
                    }
                    else
                    {
                        SignResultViewModel.FailCount++;
                    }
                }
                else
                {
                    //TODO: 摳log參數調整
                    SignResultViewModel.FailCount++;
                    OperlogDto operlog = new OperlogDto();
                    var operParam = signRequest.Clone();
                    operlog.PrepareBaseData(parameterService, deviceService);
                    operlog.PrepareExceptionData(new OperlogDto()
                    {
                        businessType = BusinessTypeEnum.簽章.GetHashCode(),
                        method = "mcs:sign",
                        requestMethod = "URL Scheme",
                        operUrl = "",
                        operParam = JsonConvert.SerializeObject(operParam),
                        operName = globalObject.UserName,
                        jsonResult = JsonConvert.SerializeObject(message),
                        errorMsg = message.Message,
                    });

                    await failLogService.AddPostToFileAsync(operlog);
                }

                SignProcessingViewModel.Message = $"{currentIndex + 1}/{data.Count}";
                SignProcessingViewModel.Progress = (double)(currentIndex + 1) / data.Count;
                currentIndex++;
                isSigningAndSaving = false;
                await Signature();
            }
            catch (Exception ex)
            {
                await dialogService.DisplayAlertAsync("簽章失敗", ex.ToString(), "確定");
            }
        });

        //TODO: 要直接call參數api，因可能直接進到home
        if (globalObject.config == null)
        {
            (var configApiResult, var specifyLog) = await configService.GetAsync();
            if (await checkSessionService.ReloadDataAsync(configApiResult, specifyLog)) return;
            globalObject.config = configApiResult.data;
        }
        var localSetting = await settingStorageJSONService.ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.SettingModelFilename);

        Title = await parameterService.GetReportListNameAsync();

        SignProcessingViewModel.EventAggregator = eventAggregator;
        SignProcessingViewModel.Title = "簽章報告";
        SignProcessingViewModel.SubTitle1 = "簽章執行中...";
        SignProcessingViewModel.SubTitle2 = "(請勿關閉頁面)";
        SignProcessingViewModel.Message = "0/0";
        SignProcessingViewModel.Progress = 0;
        SignResultViewModel.Message = $"(3秒後將自動繼續簽章...)";

        if (parameters.GetNavigationMode() == Prism.Navigation.NavigationMode.New)
        {
            await ReloadAsync();
            ShowNavigationPage = true;
        }
    }

    private async Task ReloadAsync()
    {
        RefreshReportStatusViewModel.Progress = 0;
        ShowNavigationPage = false;
        IsBusy = true;
        RefreshingView = true;
        RefreshReportStatusViewModel.Title = "更新報告狀態";
        RefreshReportStatusViewModel.SubTitle1 = "更新中";
        RefreshReportStatusViewModel.SubTitle2 = "請稍後";
        eventAggregator.GetEvent<OnOffNavigationPageEvent>().Publish(new OnOffNavigationPagePayload { IsOn = !IsBusy });

        UnSignItems.Clear();
        (var dto, var specifyLog) = await unsignService.GetAsync(DateTime.Now.AddDays(-int.Parse(parameterService.GetUnsignListDateRangeAsync().Result)), DateTime.Now, globalObject.UserName, SignTpEnum.全部, CertTpEnum.人員);

        if (await checkSessionService.ReloadDataAsync(dto, specifyLog)) return;
        data = dto.data;

        globalObject.unSignItem = data;
        await storageJSONService
                   .WriteToDataFileAsync(MagicValueHelper.DataPath, MagicValueHelper.GlobalObjectFilename, globalObject);

        int currentCounter = 0;
        var frmCpdeGroup = data.GroupBy(x => x.frmCode);
        foreach (var item in frmCpdeGroup)
        {
            UnSignItems.Add(new UnSignItem()
            {
                ReportCode = item.FirstOrDefault().frmCode,
                ReportName = item.FirstOrDefault().frmNm,
                TotalReport = item.Count(),
            });

            currentCounter++;
            RefreshReportStatusViewModel.Message = $"{currentCounter} / {frmCpdeGroup.Count()}";
            RefreshReportStatusViewModel.Progress = (double)currentCounter / frmCpdeGroup.Count();
            await Task.Yield();
        }

        IsRefreshing = false;
        IsBusy = false;
        RefreshingView = false;
        //ShowNavigationPage = true;
        eventAggregator.GetEvent<OnOffNavigationPageEvent>().Publish(new OnOffNavigationPagePayload { IsOn = !IsBusy });

    }
    #endregion

    #region Other Method
    private async Task Signature()
    {
        Debug.WriteLine("------------- beforeCallMcs");
        //await Task.Delay(3000);
        if (isPauseSign || isStopSign) return;
        if (currentIndex >= data.Count)
        {
            ShowSignProcessingView = false;
            ShowSignResultView = true;
            AutoSignAsync();
            return;
        }
        else
        {
            if (isSigningAndSaving) return;
            isSigningAndSaving = true;
            currentSignItem = data[currentIndex];

            signRequest = new SignRequest()
            {
                HostialCode = globalObject.TenantCode,
                Account = globalObject.UserName,
                Password = globalObject.PWord,
                Scheme = MagicValueHelper.SignScheme,
                Operation = MagicValueHelper.SignOperation,
                HashFlag = parameterService.GetHashFlag(),
                Data = currentSignItem.digest
            };
#if ANDROID
            WeakReferenceMessenger.Default.Send(signRequest);
#elif IOS
            bool supportsUri = await Launcher.Default.CanOpenAsync("cgappsign://");

            if (supportsUri)
            {
                AppDelegate.requestCode = ActivityRequestCodeEnum.SignRequestCode;
                await Launcher.Default.OpenAsync("cgappsign://ridetype/?urlSchemes=exentricemrapp&" +
                    $"operation=sign&account={globalObject.UserName}&password={globalObject.PWord}&hospital_code={globalObject.TenantCode}&" +
                    $"hash_flag={parameterService.GetHashFlag()}&data={currentSignItem.digest}");
            }
#endif
        }
    }

    private async Task AutoSignAsync()
    {
        var repeatSeconds = Convert.ToInt32(await parameterService.GetRepeatIntervalAsync());
        for (var i = 0; i < repeatSeconds; i++)
        {
            if (!autoSignMode) return;
            SignResultViewModel.Message = $"({repeatSeconds - i}秒後將自動繼續簽章...)";
            await Task.Delay(1000);
        }
        await ReloadAsync();
        if (data.Count < 1)
        {
            isStopSign = true;
            ShowStopSignView = false;
            ShowSignResultView = true;
            currentIndex = 0;
            SignResultViewModel.Title = "簽章完成";
            SignResultViewModel.SuccessCount = 0;
            SignResultViewModel.FailCount = 0;
            SignResultViewModel.Total = SignResultViewModel.SuccessCount + SignResultViewModel.FailCount;
            SignResultViewModel.ButtonColor = MagicValueHelper.Danger;
            SignResultViewModel.ButtonText = "關閉自動簽章";

            AutoSignAsync();
            return;
        }
        SignAllAsync();
    }

    public void OnResume()
    {
        //throw new NotImplementedException();
    }

    public void OnSleep()
    {
        //if (autoSignMode)
        //    StopButton();
    }

    #endregion
    #endregion
}
