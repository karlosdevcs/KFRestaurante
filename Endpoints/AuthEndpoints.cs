using KFRestaurante.Dominio;
using KFRestaurante.DTOs;
using KFRestaurante.Infraestrutura;
using KFRestaurante.Infraestrutura.Autenticacao;
using Microsoft.EntityFrameworkCore;

namespace KFRestaurante.Endpoints
{
    public static class AuthEndpoints
    {
        // a mesma mensagem para e-mail inexistente, senha errada, conta inativa ou bloqueada:
        // quem erra nao descobre quais e-mails existem
        private const string FalhaDeLogin = "E-mail ou senha inválidos, ou acesso bloqueado.";

        public static void MapAuth(this IEndpointRouteBuilder app)
        {
            app.MapPost("/auth/login", async (LoginRequest request, KFContext contexto, ServicoDeSenha senhas, ServicoDeToken tokens) =>
            {
                string email = (request.Email ?? "").Trim().ToLowerInvariant();
                string senha = request.Senha ?? "";
                DateTime agora = DateTime.UtcNow;

                Usuario? usuario = await contexto.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

                if (usuario == null || !usuario.Ativo || usuario.EstaBloqueado(agora))
                {
                    senhas.GastarTempoComoSeVerificasse(senha); // demora o mesmo que uma senha errada
                    return Results.Problem(detail: FalhaDeLogin, statusCode: StatusCodes.Status401Unauthorized);
                }

                ResultadoDaSenha resultado = senhas.Verificar(usuario.SenhaHash, senha);

                if (resultado == ResultadoDaSenha.Incorreta)
                {
                    usuario.RegistrarFalhaDeLogin(agora);
                    await contexto.SaveChangesAsync();
                    return Results.Problem(detail: FalhaDeLogin, statusCode: StatusCodes.Status401Unauthorized);
                }

                if (resultado == ResultadoDaSenha.CorretaPrecisaNovoHash)
                {
                    usuario.DefinirSenhaHash(senhas.Gerar(senha)); // o algoritmo do hash evoluiu: atualiza no login
                }

                usuario.RegistrarLoginComSucesso();
                await contexto.SaveChangesAsync();

                (string token, DateTime expiraEm) = tokens.Gerar(usuario);

                return Results.Ok(new LoginResponse(token, expiraEm, UsuarioResponse.De(usuario)));
            })
                .Produces<LoginResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized);

            // o front chama ao abrir: descobre quem e e o que pode fazer (para mostrar so as abas permitidas)
            app.MapGet("/auth/eu", async (UsuarioAtual atual) =>
            {
                Usuario usuario = await atual.ObterObrigatorioAsync();

                return Results.Ok(UsuarioResponse.De(usuario));
            })
                .Produces<UsuarioResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .RequireAuthorization(Politicas.Autenticado);

            app.MapPost("/auth/trocar-senha", async (TrocarSenhaRequest request, UsuarioAtual atual, KFContext contexto, ServicoDeSenha senhas) =>
            {
                Usuario usuario = await atual.ObterObrigatorioAsync();

                if (senhas.Verificar(usuario.SenhaHash, request.SenhaAtual ?? "") == ResultadoDaSenha.Incorreta)
                {
                    // errar a senha atual conta como tentativa: um token roubado nao serve para adivinhar a senha
                    usuario.RegistrarFalhaDeLogin(DateTime.UtcNow);
                    await contexto.SaveChangesAsync();
                    return Results.Problem(detail: "Senha atual incorreta.", statusCode: StatusCodes.Status400BadRequest);
                }

                PoliticaDeSenha.Validar(request.NovaSenha);

                usuario.DefinirSenhaHash(senhas.Gerar(request.NovaSenha));
                usuario.RegistrarLoginComSucesso();
                await contexto.SaveChangesAsync();

                return Results.NoContent();
            })
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .RequireAuthorization(Politicas.Autenticado);
        }
    }
}
