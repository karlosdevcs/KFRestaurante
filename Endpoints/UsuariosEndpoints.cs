using KFRestaurante.Dominio;
using KFRestaurante.DTOs;
using KFRestaurante.Infraestrutura;
using KFRestaurante.Infraestrutura.Autenticacao;
using Microsoft.EntityFrameworkCore;

namespace KFRestaurante.Endpoints
{
    // Toda a area exige dono ou superuser. DENTRO dela, a hierarquia decide em quem cada um pode mexer
    // (Usuario.PodeCriar / PodeGerenciar / PodeConceder).
    public static class UsuariosEndpoints
    {
        private static IResult Proibido(string detalhe) => Results.Problem(detail: detalhe, statusCode: StatusCodes.Status403Forbidden);

        private static IResult NaoEncontrado(int id) =>
            Results.Problem(detail: $"O usuário com o id '{id}' não foi encontrado.", statusCode: StatusCodes.Status404NotFound);

        public static void MapUsuarios(this IEndpointRouteBuilder app)
        {
            // lista a si mesmo e quem ele pode gerenciar (o dono nao ve outros donos nem superusers)
            app.MapGet("/usuarios", async (UsuarioAtual atual, KFContext contexto) =>
            {
                Usuario ator = await atual.ObterObrigatorioAsync();

                List<Usuario> todos = await contexto.Usuarios.OrderByDescending(u => u.Papel).ThenBy(u => u.Nome).ToListAsync();

                List<UsuarioResponse> visiveis = todos
                    .Where(u => u.Id == ator.Id || ator.PodeGerenciar(u))
                    .Select(u => UsuarioResponse.De(u))
                    .ToList();

                return Results.Ok(visiveis);
            })
                .Produces<List<UsuarioResponse>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .RequireAuthorization(Politicas.GerenciarUsuarios);

            app.MapGet("/usuarios/{id:int}", async (int id, UsuarioAtual atual, KFContext contexto) =>
            {
                Usuario ator = await atual.ObterObrigatorioAsync();
                Usuario? alvo = await contexto.Usuarios.FindAsync(id);

                if (alvo == null)
                {
                    return NaoEncontrado(id);
                }

                if (alvo.Id != ator.Id && !ator.PodeGerenciar(alvo))
                {
                    return Proibido("Você não pode ver este usuário.");
                }

                return Results.Ok(UsuarioResponse.De(alvo));
            })
                .Produces<UsuarioResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .RequireAuthorization(Politicas.GerenciarUsuarios);

            app.MapPost("/usuarios", async (CriarUsuarioRequest request, UsuarioAtual atual, KFContext contexto, ServicoDeSenha senhas) =>
            {
                Usuario ator = await atual.ObterObrigatorioAsync();

                if (!ator.PodeCriar(request.Papel))
                {
                    return Proibido($"Você não pode criar usuários com o papel {request.Papel}.");
                }

                // ninguem concede uma permissao que nao tem
                Permissao pedidas = PermissaoExtensions.Combinar(request.Permissoes);
                Permissao faltam = pedidas & ~ator.Permissoes;

                if (ator.Papel != Papel.Superuser && faltam != Permissao.Nenhuma)
                {
                    return Proibido($"Você não pode conceder permissões que não possui: {string.Join(", ", faltam.Separar())}.");
                }

                PoliticaDeSenha.Validar(request.Senha);

                // o dominio valida nome e e-mail e grava o e-mail em minusculo
                Usuario novo = new Usuario(request.Nome, request.Email, senhas.Gerar(request.Senha), request.Papel, request.Permissoes);

                if (await contexto.Usuarios.AnyAsync(u => u.Email == novo.Email))
                {
                    return Results.Problem(detail: $"Já existe um usuário com o e-mail '{novo.Email}'.", statusCode: StatusCodes.Status409Conflict);
                }

                contexto.Add(novo);
                await contexto.SaveChangesAsync();

                return Results.Created($"/usuarios/{novo.Id}", UsuarioResponse.De(novo));
            })
                .Produces<UsuarioResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status409Conflict)
                .RequireAuthorization(Politicas.GerenciarUsuarios);

