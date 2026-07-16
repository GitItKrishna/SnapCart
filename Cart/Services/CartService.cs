namespace Cart.Services;

public class CartService(IDistributedCache cache)
{
    public async Task<ShoppingCart?> GetCart(string userId)
    {
        var cart = await cache.GetStringAsync(userId);
        return string.IsNullOrWhiteSpace(cart) ? null : 
            System.Text.Json.JsonSerializer.Deserialize<ShoppingCart>(cart);
    }
    public async Task UpdateCart(ShoppingCart cart)
    {
        await cache.SetStringAsync(cart.UserName!, System.Text.Json.JsonSerializer.Serialize(cart));
    }
    public async Task ClearCart(string userName)
    {
        await cache.RemoveAsync(userName);
    }
}