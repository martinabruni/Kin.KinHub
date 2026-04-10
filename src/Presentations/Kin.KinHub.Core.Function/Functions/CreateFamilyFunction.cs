using Kin.KinHub.Core.Business;
using Kin.KinHub.Core.Function.Validators.Interfaces;
using Kin.KinHub.Identity.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

namespace Kin.KinHub.Core.Function.Functions;

public sealed class CreateFamilyFunction
{
    private readonly IFamilyService _familyService;
    private readonly IRequestValidator<CreateFamilyRequest> _validator;
    private readonly ICurrentUser _currentUser;

    public CreateFamilyFunction(
        IFamilyService familyService,
        IRequestValidator<CreateFamilyRequest> validator,
        ICurrentUser currentUser)
    {
        _familyService = familyService;
        _validator = validator;
        _currentUser = currentUser;
    }

    [Function(nameof(CreateFamilyFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "families")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return new UnauthorizedObjectResult(new { message = "Missing or invalid Authorization header." });

        try
        {
            var request = await req.ReadFromJsonAsync<CreateFamilyRequest>(cancellationToken);

            if (request is null)
                return new BadRequestObjectResult(new { message = "Invalid request body." });

            var validation = await _validator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
                return new BadRequestObjectResult(new { errors = validation.Errors });

            var result = await _familyService.CreateFamilyAsync(request, _currentUser.UserId, cancellationToken);

            return HttpResultMapper.ToCreatedActionResult(result);
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult(new { errors = new[] { "Malformed JSON in request body." } });
        }
    }
}
