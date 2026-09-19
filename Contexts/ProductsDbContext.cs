using Microsoft.EntityFrameworkCore;
using ProductsApi.Models;

namespace ProductsApi.Contexts;

public class ProductsDbContext(DbContextOptions<ProductsDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}
