using KFRestaurante.Dominio;
using Microsoft.AspNetCore.Authorization;

namespace KFRestaurante.Infraestrutura.Autenticacao
{
    public class PermissaoRequirement : IAuthorizationRequirement
    {
        public Permissao Permissao { get; }

        public PermissaoRequirement(Permissao permissao)
        {
            Permissao = permissao;
        }
    }

    // a ordem numerica do enum Papel E a hierarquia (Funcionario < Dono < Superuser)
    public class PapelMinimoRequirement : IAuthorizationRequirement
    {
        public Papel Papel { get; }

        public PapelMinimoRequirement(Papel papel)
        {
            Papel = papel;
        }
    }

    // Decide a partir do usuario NO BANCO, nao do que veio no token.
    public class AutorizacaoHandler : IAuthorizationHandler
    {
        private readonly UsuarioAtual _atual;

        public AutorizacaoHandler(UsuarioAtual atual)
        {
            _atual = atual;
        }

        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            Usuario? usuario = await _atual.ObterAsync();

            if (usuario == null || !usuario.Ativo)
            {
                return; // nao concede nada: os requisitos pendentes falham
            }

            foreach (IAuthorizationRequirement requisito in context.PendingRequirements.ToList())
            {
                bool ok = requisito switch
                {
                    PermissaoRequirement p => usuario.TemPermissao(p.Permissao),
                    PapelMinimoRequirement m => usuario.Papel >= m.Papel,
                    _ => false
                };

                if (ok)
                {
                    context.Succeed(requisito);
                }
            }
        }
    }

    // nomes das politicas usados nos endpoints: .RequireAuthorization(Politicas.De(Permissao.VerPedidos))
    public static class Politicas
    {
        public const string Autenticado = "Autenticado";
        public const string GerenciarUsuarios = "GerenciarUsuarios";

        public static string De(Permissao permissao) => $"Permissao:{permissao}";

        public static void Registrar(AuthorizationOptions opcoes)
        {
            // qualquer usuario logado e ativo (a checagem de "ativo" acontece na validacao do token)
            opcoes.AddPolicy(Autenticado, p => p.RequireAuthenticatedUser());

            // criar/editar/desativar usuarios: dono ou superuser (o que cada um pode criar, a hierarquia decide)
            opcoes.AddPolicy(GerenciarUsuarios, p => p.RequireAuthenticatedUser().AddRequirements(new PapelMinimoRequirement(Papel.Dono)));

            foreach (Permissao permissao in PermissaoExtensions.Individuais)
            {
                opcoes.AddPolicy(De(permissao), p => p.RequireAuthenticatedUser().AddRequirements(new PermissaoRequirement(permissao)));
            }
        }
    }
}
