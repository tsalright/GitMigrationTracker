using System.Collections.Generic;
using System.Text;

namespace GitMigrationTracker.Models
{
    public class ProjectData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<RepoData> Repositories { get; }

        public ProjectData()
        {
            Repositories = new List<RepoData>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            Repositories.ForEach(r =>
                sb.AppendLine(
                    $"{Name},{r.RepoName},{r.DefaultBranch},{r.IsFork},{r.Size}"));
            return sb.ToString();
        }
    }
}