            app.MapPut("/usuarios/{id:int}", async (int id, AlterarUsuarioRequest request, UsuarioAtual atual, KFContext contexto) =>
            {
                Usuario ator = await atual.ObterObrigatorioAsync();
                Usuario? alvo = await contexto.Usuarios.FindAsync(id);

                if (alvo == null)
                {
                    return NaoEncontrado(id);
                }

                if (!ator.PodeGerenciar(alvo))
                {
                    return Proibido("Você não pode alterar este usuário.");
                }

                // so o que e NOVO conta como conceder: manter uma permissao que ele ja tinha nao exige que voce a tenha
                Permissao novas = PermissaoExtensions.Combinar(request.Permissoes) & ~alvo.Permissoes;

                if (ator.Papel != Papel.Superuser && (novas & ~ator.Permissoes) != Permissao.Nenhuma)
                {
                    return Proibido($"Você não pode conceder permissões que não possui: {string.Join(", ", (novas & ~ator.Permissoes).Separar())}.");
                }

                alvo.AlterarDados(request.Nome, request.Permissoes);
                await contexto.SaveChangesAsync();

                return Results.Ok(UsuarioResponse.De(alvo));
            })
                .Produces<UsuarioResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .RequireAuthorization(Politicas.GerenciarUsuarios);

            app.MapPost("/usuarios/{id:int}/desativar", async (int id, UsuarioAtual atual, KFContext contexto) =>
            {
                Usuario ator = await atual.ObterObrigatorioAsync();
                Usuario? alvo = await contexto.Usuarios.FindAsync(id);

                if (alvo == null)
                {
                    return NaoEncontrado(id);
                }

                if (!ator.PodeGerenciar(alvo)) // inclui: ninguem desativa a si mesmo
                {
                    return Proibido("Você não pode desativar este usuário.");
                }

                alvo.Desativar();
                await contexto.SaveChangesAsync();

                return Results.Ok(UsuarioResponse.De(alvo));
            })
                .Produces<UsuarioResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .RequireAuthorization(Politicas.GerenciarUsuarios);

            app.MapPost("/usuarios/{id:int}/ativar", async (int id, UsuarioAtual atual, KFContext contexto) =>
            {
                Usuario ator = await atual.ObterObrigatorioAsync();
                Usuario? alvo = await contexto.Usuarios.FindAsync(id);

                if (alvo == null)
                {
                    return NaoEncontrado(id);
                }

                if (!ator.PodeGerenciar(alvo))
                {
                    return Proibido("Você não pode ativar este usuário.");
                }

                alvo.Ativar();
                await contexto.SaveChangesAsync();

                return Results.Ok(UsuarioResponse.De(alvo));
            })
                .Produces<UsuarioResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .RequireAuthorization(Politicas.GerenciarUsuarios);

            // esqueceu a senha: quem gerencia a pessoa define uma nova (ela troca depois)
            app.MapPut("/usuarios/{id:int}/senha", async (int id, RedefinirSenhaRequest request, UsuarioAtual atual, KFContext contexto, ServicoDeSenha senhas) =>
            {
                Usuario ator = await atual.ObterObrigatorioAsync();
                Usuario? alvo = await contexto.Usuarios.FindAsync(id);

                if (alvo == null)
                {
                    return NaoEncontrado(id);
                }

                if (!ator.PodeGerenciar(alvo))
                {
                    return Proibido("Você não pode redefinir a senha deste usuário.");
                }

                PoliticaDeSenha.Validar(request.NovaSenha);

                alvo.DefinirSenhaHash(senhas.Gerar(request.NovaSenha));
                alvo.RegistrarLoginComSucesso(); // redefinir tambem destrava a conta
                await contexto.SaveChangesAsync();

                return Results.NoContent();
            })
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .RequireAuthorization(Politicas.GerenciarUsuarios);
        }
    }
}
