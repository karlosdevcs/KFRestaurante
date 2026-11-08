using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    public record LimiteCategoriaResponse(Categoria Categoria, int Maximo)
    {
        public static LimiteCategoriaResponse De(LimiteCategoria limite)
        {
            LimiteCategoriaResponse limiteResponse = new LimiteCategoriaResponse(limite.Categoria, limite.Maximo);

            return limiteResponse;
        }
    }
}
