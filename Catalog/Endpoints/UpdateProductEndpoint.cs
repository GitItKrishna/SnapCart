using FastEndpoints;

namespace Catalog.Endpoints;

public class UpdateProductRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}

[HttpPut("/products/{id}")]
public class UpdateProductEndpoint(ProductService service, ILogger<UpdateProductEndpoint> logger) : Endpoint<UpdateProductRequest>
{

    public override async Task HandleAsync(UpdateProductRequest req, CancellationToken ct)
    {
        try
        {
            logger.LogInformation("Updating product with id {ProductId}", req.Id);
            var existing = await service.GetProductById(req.Id);
            if (existing is null)
            {
                logger.LogWarning("Product with id {ProductId} not found for update", req.Id);
                await SendNotFoundAsync(ct);
                return;
            }

            var input = new Product
            {
                Id = req.Id,
                Name = req.Name,
                Description = req.Description,
                Price = req.Price,
                ImageUrl = req.ImageUrl
            };

            await service.UpdateProductAsync(existing, input);
            logger.LogInformation("Successfully updated product with id {ProductId}", req.Id);
            await SendNoContentAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating product with id {ProductId}", req.Id);
            ThrowError(ex.Message);
        }
    }
}

