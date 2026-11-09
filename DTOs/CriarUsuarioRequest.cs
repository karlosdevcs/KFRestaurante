using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    // a senha definida aqui e provisoria: a pessoa troca em POST /auth/trocar-senha
    public record CriarUsuarioRequest(string Nome, string Email, string Senha, Papel Papel, List<Permissao>? Permissoes);
}
