using KFRestaurante.Dominio;

namespace KFRestaurante.Tests.Dominio
{
    public class TamanhoMarmitaTests
    {
        [Fact]
        public void Cria_tamanho_sem_limites_por_categoria()
        {
            TamanhoMarmita m = new TamanhoMarmita("M", 20m, 3);

            Assert.Equal("M", m.Nome);
            Assert.Equal(20m, m.Preco);
            Assert.Equal(3, m.LimitedeComponente);
            Assert.Empty(m.Limites);
            Assert.Null(m.LimiteDe(Categoria.Carne));
        }

        [Fact]
        public void Recusa_preco_negativo()
        {
            Assert.Throws<DominioException>(() => new TamanhoMarmita("M", -1m, 3));
        }

        [Fact]
        public void Recusa_limite_total_negativo()
        {
            Assert.Throws<DominioException>(() => new TamanhoMarmita("M", 20m, -1));
        }

        [Fact]
        public void Alterar_aplica_as_mesmas_regras_da_criacao()
        {
            TamanhoMarmita m = new TamanhoMarmita("M", 20m, 3);

            Assert.Throws<DominioException>(() => m.Alterar("M", -5m, 3));

            m.Alterar("Média", 22m, 4);

            Assert.Equal("Média", m.Nome);
            Assert.Equal(22m, m.Preco);
            Assert.Equal(4, m.LimitedeComponente);
        }

        [Fact]
        public void DefinirLimites_guarda_um_limite_por_categoria()
        {
            TamanhoMarmita g = new TamanhoMarmita("G", 25m, 5);

            g.DefinirLimites(new[] { new LimiteCategoria(Categoria.Carne, 2), new LimiteCategoria(Categoria.Salada, 1) });

            Assert.Equal(2, g.LimiteDe(Categoria.Carne));
            Assert.Equal(1, g.LimiteDe(Categoria.Salada));
            Assert.Null(g.LimiteDe(Categoria.Acompanhamento)); // sem limite = livre
        }

        [Fact]
        public void DefinirLimites_substitui_os_anteriores()
        {
            TamanhoMarmita g = new TamanhoMarmita("G", 25m, 5);
            g.DefinirLimites(new[] { new LimiteCategoria(Categoria.Carne, 2) });

            g.DefinirLimites(new[] { new LimiteCategoria(Categoria.Carne, 3) });

            Assert.Equal(3, g.LimiteDe(Categoria.Carne));
            Assert.Single(g.Limites);
        }

        [Fact]
        public void DefinirLimites_com_lista_vazia_remove_todos()
        {
            TamanhoMarmita g = new TamanhoMarmita("G", 25m, 5);
            g.DefinirLimites(new[] { new LimiteCategoria(Categoria.Carne, 2) });

            g.DefinirLimites(Array.Empty<LimiteCategoria>());

            Assert.Empty(g.Limites);
        }

        [Fact]
        public void DefinirLimites_recusa_categoria_repetida()
        {
            TamanhoMarmita g = new TamanhoMarmita("G", 25m, 5);

            DominioException erro = Assert.Throws<DominioException>(() =>
                g.DefinirLimites(new[] { new LimiteCategoria(Categoria.Carne, 1), new LimiteCategoria(Categoria.Carne, 2) }));

            Assert.Contains("Carne", erro.Message);
        }

        [Theory]
        [InlineData(Categoria.Bebida)]
        [InlineData(Categoria.Sobremesa)]
        public void LimiteCategoria_recusa_categoria_que_nao_e_componente(Categoria categoria)
        {
            Assert.Throws<DominioException>(() => new LimiteCategoria(categoria, 1));
        }

        [Fact]
        public void LimiteCategoria_recusa_maximo_negativo()
        {
            Assert.Throws<DominioException>(() => new LimiteCategoria(Categoria.Carne, -1));
        }
    }
}
