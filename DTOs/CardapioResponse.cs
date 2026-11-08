using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    public record CardapioResponse(int Id, DateOnly DataPublicacao, SituacaoCardapio Situacao, List<OpcaoResponse> Opcoes)
    {
        public static CardapioResponse De(CardapioDoDia cardapio)
        {
            List<OpcaoResponse> opcoes = cardapio.Opcoes.Select(opcao => OpcaoResponse.De(opcao)).ToList();

            CardapioResponse cardapioResponse = new CardapioResponse(cardapio.Id, cardapio.DataPublicacao, cardapio.Situacao, opcoes);

            return cardapioResponse;
        }
    }
}
