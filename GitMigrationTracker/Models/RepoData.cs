namespace GitMigrationTracker.Models
{
    public class RepoData
    {
        public string RepoName { get; set; }
        public string DefaultBranch { get; set; }
        public bool IsFork { get; set; }
        public long? Size { get; set; }
        
    }
}