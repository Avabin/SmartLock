using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Avalonia;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PierogiesBotUI.Features.Settings;
using ReactiveUI;
using Serilog;
using Serilog.Extensions.Logging;
using Splat;
using Splat.Autofac;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PierogiesBotUI;

public static class BuilderExtensions
{
    private static string OutputTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] ({SourceContext}.{Method}) {Message:lj}{NewLine}{Exception}";

    private static LoggerConfiguration LoggerConfiguration => new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.Console(outputTemplate: OutputTemplate)
        .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7,
            rollOnFileSizeLimit: true, fileSizeLimitBytes: 10000000, outputTemplate: OutputTemplate);

    private static ILoggerFactory LoggerFactory => new LoggerFactory(new List<ILoggerProvider>
    {
        new SerilogLoggerProvider(LoggerConfiguration.CreateLogger())
    });

    public static AppBuilder UseAutofac(this AppBuilder appBuilder)
    {
        var builder = new ContainerBuilder();
        builder.RegisterInstance(LoggerFactory).As<ILoggerFactory>().SingleInstance();
        builder.RegisterGeneric((context, types) =>
        {
            var loggerFactory = context.Resolve<ILoggerFactory>();
            var openGenericMethod = typeof(LoggerFactoryExtensions).GetMethods( BindingFlags.Public | BindingFlags.Static)
                .First(x => x is { Name: nameof(LoggerFactoryExtensions.CreateLogger), IsGenericMethod: true});

            var closedGenericMethod = openGenericMethod.MakeGenericMethod(types[0]);
            var logger = closedGenericMethod.Invoke(null, new object[] {loggerFactory});
            return logger;
        }).As(typeof(ILogger<>)).InstancePerDependency();
        var autofacResolver = builder.UseAutofacDependencyResolver();

        builder.RegisterInstance(autofacResolver);

        autofacResolver.InitializeSplat();
        autofacResolver.InitializeReactiveUI();

        builder.RegisterModule(new PierogiesBotModule());
        builder.RegisterType<AvaloniaActivationForViewFetcher>().As<IActivationForViewFetcher>().SingleInstance();
        builder.RegisterType<AutoDataTemplateBindingHook>().As<IPropertyBindingHook>().SingleInstance();
        builder.RegisterInstance(MessageBus.Current).As<IMessageBus>();
        var container = builder.Build();
        autofacResolver.SetLifetimeScope(container);
        RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;

        return appBuilder;
    }
}