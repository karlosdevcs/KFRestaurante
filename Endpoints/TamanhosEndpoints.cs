using KFRestaurante.Dominio;
using KFRestaurante.DTOs;
using KFRestaurante.Infraestrutura;
using Microsoft.EntityFrameworkCore;

namespace KFRestaurante.Endpoints
{
    public static class TamanhosEndpoints
    {
        public static void MapTamanhos(this IEndpointRouteBuilder app)
        {
            app.MapGet("/tamanhos", async (KFContext contexto) =>
            {
                return await contexto.TamanhosMarmita.Select(tamanho => TamanhoResponse.De(tamanho)).ToListAsync();
            })
                .Produces<List<TamanhoResponse>>(StatusCodes.Status200OK);

            app.MapGet("/tamanhos/{id}", async (int id, KFContext contexto) =>
                {
                    TamanhoMarmita? tamanho = await contexto.TamanhosMarmita.FindAsync(id);
                    if (tamanho == null)
                    {
                        return Results.Problem(detail: $"O tamanho com o id '{id}' não foi encontrado.", statusCode: StatusCodes.Status404NotFound);
                    }
                    return Results.Ok(TamanhoResponse.De(tamanho));
                })
                .Produces<TamanhoResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);


            app.MapPost("/tamanhos", async (CriarTamanhoRequest request, KFContext contexto) =>
            {
                if (await contexto.TamanhosMarmita.AnyAsync(tamanho => tamanho.Nome == request.Nome))
                {
                    return Results.Problem(detail: $"O tamanho com o nome '{request.Nome}' já existe.", statusCode: StatusCodes.Status409Conflict);
                }

                TamanhoMarmita tamanho1 = new TamanhoMarmita(request.Nome, request.Preco, request.LimitedeComponente);
                contexto.Add(tamanho1);
                await contexto.SaveChangesAsync();
                return Results.Created($"/tamanhos/{tamanho1.Id}", TamanhoResponse.De(tamanho1));
            })
                .Produces<TamanhoResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status409Conflict);
        }
    }
}
