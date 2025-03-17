using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MiniShoppingApp.Application.Interfaces;
using MiniShoppingApp.Infrastructure.Repositories;
using MiniShoppingApp.Infrastructure.Services;
using Moq;

namespace MiniShoppingApp.Test.Integration;

public class CustomTestServer
{
    public HttpClient Client { get; }

    public CustomTestServer()
    {
        var builder = new WebHostBuilder()
            .UseEnvironment("Testing") // Use test environment
            .ConfigureAppConfiguration((context, config) =>
            {
                var testConfig = new Dictionary<string, string>
                {
                    { "FakeStoreApi:BaseUrl", "https://fakestoreapi.com/products" }
                };
                config.AddInMemoryCollection(testConfig);
            })
            .ConfigureServices(services =>
            {
                // Add controllers (MVC support)
                services.AddControllersWithViews();

                // Register application services
                services.AddHttpClient<IProductRepository, ProductRepository>();
                services.AddScoped<IProductRepository, ProductRepository>();
                services.AddScoped<IProductService, ProductService>();
                services.AddScoped<ICartService, CartService>();

                // Enable session support
                services.AddHttpContextAccessor();
                services.AddDistributedMemoryCache();
                services.AddSession();

                // Mock Product Repository
                var productRepositoryMock = new Mock<IProductRepository>();
                productRepositoryMock.Setup(repo => repo.GetProductsAsync()).ReturnsAsync(new List<MiniShoppingApp.Domain.Models.Product>
                {
                    new() { Id = 1, Title = "Test Product 1", Price = 10.0m },
                    new() { Id = 2, Title = "Test Product 2", Price = 15.0m }
                });

                services.AddSingleton(productRepositoryMock.Object);
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseStaticFiles();
                app.UseSession();
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Product}/{action=Index}/{id?}");
                });
            });

        var server = new TestServer(builder);
        Client = server.CreateClient();
    }
}