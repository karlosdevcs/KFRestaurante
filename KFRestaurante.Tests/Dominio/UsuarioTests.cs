using KFRestaurante.Dominio;

namespace KFRestaurante.Tests.Dominio
{
    public class UsuarioTests
    {
        private static Usuario Novo(Papel papel = Papel.Funcionario, params Permissao[] permissoes)
        {
            return new Usuario("Maria", "maria@kf.com", "hash-qualquer", papel, permissoes);
        }

        // o Id so existe depois de gravar no banco; nos testes de hierarquia precisamos distinguir usuarios
        private static Usuario ComId(Usuario usuario, int id)
        {
            typeof(Usuario).GetProperty(nameof(Usuario.Id))!.SetValue(usuario, id);
            return usuario;
        }

        // ---------- criacao ----------

        [Fact]
        public void Cria_ativo_com_email_minusculo_e_nome_sem_espacos()
        {
            Usuario u = new Usuario("  Maria  ", "  Maria@KF.com ", "hash", Papel.Funcionario, new[] { Permissao.VerPedidos });

            Assert.Equal("Maria", u.Nome);
            Assert.Equal("maria@kf.com", u.Email);
            Assert.True(u.Ativo);
            Assert.Equal(Permissao.VerPedidos, u.Permissoes);
            Assert.Equal(DateTimeKind.Utc, u.CriadoEm.Kind);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("sem-arroba")]
        [InlineData("a@b")]
        [InlineData("a b@c.com")]
        public void Recusa_email_invalido(string email)
        {
            DominioException erro = Assert.Throws<DominioException>(() => new Usuario("Maria", email, "hash", Papel.Funcionario, null));

            Assert.Contains("E-mail inválido", erro.Message);
        }

        [Fact]
        public void Recusa_nome_em_branco()
        {
            Assert.Throws<DominioException>(() => new Usuario("  ", "maria@kf.com", "hash", Papel.Funcionario, null));
        }

        [Fact]
        public void Recusa_papel_inexistente()
        {
            Assert.Throws<DominioException>(() => new Usuario("Maria", "maria@kf.com", "hash", (Papel)99, null));
        }

        [Fact]
        public void Recusa_hash_de_senha_vazio()
        {
            Assert.Throws<DominioException>(() => new Usuario("Maria", "maria@kf.com", " ", Papel.Funcionario, null));
        }

        // ---------- permissoes ----------

        [Fact]
        public void Funcionario_so_tem_as_permissoes_que_recebeu()
        {
            Usuario u = Novo(Papel.Funcionario, Permissao.VerPedidos, Permissao.EsgotarOpcoes);

            Assert.True(u.TemPermissao(Permissao.VerPedidos));
            Assert.True(u.TemPermissao(Permissao.EsgotarOpcoes));
            Assert.False(u.TemPermissao(Permissao.AbrirFecharLoja));
        }

        [Fact]
        public void Sem_permissoes_nao_pode_nada()
        {
            Usuario u = Novo();

            foreach (Permissao p in PermissaoExtensions.Individuais)
            {
                Assert.False(u.TemPermissao(p));
            }
        }

        [Fact]
        public void Superuser_tem_todas_sempre_mesmo_sem_receber()
        {
            Usuario u = Novo(Papel.Superuser);

            foreach (Permissao p in PermissaoExtensions.Individuais)
            {
                Assert.True(u.TemPermissao(p));
            }
        }

        [Fact]
        public void Recusa_permissao_combinada_ou_desconhecida()
        {
            Assert.Throws<DominioException>(() => Novo(Papel.Funcionario, Permissao.VerPedidos | Permissao.EsgotarOpcoes));
            Assert.Throws<DominioException>(() => Novo(Papel.Funcionario, (Permissao)4096));
            Assert.Throws<DominioException>(() => Novo(Papel.Funcionario, Permissao.Nenhuma));
        }

        [Fact]
        public void AlterarDados_troca_nome_e_permissoes()
        {
            Usuario u = Novo(Papel.Funcionario, Permissao.VerPedidos);

            u.AlterarDados(" Maria Silva ", new[] { Permissao.AbrirFecharLoja });

            Assert.Equal("Maria Silva", u.Nome);
            Assert.False(u.TemPermissao(Permissao.VerPedidos));
            Assert.True(u.TemPermissao(Permissao.AbrirFecharLoja));
        }

        [Fact]
        public void AlterarDados_nao_tira_permissoes_do_superuser()
        {
            Usuario u = Novo(Papel.Superuser);

            u.AlterarDados("Admin", Array.Empty<Permissao>());

            Assert.Equal(PermissaoExtensions.Todas, u.Permissoes);
        }

        [Fact]
        public void Permissoes_viram_lista_e_voltam_ao_mesmo_conjunto()
        {
            Permissao conjunto = PermissaoExtensions.Combinar(new[] { Permissao.VerPedidos, Permissao.AlterarWhatsApp });

            List<Permissao> lista = conjunto.Separar();

            Assert.Equal(new[] { Permissao.VerPedidos, Permissao.AlterarWhatsApp }, lista);
            Assert.Equal(conjunto, PermissaoExtensions.Combinar(lista));
        }

        // ---------- hierarquia ----------

        [Theory]
        [InlineData(Papel.Superuser, Papel.Superuser, true)]
        [InlineData(Papel.Superuser, Papel.Dono, true)]
        [InlineData(Papel.Superuser, Papel.Funcionario, true)]
        [InlineData(Papel.Dono, Papel.Superuser, false)]
        [InlineData(Papel.Dono, Papel.Dono, false)]
        [InlineData(Papel.Dono, Papel.Funcionario, true)]
        [InlineData(Papel.Funcionario, Papel.Superuser, false)]
        [InlineData(Papel.Funcionario, Papel.Dono, false)]
        [InlineData(Papel.Funcionario, Papel.Funcionario, false)]
        public void Quem_pode_criar_quem(Papel ator, Papel alvo, bool esperado)
        {
            Assert.Equal(esperado, Novo(ator).PodeCriar(alvo));
        }

