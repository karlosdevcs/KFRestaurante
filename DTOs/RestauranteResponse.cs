using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    public record RestauranteResponse(bool Aberto, string? WhatsApp)
    {
        public static RestauranteResponse De(Restaurante restaurante)
        {
            RestauranteResponse restauranteResponse = new RestauranteResponse(restaurante.Aberto, restaurante.WhatsApp);

            return restauranteResponse;
        }
    }
}
