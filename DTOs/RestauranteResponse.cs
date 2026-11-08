using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    public record RestauranteResponse(bool Aberto)
    {
        public static RestauranteResponse De(Restaurante restaurante)
        {
            RestauranteResponse restauranteResponse = new RestauranteResponse(restaurante.Aberto);

            return restauranteResponse;
        }
    }
}
