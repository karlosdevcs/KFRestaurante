using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    // lista do painel (GET /pedidos): tudo do pedido, MENOS a mensagem do WhatsApp e o link (esses so no GET /pedidos/{id}).
    // Traz os itens porque a lista ja os carrega do banco: o painel nao precisa de uma chamada por pedido
    public record PedidoResumoResponse(
        int Id,
        int Numero,
        DateTime HoraPedido,
        string NomeCliente,
        string Telefone,
        StatusPedido Status,
        FormadePagamento FormaDePagamento,
        decimal? TrocoPara,
        TipoDeEntrega TipoDeEntrega,
        string? Endereco,
        string? Referencia,
        string? Observacao,
        List<ItemDoPedidoResponse> Itens,
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
                pedido.TrocoPara,
                pedido.TipoDeEntrega,
                pedido.Endereco,
                pedido.Referencia,
                pedido.Observacao,
                pedido.Itens.Select(item => ItemDoPedidoResponse.De(item)).ToList(),
                pedido.CalcularTotal());

            return resumo;
        }
    }
}
