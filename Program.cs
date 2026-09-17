
using KFRestaurante.Dominio;
using KFRestaurante.DTOs;
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

            app.MapGet("/opcoes", (KFContext contexto) => { return contexto.Opcoes; });
            app.MapPost("/opcoes", (CriarOpcaoRequest request, KFContext contexto) =>
            {
                Opcao opcao1 = new Opcao(request.Nome, request.Categoria, request.Preco);

                contexto.Add(opcao1);

                contexto.SaveChanges();


            });


            app.Run();
            
            
        }

    }
}
