using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitMigrationTracker.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace GitMigrationTracker.Clients
{
    public interface IAdoClient
    {
        Task<List<ProjectData>> GetData();
        void SetPatAndOrg(string pat = null, string orgName = null);
    }

    public class AdoClient : IAdoClient
    {
        private readonly ILogger<AdoClient> _logger;
        private readonly IConfigurationRoot _config;
        private string _pat;

        private VssConnection _connection;

        public AdoClient(ILogger<AdoClient> logger, IConfigurationRoot config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<List<ProjectData>> GetData()
        {
            _logger.LogInformation("Starting Data Gathering");
            return await GetProjectData();
        }

        private async Task<List<ProjectData>> GetProjectData( string continuationToken = null)
        {
            using var projectClient = _connection.GetClient<ProjectHttpClient>();
            var projectDataList = new List<ProjectData>();
            var projects = await projectClient.GetProjects(ProjectState.All, continuationToken: continuationToken);
            using var gitClient = _connection.GetClient<GitHttpClient>();
            foreach(var project in projects)
            {
                var projectData = new ProjectData
                {
                    Name = project.Name,
                    Description = project.Description
                };
                var repos = await gitClient.GetRepositoriesAsync(project.Name);
                
                repos.ForEach(r =>
                {
                    projectData.Repositories.Add(new RepoData
                    {
                        DefaultBranch = r.DefaultBranch,
                        IsFork = r.IsFork,
                        RepoName = r.Name,
                        Size = r.Size
                    });
                });
                projectDataList.Add(projectData);
            }

            _logger.LogInformation($"Continuation Token: {projects.ContinuationToken}");
            if (!string.IsNullOrWhiteSpace(projects.ContinuationToken))
            {
                projectDataList.AddRange(await GetProjectData(projects.ContinuationToken));    
            }
            return projectDataList;
        }
        
        public void SetPatAndOrg(string pat = null, string orgName = null)
        {
            _pat = pat ?? _config["ADO:Pat"];
            var org = orgName ?? _config["ADO:Org"];
            
            var credentials = new VssBasicCredential(string.Empty, _pat);
            _connection = new VssConnection(new Uri($"https://dev.azure.com/{org}"), credentials);
        }
    }
}