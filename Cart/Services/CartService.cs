namespace Cart.Services;

public class CartService(IDistributedCache cache, CatalogApiClient catalogApiClient)
{
    public async Task<ShoppingCart?> GetCart(string userId)
    {
        var cart = await cache.GetStringAsync(userId);
        return string.IsNullOrWhiteSpace(cart) ? null : 
            System.Text.Json.JsonSerializer.Deserialize<ShoppingCart>(cart);
    }
    public async Task UpdateCart(ShoppingCart cart)
    {
        //Before updating the cart, call CatalogApiClient to get the product details
        foreach (var item in cart.Items)
        {
            var product = await catalogApiClient.GetProductById(item.ProductId);    
            item.Price = product.Price;
            item.ProductName = product.Name;
        }
        await cache.SetStringAsync(cart.UserName!, System.Text.Json.JsonSerializer.Serialize(cart));
    }
    public async Task ClearCart(string userName)
    {
        await cache.RemoveAsync(userName);
    }
}