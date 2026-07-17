using Catalog.Models;

namespace Cart.ApiClients;

public class CatalogApiClient(HttpClient httpClient)
{
    public async Task<Product> GetProductById(int productId)
    {
        var response = await httpClient.GetFromJsonAsync<Product>($"/products/{productId}");
        return response!;
    }
}