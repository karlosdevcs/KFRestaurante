
using KFRestaurante.Dominio;
using KFRestaurante.Infraestrutura;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Net.Security;

namespace KFRestaurante
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("KFRestaurante");
            builder.Services.AddDbContext<KFContext>(options => options.UseNpgsql(connectionString));
            var app = builder.Build();
            app.Run();
            
            
        }

    }
}