        [Fact]
        public void Dono_gerencia_funcionario_mas_nao_outro_dono()
        {
            Usuario dono = ComId(Novo(Papel.Dono), 1);
            Usuario funcionario = ComId(Novo(Papel.Funcionario), 2);
            Usuario outroDono = ComId(Novo(Papel.Dono), 3);

            Assert.True(dono.PodeGerenciar(funcionario));
            Assert.False(dono.PodeGerenciar(outroDono));
        }

        [Fact]
        public void Ninguem_se_gerencia_pelo_fluxo_de_gerenciar_outro()
        {
            Usuario superuser = ComId(Novo(Papel.Superuser), 1);

            Assert.False(superuser.PodeGerenciar(superuser));
        }

        [Fact]
        public void Usuario_inativo_nao_gerencia_ninguem()
        {
            Usuario dono = ComId(Novo(Papel.Dono), 1);
            Usuario funcionario = ComId(Novo(Papel.Funcionario), 2);
            dono.Desativar();

            Assert.False(dono.PodeGerenciar(funcionario));
        }

        // ---------- ninguem concede o que nao tem ----------

        [Fact]
        public void Dono_so_concede_permissoes_que_ele_mesmo_tem()
        {
            Usuario dono = Novo(Papel.Dono, Permissao.VerPedidos, Permissao.EsgotarOpcoes);

            Assert.True(dono.PodeConceder(Permissao.VerPedidos));
            Assert.True(dono.PodeConceder(Permissao.VerPedidos | Permissao.EsgotarOpcoes));
            Assert.False(dono.PodeConceder(Permissao.AbrirFecharLoja));
            Assert.False(dono.PodeConceder(Permissao.VerPedidos | Permissao.AbrirFecharLoja));
        }

        [Fact]
        public void Superuser_concede_qualquer_permissao()
        {
            Assert.True(Novo(Papel.Superuser).PodeConceder(PermissaoExtensions.Todas));
        }

        [Fact]
        public void Conceder_nada_e_sempre_permitido()
        {
            Assert.True(Novo(Papel.Funcionario).PodeConceder(Permissao.Nenhuma));
        }

        // ---------- ativar/desativar ----------

        [Fact]
        public void Ativar_e_desativar_sao_idempotentes()
        {
            Usuario u = Novo();

            u.Desativar();
            u.Desativar();
            Assert.False(u.Ativo);

            u.Ativar();
            u.Ativar();
            Assert.True(u.Ativo);
        }

        // ---------- bloqueio por tentativas ----------

        private static readonly DateTime Agora = new DateTime(2026, 11, 9, 12, 0, 0, DateTimeKind.Utc);

        [Fact]
        public void Bloqueia_na_quinta_falha_por_quinze_minutos()
        {
            Usuario u = Novo();

            for (int i = 0; i < Usuario.MaximoDeFalhasDeLogin - 1; i++)
            {
                u.RegistrarFalhaDeLogin(Agora);
            }
            Assert.False(u.EstaBloqueado(Agora));

            u.RegistrarFalhaDeLogin(Agora);

            Assert.True(u.EstaBloqueado(Agora));
            Assert.True(u.EstaBloqueado(Agora.AddMinutes(14)));
            Assert.False(u.EstaBloqueado(Agora.AddMinutes(16)));
        }

        [Fact]
        public void Login_com_sucesso_zera_as_falhas()
        {
            Usuario u = Novo();
            u.RegistrarFalhaDeLogin(Agora);
            u.RegistrarFalhaDeLogin(Agora);

            u.RegistrarLoginComSucesso();

            Assert.Equal(0, u.FalhasDeLogin);
            Assert.Null(u.BloqueadoAte);
        }

        [Fact]
        public void Depois_de_bloquear_o_contador_recomeca()
        {
            Usuario u = Novo();

            for (int i = 0; i < Usuario.MaximoDeFalhasDeLogin; i++)
            {
                u.RegistrarFalhaDeLogin(Agora);
            }

            Assert.Equal(0, u.FalhasDeLogin);
        }

        // ---------- senha ----------

        [Fact]
        public void DefinirSenhaHash_recusa_vazio()
        {
            Assert.Throws<DominioException>(() => Novo().DefinirSenhaHash(""));
        }
    }

    public class PoliticaDeSenhaTests
    {
        [Theory]
        [InlineData("12345678")]
        [InlineData("uma senha longa com espaços")]
        public void Aceita_senha_valida(string senha)
        {
            PoliticaDeSenha.Validar(senha);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("         ")]
        [InlineData("1234567")]
        public void Recusa_senha_curta_ou_vazia(string? senha)
        {
            DominioException erro = Assert.Throws<DominioException>(() => PoliticaDeSenha.Validar(senha));

            Assert.Contains("mínimo 8", erro.Message);
        }

        [Fact]
        public void Recusa_senha_gigante()
        {
            Assert.Throws<DominioException>(() => PoliticaDeSenha.Validar(new string('a', PoliticaDeSenha.Maximo + 1)));
        }

        [Fact]
        public void A_mensagem_de_erro_nunca_repete_a_senha()
        {
            DominioException erro = Assert.Throws<DominioException>(() => PoliticaDeSenha.Validar("segredo"));

            Assert.DoesNotContain("segredo", erro.Message);
        }
    }
}
