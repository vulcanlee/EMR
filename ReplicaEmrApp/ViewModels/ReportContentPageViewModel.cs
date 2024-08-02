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
    UnsignReportData reportData;
    #endregion

    #region Property Member
    [ObservableProperty]
    string htmlContent = string.Empty;
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

        if (parameters.ContainsKey("html"))
        {
            HtmlContent = parameters.GetValue<string>("html");
        }
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
