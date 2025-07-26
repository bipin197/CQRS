using ProjectPlanner.Domain.Models;

namespace ProjectPlanner.Commands.Interfaces
{
    public interface IActivityCommands
    {
        Task CreateActivity(Activity activity);
        Task UpdateActivity(Activity activity);
        Task DeleteActivity(string activityId);
    }
}