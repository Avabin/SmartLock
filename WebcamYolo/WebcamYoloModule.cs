using Autofac;
using ReactiveUI;
using WebcamYolo.Services;
using WebcamYolo.ViewModels;
using WebcamYolo.Views;

namespace WebcamYolo;

public class WebcamYoloModule : Module
{

    public WebcamYoloModule()
    {
    }
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MediaPlayerFactory>();
        builder.RegisterType<MainWindowViewModel>().AsSelf().SingleInstance();
        builder.RegisterType<MainWindow>().As<IViewFor<MainWindowViewModel>>().AsSelf().SingleInstance();
    }
}