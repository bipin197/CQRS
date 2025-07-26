using ProjectPlanner.Domain.Models;
using ProjectPlanner.Queries.Interfaces;

namespace ProjectPlanner.Queries.Implementations
{
    public class ActivityQueries : IActivityQueries
    {
        private readonly InMemoryStore _store;

        public ActivityQueries(InMemoryStore store)
        {
            _store = store;
        }

        public Task<IEnumerable<Activity>> GetActivitiesByProjectId(string projectId)
        {
            var activities = _store.GetActivities().Where(a => a.ProjectId == projectId);
            return Task.FromResult(activities);
        }

        public Task<Activity?> GetActivityById(string activityId)
        {
            var activity = _store.GetActivities().FirstOrDefault(a => a.ActivityId == activityId);
            return Task.FromResult(activity);
        }
    }
}