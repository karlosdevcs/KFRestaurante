using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    public record CriarPedidoRequest(
        string NomeCliente,
        string Telefone,
        int CardapioId,
        List<CriarItemDoPedidoRequest> Itens,
        FormadePagamento FormaDePagamento,
        decimal? TrocoPara,
        TipoDeEntrega TipoDeEntrega,
        string? Endereco,
        string? Referencia,
        string? Observacao);
}
