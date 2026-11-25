public static class Errors
{
    // A 400 answer with the same shape as the automatic validation errors.
    public static IResult BadRequest(string message) =>
        Results.ValidationProblem(new Dictionary<string, string[]> { ["error"] = [message] });
}
