using KFRestaurante.Dominio;
using KFRestaurante.DTOs;
using KFRestaurante.Infraestrutura;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Net.Security;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace KFRestaurante
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
               
            });

            var connectionString = builder.Configuration.GetConnectionString("KFRestaurante");
            builder.Services.AddDbContext<KFContext>(options => options.UseNpgsql(connectionString));

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

                



            app.MapGet("/opcoes", (KFContext contexto) => {

                return contexto.Opcoes.Select(opcao => OpcaoResponse.De(opcao)); 


            });


            app.MapPost("/opcoes", (CriarOpcaoRequest request, KFContext contexto) =>
            {
                if (contexto.Opcoes.Any(opcao => opcao.Nome == request.Nome))
                {
                    return Results.Conflict($"A opção com o nome '{request.Nome}' já existe.");
                }

                Opcao opcao1 = new Opcao(request.Nome, request.Categoria, request.Preco);

                contexto.Add(opcao1);

                contexto.SaveChanges(); //--save changes nao manda so INSERT, ele le de volta o id gerado pelo pg e escreve na inst. do change tracker. 


                return Results.Created($"/opcoes/{opcao1.Id}", OpcaoResponse.De(opcao1));

            });

            app.MapGet("/opcoes/{id}", (int id, KFContext contexto) =>
            {

                Opcao opcao = contexto.Opcoes.Find(id);

                if (opcao == null)
                {
                    return Results.NotFound($"A opção com o id '{id}' não foi encontrada.");
                }


                return Results.Ok(OpcaoResponse.De(opcao));
               
            });

            app.UseSwagger();
            app.UseSwaggerUI();

            app.Run();
        }
    }
}
