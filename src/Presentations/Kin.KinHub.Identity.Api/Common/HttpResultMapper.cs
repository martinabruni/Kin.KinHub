using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kin.KinHub.Identity.Api.Common;

internal static class HttpResultMapper
{
    internal static IActionResult ToActionResult<T>(Result<T> result) =>
        result.Status switch
        {
            ResultStatus.Success => new OkObjectResult(result.Value),
            ResultStatus.NotFound => new NotFoundObjectResult(new { message = result.Message }),
            ResultStatus.Conflict => new ConflictObjectResult(new { message = result.Message }),
            ResultStatus.ValidationError => new BadRequestObjectResult(new { message = result.Message }),
            ResultStatus.Unauthorized => new UnauthorizedObjectResult(new { message = result.Message }),
            _ => new ObjectResult(new { message = result.Message }) { StatusCode = StatusCodes.Status500InternalServerError },
        };
}
