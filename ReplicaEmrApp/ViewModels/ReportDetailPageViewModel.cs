using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using ReplicaEmrApp.Services;
using System.Collections.ObjectModel;

namespace ReplicaEmrApp.ViewModels;

public partial class ReportDetailPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly UnsignService unsignService;
    private readonly DocumentReportService documentReportService;
    private readonly CheckSessionService checkSessionService;
    private readonly ParameterService parameterService;
    string reportCode;
    #endregion

    #region Property Member
    [ObservableProperty]
    string pageTitle = string.Empty;

    [ObservableProperty]
    ObservableCollection<UnsignReportData> reportDatas = new();

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    string summary = "共0筆";
    #endregion

    #region Constructor
    public ReportDetailPageViewModel(INavigationService navigationService,
        UnsignService unsignService, DocumentReportService documentReportService,
        CheckSessionService checkSessionService, ParameterService parameterService)
    {
        this.navigationService = navigationService;
        this.unsignService = unsignService;
        this.documentReportService = documentReportService;
        this.checkSessionService = checkSessionService;
        this.parameterService = parameterService;
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    public async Task TapItem(UnsignReportData reportData)
    {
        if (await parameterService.GetShowReportContentAsync())
        {
            (string html, var specifyLog) = await documentReportService.GetAsync(reportData.docId);
            try {
                var apiResult = JsonConvert.DeserializeObject<ApiResultModel<string>>(html);
                if (await checkSessionService.ReloadDataAsync(apiResult, specifyLog)) return;
            }
            catch (Exception ex)
            {
            }

            NavigationParameters parameters = new();
            parameters.Add("html", html);
            await navigationService.NavigateAsync(MagicValueHelper.ReportContentPage, parameters);
        }
    }

    [RelayCommand]
    public void Empty()
    {
    }
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {

    }

    public async void OnNavigatedTo(INavigationParameters parameters)
    {
        IsBusy = true;
        await Task.Yield();
        if (parameters.ContainsKey("UnsignReport"))
        {
            List<UnsignReportData> unsignReport = parameters.GetValue<List<UnsignReportData>>("UnsignReport");
            PageTitle = unsignReport.FirstOrDefault()?.frmNm;
            reportCode = unsignReport.FirstOrDefault()?.frmCode;

            ReportDatas.Clear();
            foreach (var item in unsignReport)
            {
                ReportDatas.Add(item);
                Summary = $"共 {ReportDatas.Count} 筆";
                await Task.Yield();
            }
        }
        IsBusy = false;

    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
