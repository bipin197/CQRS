using ProjectPlanner.Domain.Models;

namespace ProjectPlanner.Queries.Interfaces
{
    public interface IActivityQueries
    {
        Task<IEnumerable<Activity>> GetActivitiesByProjectId(string projectId);
        Task<Activity?> GetActivityById(string activityId);
    }
}