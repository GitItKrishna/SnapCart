using Cart.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Cart.Endpoints;

[HttpPost("/cart")]
[AllowAnonymous]
public class UpdateCartEndpoint(CartService service, ILogger<UpdateCartEndpoint> logger) : Endpoint<ShoppingCart, ShoppingCart>
{

    public override async Task HandleAsync(ShoppingCart req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.UserName))
        {
            logger.LogWarning("UpdateCart called without a UserName");
            AddError(c => c.UserName, "UserName is required.");
            await SendErrorsAsync(cancellation: ct);
            return;
        }
        try
        {
            logger.LogInformation("Updating cart for user {UserName} with {ItemCount} item(s)", req.UserName, req.Items.Count);
            await service.UpdateCart(req);
            logger.LogInformation("Successfully updated cart for user {UserName}", req.UserName);
            await SendAsync(req, cancellation: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating cart for user {UserName}", req.UserName);
            ThrowError(ex.Message);
        }
    }
}
