using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ReplicaEmrApp.ViewModels;

public partial class MDPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly IPageDialogService dialogService;
    #endregion

    #region Property Member
    #endregion

    #region Constructor
    public MDPageViewModel(INavigationService navigationService,
        IPageDialogService dialogService)
    {
        this.navigationService = navigationService;
        this.dialogService = dialogService;
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    public async Task DoLogoutAsync()
    {
        bool result = await dialogService.DisplayAlertAsync("確定要登出嗎?", "登出", "確定", "取消");

        if (result)
        {
            await navigationService.NavigateAsync("/LoginPage");
        }
    }

    [RelayCommand]
    public async Task DoHomeAsync()
    {
        await navigationService.NavigateAsync("/MDPage/NaviPage/HomePage");
    }
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
