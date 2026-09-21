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

            app.MapGet("/opcoes", (KFContext contexto) => {

                return contexto.Opcoes;


            });


            app.MapPost("/opcoes", (CriarOpcaoRequest request, KFContext contexto) =>
            {
                if (contexto.Opcoes.Any(opcao => opcao.Nome == request.Nome))
                {
                    return Results.Conflict($"A opção com o nome '{request.Nome}' já existe.");
                }

                Opcao opcao1 = new Opcao(request.Nome, request.Categoria, request.Preco);

                contexto.Add(opcao1);

                contexto.SaveChanges();

                return Results.Created($"/opcoes/{opcao1.Id}", opcao1);
            });

            app.MapGet("/opcoes/{id}", (int id, KFContext contexto) =>
            {

                Opcao opcao = contexto.Opcoes.Find(id);

                if (opcao == null)
                {
                    return Results.NotFound($"A opção com o id '{id}' não foi encontrada.");
                }

                OpcaoResponse opcaoResponse = new OpcaoResponse(opcao.Id, opcao.Nome, opcao.Categoria, opcao.Preco, opcao.Esgotado);

                return Results.Ok(opcaoResponse);
               
            });

            app.Run();
        }
    }
}
