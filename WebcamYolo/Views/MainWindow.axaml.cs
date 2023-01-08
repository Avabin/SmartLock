using Avalonia.ReactiveUI;
using WebcamYolo.ViewModels;

namespace WebcamYolo.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }
}