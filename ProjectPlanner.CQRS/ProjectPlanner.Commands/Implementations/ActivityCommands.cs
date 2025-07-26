using ProjectPlanner.Commands.Interfaces;
using ProjectPlanner.Domain.Models;

namespace ProjectPlanner.Commands.Implementations
{
    public class ActivityCommands : IActivityCommands
    {
        private readonly S3Storage _storage;

        public ActivityCommands(S3Storage storage)
        {
            _storage = storage;
        }

        public async Task CreateActivity(Activity activity)
        {
            await _storage.SaveObject($"activities/{activity.ActivityId}", activity);
        }

        public async Task UpdateActivity(Activity activity)
        {
            await _storage.SaveObject($"activities/{activity.ActivityId}", activity);
        }

        public async Task DeleteActivity(string activityId)
        {
            await _storage.DeleteObject($"activities/{activityId}");
        }
    }
}