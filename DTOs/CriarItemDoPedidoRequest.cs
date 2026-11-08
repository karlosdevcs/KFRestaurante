namespace KFRestaurante.DTOs
{
    // item avulso: preenche OpcaoId. Marmita: preenche TamanhoId e ComponentesIds. Nunca os dois.
    public record CriarItemDoPedidoRequest(int? OpcaoId, int? TamanhoId, List<int>? ComponentesIds, int Quantidade);
}
