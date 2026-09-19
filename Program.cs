using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Contexts;
using ProductsApi.Core;
using ProductsApi.Models;
using ProductsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ProductsDbContext>(options => options.UseSqlite("Data source=products.db"));
builder.Services.AddScoped<ProductsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/product", async (Product product, [FromServices] ProductsService productsService) =>
{
    await productsService.Add(product);
    return Results.Created();
})
.WithName("CreateProduct");

app.MapPut("/product", async (Product product, [FromServices] ProductsService productsService) =>
{
    var result = await productsService.Update(product);
    return result switch
    {
        Uoid => Results.NoContent(),
        List<string> errors => Results.BadRequest(errors)
    };
})
.WithName("UpdateProduct");

app.MapGet("/product", async ([FromServices] ProductsService productsService) =>
{
    return Results.Ok(await productsService.GetAll());
})
.WithName("AllProducts");

app.MapGet("/product/{id}", async (int id, [FromServices] ProductsService productsService) =>
{
    return await productsService.GetById(id) switch
    {
        Product product => Results.Ok(product),
        string error => Results.NotFound(error)
    };
})
.WithName("GetById");

app.MapDelete("/product/{id}", async (int id, [FromServices] ProductsService productsService) =>
{
    return await productsService.Remove(id) switch
    {
        Uoid => Results.Accepted(),
        string error => Results.NotFound(error)
    };
})
.WithName("DeleteProduct");

app.Run();

public record ProductsQuery();