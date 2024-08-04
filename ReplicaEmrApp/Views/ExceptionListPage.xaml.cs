using ReplicaEmrApp.ViewModels;

namespace ReplicaEmrApp.Views;

public partial class ExceptionListPage : ContentPage
{
    public ExceptionListPage(ExceptionListPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}

