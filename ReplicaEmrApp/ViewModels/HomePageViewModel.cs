using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Enums;
using ReplicaEmrApp.Events;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using ReplicaEmrApp.Models.Mcs;
using ReplicaEmrApp.Services;
using System.Collections.ObjectModel;

namespace ReplicaEmrApp.ViewModels;

public partial class HomePageViewModel : ObservableObject, INavigatedAware, IApplicationLifecycleAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly ILogger<HomePageViewModel> logger;
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
    private readonly SignItemLogService signItemLogService;

    //debug用
    private readonly IStorageJSONService<SettingModel> settingStorageJSONService;

    List<UnsignReportData> data { get; set; }
    UnsignReportData currentSignItem { get; set; }
    SignRequest signRequest = new SignRequest();
    /// <summary>
    /// 現在簽章到第幾筆報告
    /// </summary>
    int currentIndex { get; set; }
    /// <summary>
    /// 是否暫停簽章
    /// </summary>
    bool isPauseSign = false;
    /// <summary>
    /// 是否取消簽章
    /// </summary>
    bool isCancelSign = false;
    /// <summary>
    /// 是否停止簽章
    /// </summary>
    bool isStopSign = false;
    /// <summary>
    /// 是否正在自動簽章模式下
    /// </summary>
    bool autoSignMode = false;
    /// <summary>
    /// 是否正在進行簽章並儲存作業，避免多次同時執行同一筆報告簽章
    /// </summary>
    bool isSigningAndSaving = false;
    /// <summary>
    /// 上次啟動 MCS App 的時間
    /// </summary>
    DateTime? lastLaunchMcsAppTime = null;
    /// <summary>
    /// 等待 MCS App 回應的最大秒數
    /// </summary>
    int waitMcsAppResponseMaxSeconds = 10;
    /// <summary>
    /// 是否進入睡眠模式(背景模式)
    /// </summary>
    bool isSleepMode = false;

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

    [ObservableProperty]
    StopSignViewModel stopSignViewModel = new StopSignViewModel();
    [ObservableProperty]
    bool showStopSignView = false;

    [ObservableProperty]
    SignResultViewModel signResultViewModel = new SignResultViewModel();
    [ObservableProperty]
    bool showSignResultView = false;

    [ObservableProperty]
    string testValue = string.Empty;

    #endregion

    #region Constructor
    public HomePageViewModel(INavigationService navigationService,
        ILogger<HomePageViewModel> logger, GlobalObject globalObject,
        IEventAggregator eventAggregator, UnsignService unsignService,
        IPageDialogService dialogService, IStorageJSONService<GlobalObject> storageJSONService,
        CurrentDeviceInformationService currentDeviceInformationService
        , IStorageJSONService<SettingModel> settingStorageJSONService,
        ParameterService parameterService, ConfigService configService,
        CheckSessionService checkSessionService, SignatureAddService signatureAddService,
        IDeviceService deviceService, FailLogService failLogService,
        CheckUploadService checkUploadService,
        SignItemLogService signItemLogService)
    {
        this.navigationService = navigationService;
        this.logger = logger;
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
        this.signItemLogService = signItemLogService;
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
        await PressSingAllButtonAsync();
    }


    [RelayCommand]
    public void StopSignProcessing()
    {
        HiddenAllProcessingView();
        ShowStopSignView = true;
        isPauseSign = true;
    }

    [RelayCommand]
    public async Task CancelButton()
    {
        HiddenAllProcessingView();
        ShowSignProcessingView = true;
        isPauseSign = false;
        isStopSign = false;
        Signature();
    }

    [RelayCommand]
    public void StopButton()
    {
        PressStopButton();
    }

    [RelayCommand]
    public async Task StopAutoSignButtonAsync()
    {
        MainThread.BeginInvokeOnMainThread(() => { DeviceDisplay.Current.KeepScreenOn = false; autoSignMode = false; });
        ShowSignProcessingView = ShowStopSignView = ShowSignResultView = false;

        await ReloadAsync();
        ShowNavigationPage = true;
        lastLaunchMcsAppTime = null;
    }

    [RelayCommand]
    public void Empty()
    {
    }
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
        PressStopButton();
        WeakReferenceMessenger.Default.Unregister<SignResponse>(this);
    }


    public async void OnNavigatedTo(INavigationParameters parameters)
    {
        WeakReferenceMessenger.Default.Register<SignResponse>(this, async (sender, message) =>
        {
            #region 處理 MCS 的報告簽章結果，並且儲存到 EMR Server
            try
            {
                if (isSleepMode)
                {
                    logger.LogInformation("************ HomePageViewModel WeakReferenceMessenger(0) 因為睡眠模式，所以，終止自動簽章:" + DateTime.Now);
                    return;
                }
                logger.LogInformation("------------ 處理 MCS App 報告簽章結果 getMcsResponse:" + DateTime.Now);
                lastLaunchMcsAppTime = null;
                //await Task.Delay(3000);
                if (isPauseSign || isStopSign) { isSigningAndSaving = false; return; }
                if (message.Code == "0")
                {
                    #region 此報告已經成功透過 MCS App 進行簽章行為
                    logger.LogInformation($"{this.GetHashCode()}");
                    if (currentSignItem == null) logger.LogInformation("currentSignItem is null");
                    if (message == null) logger.LogInformation("message is null");
                    if (globalObject == null) logger.LogInformation("globalObject is null");
                    if (signatureAddService == null) logger.LogInformation("signatureAddService is null");
                    logger.LogInformation("------------ 進行報告儲存作業前 beforeSave:" + DateTime.Now);
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
                    logger.LogInformation("------------ 進行報告儲存作業後 afterSave:" + DateTime.Now);
                    bool needRelogin = await checkSessionService.ReloadDataAsync(addResult, specifyLog, false);
                    logger.LogInformation("------------ 檢查是否需要重新登入  afterReload:" + DateTime.Now);
                    if (needRelogin) { isSigningAndSaving = false; return; }

                    if (addResult.code == MagicValueHelper.SuccessStatus)
                    {
                        SignResultViewModel.SuccessCount++;
                    }
                    else
                    {
                        SignResultViewModel.FailCount++;
                    }
                    #endregion
                }
                else
                {
                    #region 此報告透過 MCS App 進行簽章，但是發生了問題
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
                    #endregion
                }

                SignProcessingViewModel.Message = $"{currentIndex + 1}/{data.Count}";
                SignProcessingViewModel.Progress = (double)(currentIndex + 1) / data.Count;
                currentIndex++;
                isSigningAndSaving = false;
                Signature();
            }
            catch (Exception ex)
            {
                await dialogService.DisplayAlertAsync("簽章失敗", ex.ToString(), "確定");
            }
            #endregion
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
    #endregion

    #region Other Method

    public void OnResume()
    {
        logger.LogInformation("------------ HomePageViewModel OnResume:" + DateTime.Now);
        //logger.LogInformation("------------ HomePageViewModel OnResume isSleepMode:" + isSleepMode);
        //logger.LogInformation("------------ HomePageViewModel OnResume autoSignMode:" + autoSignMode);
        //logger.LogInformation("------------ HomePageViewModel OnResume isPauseSign:" + isPauseSign);
        //logger.LogInformation("------------ HomePageViewModel OnResume isStopSign:" + isStopSign);
        //logger.LogInformation("------------ HomePageViewModel OnResume isSigningAndSaving:" + isSigningAndSaving);
        //logger.LogInformation("------------ HomePageViewModel OnResume currentIndex:" + currentIndex);
        //logger.LogInformation("------------ HomePageViewModel OnResume data.Count:" + data.Count);

        if (autoSignMode)
        {
            if (ShowSignResultView == true)
            {
                isSigningAndSaving = false;
                isSleepMode = false;
                return;
            }

            if (lastLaunchMcsAppTime.HasValue)
            {
                var timeSpan = DateTime.Now - lastLaunchMcsAppTime.Value;
                if (timeSpan.TotalSeconds < waitMcsAppResponseMaxSeconds)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        DeviceDisplay.Current.KeepScreenOn = true;
                    });
                    isSleepMode = false;
                    isSigningAndSaving = false;
                    if (ShowSignResultView == false)
                        Signature();
                    return;
                }
            }
            isSleepMode = false;
            PressStopButton();
        }
        isSleepMode = false;
    }

    public void OnSleep()
    {
        logger.LogInformation("------------ HomePageViewModel OnSleep:" + DateTime.Now);
        isSleepMode = true;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            DeviceDisplay.Current.KeepScreenOn = false;
        });
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

    /// <summary>
    /// 進行自動簽章
    /// </summary>
    /// <returns></returns>
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
            HiddenAllProcessingView();
            ShowSignResultView = true;
            currentIndex = 0;
            SignResultViewModel.Title = "簽章完成";
            SignResultViewModel.SuccessCount = 0;
            SignResultViewModel.FailCount = 0;
            SignResultViewModel.Total = SignResultViewModel.SuccessCount + SignResultViewModel.FailCount;
            SignResultViewModel.ButtonColor = MagicValueHelper.Danger;
            SignResultViewModel.ButtonText = "關閉自動簽章";

            await AutoSignAsync();
            return;
        }
        await SignAllAsync();
    }

    private async Task PressSingAllButtonAsync()
    {
        logger.LogInformation("------------ PressSingAllButtonAsync:" + DateTime.Now);
        lastLaunchMcsAppTime = null;
        isSleepMode = false;
        if (data.Count < 1)
        {
            await dialogService.DisplayAlertAsync("提示", "無簽章資料", "確定");
            return;
        }
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            DeviceDisplay.Current.KeepScreenOn = true;
            autoSignMode = true;
        });

        isPauseSign = false;
        isCancelSign = false;
        isStopSign = false;
        HiddenAllProcessingView();
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

        Signature();
    }
    /// <summary>
    /// 
    /// 進行報告簽章
    /// </summary>
    /// <returns></returns>
    private async Task Signature()
    {
        if (isSleepMode)
        {
            logger.LogInformation("************ HomePageViewModel Signature(0) 因為睡眠模式，所以，終止自動簽章:" + DateTime.Now);
            return;
        }

        logger.LogInformation(">>>>>>>>>>>>> HomePageViewModel Signature 正在簽章報告索引數 : " + currentIndex);
        logger.LogInformation("------------- 準備進行報告簽章前準備工作 beforeCallMcs");
        //await Task.Delay(3000);
        if (isPauseSign || isStopSign)
        {
            logger.LogInformation("************ HomePageViewModel Signature 因為暫停或停止簽章，所以，終止自動簽章:" + DateTime.Now);
            logger.LogInformation("------------ HomePageViewModel Signature isPauseSign:" + isPauseSign);
            logger.LogInformation("------------ HomePageViewModel Signature isStopSign:" + isStopSign);
            return;
        }
        if (currentIndex >= data.Count)
        {
            #region 進入自動簽章模式，稍後一段時間，將會重新簽章
            logger.LogInformation("------------ HomePageViewModel Signature 全部報告簽章完畢，重新進入自動簽章模式:" + DateTime.Now);
            HiddenAllProcessingView();
            ShowSignResultView = true;
            await AutoSignAsync();
            return;
            #endregion
        }
        else
        {
            if (isSigningAndSaving)
            {
                logger.LogInformation("************ HomePageViewModel Signature 因為正在進行簽章並儲存作業，所以，終止這次要求簽章行為:" + DateTime.Now);
                return; //避免多次同時執行同一筆報告簽章
            }
            isSigningAndSaving = true;
            currentSignItem = data[currentIndex]; //取得目前要簽章的報告

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

            #region 準備進行呼叫 MCS App
            if (isSleepMode)
            {
                logger.LogInformation("************ HomePageViewModel Signature 因為睡眠模式，所以，終止自動簽章:" + DateTime.Now);
                //isSigningAndSaving = false;
                //PressStopButton();
                return;
            }
            lastLaunchMcsAppTime = DateTime.Now;
            logger.LogInformation("------------ HomePageViewModel Signature 進行呼叫 MCS App:" + DateTime.Now);
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
            #endregion
        }
    }

    private void PressStopButton()
    {
        logger.LogInformation("------------ PressStopButton:" + DateTime.Now);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            DeviceDisplay.Current.KeepScreenOn = false;
            autoSignMode = false;
        });
        isStopSign = true;
        HiddenAllProcessingView();
        ShowSignResultView = true;
        isSigningAndSaving = false;
        //TODO:關閉自動簽章
        SignResultViewModel.Title = "簽章中斷";
        SignResultViewModel.Total = SignResultViewModel.SuccessCount + SignResultViewModel.FailCount;
        SignResultViewModel.Message = "已關閉自動簽章模式";
        SignResultViewModel.ButtonColor = MagicValueHelper.Primary;
        SignResultViewModel.ButtonText = "確認";
    }

    void HiddenAllProcessingView()
    {
        ShowSignProcessingView = false;
        ShowStopSignView = false;
        ShowSignResultView = false;
    }

    #endregion
    #endregion
}
