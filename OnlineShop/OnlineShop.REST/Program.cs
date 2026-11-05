using Microsoft.EntityFrameworkCore;
using OnlineShop.Common.Repositories;
using OnlineShop.Common.Services;
using OnlineShop.Infrastructure;
using OnlineShop.Infrastructure.Models;
using OnlineShop.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Reuse the database context created for laboratory work No. 3.
builder.Services.AddDbContext<OnlineShopContext>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(EntityFrameworkRepository<>));

// Explicit factories avoid constructor ambiguity because CrudServiceAsync<T>
// supports both JSON-file storage (Lab 2) and repository-backed storage (Lab 3/4).
builder.Services.AddScoped<ICrudServiceAsync<ProductModel>>(services =>
    new CrudServiceAsync<ProductModel>(services.GetRequiredService<IRepository<ProductModel>>()));
builder.Services.AddScoped<ICrudServiceAsync<CustomerModel>>(services =>
    new CrudServiceAsync<CustomerModel>(services.GetRequiredService<IRepository<CustomerModel>>()));

var app = builder.Build();

// Apply any pending Lab 3 database migrations before accepting requests.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OnlineShopContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
