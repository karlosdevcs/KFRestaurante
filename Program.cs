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

                Opcao? opcao = contexto.Opcoes.Find(id);

                if (opcao == null)
                {
                    return Results.NotFound($"A opção com o id '{id}' não foi encontrada.");
                }


                return Results.Ok(OpcaoResponse.De(opcao));

            });

            app.MapGet("/tamanhos", (KFContext contexto) =>
            {
                return contexto.TamanhosMarmita.Select(tamanho => TamanhoResponse.De(tamanho));
            });

            app.MapGet("/tamanhos/{id}", (int id, KFContext contexto) =>
                {
                    TamanhoMarmita? tamanho = contexto.TamanhosMarmita.Find(id);
                    if (tamanho == null)
                    {
                        return Results.NotFound($"O tamanho com o id '{id}' não foi encontrado.");
                    }
                    return Results.Ok(TamanhoResponse.De(tamanho));
                });


            app.MapPost("/tamanhos", (CriarTamanhoRequest request, KFContext contexto) =>
            {
                if (contexto.TamanhosMarmita.Any(tamanho => tamanho.Nome == request.Nome))
                {
                    return Results.Conflict($"O tamanho com o nome '{request.Nome}' já existe.");
                }

                TamanhoMarmita tamanho1 = new TamanhoMarmita(request.Nome, request.Preco, request.LimitedeComponente);
                contexto.Add(tamanho1);
                contexto.SaveChanges();
                return Results.Created($"/tamanhos/{tamanho1.Id}", TamanhoResponse.De(tamanho1));
            });

            app.MapPost("/cardapios", (CriarCardapioRequest request, KFContext contexto) =>
            {
                List<int> idsRepetidos = request.OpcoesIds
                    .GroupBy(id => id)
                    .Where(grupo => grupo.Count() > 1)
                    .Select(grupo => grupo.Key)
                    .ToList();

                if (idsRepetidos.Count > 0)
                {
                    return Results.BadRequest($"Ids enviados mais de uma vez: {string.Join(", ", idsRepetidos)}.");
                }

                DateOnly hoje = DateOnly.FromDateTime(DateTime.Now);

                if (contexto.CardapiosDoDia.Any(cardapio => cardapio.DataPublicacao == hoje))
                {
                    return Results.Conflict($"Já existe um cardápio para a data {hoje:dd/MM/yyyy}.");
                }

                List<Opcao> opcoes = contexto.Opcoes.Where(opcao => request.OpcoesIds.Contains(opcao.Id)).ToList();

                List<int> idsInvalidos = request.OpcoesIds.Except(opcoes.Select(opcao => opcao.Id)).ToList();

                if (idsInvalidos.Count > 0)
                {
                    return Results.BadRequest($"Ids de opções não encontrados: {string.Join(", ", idsInvalidos)}.");
                }

                CardapioDoDia cardapio1 = new CardapioDoDia(hoje);

                foreach (Opcao opcao in opcoes)
                {
                    cardapio1.AdicionarOpcao(opcao);
                }

                contexto.Add(cardapio1);
                contexto.SaveChanges();

                return Results.Created($"/cardapios/{cardapio1.Id}", CardapioResponse.De(cardapio1));
            });

            app.MapGet("/cardapios/{id}", (int id, KFContext contexto) =>
            {
                CardapioDoDia? cardapio = contexto.CardapiosDoDia
                    .Include(cardapio => cardapio.Opcoes)
                    .FirstOrDefault(cardapio => cardapio.Id == id);

                if (cardapio == null)
                {
                    return Results.NotFound($"O cardápio com o id '{id}' não foi encontrado.");
                }

                return Results.Ok(CardapioResponse.De(cardapio));
            });

            app.MapPost("/cardapios/{id}/publicar", (int id, KFContext contexto) =>
            {
                CardapioDoDia? cardapio = contexto.CardapiosDoDia
                    .Include(cardapio => cardapio.Opcoes)
                    .FirstOrDefault(cardapio => cardapio.Id == id);

                if (cardapio == null)
                {
                    return Results.NotFound($"O cardápio com o id '{id}' não foi encontrado.");
                }

                try
                {
                    cardapio.Publicar();
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }

                contexto.SaveChanges();

                return Results.Ok(CardapioResponse.De(cardapio));
            });

            app.MapPost("/cardapios/{id}/fechar", (int id, KFContext contexto) =>
            {
                CardapioDoDia? cardapio = contexto.CardapiosDoDia
                    .Include(cardapio => cardapio.Opcoes)
                    .FirstOrDefault(cardapio => cardapio.Id == id);

                if (cardapio == null)
                {
                    return Results.NotFound($"O cardápio com o id '{id}' não foi encontrado.");
                }

                try
                {
                    cardapio.Fechar();
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }

                contexto.SaveChanges();

                return Results.Ok(CardapioResponse.De(cardapio));
            });


                app.UseSwagger();
                app.UseSwaggerUI();

                app.Run();
            }
    }
}
