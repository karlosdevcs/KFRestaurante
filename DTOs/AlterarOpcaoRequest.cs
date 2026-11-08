using KFRestaurante.Dominio;

namespace KFRestaurante.DTOs
{
    public record AlterarOpcaoRequest(string Nome, Categoria Categoria, decimal Preco);
}
