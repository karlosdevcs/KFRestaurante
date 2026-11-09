using System.Text.RegularExpressions;

namespace KFRestaurante.Dominio
{
    public class Usuario
    {
        public const int MaximoDeFalhasDeLogin = 5;
        public static readonly TimeSpan TempoDeBloqueio = TimeSpan.FromMinutes(15);

        private static readonly Regex FormatoDeEmail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public int Id { get; private set; }
        public string Nome { get; private set; } = null!;
        public string Email { get; private set; } = null!;   // sempre minusculo: e o login
        public string SenhaHash { get; private set; } = null!;
        public Papel Papel { get; private set; }
        public Permissao Permissoes { get; private set; }     // do superuser, sempre todas
        public bool Ativo { get; private set; }
        public int FalhasDeLogin { get; private set; }
        public DateTime? BloqueadoAte { get; private set; }   // UTC
        public DateTime CriadoEm { get; private set; }        // UTC

        public Usuario(string nome, string email, string senhaHash, Papel papel, IEnumerable<Permissao>? permissoes)
        {
            if (!Enum.IsDefined(papel))
            {
                throw new DominioException($"Papel inválido: '{papel}'.");
            }

            if (string.IsNullOrWhiteSpace(senhaHash))
            {
                throw new DominioException("A senha do usuário não foi informada.");
            }

            this.Nome = NomeValido(nome);
            this.Email = EmailValido(email);
            this.SenhaHash = senhaHash;
            this.Papel = papel;
            this.Permissoes = papel == Papel.Superuser ? PermissaoExtensions.Todas : PermissaoExtensions.Combinar(permissoes);
            this.Ativo = true;
            this.CriadoEm = DateTime.UtcNow;
        }

        // ---------- o que o usuario pode FAZER ----------

        public bool TemPermissao(Permissao permissao)
        {
            return Papel == Papel.Superuser || Permissoes.HasFlag(permissao);
        }

        // ---------- hierarquia: quem pode gerenciar quem ----------

        // Superuser cria qualquer papel; Dono so cria Funcionario; Funcionario nao cria ninguem
        public bool PodeCriar(Papel papelDoNovoUsuario)
        {
            return Papel switch
            {
                Papel.Superuser => true,
                Papel.Dono => papelDoNovoUsuario == Papel.Funcionario,
                _ => false
            };
        }

        // gerenciar = editar, ativar/desativar e redefinir a senha de OUTRO usuario.
        // Ninguem se gerencia por aqui (a propria senha tem fluxo proprio).
        public bool PodeGerenciar(Usuario alvo)
        {
            return Ativo && alvo.Id != Id && PodeCriar(alvo.Papel);
        }

        // ninguem concede uma permissao que nao tem
        public bool PodeConceder(Permissao permissoes)
        {
            return Papel == Papel.Superuser || (Permissoes & permissoes) == permissoes;
        }

        // ---------- alteracoes ----------

        public void AlterarDados(string nome, IEnumerable<Permissao>? permissoes)
        {
            this.Nome = NomeValido(nome);

            if (Papel != Papel.Superuser) // superuser sempre tem todas
            {
                this.Permissoes = PermissaoExtensions.Combinar(permissoes);
            }
        }

        public void DefinirSenhaHash(string senhaHash)
        {
            if (string.IsNullOrWhiteSpace(senhaHash))
            {
                throw new DominioException("A senha do usuário não foi informada.");
            }

            this.SenhaHash = senhaHash;
        }

        public void Ativar()
        {
            this.Ativo = true; // ja ativo: continua ativo, sem erro (idempotente)
        }

        public void Desativar()
        {
            this.Ativo = false; // ja inativo: continua inativo, sem erro (idempotente)
        }

        // ---------- bloqueio por tentativas de login ----------

        public bool EstaBloqueado(DateTime agoraUtc)
        {
            return BloqueadoAte != null && BloqueadoAte > agoraUtc;
        }

        public void RegistrarFalhaDeLogin(DateTime agoraUtc)
        {
            this.FalhasDeLogin++;

            if (FalhasDeLogin >= MaximoDeFalhasDeLogin)
            {
                this.BloqueadoAte = agoraUtc + TempoDeBloqueio;
                this.FalhasDeLogin = 0;
            }
        }

        public void RegistrarLoginComSucesso()
        {
            this.FalhasDeLogin = 0;
            this.BloqueadoAte = null;
        }

        // ---------- validacoes ----------

        private static string NomeValido(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new DominioException("Informe o nome do usuário.");
            }

            return nome.Trim();
        }

        private static string EmailValido(string? email)
        {
            string normalizado = (email ?? "").Trim().ToLowerInvariant();

            if (!FormatoDeEmail.IsMatch(normalizado))
            {
                throw new DominioException($"E-mail inválido: '{email}'.");
            }

            return normalizado;
        }

        // o EF precisa de um construtor sem parametros
        private Usuario() { }
    }
}
