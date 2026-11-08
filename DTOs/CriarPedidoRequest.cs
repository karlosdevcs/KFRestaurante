using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    public record CriarPedidoRequest(int CardapioId, List<CriarItemDoPedidoRequest> Itens, FormadePagamento FormaDePagamento, TipoDeEntrega TipoDeEntrega, string? Endereco);
}
