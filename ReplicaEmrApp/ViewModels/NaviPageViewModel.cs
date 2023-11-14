using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReplicaEmrApp.Events;

namespace ReplicaEmrApp.ViewModels;

public partial class NaviPageViewModel : ObservableObject, INavigatedAware
{
    #region Field Member
    private readonly INavigationService navigationService;
    private readonly IEventAggregator eventAggregator;
    #endregion

    #region Property Member
    [ObservableProperty]
    bool show = false;
    #endregion

    #region Constructor
    public NaviPageViewModel(INavigationService navigationService,IEventAggregator eventAggregator)
    {
        this.navigationService = navigationService;
        this.eventAggregator = eventAggregator;
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
        eventAggregator.GetEvent<OnOffNavigationPageEvent>().Subscribe(x =>
        {
            Show = x.IsOn;
        });
    }
    #endregion

    #region Other Method
    #endregion
    #endregion
}
