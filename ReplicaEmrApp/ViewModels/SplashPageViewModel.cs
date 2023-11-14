using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReplicaEmrApp.Helpers;
using ReplicaEmrApp.Models;
using ReplicaEmrApp.Services;
using ReplicaEmrApp.Views;
using ShareResource.Models;
using System.Diagnostics;
using System.Reflection;

namespace ReplicaEmrApp.ViewModels;

public partial class SplashPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly GlobalObject globalObject;
    private readonly ExceptionService exceptionService;
    #endregion

    #region Property Member
    #endregion

    #region Constructor
    public SplashPageViewModel(INavigationService navigationService, GlobalObject globalObject,
        ExceptionService exceptionService)
    {
        this.navigationService = navigationService;
        this.globalObject = globalObject;
        this.exceptionService = exceptionService;
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
        globalObject.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        try
        {
            List<ExceptionRecord> datas = StorageJSONService<List<ExceptionRecord>>.ReadFromFileAsync("data", "ExceptionRecord.json").Result;
            if (datas != null && datas.Count > 0)
            {
                await exceptionService.UploadAsync(datas);
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        GlobalObject gObject = await StorageJSONService<GlobalObject>.ReadFromFileAsync("data", "GlobalObject.json");
        if (gObject == null || string.IsNullOrEmpty(gObject.JSESSIONID))
        {
            await navigationService.NavigateAsync(MagicValueHelper.LoginPage);
        }
        else
        {
            globalObject.Copy(gObject, globalObject);
            await navigationService.NavigateAsync(MagicValueHelper.HomePage);
        }

    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
