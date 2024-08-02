using ReplicaEmrApp.ViewModels;

namespace ReplicaEmrApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();

        //this.Disappearing += HomePage_Disappearing;
    }

    private void HomePage_Disappearing(object sender, EventArgs e)
    {
        //HomePageViewModel viewModel = (HomePageViewModel)this.BindingContext;
        //viewModel.Disappearing();
    }
}

