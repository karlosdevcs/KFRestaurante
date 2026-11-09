using Microsoft.AspNetCore.Identity;

namespace KFRestaurante.Infraestrutura.Autenticacao
{
    // Hash de senha pelo PasswordHasher do framework (PBKDF2 com sal e iteracoes). Nao escrevemos criptografia a mao.
    public class ServicoDeSenha
    {
        private readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();
        private readonly object _semUsuario = new object();

        // hash calculado uma vez: serve para gastar o mesmo tempo quando o e-mail nao existe
        private readonly string _hashFalso;

        public ServicoDeSenha()
        {
            _hashFalso = _hasher.HashPassword(_semUsuario, Guid.NewGuid().ToString());
        }

        public string Gerar(string senha)
        {
            return _hasher.HashPassword(_semUsuario, senha);
        }

        public ResultadoDaSenha Verificar(string hash, string senha)
        {
            PasswordVerificationResult resultado = _hasher.VerifyHashedPassword(_semUsuario, hash, senha);

            return resultado switch
            {
                PasswordVerificationResult.Success => ResultadoDaSenha.Correta,
                PasswordVerificationResult.SuccessRehashNeeded => ResultadoDaSenha.CorretaPrecisaNovoHash,
                _ => ResultadoDaSenha.Incorreta
            };
        }

        // e-mail inexistente deve demorar o mesmo que senha errada, senao o tempo de resposta revela quais e-mails existem
        public void GastarTempoComoSeVerificasse(string senha)
        {
            _hasher.VerifyHashedPassword(_semUsuario, _hashFalso, senha);
        }
    }

    public enum ResultadoDaSenha
    {
        Incorreta,
        Correta,
        CorretaPrecisaNovoHash
    }
}
