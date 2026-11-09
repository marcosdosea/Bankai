using BankaiWeb.Mappers;
using Core;
using Core.Service;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Service;

namespace BankaiWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            TypeAdapterConfig.GlobalSettings.Scan(typeof(AgendamentoServicoProfile).Assembly);
            builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
            builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();

            // Mensagem de erro caso a connection string não seja encontrada
            var connectionString = builder.Configuration.GetConnectionString("BankaiDatabase")
            ?? throw new InvalidOperationException("Connection string 'BankaiDatabase' não encontrada.");

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
