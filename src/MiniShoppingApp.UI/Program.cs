using MiniShoppingApp.Application.Interfaces;
using MiniShoppingApp.Application.Services;
using MiniShoppingApp.Infrastructure.Configuration;
using MiniShoppingApp.Infrastructure.Repositories;
using MiniShoppingApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Bind ApiSettings from appsettings.json
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Enable session storage
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Keeps session alive for 30 minutes
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession(); // Enable session middleware

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();