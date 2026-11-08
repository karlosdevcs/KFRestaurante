using KFRestaurante.Dominio;
using KFRestaurante.DTOs;
using KFRestaurante.Infraestrutura;
using Microsoft.EntityFrameworkCore;

namespace KFRestaurante.Endpoints
{
    public static class OpcoesEndpoints
    {
        public static void MapOpcoes(this IEndpointRouteBuilder app)
        {
            app.MapGet("/opcoes", async (KFContext contexto) => {

                return await contexto.Opcoes.Select(opcao => OpcaoResponse.De(opcao)).ToListAsync();


            })
                .Produces<List<OpcaoResponse>>(StatusCodes.Status200OK);




            app.MapPost("/opcoes", async (CriarOpcaoRequest request, KFContext contexto) =>
            {
                if (await contexto.Opcoes.AnyAsync(opcao => opcao.Nome == request.Nome))
                {
                    return Results.Problem(detail: $"A opção com o nome '{request.Nome}' já existe.", statusCode: StatusCodes.Status409Conflict);
                }

                Opcao opcao1 = new Opcao(request.Nome, request.Categoria, request.Preco);

                contexto.Add(opcao1);

                await contexto.SaveChangesAsync(); //--save changes nao manda so INSERT, ele le de volta o id gerado pelo pg e escreve na inst. do change tracker. 


                return Results.Created($"/opcoes/{opcao1.Id}", OpcaoResponse.De(opcao1));

            })
                .Produces<OpcaoResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status409Conflict);

            app.MapGet("/opcoes/{id}", async (int id, KFContext contexto) =>
            {

                Opcao? opcao = await contexto.Opcoes.FindAsync(id);

                if (opcao == null)
                {
                    return Results.Problem(detail: $"A opção com o id '{id}' não foi encontrada.", statusCode: StatusCodes.Status404NotFound);
                }


                return Results.Ok(OpcaoResponse.De(opcao));

            })
                .Produces<OpcaoResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);

            app.MapPatch("/opcoes/{id}/esgotado", async (int id, AlterarEsgotadoRequest request, KFContext contexto) =>
            {
                Opcao? opcao = await contexto.Opcoes.FindAsync(id);

                if (opcao == null)
                {
                    return Results.Problem(detail: $"A opção com o id '{id}' não foi encontrada.", statusCode: StatusCodes.Status404NotFound);
                }

                opcao.AlterarEsgotado(request.Esgotado);
                await contexto.SaveChangesAsync();

                return Results.Ok(OpcaoResponse.De(opcao));
            })
                .Produces<OpcaoResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
