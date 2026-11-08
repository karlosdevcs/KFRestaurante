using KFRestaurante.Dominio;
using KFRestaurante.DTOs;
using KFRestaurante.Infraestrutura;
using Microsoft.EntityFrameworkCore;

namespace KFRestaurante.Endpoints
{
    public static class CardapiosEndpoints
    {
        public static void MapCardapios(this IEndpointRouteBuilder app)
        {
            app.MapPost("/cardapios", async (CriarCardapioRequest request, KFContext contexto) =>
            {
                List<int> idsRepetidos = request.OpcoesIds
                    .GroupBy(id => id)
                    .Where(grupo => grupo.Count() > 1)
                    .Select(grupo => grupo.Key)
                    .ToList();

                if (idsRepetidos.Count > 0)
                {
                    return Results.Problem(detail: $"Ids enviados mais de uma vez: {string.Join(", ", idsRepetidos)}.", statusCode: StatusCodes.Status400BadRequest);
                }

                DateOnly hoje = DateOnly.FromDateTime(DateTime.Now);

                if (await contexto.CardapiosDoDia.AnyAsync(cardapio => cardapio.DataPublicacao == hoje))
                {
                    return Results.Problem(detail: $"Já existe um cardápio para a data {hoje:dd/MM/yyyy}.", statusCode: StatusCodes.Status409Conflict);
                }

                List<Opcao> opcoes = await contexto.Opcoes.Where(opcao => request.OpcoesIds.Contains(opcao.Id)).ToListAsync();

                List<int> idsInvalidos = request.OpcoesIds.Except(opcoes.Select(opcao => opcao.Id)).ToList();

                if (idsInvalidos.Count > 0)
                {
                    return Results.Problem(detail: $"Ids de opções não encontrados: {string.Join(", ", idsInvalidos)}.", statusCode: StatusCodes.Status400BadRequest);
                }

                CardapioDoDia cardapio1 = new CardapioDoDia(hoje);

                foreach (Opcao opcao in opcoes)
                {
                    cardapio1.AdicionarOpcao(opcao);
                }

                contexto.Add(cardapio1);
                await contexto.SaveChangesAsync();

                return Results.Created($"/cardapios/{cardapio1.Id}", CardapioResponse.De(cardapio1));
            })
                .Produces<CardapioResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status409Conflict);

            app.MapGet("/cardapios/{id}", async (int id, KFContext contexto) =>
            {
                CardapioDoDia? cardapio = await contexto.CardapiosDoDia
                    .Include(cardapio => cardapio.Opcoes)
                    .FirstOrDefaultAsync(cardapio => cardapio.Id == id);

                if (cardapio == null)
                {
                    return Results.Problem(detail: $"O cardápio com o id '{id}' não foi encontrado.", statusCode: StatusCodes.Status404NotFound);
                }

                return Results.Ok(CardapioResponse.De(cardapio));
            })
                .Produces<CardapioResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            app.MapPost("/cardapios/{id}/publicar", async (int id, KFContext contexto) =>
            {
                CardapioDoDia? cardapio = await contexto.CardapiosDoDia
                    .Include(cardapio => cardapio.Opcoes)
                    .FirstOrDefaultAsync(cardapio => cardapio.Id == id);

                if (cardapio == null)
                {
                    return Results.Problem(detail: $"O cardápio com o id '{id}' não foi encontrado.", statusCode: StatusCodes.Status404NotFound);
                }

                cardapio.Publicar();
                await contexto.SaveChangesAsync();

                return Results.Ok(CardapioResponse.De(cardapio));
            })
                .Produces<CardapioResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound);

            app.MapPost("/cardapios/{id}/fechar", async (int id, KFContext contexto) =>
            {
                CardapioDoDia? cardapio = await contexto.CardapiosDoDia
                    .Include(cardapio => cardapio.Opcoes)
                    .FirstOrDefaultAsync(cardapio => cardapio.Id == id);

                if (cardapio == null)
                {
                    return Results.Problem(detail: $"O cardápio com o id '{id}' não foi encontrado.", statusCode: StatusCodes.Status404NotFound);
                }

                cardapio.Fechar();
                await contexto.SaveChangesAsync();

                return Results.Ok(CardapioResponse.De(cardapio));
            })
                .Produces<CardapioResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
