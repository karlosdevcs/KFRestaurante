using System.IdentityModel.Tokens.Jwt;
using KFRestaurante.Dominio;
using Microsoft.EntityFrameworkCore;

namespace KFRestaurante.Infraestrutura.Autenticacao
{
    // O usuario da requisicao, lido do banco UMA vez e compartilhado (escopo da requisicao) entre a validacao
    // do token, as regras de autorizacao e o proprio endpoint.
    public class UsuarioAtual
    {
        private readonly IHttpContextAccessor _http;
        private readonly KFContext _contexto;
        private bool _carregado;
        private Usuario? _usuario;

        public UsuarioAtual(IHttpContextAccessor http, KFContext contexto)
        {
            _http = http;
            _contexto = contexto;
        }

        // usado na validacao do token, quando o HttpContext.User ainda nao foi preenchido
        public async Task<Usuario?> CarregarAsync(int id)
        {
            _usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            _carregado = true;
            return _usuario;
        }

        public async Task<Usuario?> ObterAsync()
        {
            if (_carregado)
            {
                return _usuario;
            }

            return int.TryParse(_http.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out int id)
                ? await CarregarAsync(id)
                : null;
        }

        // para endpoints ja protegidos por [RequireAuthorization]: o usuario existe e esta ativo
        public async Task<Usuario> ObterObrigatorioAsync()
        {
            return await ObterAsync() ?? throw new InvalidOperationException("Endpoint protegido executou sem usuário autenticado.");
        }
    }
}
