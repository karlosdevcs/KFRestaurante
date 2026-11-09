namespace KFRestaurante.DTOs
{
    // a lista SUBSTITUI as opcoes atuais do cardapio (so em rascunho)
    public record AlterarOpcoesDoCardapioRequest(List<int> OpcoesIds);
}
