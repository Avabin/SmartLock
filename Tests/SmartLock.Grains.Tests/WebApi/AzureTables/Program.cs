﻿// Unit tests entrypoint based on SmartLock.WebApi.Program

using System.Reflection;
using SmartLock.Client;
using SmartLock.Client.NotificationHub;
using SmartLock.WebApi.Hubs;
using Tests.Shared;

namespace SmartLock.Grains.Tests.WebApi.AzureTables
{
    public partial class Program : IHasClusterClient
    {
        public static IClusterClient? Client => IHasClusterClient.Client;

        public static void Main(string[] args)
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var assemblyDirectory = new DirectoryInfo(assemblyPath);
            // set the content root path to the SmartLock.Grains.Tests project, WebApi/Unit subdirectory
            // so that the web api project can find the appsettings.json and appsettings.Development.json files
            var contentRoot = Path.Combine(assemblyDirectory.Parent?.Parent?.Parent?.FullName ?? throw new InvalidOperationException("Cannot find the content root!"), "WebApi", "Unit");
            // replace `--contentRoot` from the command line arguments
            var newArgs = args.Where(x => !x.StartsWith("--contentRoot")).ToList();
            newArgs.Add($"--contentRoot={contentRoot}");
            

            var builder = WebApplication.CreateBuilder(newArgs.ToArray());
            // get the path to the web api project

            builder.Services.AddSingleton(Mongo.Program.Client ??
                                          throw new InvalidOperationException(
                                              "Cluster client is not implemented! Copy client from cluster into IHasClusterClient.Client!"));

            // Add services to the container.
            builder.Services.AddSingleton<NotificationsHubService>();
            builder.Services.AddSignalR();
            builder.Services.AddControllers();

            var app = builder.Build();

            app.MapControllers();
            app.MapHub<NotificationsHub>(INotificationsHubClient.HubPath);

            app.Run();
        }
    }
}