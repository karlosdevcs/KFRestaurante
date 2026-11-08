using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    public record LimiteCategoriaRequest(Categoria Categoria, int Maximo);
}
