using Microsoft.AspNetCore.Authorization;

namespace WebApp_Security.Authorization
{
    public class HRManagerProbationRequirement:IAuthorizationRequirement
    {
        public HRManagerProbationRequirement(int probationMonths)
        {
            ProbationMonths = probationMonths;
        }

        public int ProbationMonths { get; }
    }
    public class HRManagerProbationHandler : AuthorizationHandler<HRManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == "Department" && c.Value == "HR") &&
                context.User.HasClaim(c => c.Type == "Manager" && c.Value == "true") &&
                context.User.HasClaim(c => c.Type == "DepartmentDate"))
            {
                var departmentDateClaim = context.User.FindFirst(c => c.Type == "DepartmentDate");
                if (DateTime.TryParse(departmentDateClaim?.Value, out var departmentDate))
                {
                    var monthsInDepartment = ((DateTime.Now - departmentDate).Days) / 30;
                    if (monthsInDepartment >= requirement.ProbationMonths)
                    {
                        context.Succeed(requirement);
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
