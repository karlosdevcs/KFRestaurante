using KFRestaurante.Dominio;
using KFRestaurante.Servicos;

namespace KFRestaurante.Tests.Servicos
{
    public class MensagemWhatsAppTests
    {
        private readonly Opcao _arroz = new Opcao("Arroz", Categoria.Acompanhamento);
        private readonly Opcao _feijoada = new Opcao("Feijoada", Categoria.Carne, 25m);
        private readonly Opcao _tropeiro = new Opcao("Feijão Tropeiro", Categoria.Guarnicao, 24m);
        private readonly TamanhoMarmita _m = new TamanhoMarmita("M", 20m, 3);

        private Pedido Pedido(FormadePagamento pagamento = FormadePagamento.Pix, decimal? troco = null,
            TipoDeEntrega entrega = TipoDeEntrega.Entrega, string? referencia = null, string? observacao = null)
        {
            CardapioDoDia cardapio = new CardapioDoDia(new DateOnly(2026, 11, 8));
            cardapio.AdicionarOpcao(_arroz);
            cardapio.AdicionarOpcao(_feijoada);
            cardapio.AdicionarOpcao(_tropeiro);
            cardapio.Publicar();

            Restaurante restaurante = new Restaurante();
            restaurante.AlterarWhatsApp("5531999999999");
            restaurante.Abrir();

            Pedido pedido = new Pedido("Maria", "31999999999", pagamento, troco, entrega, "Rua A, 10", referencia, observacao, restaurante, cardapio);

            Marmita marmita = new Marmita(_m);
            marmita.AdicionarOpcao(_arroz);
            marmita.AdicionarOpcao(_arroz);
            marmita.AdicionarOpcao(_tropeiro);

            pedido.AdicionarItem(_feijoada, 1);
            pedido.AdicionarItem(marmita, 2);
            pedido.Finalizar();

            return pedido;
        }

        private static string[] Linhas(string mensagem) => mensagem.Split('\n');

        [Fact]
        public void Quebra_de_linha_e_sempre_barra_n()
        {
            string mensagem = MensagemWhatsApp.Montar(Pedido());

            Assert.DoesNotContain("\r", mensagem); // igual em servidor Windows ou Linux
        }

        [Fact]
        public void Mensagem_tem_cliente_itens_componentes_e_total()
        {
            string mensagem = MensagemWhatsApp.Montar(Pedido());

            Assert.Contains("Cliente: Maria · (31) 99999-9999", Linhas(mensagem));
            Assert.Contains("1x Feijoada — R$ 25,00", Linhas(mensagem));
            Assert.Contains("2x Marmita M — R$ 40,00", Linhas(mensagem));
            Assert.Contains("   Arroz, Arroz, Feijão Tropeiro", Linhas(mensagem));
            Assert.Contains("*Total: R$ 65,00*", Linhas(mensagem));
        }

        [Fact]
        public void Telefone_fixo_sai_formatado_com_oito_digitos()
        {
            Restaurante restaurante = new Restaurante();
            restaurante.AlterarWhatsApp("5531999999999");
            restaurante.Abrir();
            CardapioDoDia cardapio = new CardapioDoDia(new DateOnly(2026, 11, 8));
            cardapio.AdicionarOpcao(_feijoada);
            cardapio.Publicar();
            Pedido pedido = new Pedido("João", "3133334444", FormadePagamento.Pix, null, TipoDeEntrega.Retirada, null, null, null, restaurante, cardapio);
            pedido.AdicionarItem(_feijoada, 1);

            string mensagem = MensagemWhatsApp.Montar(pedido);

            Assert.Contains("Cliente: João · (31) 3333-4444", Linhas(mensagem));
        }

        [Fact]
        public void Dinheiro_com_troco_informa_o_valor()
        {
            string mensagem = MensagemWhatsApp.Montar(Pedido(FormadePagamento.Dinheiro, troco: 100m));

            Assert.Contains("Pagamento: Dinheiro – troco para R$ 100,00", Linhas(mensagem));
        }

        [Fact]
        public void Dinheiro_sem_troco_diz_sem_troco()
        {
            string mensagem = MensagemWhatsApp.Montar(Pedido(FormadePagamento.Dinheiro));

            Assert.Contains("Pagamento: Dinheiro – sem troco", Linhas(mensagem));
        }

        [Theory]
        [InlineData(FormadePagamento.Pix, "Pagamento: Pix")]
        [InlineData(FormadePagamento.CartaoDeDebito, "Pagamento: Cartão de débito")]
        [InlineData(FormadePagamento.CartaoDeCredito, "Pagamento: Cartão de crédito")]
        public void Pagamento_aparece_por_extenso(FormadePagamento pagamento, string linha)
        {
            string mensagem = MensagemWhatsApp.Montar(Pedido(pagamento));

            Assert.Contains(linha, Linhas(mensagem));
        }

        [Fact]
        public void Entrega_mostra_endereco_e_referencia()
        {
            string mensagem = MensagemWhatsApp.Montar(Pedido(referencia: "portão azul"));

            Assert.Contains("Entrega: Rua A, 10", Linhas(mensagem));
            Assert.Contains("Referência: portão azul", Linhas(mensagem));
        }

        [Fact]
        public void Retirada_nao_mostra_endereco()
        {
            string mensagem = MensagemWhatsApp.Montar(Pedido(entrega: TipoDeEntrega.Retirada));

            Assert.Contains("Retirada no local", Linhas(mensagem));
            Assert.DoesNotContain("Entrega:", mensagem);
        }

        [Fact]
        public void Linhas_opcionais_so_aparecem_quando_preenchidas()
        {
            string mensagem = MensagemWhatsApp.Montar(Pedido());

            Assert.DoesNotContain("Referência:", mensagem);
            Assert.DoesNotContain("Obs.:", mensagem);
        }

        [Fact]
        public void Observacao_aparece_no_fim()
        {
            string mensagem = MensagemWhatsApp.Montar(Pedido(observacao: "sem cebola"));

            Assert.Equal("Obs.: sem cebola", Linhas(mensagem).Last());
        }

        [Fact]
        public void Horario_sai_no_fuso_de_brasilia()
        {
            Pedido pedido = Pedido();
            TimeZoneInfo brasilia = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
            DateTime esperado = TimeZoneInfo.ConvertTimeFromUtc(pedido.HoraPedido, brasilia);

            string mensagem = MensagemWhatsApp.Montar(pedido);

            Assert.Contains($"{esperado:dd/MM/yyyy} às {esperado:HH:mm}", Linhas(mensagem));
        }

        [Fact]
        public void Link_aponta_para_a_loja_com_a_mensagem_codificada()
        {
            string link = MensagemWhatsApp.Link("5531999999999", "*Pedido nº 1*\nTotal: R$ 6,00");

            Assert.StartsWith("https://wa.me/5531999999999?text=", link);
            Assert.DoesNotContain(" ", link);  // espacos e quebras de linha precisam ir codificados
            Assert.DoesNotContain("\n", link);

            string texto = Uri.UnescapeDataString(link.Split("?text=")[1]);
            Assert.Equal("*Pedido nº 1*\nTotal: R$ 6,00", texto);
        }
    }
}
