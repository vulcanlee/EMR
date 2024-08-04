using MyApp.ViewModels;

namespace MyApp.Views;

public partial class ExceptionDetailPage : ContentPage
{
    public ExceptionDetailPage(ExceptionDetailPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}

