using ProjectPlanner.Commands.Interfaces;
using ProjectPlanner.Domain.Models;

namespace ProjectPlanner.Commands.Implementations
{
    public class ProjectCommands : IProjectCommands
    {
        private readonly S3Storage _storage;

        public ProjectCommands(S3Storage storage)
        {
            _storage = storage;
        }

        public async Task CreateProject(Project project)
        {
            await _storage.SaveObject($"projects/{project.Id}", project);
        }

        public async Task UpdateProject(Project project)
        {
            await _storage.SaveObject($"projects/{project.Id}", project);
        }

        public async Task DeleteProject(int id)
        {
            await _storage.DeleteObject($"projects/{id}");
        }
    }
}