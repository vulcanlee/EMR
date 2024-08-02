using ReplicaEmrApp.ViewModels;

namespace ReplicaEmrApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();

        this.HideSoftInputOnTapped = true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoginPageViewModel viewModel = (LoginPageViewModel)this.BindingContext;
        viewModel.TurnOffSoftKeyboard = TurnOffSoftKeyboard;
    }

    void TurnOffSoftKeyboard()
    {
        this.HideSoftInputOnTapped = true;
    }
}

