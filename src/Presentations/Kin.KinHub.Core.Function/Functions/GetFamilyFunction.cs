using Kin.KinHub.Core.Business;
using Kin.KinHub.Identity.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Kin.KinHub.Core.Function.Functions;

public sealed class GetFamilyFunction
{
    private readonly IFamilyService _familyService;
    private readonly ICurrentUser _currentUser;

    public GetFamilyFunction(
        IFamilyService familyService,
        ICurrentUser currentUser)
    {
        _familyService = familyService;
        _currentUser = currentUser;
    }

    [Function(nameof(GetFamilyFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "families")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return new UnauthorizedObjectResult(new { message = "Missing or invalid Authorization header." });

        var result = await _familyService.GetFamilyAsync(_currentUser.UserId, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }
}
