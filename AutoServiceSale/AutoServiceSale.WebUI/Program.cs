using AutoServiceSale.Data.Context;
using AutoServiceSale.Data.Repositories.Concrete;
using AutoServiceSale.Entities.Concrete;
using AutoServiceSale.Service.Abstract;
using AutoServiceSale.Service.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AutoServiceSale.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddTransient(typeof(IService<>), typeof(Service<>));
            builder.Services.AddTransient(typeof(IAutoService), typeof(AutoService));
            builder.Services.AddDbContext<ApplicationContext>();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x =>
            {
                x.LoginPath = "/Account/Login";
                x.AccessDeniedPath = "/AccessDenied";
                x.LogoutPath = "/Account/Logout";
                x.Cookie.Name = "Admin";
                x.Cookie.MaxAge=TimeSpan.FromDays(7);
                x.Cookie.IsEssential = true;
            });

            builder.Services.AddAuthorization(x =>
            {
                x.AddPolicy("AdminPolicy", policy => policy.RequireClaim("Role", "Admin"));
                x.AddPolicy("UserPolicy", policy => policy.RequireClaim("Role", "User"));
                x.AddPolicy("CustomerPolicy", policy => policy.RequireClaim("Role", "Customer"));
            });

            
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
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
           name: "admin",
           pattern: "{area:exists}/{controller=Main}/{action=Index}/{id?}"
         );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}