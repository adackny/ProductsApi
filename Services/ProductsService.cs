using Microsoft.EntityFrameworkCore;
using ProductsApi.Contexts;
using ProductsApi.Core;
using ProductsApi.Models;

namespace ProductsApi.Services;

public class ProductsService(ProductsDbContext context)
{
    public async Task<OperationResult<Uoid, string>> Add(Product product)
    {
        var existingProduct = await context.Products
            .Where(p => p.Name == product.Name)
            .FirstOrDefaultAsync();
        if (existingProduct is not null)
            return $"ERR: The product '{product.Name}' already exist";
        await context.SaveChangesAsync();
        return new Uoid();
    }

    public async Task<IAsyncEnumerable<Product>> GetAll() => context.Products.AsAsyncEnumerable();

    public async Task<OperationResult<Uoid, List<string>>> Update(Product product)
    {
        var existingProduct = await context.Products
            .Where(p => p.Id == product.Id)
            .FirstOrDefaultAsync();

        List<string> errors = [];

        if (existingProduct is null)
        {
            errors.Add($"There is no product with id = '{product.Id}'");
            return errors;
        }

        if (existingProduct.Name != product.Name)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                errors.Add("The Name property cannot be empty");
            var existingProductWithName = await context.Products
                .Where(p => p.Name == product.Name)
                .FirstOrDefaultAsync();
            if (existingProductWithName is not null)
                errors.Add($"Already exist a product with name = '{product.Name}'");
            
            existingProduct.Name = product.Name;
        }

        if (existingProduct.Description != product.Description)
        {
            if (string.IsNullOrWhiteSpace(product.Description))
                errors.Add("The Description property cannot be empty");
            existingProduct.Description = product.Description;
        }

        if (existingProduct.Price != product.Price)
        {
            if (product.Price <= 0M)
                errors.Add("Product price must be greater than zero");
            existingProduct.Price = product.Price;
        }

        if (errors.Count > 0)
            return errors;

        await context.SaveChangesAsync();
        return new Uoid();
    }

    public async Task<OperationResult<Product, string>> GetById(int id)
    {
        var existingProduct = await context.Products.FindAsync(id);
        if (existingProduct is null)
            return $"Product withd id = '{id}' not found";
        return existingProduct;
    }

    public async Task<OperationResult<Uoid, string>> Remove(int id)
    {
        var existingProduct = await context.Products
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
        
        if (existingProduct is null)
            return $"Product withd id = '{id}' not found";
        
        if (!existingProduct.IsDeleted)
        {
            context.Products.Remove(existingProduct);
            await context.SaveChangesAsync();
        }

        return new Uoid();
    }
}
