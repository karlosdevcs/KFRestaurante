using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    public record PedidoResponse(
        int Id,
        int Numero,
        DateTime HoraPedido,
        string NomeCliente,
        string Telefone,
        FormadePagamento FormaDePagamento,
        decimal? TrocoPara,
        TipoDeEntrega TipoDeEntrega,
        string? Endereco,
        string? Referencia,
        string? Observacao,
        int CardapioId,
        List<ItemDoPedidoResponse> Itens,
        decimal Total,
        string MensagemWhatsApp,
        string? LinkWhatsApp)
    {
        // whatsAppDaLoja null = loja sem numero cadastrado: devolve a mensagem, mas sem link
        public static PedidoResponse De(Pedido pedido, string? whatsAppDaLoja)
        {
            List<ItemDoPedidoResponse> itens = pedido.Itens.Select(item => ItemDoPedidoResponse.De(item)).ToList();

            // nome completo: dentro do record, "MensagemWhatsApp" sozinho e a propriedade, nao a classe
            string mensagem = Servicos.MensagemWhatsApp.Montar(pedido);
            string? link = whatsAppDaLoja != null ? Servicos.MensagemWhatsApp.Link(whatsAppDaLoja, mensagem) : null;

            PedidoResponse pedidoResponse = new PedidoResponse(
                pedido.Id,
                pedido.Numero,
                pedido.HoraPedido,
                pedido.NomeCliente,
                pedido.Telefone,
                pedido.FormaDePagamento,
                pedido.TrocoPara,
                pedido.TipoDeEntrega,
                pedido.Endereco,
                pedido.Referencia,
                pedido.Observacao,
                pedido.CardapioDoDia.Id,
                itens,
                pedido.CalcularTotal(),
                mensagem,
                link);

            return pedidoResponse;
        }
    }
}
