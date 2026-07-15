namespace Catalog.Services;

public class ProductService(ProductDbContext dbContext)
{
    public async Task CreateProduct(Product product)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
    }
    public async Task<Product?> GetProductById(int id)
    {
        return await dbContext.Products.FindAsync(id);
    }
    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await dbContext.Products.ToListAsync();
    }

    public async Task UpdateProductAsync(Product updatedProduct, Product inputProduct)
    {
        updatedProduct.Name = inputProduct.Name;
        updatedProduct.Description = inputProduct.Description;
        updatedProduct.Price = inputProduct.Price;
        updatedProduct.ImageUrl = inputProduct.ImageUrl;

        dbContext.Products.Update(updatedProduct);
        await dbContext.SaveChangesAsync();
    }
   
    
    public async Task DeleteProduct(Product toDeleteProduct)
    {
        dbContext.Products.Remove(toDeleteProduct);
        await dbContext.SaveChangesAsync();
    }
}