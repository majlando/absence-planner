using Business;

public static class RuleEndpoints
{
    public static void MapRuleEndpoints(this WebApplication app)
    {
        // Only admins may see the rules.
        var rules = app.MapGroup("/api/rules").RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        rules.MapGet("", async (RuleService service) =>
                (await service.GetAllAsync()).Select(r => new RuleResponse(r.Id, r.Name, r.Value, r.StartDate)))
            .Produces<IEnumerable<RuleResponse>>();
    }
}

public record RuleResponse(Guid Id, string Name, decimal Value, DateOnly StartDate);
