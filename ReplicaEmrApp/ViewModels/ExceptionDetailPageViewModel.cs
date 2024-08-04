using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using ShareResource.Models;

namespace MyApp.ViewModels;

public partial class ExceptionDetailPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    #endregion

    #region Property Member
    [ObservableProperty]
    ExceptionRecord exceptionRecord = new ExceptionRecord();
    #endregion

    #region Constructor
    public ExceptionDetailPageViewModel(INavigationService navigationService)
    {
        this.navigationService = navigationService;
    }
    #endregion

    #region Method Member
    #region Command Method
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public async void OnNavigatedTo(INavigationParameters parameters)
    {
        if(parameters.GetNavigationMode() == Prism.Navigation.NavigationMode.New)
        {
            if (parameters.ContainsKey("ExceptionRecord"))
            {
                ExceptionRecord = parameters.GetValue<ExceptionRecord>("ExceptionRecord");
                var exceptionJson = JsonConvert.SerializeObject(ExceptionRecord);
                await Clipboard.Default.SetTextAsync(exceptionJson);
            }
        }
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
