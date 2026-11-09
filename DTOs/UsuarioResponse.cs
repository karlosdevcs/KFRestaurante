using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    // nunca inclui o hash da senha
    public record UsuarioResponse(int Id, string Nome, string Email, Papel Papel, bool Ativo, List<Permissao> Permissoes)
    {
        public static UsuarioResponse De(Usuario usuario)
        {
            // para o superuser, Permissoes ja guarda todas; a lista e o que ele pode fazer de fato
            UsuarioResponse resposta = new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email, usuario.Papel, usuario.Ativo, usuario.Permissoes.Separar());

            return resposta;
        }
    }
}
