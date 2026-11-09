using KFRestaurante.Dominio;
using Microsoft.EntityFrameworkCore;

namespace KFRestaurante.Infraestrutura.Autenticacao
{
    // O primeiro superuser nao nasce de um cadastro publico: vem da configuracao local (user-secrets):
    //   dotnet user-secrets set "Seed:Superuser:Nome"  "Seu Nome"
    //   dotnet user-secrets set "Seed:Superuser:Email" "voce@exemplo.com"
    //   dotnet user-secrets set "Seed:Superuser:Senha" "uma senha forte"
    // Cria so se nao existir usuario com esse e-mail; nunca altera um usuario que ja existe.
    public static class SeedDeUsuarios
    {
        public static async Task GarantirSuperuserAsync(IServiceProvider servicos, IConfiguration configuracao, ILogger logger)
        {
            string? nome = configuracao["Seed:Superuser:Nome"];
            string? email = configuracao["Seed:Superuser:Email"];
            string? senha = configuracao["Seed:Superuser:Senha"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                logger.LogInformation("Seed de superuser não configurado (Seed:Superuser:Email/Senha); nada a fazer.");
                return;
            }

            using IServiceScope escopo = servicos.CreateScope();
            KFContext contexto = escopo.ServiceProvider.GetRequiredService<KFContext>();
            ServicoDeSenha servicoDeSenha = escopo.ServiceProvider.GetRequiredService<ServicoDeSenha>();

            string emailNormalizado = email.Trim().ToLowerInvariant();

            if (await contexto.Usuarios.AnyAsync(u => u.Email == emailNormalizado))
            {
                return;
            }

            // senha fraca no seed: erro claro no log, sem derrubar a API
            PoliticaDeSenha.Validar(senha);

            Usuario superuser = new Usuario(string.IsNullOrWhiteSpace(nome) ? "Superuser" : nome, email, servicoDeSenha.Gerar(senha), Papel.Superuser, null);
            contexto.Add(superuser);
            await contexto.SaveChangesAsync();

            logger.LogWarning("Superuser {Email} criado a partir da configuração. Troque a senha no primeiro acesso.", superuser.Email);
        }
    }
}
