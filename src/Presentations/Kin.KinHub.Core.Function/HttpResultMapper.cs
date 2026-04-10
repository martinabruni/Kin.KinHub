using Kin.KinHub.Core.Business.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kin.KinHub.Core.Function;

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

    internal static IActionResult ToCreatedActionResult<T>(Result<T> result) =>
        result.Status switch
        {
            ResultStatus.Success => new ObjectResult(result.Value) { StatusCode = StatusCodes.Status201Created },
            ResultStatus.NotFound => new NotFoundObjectResult(new { message = result.Message }),
            ResultStatus.Conflict => new ConflictObjectResult(new { message = result.Message }),
            ResultStatus.ValidationError => new BadRequestObjectResult(new { message = result.Message }),
            ResultStatus.Unauthorized => new UnauthorizedObjectResult(new { message = result.Message }),
            _ => new ObjectResult(new { message = result.Message }) { StatusCode = StatusCodes.Status500InternalServerError },
        };
}
