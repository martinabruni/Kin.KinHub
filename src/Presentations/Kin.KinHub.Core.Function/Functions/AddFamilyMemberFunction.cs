using Kin.KinHub.Core.Business;
using Kin.KinHub.Core.Function.Validators.Interfaces;
using Kin.KinHub.Identity.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

namespace Kin.KinHub.Core.Function.Functions;

public sealed class AddFamilyMemberFunction
{
    private readonly IFamilyService _familyService;
    private readonly IRequestValidator<AddFamilyMemberRequest> _validator;
    private readonly ICurrentUser _currentUser;

    public AddFamilyMemberFunction(
        IFamilyService familyService,
        IRequestValidator<AddFamilyMemberRequest> validator,
        ICurrentUser currentUser)
    {
        _familyService = familyService;
        _validator = validator;
        _currentUser = currentUser;
    }

    [Function(nameof(AddFamilyMemberFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "families/{familyId}/members")] HttpRequest req,
        Guid familyId,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return new UnauthorizedObjectResult(new { message = "Missing or invalid Authorization header." });

        try
        {
            var request = await req.ReadFromJsonAsync<AddFamilyMemberRequest>(cancellationToken);

            if (request is null)
                return new BadRequestObjectResult(new { message = "Invalid request body." });

            var validation = await _validator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
                return new BadRequestObjectResult(new { errors = validation.Errors });

            var result = await _familyService.AddFamilyMemberAsync(familyId, request, _currentUser.UserId, cancellationToken);

            return HttpResultMapper.ToCreatedActionResult(result);
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult(new { errors = new[] { "Malformed JSON in request body." } });
        }
    }
}
