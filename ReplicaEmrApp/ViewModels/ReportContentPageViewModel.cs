using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReplicaEmrApp.Dto;
using ReplicaEmrApp.Models;

namespace ReplicaEmrApp.ViewModels;

public partial class ReportContentPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly GlobalObject globalObject;
    ReportData reportData;
    #endregion

    #region Property Member
    [ObservableProperty]
    string url = string.Empty;
    #endregion

    #region Constructor
    public ReportContentPageViewModel(INavigationService navigationService,GlobalObject globalObject)
    {
        this.navigationService = navigationService;
        this.globalObject = globalObject;
    }
    #endregion

    #region Method Member
    #region Command Method
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {

        if (parameters.ContainsKey("ReportData")&& parameters.ContainsKey("ReportCode"))
        {
            string reportId = parameters.GetValue<ReportData>("ReportData").reportId;
            string reportCode = parameters.GetValue<string>("ReportCode");
            string reportContentEndpoint = $"http://office.exentric.com.tw:8080/webemr/API/API4L000.jsp;jsessionid={globalObject.JSESSIONID}?pageid=API4L004&reportCode={reportCode}&reportId={reportId}&tableSource=2&format=HTML&sessionId={globalObject.JSESSIONID}";
            Url = reportContentEndpoint;
        }
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
