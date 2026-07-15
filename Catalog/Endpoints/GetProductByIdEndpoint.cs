using FastEndpoints;

namespace Catalog.Endpoints;

public class GetProductByIdRequest
{
    public int Id { get; set; }
}

[HttpGet("/products/{id}")]
public class GetProductByIdEndpoint(ProductService service, ILogger<GetProductByIdEndpoint> logger) : Endpoint<GetProductByIdRequest, Product>
{

    public override async Task HandleAsync(GetProductByIdRequest req, CancellationToken ct)
    {
        try
        {
            logger.LogInformation("Fetching product with id {ProductId}", req.Id);
            var product = await service.GetProductById(req.Id);
            if (product is null)
            {
                logger.LogWarning("Product with id {ProductId} not found", req.Id);
                await SendNotFoundAsync(ct);
            }
            else
            {
                logger.LogInformation("Successfully retrieved product with id {ProductId}", req.Id);
                await SendAsync(product, cancellation: ct);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while fetching product with id {ProductId}", req.Id);
            ThrowError(ex.Message);
        }
    }
}

