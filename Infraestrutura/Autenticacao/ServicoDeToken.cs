using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using KFRestaurante.Dominio;
using Microsoft.IdentityModel.Tokens;

namespace KFRestaurante.Infraestrutura.Autenticacao
{
    public class ServicoDeToken
    {
        private readonly OpcoesJwt _opcoes;

        public ServicoDeToken(OpcoesJwt opcoes)
        {
            _opcoes = opcoes;
        }

        // O token carrega SO a identidade (id). Papel e permissoes ficam no banco e sao lidos a cada
        // requisicao: assim revogar uma permissao ou desativar o usuario vale na hora, sem esperar o token vencer.
        public (string Token, DateTime ExpiraEm) Gerar(Usuario usuario)
        {
            DateTime expiraEm = DateTime.UtcNow.AddHours(_opcoes.HorasDeValidade);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _opcoes.Emissor,
                audience: _opcoes.Audiencia,
                claims: new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                },
                expires: expiraEm,
                signingCredentials: new SigningCredentials(_opcoes.ChaveDeAssinatura(), SecurityAlgorithms.HmacSha256));

            return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
        }
    }
}
