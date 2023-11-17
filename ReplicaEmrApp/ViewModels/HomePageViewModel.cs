using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Services;
using ReplicaEmrApp.Events;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using ReplicaEmrApp.Services;

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
    #endregion

    #region Property Member
    [ObservableProperty]
    List<UnSignItem> unSignItems = new();

    [ObservableProperty]
    UnSignItem selectedItem = new();

    [ObservableProperty]
    bool isRefreshing = true;
    [ObservableProperty]
    bool isBusy = false;
    #endregion

    #region Constructor
    public HomePageViewModel(INavigationService navigationService,GlobalObject globalObject,ReportDetailService reportDetailService,
        IEventAggregator eventAggregator,ReportCodeService reportCodeService,IPageDialogService dialogService)
    {
        this.navigationService = navigationService;
        this.globalObject = globalObject;
        this.reportDetailService = reportDetailService;
        this.eventAggregator = eventAggregator;
        this.reportCodeService = reportCodeService;
        this.dialogService = dialogService;
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
        await navigationService.NavigateAsync("ReportDetailPage", parameters);
    }

    [RelayCommand]
    public async Task DoLogoutAsync()
    {
        globalObject.CleanUp();
        await StorageJSONService<GlobalObject>
            .WriteToDataFileAsync(MagicValueHelper.DataPath, MagicValueHelper.GlobalObjectFilename,
            globalObject);

        bool result = await dialogService.DisplayAlertAsync("確定要登出嗎?", "登出", "確定", "取消");

        if (result)
        {
            await navigationService.NavigateAsync("/LoginPage");
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

        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        IsBusy = true;
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
        Task.Run(async () =>
        {
            foreach (var item in UnSignItems)
            {
                var dto = await reportDetailService.GetAsync(item.ReportCode);
                item.TotalReport = dto.returnMessage[0].reportCount;
            }
        }).ContinueWith(T =>
        {
            UnSignItems = UnSignItems.Where(x => x.TotalReport > 0).ToList();
            IsRefreshing = false;
            IsBusy = false;
            eventAggregator.GetEvent<OnOffNavigationPageEvent>().Publish(new OnOffNavigationPAgePayload { IsOn = !IsBusy });


        });

    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
