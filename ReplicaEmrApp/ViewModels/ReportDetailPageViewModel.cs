using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using ReplicaEmrApp.Services;

namespace ReplicaEmrApp.ViewModels;

public partial class ReportDetailPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly ReportDetailService reportDetailService;
    string reportCode;
    #endregion

    #region Property Member
    [ObservableProperty]
    string pageTitle = string.Empty;

    [ObservableProperty]
    List<ReportData> reportDatas = new();

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    string summary = string.Empty;
    #endregion

    #region Constructor
    public ReportDetailPageViewModel(INavigationService navigationService,
        ReportDetailService reportDetailService)
    {
        this.navigationService = navigationService;
        this.reportDetailService = reportDetailService;
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    public async Task TapItem(ReportData reportData)
    {
        NavigationParameters parameters = new();
        parameters.Add("ReportData", reportData);
        parameters.Add("ReportCode", reportCode);
        await navigationService.NavigateAsync(MagicValueHelper.ReportContentPage, parameters);
    }
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {

    }

    public async void OnNavigatedTo(INavigationParameters parameters)
    {
        if (parameters.ContainsKey("UnsignReport"))
        {
            var unsignReport = parameters.GetValue<UnSignItem>("UnsignReport");
            PageTitle = unsignReport.ReportName;
            reportCode = unsignReport.ReportCode;
            IsBusy = true;

            Summary = $"共 {unsignReport.TotalReport} 筆";
            ReportDatas.Clear();
            var items = await reportDetailService.GetAsync(unsignReport.ReportCode);
            ReportDatas = items.returnMessage.FirstOrDefault(x => x.reportCode == unsignReport.ReportCode).reportDatas;
            IsBusy = false;
        }
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
