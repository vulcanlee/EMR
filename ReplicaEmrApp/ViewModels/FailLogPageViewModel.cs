using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Models;
using ReplicaEmrApp.Services;
using System.Collections.ObjectModel;

namespace ReplicaEmrApp.ViewModels;

public partial class FailLogPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly FailLogService failLogService;
    private readonly GlobalObject globalObject;
    private readonly CheckSessionService checkSessionService;
    #endregion

    #region Property Member
    [ObservableProperty]
    bool isRefreshing = false;

    [ObservableProperty]
    OperlogDto operlogDto = new OperlogDto();

    [ObservableProperty]
    string summary = "共0筆";

    [ObservableProperty]
    ObservableCollection<OperlogDto> logList = new();

    [ObservableProperty]
    bool isBusy = true;

    #endregion

    #region Constructor
    public FailLogPageViewModel(INavigationService navigationService,
        FailLogService failLogService, GlobalObject globalObject,
       CheckSessionService checkSessionService)
    {
        this.navigationService = navigationService;
        this.failLogService = failLogService;
        this.globalObject = globalObject;
        this.checkSessionService = checkSessionService;
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    public async Task RefreshAsync()
    {
        //IsRefreshing = true;
        await ReloadAsync();
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
        await Task.Yield();
        IsBusy = true;
        (ApiResultModel<OperlogDto> apiResult, var specifyLog) = await failLogService.ListPostAsync();
        if (await checkSessionService.ReloadDataAsync(apiResult, specifyLog)) return;

        //LogList = new ObservableCollection<OperlogDto>(apiResult.data);
        LogList.Clear();
        foreach (var item in apiResult.data)
        {
            LogList.Add(item);
            Summary = $"共 {LogList.Count} 筆";
            await Task.Yield();
        }

        IsRefreshing = false;
        IsBusy = false;
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
