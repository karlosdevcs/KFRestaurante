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

        public static string Montar(Pedido pedido)
        {
            DateTime horaLocal = HorarioBrasilia.DeUtc(pedido.HoraPedido);

            StringBuilder texto = new StringBuilder();

            // *texto* e negrito no WhatsApp
            texto.AppendLine($"*Pedido nº {pedido.Numero}*");
            texto.AppendLine($"{horaLocal.ToString("dd/MM/yyyy", PtBr)} às {horaLocal.ToString("HH:mm", PtBr)}");
            // pedidos anteriores ao telefone obrigatorio tem telefone vazio: mostra so o nome
            texto.AppendLine(pedido.Telefone.Length > 0
                ? $"Cliente: {pedido.NomeCliente} · {TelefoneFormatado(pedido.Telefone)}"
                : $"Cliente: {pedido.NomeCliente}");
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

        // 31999999999 -> (31) 99999-9999 | 3133334444 -> (31) 3333-4444
        private static string TelefoneFormatado(string digitos)
        {
            string ddd = digitos.Substring(0, 2);
            string numero = digitos.Substring(2);
            int corte = numero.Length - 4;

            return $"({ddd}) {numero.Substring(0, corte)}-{numero.Substring(corte)}";
        }

        private static string Reais(decimal valor)
        {
            return $"R$ {valor.ToString("N2", PtBr)}";
        }
    }
}
