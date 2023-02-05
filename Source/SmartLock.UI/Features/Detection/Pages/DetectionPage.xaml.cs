using SmartLock.UI.Features.Detection.ViewModels;

namespace SmartLock.UI.Features.Detection.Pages;

public partial class DetectionPage : ContentPage
{
    public DetectionPage(DetectionViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}