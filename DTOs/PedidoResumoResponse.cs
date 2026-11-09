using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    // resumo para a lista (GET /pedidos): sem itens, sem mensagem do WhatsApp.
    // O painel usa GET /pedidos/{id} quando precisar do pedido completo
    public record PedidoResumoResponse(
        int Id,
        int Numero,
        DateTime HoraPedido,
        string NomeCliente,
        string Telefone,
        StatusPedido Status,
        FormadePagamento FormaDePagamento,
        TipoDeEntrega TipoDeEntrega,
        decimal Total)
    {
        public static PedidoResumoResponse De(Pedido pedido)
        {
            PedidoResumoResponse resumo = new PedidoResumoResponse(
                pedido.Id,
                pedido.Numero,
                pedido.HoraPedido,
                pedido.NomeCliente,
                pedido.Telefone,
                pedido.Status,
                pedido.FormaDePagamento,
                pedido.TipoDeEntrega,
                pedido.CalcularTotal());

            return resumo;
        }
    }
}
