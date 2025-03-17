using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MiniShoppingApp.Application.Interfaces;
using Moq;
namespace MiniShoppingApp.Test.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Mock Cart Service
            var cartServiceMock = new Mock<ICartService>();
            cartServiceMock.Setup(service => service.GetCartItemCount()).Returns(0);
            cartServiceMock.Setup(service => service.GetCartItems()).Returns(new List<Domain.Models.CartItem>());
            cartServiceMock.Setup(service => service.GetProductQuantity(It.IsAny<int>())).Returns(0);
            cartServiceMock.Setup(service => service.AddToCart(It.IsAny<int>())).ReturnsAsync(true);
            cartServiceMock.Setup(service => service.RemoveFromCart(It.IsAny<int>())).Returns(true);

            services.AddSingleton(cartServiceMock.Object);
        });
    }
}