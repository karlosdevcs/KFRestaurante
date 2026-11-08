using KFRestaurante.Dominio;

namespace KFRestaurante.Tests.Dominio
{
    public class MarmitaTests
    {
        private readonly Opcao _arroz = new Opcao("Arroz", Categoria.Acompanhamento);
        private readonly Opcao _feijoada = new Opcao("Feijoada", Categoria.Carne, 25m);
        private readonly Opcao _bife = new Opcao("Bife", Categoria.Carne);
        private readonly Opcao _tropeiro = new Opcao("Feijao Tropeiro", Categoria.Guarnicao, 24m);

        [Fact]
        public void Preco_e_nome_vem_do_tamanho()
        {
            Marmita marmita = new Marmita(new TamanhoMarmita("M", 20m, 3));

            Assert.Equal(20m, marmita.Preco);
            Assert.Equal("Marmita M", marmita.Nome);
        }

        [Fact]
        public void Adiciona_componentes_ate_o_limite_total()
        {
            Marmita marmita = new Marmita(new TamanhoMarmita("M", 20m, 3));

            marmita.AdicionarOpcao(_arroz);
            marmita.AdicionarOpcao(_feijoada);
            marmita.AdicionarOpcao(_tropeiro);

            Assert.Equal(3, marmita.Opcoes.Count());
        }

        [Fact]
        public void Recusa_componente_alem_do_limite_total()
        {
            Marmita marmita = new Marmita(new TamanhoMarmita("P", 15m, 1));
            marmita.AdicionarOpcao(_arroz);

            DominioException erro = Assert.Throws<DominioException>(() => marmita.AdicionarOpcao(_feijoada));

            Assert.Contains("no máximo 1 componentes", erro.Message);
            Assert.Contains("Feijoada", erro.Message);
        }

        [Fact]
        public void Recusa_bebida_como_componente()
        {
            Marmita marmita = new Marmita(new TamanhoMarmita("M", 20m, 3));

            Assert.Throws<DominioException>(() => marmita.AdicionarOpcao(new Opcao("Coca-Cola", Categoria.Bebida, 6m)));
        }

        [Fact]
        public void Recusa_alem_do_limite_da_categoria()
        {
            TamanhoMarmita g = new TamanhoMarmita("G", 25m, 5);
            g.DefinirLimites(new[] { new LimiteCategoria(Categoria.Carne, 1) });
            Marmita marmita = new Marmita(g);
            marmita.AdicionarOpcao(_feijoada);

            DominioException erro = Assert.Throws<DominioException>(() => marmita.AdicionarOpcao(_bife));

            Assert.Contains("no máximo 1 de Carne", erro.Message);
            Assert.DoesNotContain("Porção dupla", erro.Message);
        }

        [Fact]
        public void Porcao_dupla_conta_como_dois_no_limite_da_categoria()
        {
            TamanhoMarmita g = new TamanhoMarmita("G", 25m, 5);
            g.DefinirLimites(new[] { new LimiteCategoria(Categoria.Carne, 1) });
            Marmita marmita = new Marmita(g);
            marmita.AdicionarOpcao(_feijoada);

            DominioException erro = Assert.Throws<DominioException>(() => marmita.AdicionarOpcao(_feijoada));

            Assert.Contains("Porção dupla conta como 2", erro.Message);
        }

        [Fact]
        public void Porcao_dupla_e_aceita_quando_cabe()
        {
            TamanhoMarmita g = new TamanhoMarmita("G", 25m, 5);
            g.DefinirLimites(new[] { new LimiteCategoria(Categoria.Carne, 2) });
            Marmita marmita = new Marmita(g);

            marmita.AdicionarOpcao(_feijoada);
            marmita.AdicionarOpcao(_feijoada);

            Assert.Equal(2, marmita.Opcoes.Count());
        }

        [Fact]
        public void Categoria_sem_limite_respeita_so_o_total()
        {
            TamanhoMarmita g = new TamanhoMarmita("G", 25m, 3);
            g.DefinirLimites(new[] { new LimiteCategoria(Categoria.Carne, 1) });
            Marmita marmita = new Marmita(g);

            marmita.AdicionarOpcao(_arroz);
            marmita.AdicionarOpcao(_arroz);
            marmita.AdicionarOpcao(_arroz);

            Assert.Equal(3, marmita.Opcoes.Count());
        }
    }
}
