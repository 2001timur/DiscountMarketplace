using DiscountMarketplace.Components;
using DiscountMarketplace.Interfaces;
using DiscountMarketplace.Models;
using DiscountMarketplace.Services;
using Microsoft.EntityFrameworkCore;

namespace DiscountMarketplace
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = "server=localhost;database=DiscountMarketplace;user=root;password=admin";

            builder.Services.AddDbContext<MarketplaceContext>(options =>
                options.UseMySql(connectionString, ServerVersion.Parse("8.0.46-mysql")));

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBusinessService, BusinessService>();
            builder.Services.AddScoped<IServiceCategoryService, ServiceCategoryService>();
            builder.Services.AddScoped<IDealService, DealService>();
            builder.Services.AddScoped<ICouponService, CouponService>();


            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
