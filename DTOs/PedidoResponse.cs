using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    public record PedidoResponse(int Id, int Numero, DateTime HoraPedido, FormadePagamento FormaDePagamento, TipoDeEntrega TipoDeEntrega, string? Endereco, int CardapioId, List<ItemDoPedidoResponse> Itens, decimal Total)
    {
        public static PedidoResponse De(Pedido pedido)
        {
            List<ItemDoPedidoResponse> itens = pedido.Itens.Select(item => ItemDoPedidoResponse.De(item)).ToList();

            PedidoResponse pedidoResponse = new PedidoResponse(pedido.Id, pedido.Numero, pedido.HoraPedido, pedido.FormaDePagamento, pedido.TipoDeEntrega, pedido.Endereco, pedido.CardapioDoDia.Id, itens, pedido.CalcularTotal());

            return pedidoResponse;
        }
    }
}
