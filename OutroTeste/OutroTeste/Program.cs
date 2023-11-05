using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using agenda.Models;

namespace agenda
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add session services to the DI container
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // You can adjust the timeout as needed
                options.Cookie.HttpOnly = true; // Prevent the client-side script from accessing the cookie
                options.Cookie.IsEssential = true; // Make the session cookie essential
            });

            // Configure the database context
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DataBase")));

            var app = builder.Build();

            // Migrate the database. This is typically for deploying updates to the database.
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    // Handle any errors
                    // Log the error or take appropriate action
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            // Insert the UseSession call here to enable session before UseAuthorization
            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "especialidades",
                pattern: "CentroAtendimento/Especialidades/{id}",
                defaults: new { controller = "CentroAtendimento", action = "Especialidades" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=CentroAtendimento}/{action=Index}/{id?}");

            app.Run();
        }
    }
}