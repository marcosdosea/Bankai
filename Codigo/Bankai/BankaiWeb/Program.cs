using Core;
using Core.Service;
using Service;
using Microsoft.EntityFrameworkCore;

namespace BankaiWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Mensagem de erro caso a connection string não seja encontrada
            var connectionString = builder.Configuration.GetConnectionString("BankaiDatabase")
            ?? throw new InvalidOperationException("Connection string 'BankaiDatabase' não encontrada.");

            builder.Services.AddTransient<IDisponibilidadeService, DisponibilidadeService>();

            builder.Services.AddDbContext<BankaiContext>(options =>
                options.UseMySQL(connectionString));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
