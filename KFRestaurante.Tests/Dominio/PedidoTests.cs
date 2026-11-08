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
            restaurante.AlterarWhatsApp("5531999999999");
            restaurante.Abrir();
            return restaurante;
        }

        private Pedido NovoPedido()
        {
            return new Pedido("Maria", "31999999999", FormadePagamento.Pix, null, TipoDeEntrega.Retirada, null, null, null, RestauranteAberto(), CardapioPublicado());
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
            Pedido pedido = new Pedido("Maria", "31999999999", FormadePagamento.Pix, null, TipoDeEntrega.Entrega, "  Rua A, 10  ", null, null, RestauranteAberto(), CardapioPublicado());

            Assert.Equal("Rua A, 10", pedido.Endereco);
        }

        [Fact]
        public void Retirada_descarta_endereco()
        {
            Pedido pedido = new Pedido("Maria", "31999999999", FormadePagamento.Dinheiro, null, TipoDeEntrega.Retirada, "Rua B", null, null, RestauranteAberto(), CardapioPublicado());

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
                new Pedido("Maria", "31999999999", FormadePagamento.Pix, null, TipoDeEntrega.Retirada, null, null, null, new Restaurante(), CardapioPublicado()));

            Assert.Contains("fechado", erro.Message);
        }

        [Fact]
        public void Recusa_cardapio_em_rascunho()
        {
            CardapioDoDia rascunho = new CardapioDoDia(new DateOnly(2026, 11, 8));
            rascunho.AdicionarOpcao(_arroz);

            Assert.Throws<DominioException>(() =>
                new Pedido("Maria", "31999999999", FormadePagamento.Pix, null, TipoDeEntrega.Retirada, null, null, null, RestauranteAberto(), rascunho));
        }

        [Fact]
        public void Recusa_cardapio_fechado()
        {
            CardapioDoDia cardapio = CardapioPublicado();
            cardapio.Fechar();

            DominioException erro = Assert.Throws<DominioException>(() =>
                new Pedido("Maria", "31999999999", FormadePagamento.Pix, null, TipoDeEntrega.Retirada, null, null, null, RestauranteAberto(), cardapio));

            Assert.Contains("Fechado", erro.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Recusa_entrega_sem_endereco(string? endereco)
        {
            Assert.Throws<DominioException>(() =>
                new Pedido("Maria", "31999999999", FormadePagamento.Pix, null, TipoDeEntrega.Entrega, endereco, null, null, RestauranteAberto(), CardapioPublicado()));
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

        // ---------- dados para a loja nao precisar perguntar nada no WhatsApp ----------

        private Pedido PedidoCom(FormadePagamento pagamento = FormadePagamento.Pix, decimal? troco = null,
            TipoDeEntrega entrega = TipoDeEntrega.Entrega, string? referencia = null, string? observacao = null, string nome = "Maria")
        {
            return new Pedido(nome, "31999999999", pagamento, troco, entrega, "Rua A, 10", referencia, observacao, RestauranteAberto(), CardapioPublicado());
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Recusa_pedido_sem_nome_do_cliente(string nome)
        {
            DominioException erro = Assert.Throws<DominioException>(() => PedidoCom(nome: nome));

            Assert.Contains("nome do cliente", erro.Message);
        }

        [Fact]
        public void Guarda_nome_referencia_e_observacao_sem_espacos_nas_pontas()
        {
            Pedido pedido = PedidoCom(nome: "  Maria  ", referencia: "  portão azul ", observacao: " sem cebola ");

            Assert.Equal("Maria", pedido.NomeCliente);
            Assert.Equal("portão azul", pedido.Referencia);
            Assert.Equal("sem cebola", pedido.Observacao);
        }

        [Fact]
        public void Observacao_e_referencia_em_branco_viram_nulo()
        {
            Pedido pedido = PedidoCom(referencia: "   ", observacao: "");

            Assert.Null(pedido.Referencia);
            Assert.Null(pedido.Observacao);
        }

        [Fact]
        public void Retirada_descarta_referencia()
        {
            Pedido pedido = PedidoCom(entrega: TipoDeEntrega.Retirada, referencia: "portão azul");

            Assert.Null(pedido.Referencia);
        }

        [Theory]
        [InlineData(FormadePagamento.Pix)]
        [InlineData(FormadePagamento.CartaoDeCredito)]
        [InlineData(FormadePagamento.CartaoDeDebito)]
        public void Troco_e_descartado_fora_do_dinheiro(FormadePagamento pagamento)
        {
            Pedido pedido = PedidoCom(pagamento: pagamento, troco: 100m);

            Assert.Null(pedido.TrocoPara);
        }

        [Fact]
        public void Aceita_troco_maior_ou_igual_ao_total()
        {
            Pedido pedido = PedidoCom(pagamento: FormadePagamento.Dinheiro, troco: 12m);
            pedido.AdicionarItem(_coca, 2); // total 12

            pedido.Finalizar();

            Assert.Equal(12m, pedido.TrocoPara);
        }

        [Fact]
        public void Recusa_troco_menor_que_o_total()
        {
            Pedido pedido = PedidoCom(pagamento: FormadePagamento.Dinheiro, troco: 10m);
            pedido.AdicionarItem(_coca, 2); // total 12

            DominioException erro = Assert.Throws<DominioException>(() => pedido.Finalizar());

            Assert.Contains("R$ 10,00", erro.Message); // formato brasileiro mesmo em servidor em ingles
            Assert.Contains("R$ 12,00", erro.Message);
        }

        [Fact]
        public void Dinheiro_sem_troco_e_aceito()
        {
            Pedido pedido = PedidoCom(pagamento: FormadePagamento.Dinheiro, troco: null);
            pedido.AdicionarItem(_coca, 1);

            pedido.Finalizar();

            Assert.Null(pedido.TrocoPara);
        }

        [Fact]
        public void Finalizar_recusa_pedido_sem_itens()
        {
            Pedido pedido = NovoPedido();

            DominioException erro = Assert.Throws<DominioException>(() => pedido.Finalizar());

            Assert.Contains("ao menos um item", erro.Message);
        }

        // ---------- telefone ----------

        private Pedido PedidoComTelefone(string telefone)
        {
            return new Pedido("Maria", telefone, FormadePagamento.Pix, null, TipoDeEntrega.Retirada, null, null, null, RestauranteAberto(), CardapioPublicado());
        }

        [Theory]
        [InlineData("31999999999", "31999999999")]      // celular: 11 digitos
        [InlineData("(31) 99999-9999", "31999999999")]  // formatado
        [InlineData("3133334444", "3133334444")]        // fixo: 10 digitos
        public void Guarda_telefone_so_com_digitos(string informado, string guardado)
        {
            Pedido pedido = PedidoComTelefone(informado);

            Assert.Equal(guardado, pedido.Telefone);
        }

        [Theory]
        [InlineData("999999999")]      // 9 digitos: sem DDD
        [InlineData("5531999999999")]  // 13 digitos: com codigo do pais
        [InlineData("")]
        [InlineData(null)]
        public void Recusa_telefone_fora_de_10_ou_11_digitos(string? telefone)
        {
            DominioException erro = Assert.Throws<DominioException>(() => PedidoComTelefone(telefone!));

            Assert.Contains("Telefone inválido", erro.Message);
        }
    }
}
