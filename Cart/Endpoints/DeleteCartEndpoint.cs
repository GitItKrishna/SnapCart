using Cart.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Cart.Endpoints;

public class DeleteCartRequest
{
    public string UserId { get; set; } = null!;
}

[HttpDelete("/cart/{userId}")]
[AllowAnonymous]
public class DeleteCartEndpoint(CartService service, ILogger<DeleteCartEndpoint> logger) : Endpoint<DeleteCartRequest>
{

    public override async Task HandleAsync(DeleteCartRequest req, CancellationToken ct)
    {
        try
        {
            logger.LogInformation("Clearing cart for user {UserId}", req.UserId);
            await service.ClearCart(req.UserId);
            logger.LogInformation("Successfully cleared cart for user {UserId}", req.UserId);
            await SendNoContentAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while clearing cart for user {UserId}", req.UserId);
            ThrowError(ex.Message);
        }
    }
}
