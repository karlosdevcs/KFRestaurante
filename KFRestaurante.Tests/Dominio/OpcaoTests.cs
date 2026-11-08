using KFRestaurante.Dominio;

namespace KFRestaurante.Tests.Dominio
{
    public class OpcaoTests
    {
        [Fact]
        public void Cria_opcao_com_preco_vendida_avulsa()
        {
            Opcao coca = new Opcao("Coca-Cola", Categoria.Bebida, 6m);

            Assert.Equal("Coca-Cola", coca.Nome);
            Assert.Equal(Categoria.Bebida, coca.Categoria);
            Assert.Equal(6m, coca.Preco);
            Assert.True(coca.VendidaAvulsa);
            Assert.False(coca.Esgotado);
        }

        [Fact]
        public void Componente_sem_preco_nao_e_vendido_avulso()
        {
            Opcao arroz = new Opcao("Arroz", Categoria.Acompanhamento);

            Assert.Null(arroz.Preco);
            Assert.False(arroz.VendidaAvulsa);
        }

        [Theory]
        [InlineData(Categoria.Bebida)]
        [InlineData(Categoria.Sobremesa)]
        public void Recusa_bebida_ou_sobremesa_sem_preco(Categoria categoria)
        {
            DominioException erro = Assert.Throws<DominioException>(() => new Opcao("Item", categoria));

            Assert.Contains("precisa ter preço", erro.Message);
        }

        [Fact]
        public void Recusa_preco_negativo()
        {
            DominioException erro = Assert.Throws<DominioException>(() => new Opcao("Feijoada", Categoria.Carne, -1m));

            Assert.Contains("-1", erro.Message);
        }

        [Fact]
        public void Alterar_troca_nome_categoria_e_preco()
        {
            Opcao opcao = new Opcao("Feijao Tropeiro", Categoria.Carne, 24m);

            opcao.Alterar("Feijão Tropeiro", Categoria.Guarnicao, 26m);

            Assert.Equal("Feijão Tropeiro", opcao.Nome);
            Assert.Equal(Categoria.Guarnicao, opcao.Categoria);
            Assert.Equal(26m, opcao.Preco);
        }

        [Fact]
        public void Alterar_aplica_as_mesmas_regras_da_criacao()
        {
            Opcao coca = new Opcao("Coca-Cola", Categoria.Bebida, 6m);

            Assert.Throws<DominioException>(() => coca.Alterar("Coca-Cola", Categoria.Bebida, null));
        }

        [Fact]
        public void Marcar_esgotado_e_idempotente()
        {
            Opcao opcao = new Opcao("Feijoada", Categoria.Carne, 25m);

            opcao.AlterarEsgotado(true);
            opcao.AlterarEsgotado(true);

            Assert.True(opcao.Esgotado);

            opcao.AlterarEsgotado(false);

            Assert.False(opcao.Esgotado);
        }
    }
}
