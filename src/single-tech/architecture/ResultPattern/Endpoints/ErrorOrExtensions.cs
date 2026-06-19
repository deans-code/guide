using ErrorOr;

public static class ErrorOrExtensions
{
    public static IResult ToProblemResult(this List<Error> errors)
    {
        var first = errors[0];

        var status = first.Type switch
        {
            ErrorType.NotFound => 404,
            ErrorType.Validation => 422,
            ErrorType.Conflict => 409,
            ErrorType.Unauthorized => 401,
            ErrorType.Forbidden => 403,
            _ => 500
        };

        return Results.Problem(
            title: first.Description,
            statusCode: status,
            extensions: errors.Count > 1
                ? new Dictionary<string, object?> { ["errors"] = errors.Select(e => e.Description).ToList() }
                : null);
    }
}
