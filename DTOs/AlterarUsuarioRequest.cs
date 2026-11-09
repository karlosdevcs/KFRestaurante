using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    // e-mail e papel nao mudam por aqui. A lista de permissoes SUBSTITUI a atual.
    public record AlterarUsuarioRequest(string Nome, List<Permissao>? Permissoes);
}
