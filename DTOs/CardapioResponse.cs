using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    // AtualizadoEm em UTC: quando as opcoes do cardapio mudaram pela ultima vez
    public record CardapioResponse(int Id, DateOnly DataPublicacao, SituacaoCardapio Situacao, DateTime AtualizadoEm, List<OpcaoResponse> Opcoes)
    {
        public static CardapioResponse De(CardapioDoDia cardapio)
        {
            List<OpcaoResponse> opcoes = cardapio.Opcoes.Select(opcao => OpcaoResponse.De(opcao)).ToList();

            CardapioResponse cardapioResponse = new CardapioResponse(cardapio.Id, cardapio.DataPublicacao, cardapio.Situacao, cardapio.AtualizadoEm, opcoes);

            return cardapioResponse;
        }
    }
}
