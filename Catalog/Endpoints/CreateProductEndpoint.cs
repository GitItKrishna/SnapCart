using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Endpoints;

[HttpPost("/products"), AllowAnonymous]
public class CreateProductEndpoint(ProductService service, ILogger<CreateProductEndpoint> logger) : Endpoint<Product>
{

    public override async Task HandleAsync(Product req, CancellationToken ct)
    {
        try
        {
            logger.LogInformation("Creating new product with name {ProductName}", req.Name);
            await service.CreateProduct(req);
            logger.LogInformation("Successfully created product with id {ProductId} and name {ProductName}", req.Id, req.Name);
            await SendAsync(req, statusCode: 201, cancellation: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating product with name {ProductName}", req.Name);
            ThrowError(ex.Message);
        }
    }
}

