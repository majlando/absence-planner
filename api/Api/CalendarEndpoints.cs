using System.ComponentModel.DataAnnotations;
using Business;

public static class CalendarEndpoints
{
    public static void MapCalendarEndpoints(this WebApplication app)
    {
        var calendar = app.MapGroup("/api/calendar");

        // All days off in a year. Everyone who is logged in may see them.
        calendar.MapGet("/{year:int}", async (int year, CalendarService service) =>
                (await service.GetDaysOffAsync(year)).Select(d => new DayOffResponse(d.Date, d.Name, d.CompanyDayId)))
            .Produces<IEnumerable<DayOffResponse>>();

        // Only admins may add and remove company days off.
        var companyDays = calendar.MapGroup("/company-days").RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        companyDays.MapPost("", async (CompanyDayRequest request, CalendarService service) =>
                await service.AddCompanyDayAsync(request.Date, request.Name) is { } error
                    ? Errors.BadRequest(error)
                    : Results.NoContent())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();

        companyDays.MapDelete("/{id:guid}", async (Guid id, CalendarService service) =>
                await service.RemoveCompanyDayAsync(id) ? Results.NoContent() : Results.NotFound())
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}

// CompanyDayId is empty for public holidays.
public record DayOffResponse(DateOnly Date, string Name, Guid? CompanyDayId);

public record CompanyDayRequest(DateOnly Date, [Required] string Name);
