using KFRestaurante.Dominio;
using KFRestaurante.DTOs;
using KFRestaurante.Infraestrutura;
using KFRestaurante.Infraestrutura.Autenticacao;
using KFRestaurante.Servicos;
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

                DateOnly hoje = HorarioBrasilia.Hoje(); // nao DateTime.Now: o servidor pode estar em UTC

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
                .ProducesProblem(StatusCodes.Status409Conflict)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .RequireAuthorization(Politicas.De(Permissao.GerenciarCardapio));

            // a tela do cliente comeca aqui: ele nao sabe o id, so que quer o cardapio de hoje.
            // Rota literal "hoje" tem prioridade sobre "{id}" no roteamento do ASP.NET
            app.MapGet("/cardapios/hoje", async (KFContext contexto) =>
            {
                DateOnly hoje = HorarioBrasilia.Hoje();

                CardapioDoDia? cardapio = await contexto.CardapiosDoDia
                    .Include(cardapio => cardapio.Opcoes)
                    .FirstOrDefaultAsync(cardapio => cardapio.DataPublicacao == hoje);

                // rascunho e trabalho do dono em andamento: para o cliente, ainda nao existe
                if (cardapio == null || cardapio.Situacao == SituacaoCardapio.Rascunho)
                {
                    return Results.Problem(detail: $"O cardápio de {hoje:dd/MM/yyyy} ainda não foi publicado.", statusCode: StatusCodes.Status404NotFound);
                }

                // publicado ou fechado: a tela usa a situacao para mostrar "encerrado por hoje"
                return Results.Ok(CardapioResponse.De(cardapio));
            })
                .Produces<CardapioResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            app.MapGet("/cardapios/{id:int}", async (int id, KFContext contexto) =>
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
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .RequireAuthorization(Politicas.De(Permissao.GerenciarCardapio));

            app.MapPost("/cardapios/{id:int}/publicar", async (int id, KFContext contexto) =>
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
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .RequireAuthorization(Politicas.De(Permissao.GerenciarCardapio));

            app.MapPost("/cardapios/{id:int}/fechar", async (int id, KFContext contexto) =>
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
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .RequireAuthorization(Politicas.De(Permissao.GerenciarCardapio));
        }
    }
}
