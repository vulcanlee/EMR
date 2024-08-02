using ReplicaEmrApp.ViewModels;

namespace ReplicaEmrApp.Views;

public partial class SettingPage : ContentPage
{
    public SettingPage()
    {
        InitializeComponent();

        this.HideSoftInputOnTapped = true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        SettingPageViewModel viewModel = (SettingPageViewModel)this.BindingContext;
        viewModel.TurnOffSoftKeyboard = TurnOffSoftKeyboard;
    }

    void TurnOffSoftKeyboard()
    {
        this.HideSoftInputOnTapped = true;
    }
}

