using ShopWebApplication1;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using ShopWebApplication1.Models;
using Microsoft.Extensions.DependencyInjection;
using ShopWebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MyshopContext>(option => option.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ));

builder.Services.AddHttpContextAccessor();
//builder.Services.AddTransient<IUserService, UserService>();

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

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    // ...
    endpoints.MapControllerRoute(
        name: "categoryImport",
        pattern: "/Category/Import",
        defaults: new { controller = "Category", action = "ImportFromExcel" }
    );
});



app.Run();

