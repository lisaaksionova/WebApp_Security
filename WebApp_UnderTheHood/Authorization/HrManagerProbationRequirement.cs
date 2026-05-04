using Microsoft.AspNetCore.Authorization;

namespace WebApp_UnderTheHood.Authorization;

public class HrManagerProbationRequirement : IAuthorizationRequirement
{
    public int ProbationMonths { get; }

    public HrManagerProbationRequirement(int probationMonths)
    {
        ProbationMonths = probationMonths;
    }
}

public class HrManagerProbationRequirementHandler : AuthorizationHandler<HrManagerProbationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HrManagerProbationRequirement requirement)
    {
        if (!context.User.HasClaim(x => x.Type == "EmploymentDate"))
            return Task.CompletedTask;

        if (DateTime.TryParse(context.User.FindFirst(x => x.Type == "EmploymentDate")?.Value,
                out DateTime employmentDate))
        {
            var period = DateTime.Now - employmentDate;
            if (period.Days > requirement.ProbationMonths * 30)
            {
                context.Succeed(requirement);
            }
        }
        return Task.CompletedTask;
    }
}