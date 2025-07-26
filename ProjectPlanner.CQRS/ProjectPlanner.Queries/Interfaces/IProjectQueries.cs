using ProjectPlanner.Domain.Models;

namespace ProjectPlanner.Queries.Interfaces
{
    public interface IProjectQueries
    {
        Task<IEnumerable<Project>> GetAllProjects();
        Task<Project?> GetProjectById(int id);
    }
}