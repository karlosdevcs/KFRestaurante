using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    public record ItemDoPedidoResponse(string Nome, string? Componentes, decimal PrecoUnitario, int Quantidade, decimal Subtotal)
    {
        public static ItemDoPedidoResponse De(ItemDoPedido item)
        {
            ItemDoPedidoResponse itemResponse = new ItemDoPedidoResponse(item.Nome, item.Componentes, item.PrecoUnitario, item.Quantidade, item.Subtotal);

            return itemResponse;
        }
    }
}
