using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReplicaEmrApp.Helpers;
using ShareResource.Models;
using System.Collections.ObjectModel;

namespace ReplicaEmrApp.ViewModels;

public partial class ExceptionListPageViewModel : ObservableObject, INavigatedAware
{
    private readonly INavigationService navigationService;
    private readonly IStorageJSONService<List<ExceptionRecord>> storageJSONService;
    #region Field Member
    #endregion

    #region Property Member
    [ObservableProperty]
    ExceptionRecord exceptionRecord = new ExceptionRecord();

    [ObservableProperty]
    string summary = "共0筆";

    [ObservableProperty]
    ObservableCollection<ExceptionRecord> exceptionRecordList = new();
    #endregion

    #region Constructor
    public ExceptionListPageViewModel(INavigationService navigationService,
        IStorageJSONService<List<ExceptionRecord>> storageJSONService)
    {
        this.navigationService = navigationService;
        this.storageJSONService = storageJSONService;
    }
    #endregion

    #region Method Member
    #region Command Method
    [RelayCommand]
    void ThrowException()
    {
        throw new Exception("Test Exception");
    }

    [RelayCommand]
    void ShowExceptionDetail(ExceptionRecord exceptionRecord)
    {
        NavigationParameters parameters = new NavigationParameters();
        parameters.Add("ExceptionRecord", exceptionRecord);
        navigationService.NavigateAsync("ExceptionDetailPage", parameters);
    }
    #endregion

    #region Navigation Event
    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    public async void OnNavigatedTo(INavigationParameters parameters)
    {
        await ReloadDataAsync();
    }

    #endregion

    #region Other Method

    async Task ReloadDataAsync()
    {
        ExceptionRecordList.Clear();
        List<ExceptionRecord> datas = await storageJSONService
            .ReadFromFileAsync(MagicValueHelper.DataPath, MagicValueHelper.ExceptionRecordFilename);
        if (datas != null && datas.Count > 0)
        {
            foreach (var item in datas)
            {
                ExceptionRecordList.Add(item);
            }
        }
    }
    #endregion
    #endregion
}
