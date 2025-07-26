using ProjectPlanner.Domain.Models;
using ProjectPlanner.Queries.Interfaces;

namespace ProjectPlanner.Queries.Implementations
{
    public class ProjectQueries : IProjectQueries
    {
        private readonly InMemoryStore _store;

        public ProjectQueries(InMemoryStore store)
        {
            _store = store;
        }

        public Task<IEnumerable<Project>> GetAllProjects()
        {
            return Task.FromResult(_store.GetProjects());
        }

        public Task<Project?> GetProjectById(int id)
        {
            var project = _store.GetProjects().FirstOrDefault(p => p.Id == id);
            return Task.FromResult(project);
        }
    }
}