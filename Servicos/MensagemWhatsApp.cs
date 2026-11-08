using System.Globalization;
using System.Text;
using KFRestaurante.Dominio;

namespace KFRestaurante.Servicos
{
    // Monta o texto que o cliente envia para a loja pelo link wa.me.
    // Objetivo: a loja ler e comecar a preparar, sem precisar perguntar nada.
    // Nao e regra de negocio, e formato de saida: por isso fica fora do Dominio.
    public static class MensagemWhatsApp
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        // "America/Sao_Paulo" existe no Windows (.NET 6+) e no Linux
        private static readonly TimeZoneInfo Brasilia = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

        public static string Montar(Pedido pedido)
        {
            DateTime horaLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(pedido.HoraPedido, DateTimeKind.Utc), Brasilia);

            StringBuilder texto = new StringBuilder();

            // *texto* e negrito no WhatsApp
            texto.AppendLine($"*Pedido nº {pedido.Numero}*");
            texto.AppendLine($"{horaLocal.ToString("dd/MM/yyyy", PtBr)} às {horaLocal.ToString("HH:mm", PtBr)}");
            texto.AppendLine($"Cliente: {pedido.NomeCliente}");
            texto.AppendLine();

            foreach (ItemDoPedido item in pedido.Itens)
            {
                texto.AppendLine($"{item.Quantidade}x {item.Nome} — {Reais(item.Subtotal)}");

                if (item.Componentes != null)
                {
                    texto.AppendLine($"   {item.Componentes}");
                }
            }

            texto.AppendLine();
            texto.AppendLine($"*Total: {Reais(pedido.CalcularTotal())}*");
            texto.AppendLine();
            texto.AppendLine($"Pagamento: {Pagamento(pedido)}");

            if (pedido.TipoDeEntrega == TipoDeEntrega.Entrega)
            {
                texto.AppendLine($"Entrega: {pedido.Endereco}");

                if (pedido.Referencia != null)
                {
                    texto.AppendLine($"Referência: {pedido.Referencia}");
                }
            }
            else
            {
                texto.AppendLine("Retirada no local");
            }

            if (pedido.Observacao != null)
            {
                texto.AppendLine();
                texto.AppendLine($"Obs.: {pedido.Observacao}");
            }

            // AppendLine usa \r\n no Windows e \n no Linux: fixa \n para a mensagem ser igual em qualquer servidor
            return texto.ToString().Replace("\r\n", "\n").TrimEnd();
        }

        // abre o WhatsApp do cliente na conversa com a loja, com a mensagem ja escrita
        public static string Link(string whatsAppDaLoja, string mensagem)
        {
            return $"https://wa.me/{whatsAppDaLoja}?text={Uri.EscapeDataString(mensagem)}";
        }

        private static string Pagamento(Pedido pedido)
        {
            switch (pedido.FormaDePagamento)
            {
                case FormadePagamento.Dinheiro:
                    // sempre explicito, para a loja nao perguntar "precisa de troco?"
                    return pedido.TrocoPara != null ? $"Dinheiro – troco para {Reais(pedido.TrocoPara.Value)}" : "Dinheiro – sem troco";
                case FormadePagamento.CartaoDeDebito:
                    return "Cartão de débito";
                case FormadePagamento.CartaoDeCredito:
                    return "Cartão de crédito";
                default:
                    return "Pix";
            }
        }

        private static string Reais(decimal valor)
        {
            return $"R$ {valor.ToString("N2", PtBr)}";
        }
    }
}
