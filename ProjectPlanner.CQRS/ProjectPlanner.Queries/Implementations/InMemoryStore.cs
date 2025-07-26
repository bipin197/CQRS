using Microsoft.Extensions.Caching.Memory;
using ProjectPlanner.Domain.Models;

namespace ProjectPlanner.Queries.Implementations
{
    public class InMemoryStore
    {
        private readonly IMemoryCache _cache;
        private const string PROJECT_KEY = "projects";
        private const string ACTIVITY_KEY = "activities";

        public InMemoryStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IEnumerable<Project> GetProjects()
        {
            return _cache.Get<List<Project>>(PROJECT_KEY) ?? new List<Project>();
        }

        public IEnumerable<Activity> GetActivities()
        {
            return _cache.Get<List<Activity>>(ACTIVITY_KEY) ?? new List<Activity>();
        }

        public void UpdateProjects(IEnumerable<Project> projects)
        {
            _cache.Set(PROJECT_KEY, projects.ToList());
        }

        public void UpdateActivities(IEnumerable<Activity> activities)
        {
            _cache.Set(ACTIVITY_KEY, activities.ToList());
        }
    }
}