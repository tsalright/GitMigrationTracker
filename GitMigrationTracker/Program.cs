using System;
using System.IO;
using GitMigrationTracker.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GitMigrationTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            
            var serviceProvider = new ServiceCollection()
                .AddSingleton(config)
                .AddLogging(configure => configure.AddConsole())
                .AddSingleton<IAppHost, AppHost>()
                .AddSingleton<IAdoClient, AdoClient>()
                .BuildServiceProvider();
            
            var appHost = serviceProvider.GetService<IAppHost>();
            appHost?.Run();
        }
        
        // public void SetPatAndOrg(string pat, string orgName)
        // {
        //     _pat = pat ?? _config["ADO:Pat"];
        //     var org = orgName ?? _config["ADO:Org"];
        //     
        //     var credentials = new VssBasicCredential(string.Empty, _pat);
        //     _connection = new VssConnection(new Uri($"https://dev.azure.com/{org}"), credentials);
        // }
    }
}
