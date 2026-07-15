using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Endpoints;

[HttpGet("/products"), AllowAnonymous]
public class GetProductsEndpoint(ProductService service, ILogger<GetProductsEndpoint> logger) : EndpointWithoutRequest<IEnumerable<Product>>
{

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            logger.LogInformation("Fetching all products");
            var products = (await service.GetProductsAsync()).ToList();
            logger.LogInformation("Successfully retrieved {ProductCount} products", products.Count);
            await SendAsync(products, cancellation: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching products");
            ThrowError(ex.Message);
        }
    }
}

