using Autofac;
using ReactiveUI;
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
        builder.RegisterType<MainWindowViewModel>().As<IScreen>().AsSelf().SingleInstance();
        builder.RegisterType<MainWindow>().As<IViewFor<MainWindowViewModel>>().AsSelf().SingleInstance();
    }
}