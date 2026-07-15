using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.Endpoints;

public class DeleteProductRequest
{
    public int Id { get; set; }
}

[HttpDelete("/products/{id}"), AllowAnonymous]
public class DeleteProductEndpoint(ProductService service, ILogger<DeleteProductEndpoint> logger) : Endpoint<DeleteProductRequest>
{

    public override async Task HandleAsync(DeleteProductRequest req, CancellationToken ct)
    {
        try
        {
            logger.LogInformation("Deleting product with id {ProductId}", req.Id);
            var existing = await service.GetProductById(req.Id);
            if (existing is null)
            {
                logger.LogWarning("Product with id {ProductId} not found for deletion", req.Id);
                await SendNotFoundAsync(ct);
                return;
            }

            await service.DeleteProduct(existing);
            logger.LogInformation("Successfully deleted product with id {ProductId}", req.Id);
            await SendNoContentAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting product with id {ProductId}", req.Id);
            ThrowError(ex.Message);
        }
    }
}

