using JobMarketplace.SharedKernel.Results;

namespace JobMarketplace.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result) => result switch
    {
        { IsSuccess: true } => Results.Ok(result.Value),
        { Error.Code: "NotFound" } => Results.NotFound(new { result.Error.Code, result.Error.Description }),
        { Error.Code: "Unauthorized" } => Results.Forbid(),
        { Error.Code: "Conflict" } => Results.Conflict(new { result.Error.Code, result.Error.Description }),
        { Error.Code: "Validation" } => Results.UnprocessableEntity(new { result.Error.Code, result.Error.Description }),
        _ => Results.Problem()
    };

    public static IResult ToHttpResult(this Result result) => result switch
    {
        { IsSuccess: true } => Results.NoContent(),
        { Error.Code: "NotFound" } => Results.NotFound(new { result.Error.Code, result.Error.Description }),
        { Error.Code: "Unauthorized" } => Results.Forbid(),
        { Error.Code: "Conflict" } => Results.Conflict(new { result.Error.Code, result.Error.Description }),
        { Error.Code: "Validation" } => Results.UnprocessableEntity(new { result.Error.Code, result.Error.Description }),
        _ => Results.Problem()
    };
}