using Microsoft.AspNetCore.Mvc;
using ProjectPlanner.Domain.Models;
using ProjectPlanner.Commands.Interfaces;
using ProjectPlanner.Queries.Interfaces;

namespace ProjectPlanner.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectPlanController : ControllerBase
    {
        private readonly IProjectCommands _projectCommands;
        private readonly IProjectQueries _projectQueries;
        private readonly IActivityCommands _activityCommands;
        private readonly IActivityQueries _activityQueries;

        public ProjectPlanController(
            IProjectCommands projectCommands,
            IProjectQueries projectQueries,
            IActivityCommands activityCommands,
            IActivityQueries activityQueries)
        {
            _projectCommands = projectCommands;
            _projectQueries = projectQueries;
            _activityCommands = activityCommands;
            _activityQueries = activityQueries;
        }

        [HttpGet("projects")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            var projects = await _projectQueries.GetAllProjects();
            return Ok(projects);
        }

        [HttpGet("projects/{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _projectQueries.GetProjectById(id);
            if (project == null)
                return NotFound();
            return Ok(project);
        }

        [HttpGet("projects/{id}/activities")]
        public async Task<ActionResult<IEnumerable<Activity>>> GetActivities(int id)
        {
            var activities = await _activityQueries.GetActivitiesByProjectId(id.ToString());
            return Ok(activities);
        }

        [HttpPost("projects")]
        public async Task<ActionResult<Project>> CreateProject(Project project)
        {
            await _projectCommands.CreateProject(project);
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        [HttpPost("activities")]
        public async Task<ActionResult<Activity>> CreateActivity(Activity activity)
        {
            await _activityCommands.CreateActivity(activity);
            return CreatedAtAction(nameof(GetActivities), new { id = activity.ProjectId }, activity);
        }

        [HttpPut("projects/{id}")]
        public async Task<IActionResult> UpdateProject(int id, Project project)
        {
            if (id != project.Id)
                return BadRequest();

            await _projectCommands.UpdateProject(project);
            return NoContent();
        }

        [HttpPut("activities/{id}")]
        public async Task<IActionResult> UpdateActivity(string id, Activity activity)
        {
            if (id != activity.ActivityId)
                return BadRequest();

            await _activityCommands.UpdateActivity(activity);
            return NoContent();
        }

        [HttpDelete("projects/{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            await _projectCommands.DeleteProject(id);
            return NoContent();
        }

        [HttpDelete("activities/{id}")]
        public async Task<IActionResult> DeleteActivity(string id)
        {
            await _activityCommands.DeleteActivity(id);
            return NoContent();
        }
    }
}