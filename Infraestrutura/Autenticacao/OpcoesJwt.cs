using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace KFRestaurante.Infraestrutura.Autenticacao
{
    public class OpcoesJwt
    {
        public const string Secao = "Jwt";
        public const int TamanhoMinimoDaChave = 32; // HMAC-SHA256 pede ao menos 256 bits

        public string Chave { get; set; } = "";          // SEGREDO: vem do user-secrets, nunca do appsettings
        public string Emissor { get; set; } = "KFRestaurante";
        public string Audiencia { get; set; } = "KFRestaurante.Loja";
        public int HorasDeValidade { get; set; } = 12;   // um turno de trabalho

        public SymmetricSecurityKey ChaveDeAssinatura() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Chave));

        // falha ao subir a API (e nao no primeiro login) se o segredo estiver faltando ou fraco
        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(Chave) || Encoding.UTF8.GetByteCount(Chave) < TamanhoMinimoDaChave)
            {
                throw new InvalidOperationException(
                    $"A chave do JWT ('{Secao}:Chave') não está configurada ou tem menos de {TamanhoMinimoDaChave} caracteres. " +
                    $"Configure com: dotnet user-secrets set \"{Secao}:Chave\" \"<texto aleatório longo>\"");
            }

            if (HorasDeValidade <= 0)
            {
                throw new InvalidOperationException($"'{Secao}:HorasDeValidade' deve ser maior que zero.");
            }
        }
    }
}
