using ProjectPlanner.Domain.Models;

namespace ProjectPlanner.Commands.Interfaces
{
    public interface IProjectCommands
    {
        Task CreateProject(Project project);
        Task UpdateProject(Project project);
        Task DeleteProject(int id);
    }
}