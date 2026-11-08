using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    public record PedidoResponse(
        int Id,
        int Numero,
        DateTime HoraPedido,
        string NomeCliente,
        FormadePagamento FormaDePagamento,
        decimal? TrocoPara,
        TipoDeEntrega TipoDeEntrega,
        string? Endereco,
        string? Referencia,
        string? Observacao,
        int CardapioId,
        List<ItemDoPedidoResponse> Itens,
        decimal Total)
    {
        public static PedidoResponse De(Pedido pedido)
        {
            List<ItemDoPedidoResponse> itens = pedido.Itens.Select(item => ItemDoPedidoResponse.De(item)).ToList();

            PedidoResponse pedidoResponse = new PedidoResponse(
                pedido.Id,
                pedido.Numero,
                pedido.HoraPedido,
                pedido.NomeCliente,
                pedido.FormaDePagamento,
                pedido.TrocoPara,
                pedido.TipoDeEntrega,
                pedido.Endereco,
                pedido.Referencia,
                pedido.Observacao,
                pedido.CardapioDoDia.Id,
                itens,
                pedido.CalcularTotal());

            return pedidoResponse;
        }
    }
}
