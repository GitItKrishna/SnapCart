using Cart.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Cart.Endpoints;

public class GetCartRequest
{
    public string UserName { get; set; } = null!;
}

[HttpGet("/cart/{userName}")]
[AllowAnonymous]
public class GetCartEndpoint(CartService service, ILogger<GetCartEndpoint> logger) : Endpoint<GetCartRequest, ShoppingCart>
{

    public override async Task HandleAsync(GetCartRequest req, CancellationToken ct)
    {
        try
        {
            logger.LogInformation("Fetching cart for user {UserName}", req.UserName);
            var cart = await service.GetCart(req.UserName);
            if (cart is null)
            {
                logger.LogWarning("Cart for user {UserId} not found", req.UserName);
                await SendNotFoundAsync(ct);
            }
            else
            {
                logger.LogInformation("Successfully retrieved cart for user {UserId}", req.UserName);
                await SendAsync(cart, cancellation: ct);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching cart for user {UserId}", req.UserName);
            ThrowError(ex.Message);
        }
    }
}
