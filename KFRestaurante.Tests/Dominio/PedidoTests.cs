using KFRestaurante.Dominio;

namespace KFRestaurante.Tests.Dominio
{
    public class PedidoTests
    {
        private readonly Opcao _arroz = new Opcao("Arroz", Categoria.Acompanhamento);
        private readonly Opcao _feijoada = new Opcao("Feijoada", Categoria.Carne, 25m);
        private readonly Opcao _coca = new Opcao("Coca-Cola", Categoria.Bebida, 6m);
        private readonly Opcao _lasanha = new Opcao("Lasanha", Categoria.Carne, 30m); // fora do cardapio
        private readonly TamanhoMarmita _m = new TamanhoMarmita("M", 20m, 3);

        private CardapioDoDia CardapioPublicado()
        {
            CardapioDoDia cardapio = new CardapioDoDia(new DateOnly(2026, 11, 8));
            cardapio.AdicionarOpcao(_arroz);
            cardapio.AdicionarOpcao(_feijoada);
            cardapio.AdicionarOpcao(_coca);
            cardapio.Publicar();
            return cardapio;
        }

        private static Restaurante RestauranteAberto()
        {
            Restaurante restaurante = new Restaurante();
            restaurante.Abrir();
            return restaurante;
        }

        private Pedido NovoPedido()
        {
            return new Pedido(FormadePagamento.Pix, TipoDeEntrega.Retirada, null, RestauranteAberto(), CardapioPublicado());
        }

        // ---------- caminho feliz ----------

        [Fact]
        public void Calcula_total_com_avulso_e_marmita()
        {
            Pedido pedido = NovoPedido();
            Marmita marmita = new Marmita(_m);
            marmita.AdicionarOpcao(_arroz);
            marmita.AdicionarOpcao(_feijoada);

            pedido.AdicionarItem(_coca, 2);    // 2 x 6  = 12
            pedido.AdicionarItem(marmita, 1);  // 1 x 20 = 20

            Assert.Equal(32m, pedido.CalcularTotal());
            Assert.Equal(2, pedido.Itens.Count());
        }

        [Fact]
        public void Item_de_marmita_guarda_foto_dos_componentes()
        {
            Pedido pedido = NovoPedido();
            Marmita marmita = new Marmita(_m);
            marmita.AdicionarOpcao(_arroz);
            marmita.AdicionarOpcao(_arroz);
            marmita.AdicionarOpcao(_feijoada);

            pedido.AdicionarItem(marmita, 1);

            ItemDoPedido item = pedido.Itens.Single();
            Assert.Equal("Marmita M", item.Nome);
            Assert.Equal("Arroz, Arroz, Feijoada", item.Componentes);
        }

        [Fact]
        public void Preco_fica_congelado_no_momento_do_pedido()
        {
            Pedido pedido = NovoPedido();
            pedido.AdicionarItem(_coca, 1);

            _coca.Alterar("Coca-Cola", Categoria.Bebida, 8m); // dono aumenta o preco depois

            Assert.Equal(6m, pedido.Itens.Single().PrecoUnitario);
            Assert.Equal(6m, pedido.CalcularTotal());
        }

        [Fact]
        public void Entrega_guarda_endereco_sem_espacos_nas_pontas()
        {
            Pedido pedido = new Pedido(FormadePagamento.Pix, TipoDeEntrega.Entrega, "  Rua A, 10  ", RestauranteAberto(), CardapioPublicado());

            Assert.Equal("Rua A, 10", pedido.Endereco);
        }

        [Fact]
        public void Retirada_descarta_endereco()
        {
            Pedido pedido = new Pedido(FormadePagamento.Dinheiro, TipoDeEntrega.Retirada, "Rua B", RestauranteAberto(), CardapioPublicado());

            Assert.Null(pedido.Endereco);
        }

        [Fact]
        public void Hora_do_pedido_e_gravada_em_utc()
        {
            Pedido pedido = NovoPedido();

            Assert.Equal(DateTimeKind.Utc, pedido.HoraPedido.Kind);
        }

        // ---------- regras que recusam ----------

