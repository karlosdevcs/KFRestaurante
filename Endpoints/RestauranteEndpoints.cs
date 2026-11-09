using KFRestaurante.Dominio;
using KFRestaurante.DTOs;
using KFRestaurante.Infraestrutura;
using KFRestaurante.Infraestrutura.Autenticacao;
using Microsoft.EntityFrameworkCore;

namespace KFRestaurante.Endpoints
{
    public static class RestauranteEndpoints
    {
        public static void MapRestaurante(this IEndpointRouteBuilder app)
        {
            app.MapGet("/restaurante", async (KFContext contexto) =>
            {
                Restaurante? restaurante = await contexto.Restaurantes.OrderBy(r => r.Id).FirstOrDefaultAsync();

                if (restaurante == null)
                {
                    return Results.Ok(RestauranteResponse.De(new Restaurante())); // sem linha no banco = fechado
                }

                return Results.Ok(RestauranteResponse.De(restaurante));
            })
                .Produces<RestauranteResponse>(StatusCodes.Status200OK);

            app.MapPost("/restaurante/abrir", async (KFContext contexto) =>
            {
                Restaurante? restaurante = await contexto.Restaurantes.OrderBy(r => r.Id).FirstOrDefaultAsync();

                if (restaurante == null)
                {
                    restaurante = new Restaurante();
                    contexto.Add(restaurante);
                }

                restaurante.Abrir(); // sem WhatsApp cadastrado: DominioException -> 400
                await contexto.SaveChangesAsync();

                return Results.Ok(RestauranteResponse.De(restaurante));
            })
                .Produces<RestauranteResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .RequireAuthorization(Politicas.De(Permissao.AbrirFecharLoja));

            app.MapPost("/restaurante/fechar", async (KFContext contexto) =>
            {
                Restaurante? restaurante = await contexto.Restaurantes.OrderBy(r => r.Id).FirstOrDefaultAsync();

                if (restaurante == null)
                {
                    restaurante = new Restaurante();
                    contexto.Add(restaurante);
                }

                restaurante.Fechar();
                await contexto.SaveChangesAsync();

                return Results.Ok(RestauranteResponse.De(restaurante));
            })
                .Produces<RestauranteResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .RequireAuthorization(Politicas.De(Permissao.AbrirFecharLoja));

            app.MapPut("/restaurante/whatsapp", async (AlterarWhatsAppRequest request, KFContext contexto) =>
            {
                Restaurante? restaurante = await contexto.Restaurantes.OrderBy(r => r.Id).FirstOrDefaultAsync();

                if (restaurante == null)
                {
                    restaurante = new Restaurante();
                    contexto.Add(restaurante);
                }

                restaurante.AlterarWhatsApp(request.WhatsApp); // formato invalido: DominioException -> 400
                await contexto.SaveChangesAsync();

                return Results.Ok(RestauranteResponse.De(restaurante));
            })
                .Produces<RestauranteResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .RequireAuthorization(Politicas.De(Permissao.AlterarWhatsApp));
        }
    }
}
