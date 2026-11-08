namespace KFRestaurante.DTOs
{
    // substitui todos os limites do tamanho. Lista vazia = remove todos (so vale o limite total)
    public record AlterarLimitesRequest(List<LimiteCategoriaRequest> Limites);
}
