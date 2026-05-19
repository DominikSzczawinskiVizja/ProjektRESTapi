using api.Repositories.AuctionRepo;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api.Authorization;

public sealed class AuctionStatusHandler(IAuctionRepository repository) : AuthorizationHandler<StatusRequirement>
{
    private readonly IAuctionRepository _repository = repository;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        StatusRequirement requirement)
    {
        // Admin == true moze uzywac wszystkich endpointow 
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        // Login status check
        var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //2 ify tak naprawdę !long.TryParse zwraca true gdy sie uda po czym gdy sie uda to od razu przypisuje do userId
        if (!long.TryParse(userIdStr, out var userId))
            return;

        if (context.Resource is not AuthorizationFilterContext mvcContext)
            return;

        var httpContext = mvcContext.HttpContext;

        if (!httpContext.Request.RouteValues.TryGetValue("id", out var idObj))
            return;

        if (!long.TryParse(idObj?.ToString(), out var auctionId))
            return;

        var auction = await _repository.GetByIdAsync(auctionId);

        if (auction is null)
            return;

        if (auction.OwnerId == userId)
            context.Succeed(requirement);
    }
    
}
