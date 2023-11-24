using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;

namespace ReplicaEmrApp.ViewModels;

public partial class SettingPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly IStorageJSONService<SettingModel> storageJSONService;
    private readonly IPageDialogService pageDialogService;
    #endregion

    #region Property Member
    [ObservableProperty]
    SettingModel settingModel = new SettingModel();
    #endregion

    #region Constructor
    public SettingPageViewModel(INavigationService navigationService,
        IStorageJSONService<SettingModel> storageJSONService,
        IPageDialogService pageDialogService)
    {
        this.navigationService = navigationService;
        this.storageJSONService = storageJSONService;
        this.pageDialogService = pageDialogService;
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    public async Task SaveSettingCommand()
    {
        await storageJSONService.WriteToDataFileAsync(
            MagicValueHelper.DataPath, MagicValueHelper.SettingModelFilename, SettingModel);
        await pageDialogService.DisplayAlertAsync("通知", "設定已經儲存完成", "確定");
        await navigationService.GoBackAsync();
    }
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public async void OnNavigatedTo(INavigationParameters parameters)
    {
        SettingModel = await storageJSONService.ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.SettingModelFilename);
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
