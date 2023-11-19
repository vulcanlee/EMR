using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Services;
using ReplicaEmrApp.Events;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using ReplicaEmrApp.Services;
using ShareResource.Models;

namespace ReplicaEmrApp.ViewModels;

public partial class HomePageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly GlobalObject globalObject;
    private readonly ReportDetailService reportDetailService;
    private readonly IEventAggregator eventAggregator;
    private readonly ReportCodeService reportCodeService;
    private readonly IPageDialogService dialogService;
    private readonly IStorageJSONService<GlobalObject> storageJSONService;
    private readonly CurrentDeviceInformationService currentDeviceInformationService;
    #endregion

    #region Property Member
    [ObservableProperty]
    List<UnSignItem> unSignItems = new();

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
    #endregion

    #region Constructor
    public HomePageViewModel(INavigationService navigationService, GlobalObject globalObject, ReportDetailService reportDetailService,
        IEventAggregator eventAggregator, ReportCodeService reportCodeService,
        IPageDialogService dialogService, IStorageJSONService<GlobalObject> storageJSONService,
        CurrentDeviceInformationService currentDeviceInformationService)
    {
        this.navigationService = navigationService;
        this.globalObject = globalObject;
        this.reportDetailService = reportDetailService;
        this.eventAggregator = eventAggregator;
        this.reportCodeService = reportCodeService;
        this.dialogService = dialogService;
        this.storageJSONService = storageJSONService;
        this.currentDeviceInformationService = currentDeviceInformationService;
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    public async Task RefreshAsync()
    {
        IsRefreshing = true;
        await ReloadAsync();
    }
    [RelayCommand]
    public async Task TapItem(UnSignItem unSignItem)
    {
        NavigationParameters parameters = new();
        parameters.Add("UnsignReport", unSignItem);
        await navigationService.NavigateAsync(MagicValueHelper.ReportDetailPage, parameters);
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
            await navigationService.NavigateAsync(MagicValueHelper.LoginPage);
        }
    }

    [RelayCommand]
    public void DoException()
    {
        throw new Exception("Test Exception");
    }
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public async void OnNavigatedTo(INavigationParameters parameters)
    {
        RefreshReportStatusViewModel.Title = "更新報告狀態";
        RefreshReportStatusViewModel.SubTitle1 = "更新中";
        RefreshReportStatusViewModel.SubTitle2 = "請稍後";
        RefreshReportStatusViewModel.Message = "";
        RefreshReportStatusViewModel.Progress = 0.7;
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        int totalReportCode = globalObject.reportCodes.Count;
        RefreshReportStatusViewModel.Progress = 0;
        ShowNavigationPage = false;
        IsBusy = true;
        RefreshingView = true;
        eventAggregator.GetEvent<OnOffNavigationPageEvent>().Publish(new OnOffNavigationPAgePayload { IsOn = !IsBusy });

        UnSignItems.Clear();

        foreach (var item in globalObject.reportCodes)
        {
            UnSignItems.Add(new UnSignItem
            {
                ReportCode = item.reportCode,
                ReportName = item.reportName,
                TotalReport = 0
            });
        }
        int currentCounter = 1;
        Task.Run(async () =>
        {
            foreach (var item in UnSignItems)
            {
                RefreshReportStatusViewModel.Message = $"更新狀態({currentCounter}/{totalReportCode})";
                var dto = await reportDetailService.GetAsync(item.ReportCode);
                item.TotalReport = dto.returnMessage[0].reportCount;
                RefreshReportStatusViewModel.Progress = (double)currentCounter / totalReportCode;
                currentCounter++;
            }
        }).ContinueWith(T =>
        {
            UnSignItems = UnSignItems.Where(x => x.TotalReport > 0).ToList();
            IsRefreshing = false;
            IsBusy = false;
            RefreshingView = false;
            ShowNavigationPage = true;
            eventAggregator.GetEvent<OnOffNavigationPageEvent>().Publish(new OnOffNavigationPAgePayload { IsOn = !IsBusy });
        });

    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
