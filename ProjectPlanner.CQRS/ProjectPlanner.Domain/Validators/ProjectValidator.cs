using FluentValidation;
using ProjectPlanner.Domain.Models;

namespace ProjectPlanner.Domain.Validators
{
    public class ProjectValidator : AbstractValidator<Project>
    {
        public ProjectValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(500);
            RuleFor(x => x.StartDate).NotEmpty().LessThan(x => x.EndDate);
            RuleFor(x => x.EndDate).NotEmpty();
        }
    }
}