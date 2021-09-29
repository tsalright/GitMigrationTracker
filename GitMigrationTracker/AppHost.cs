using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using GitMigrationTracker.Clients;
using GitMigrationTracker.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GitMigrationTracker
{
    public interface IAppHost
    {
        void Run();
    }

    public class AppHost : IAppHost
    {
        private IAdoClient AdoClient { get; }
        private ILogger Logger { get; }

        public AppHost(IAdoClient adoClient, ILogger<AppHost> logger)
        {
            AdoClient = adoClient;
            Logger = logger;
        }

        public void Run()
        {
            // Self Cleaning
            var fileName = "ADOProjectData.csv";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            
            AdoClient.SetPatAndOrg();
            var data = AdoClient.GetData().Result;
            Logger.LogInformation($"Total Projects Scanned: {data.Count}");
            foreach (var projectData in data)
            {
                File.AppendAllText(fileName, projectData.ToString());
            }
            
        }
    }
}