        [Fact]
        public void Recusa_com_restaurante_fechado()
        {
            DominioException erro = Assert.Throws<DominioException>(() =>
                new Pedido(FormadePagamento.Pix, TipoDeEntrega.Retirada, null, new Restaurante(), CardapioPublicado()));

            Assert.Contains("fechado", erro.Message);
        }

        [Fact]
        public void Recusa_cardapio_em_rascunho()
        {
            CardapioDoDia rascunho = new CardapioDoDia(new DateOnly(2026, 11, 8));
            rascunho.AdicionarOpcao(_arroz);

            Assert.Throws<DominioException>(() =>
                new Pedido(FormadePagamento.Pix, TipoDeEntrega.Retirada, null, RestauranteAberto(), rascunho));
        }

        [Fact]
        public void Recusa_cardapio_fechado()
        {
            CardapioDoDia cardapio = CardapioPublicado();
            cardapio.Fechar();

            DominioException erro = Assert.Throws<DominioException>(() =>
                new Pedido(FormadePagamento.Pix, TipoDeEntrega.Retirada, null, RestauranteAberto(), cardapio));

            Assert.Contains("Fechado", erro.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Recusa_entrega_sem_endereco(string? endereco)
        {
            Assert.Throws<DominioException>(() =>
                new Pedido(FormadePagamento.Pix, TipoDeEntrega.Entrega, endereco, RestauranteAberto(), CardapioPublicado()));
        }

        [Fact]
        public void Recusa_avulso_fora_do_cardapio()
        {
            Pedido pedido = NovoPedido();

            DominioException erro = Assert.Throws<DominioException>(() => pedido.AdicionarItem(_lasanha, 1));

            Assert.Contains("Lasanha", erro.Message);
        }

        [Fact]
        public void Recusa_componente_fora_do_cardapio()
        {
            Pedido pedido = NovoPedido();
            Marmita marmita = new Marmita(_m);
            marmita.AdicionarOpcao(_lasanha);

            DominioException erro = Assert.Throws<DominioException>(() => pedido.AdicionarItem(marmita, 1));

            Assert.Contains("Lasanha", erro.Message);
        }

        [Fact]
        public void Recusa_avulso_esgotado()
        {
            Pedido pedido = NovoPedido();
            _coca.AlterarEsgotado(true);

            DominioException erro = Assert.Throws<DominioException>(() => pedido.AdicionarItem(_coca, 1));

            Assert.Contains("esgotada", erro.Message);
        }

        [Fact]
        public void Recusa_componente_esgotado()
        {
            Pedido pedido = NovoPedido();
            _feijoada.AlterarEsgotado(true);
            Marmita marmita = new Marmita(_m);
            marmita.AdicionarOpcao(_feijoada);

            Assert.Throws<DominioException>(() => pedido.AdicionarItem(marmita, 1));
        }

        [Fact]
        public void Recusa_avulso_sem_preco()
        {
            Pedido pedido = NovoPedido();

            DominioException erro = Assert.Throws<DominioException>(() => pedido.AdicionarItem(_arroz, 1));

            Assert.Contains("não é vendido avulso", erro.Message);
        }

        [Fact]
        public void Recusa_marmita_vazia()
        {
            Pedido pedido = NovoPedido();

            DominioException erro = Assert.Throws<DominioException>(() => pedido.AdicionarItem(new Marmita(_m), 1));

            Assert.Contains("ao menos um componente", erro.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public void Recusa_quantidade_menor_ou_igual_a_zero(int quantidade)
        {
            Pedido pedido = NovoPedido();

            DominioException erro = Assert.Throws<DominioException>(() => pedido.AdicionarItem(_coca, quantidade));

            Assert.Contains(quantidade.ToString(), erro.Message);
        }

        [Fact]
        public void Item_recusado_nao_entra_no_pedido()
        {
            Pedido pedido = NovoPedido();
            pedido.AdicionarItem(_coca, 1);

            Assert.Throws<DominioException>(() => pedido.AdicionarItem(_lasanha, 1));

            Assert.Single(pedido.Itens);
            Assert.Equal(6m, pedido.CalcularTotal());
        }
    }
}
